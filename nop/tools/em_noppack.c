#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/stat.h>
#include <locale.h>

#ifdef _WIN32
#include <windows.h>
#include <wchar.h>
#include <io.h>
#include <fcntl.h>
// POSIX macros not available in MSVC
#ifndef S_ISDIR
#define S_ISDIR(m) (((m) & S_IFMT) == S_IFDIR)
#endif
#ifndef S_ISREG
#define S_ISREG(m) (((m) & S_IFMT) == S_IFREG)
#endif
#else
#include <dirent.h>
#endif

#define BUFFER_SIZE (1024 * 1024 * 256)
#define MAX_FILES 10000

// macro to convert strings to UTF-8 for display (Windows only, passthrough on other platforms)
#ifdef _WIN32
#define DISPLAY(s) to_utf8(s)
#else
#define DISPLAY(s) (s)
#endif

/**
 * SONNORI LZ77 XOR Key array
 */
unsigned short lz77_customkey[] = {
    0xFF21, 0x834F, 0x675F, 0x0034, 0xF237, 0x815F, 0x4765, 0x0233
};

typedef struct {
    unsigned char name_size;
    unsigned char type;
    unsigned int offset;
    unsigned int encoded_size;
    unsigned int decoded_size;
    char *name;
    unsigned char *data;
    unsigned char *compressed_data;  // for compressed version
} nop_entry;

/**
 * NOP file types
 */
enum e_noptype {
    NOP_DATA_RAW = 0x00,
    NOP_DATA_LZ77 = 0x01,
    NOP_DATA_DIRECTORY = 0x02,
    NOP_DATA_SONNORI_LZ77 = 0x03
};

/**
 * global file list and buffers
 */
nop_entry files[MAX_FILES];
int file_count = 0;
unsigned char *buff1;
unsigned char *buff2;

#ifdef _WIN32
/**
 * convert UTF-16 to EUC-KR for Korean filename handling
 */
char* wchar_to_euc_kr(const wchar_t* wstr) {
    if (!wstr) return NULL;

    // use EUC-KR codepage (949)
    int mb_len = WideCharToMultiByte(949, 0, wstr, -1, NULL, 0, NULL, NULL);
    if (mb_len == 0) {
        // fallback to system default codepage
        mb_len = WideCharToMultiByte(CP_ACP, 0, wstr, -1, NULL, 0, NULL, NULL);
        if (mb_len == 0) return NULL;

        char* mb_str = malloc(mb_len);
        if (!mb_str) return NULL;

        WideCharToMultiByte(CP_ACP, 0, wstr, -1, mb_str, mb_len, NULL, NULL);
        return mb_str;
    }

    char* mb_str = malloc(mb_len);
    if (!mb_str) return NULL;

    WideCharToMultiByte(949, 0, wstr, -1, mb_str, mb_len, NULL, NULL);
    return mb_str;
}

/**
 * convert EUC-KR/UTF-8 to UTF-16 for Windows API calls
 */
wchar_t* euc_kr_to_wchar(const char* str) {
    if (!str) return NULL;

    // use EUC-KR codepage (949)
    int wlen = MultiByteToWideChar(949, 0, str, -1, NULL, 0);
    if (wlen == 0) {
        // fallback to system default codepage
        wlen = MultiByteToWideChar(CP_ACP, 0, str, -1, NULL, 0);
        if (wlen == 0) return NULL;

        wchar_t* wstr = malloc(wlen * sizeof(wchar_t));
        if (!wstr) return NULL;

        MultiByteToWideChar(CP_ACP, 0, str, -1, wstr, wlen);
        return wstr;
    }

    wchar_t* wstr = malloc(wlen * sizeof(wchar_t));
    if (!wstr) return NULL;

    MultiByteToWideChar(949, 0, str, -1, wstr, wlen);
    return wstr;
}

/**
 * windows-compatible stat function that handles Korean filenames
 */
int win_stat(const char* path, struct stat* statbuf) {
    wchar_t* wpath = euc_kr_to_wchar(path);
    if (!wpath) return -1;

    struct _stat wstatbuf;
    int result = _wstat(wpath, &wstatbuf);
    free(wpath);

    if (result == 0) {
        // convert _stat to stat structure
        statbuf->st_dev = wstatbuf.st_dev;
        statbuf->st_ino = wstatbuf.st_ino;
        statbuf->st_mode = wstatbuf.st_mode;
        statbuf->st_nlink = wstatbuf.st_nlink;
        statbuf->st_uid = wstatbuf.st_uid;
        statbuf->st_gid = wstatbuf.st_gid;
        statbuf->st_rdev = wstatbuf.st_rdev;
        statbuf->st_size = wstatbuf.st_size;
        statbuf->st_atime = wstatbuf.st_atime;
        statbuf->st_mtime = wstatbuf.st_mtime;
        statbuf->st_ctime = wstatbuf.st_ctime;
    }

    return result;
}

/**
 * windows-compatible fopen that handles Korean filenames
 */
FILE* win_fopen(const char* filename, const char* mode) {
    wchar_t* wfilename = euc_kr_to_wchar(filename);
    wchar_t* wmode = euc_kr_to_wchar(mode);

    if (!wfilename || !wmode) {
        if (wfilename) free(wfilename);
        if (wmode) free(wmode);
        return NULL;
    }

    FILE* fp = _wfopen(wfilename, wmode);
    free(wfilename);
    free(wmode);
    return fp;
}

/**
 * convert EUC-KR string to UTF-8 for console display
 * returns a static buffer - not thread safe, but fine for printf usage
 */
const char* to_utf8(const char* euc_kr_str) {
    static char utf8_buf[1024];
    if (!euc_kr_str) return "";

    // EUC-KR -> UTF-16
    wchar_t* wstr = euc_kr_to_wchar(euc_kr_str);
    if (!wstr) return euc_kr_str;

    // UTF-16 -> UTF-8
    int utf8_len = WideCharToMultiByte(CP_UTF8, 0, wstr, -1, NULL, 0, NULL, NULL);
    if (utf8_len == 0 || utf8_len > sizeof(utf8_buf)) {
        free(wstr);
        return euc_kr_str;
    }

    WideCharToMultiByte(CP_UTF8, 0, wstr, -1, utf8_buf, sizeof(utf8_buf), NULL, NULL);
    free(wstr);
    return utf8_buf;
}
#endif

/**
 * convert backslashes to forward slashes
 */
void normalize_path(char *path) {
    char *p = path;
    while (*p) {
        if (*p == '\\') *p = '/';
        p++;
    }
}

/**
 * determine file type based on extension
 */
int get_file_type(const char *filename) {
    // the original noppack.exe compresses everything with sonorri LZ77
    return NOP_DATA_SONNORI_LZ77;
}

/**
 * SONNORI LZ77 Compression
 */
int compress_sonnori_lz77(unsigned char *input, int input_size, unsigned char *output, int *output_size) {
    // hash table to store positions
    int* hash_table = malloc(0x40000);
    if (!hash_table) return -1;

    // initialize hash table to -1
    for (int i = 0; i < 0x10000; i++) {
        hash_table[i] = -1;
    }

    int out_pos = 0;
    int in_pos = 0;

    while (in_pos < input_size) {
        unsigned char flag_byte = 0;
        int bit_pos = 1;
        unsigned char temp_buffer[20];
        int temp_pos = 1; // start at position 1, position 0 is for flag byte

        // process up to 8 symbols while bit_pos < 0x100
        while (in_pos < input_size && bit_pos < 0x100) {
            if (input_size - in_pos < 2) {
                // less than 2 bytes remaining, store as literal
                temp_buffer[temp_pos++] = input[in_pos++];
            } else {
                // create 16-bit hash key from current 2 bytes
                unsigned short hash_key = (input[in_pos + 1] << 8) | input[in_pos];
                int stored_pos = hash_table[hash_key];

                // check validity
                if (stored_pos == -1 ||
                    (in_pos - stored_pos) > 0xFFF ||
                    (in_pos - stored_pos) < 2) {

                    // update hash table with current position
                    if (input_size - in_pos > 1) {
                        hash_table[hash_key] = in_pos;
                    }

                    // store as literal
                    temp_buffer[temp_pos++] = input[in_pos++];
                } else {
                    // found potential match - calculate length
                    int offset = in_pos - stored_pos;
                    int max_len = input_size - in_pos;
                    int max_copy_len = offset;

                    if (max_len > max_copy_len) max_len = max_copy_len;
                    if (max_len > 0x11) max_len = 0x11; // maximum 17 bytes

                    int match_len = 0;
                    while (match_len < max_len &&
                           input[stored_pos + match_len] == input[in_pos + match_len]) {
                        match_len++;
                    }

                    // valid match check
                    if (match_len > 1 && match_len < 0x12) {
                        flag_byte |= bit_pos; // set bit to indicate match

                        // encode match: (length-2) in upper 4 bits, offset in lower 12 bits
                        unsigned short match_info = ((match_len - 2) & 0xF) * 0x1000 + (offset & 0xFFF);
                        temp_buffer[temp_pos] = match_info & 0xFF;
                        temp_buffer[temp_pos + 1] = (match_info >> 8) & 0xFF;
                        temp_pos += 2;

                        in_pos += match_len;
                    } else {
                        // no valid match, store as literal
                        temp_buffer[temp_pos++] = input[in_pos++];
                    }
                }
            }
            bit_pos <<= 1;
        }

        // XOR flag byte with 0xC8
        temp_buffer[0] = flag_byte ^ 0xC8;

        // apply XOR encryption to matches and copy to output
        bit_pos = 1;
        int read_pos = 1;
        output[out_pos++] = temp_buffer[0]; // write flag byte

        while (read_pos < temp_pos) {
            if ((flag_byte & bit_pos) == 0) {
                // literal byte
                output[out_pos++] = temp_buffer[read_pos++];
            } else {
                // match, apply XOR encryption
                unsigned short match_data = temp_buffer[read_pos] | (temp_buffer[read_pos + 1] << 8);

                // get XOR key index from flag byte
                int key_index = ((flag_byte ^ 0xC8) >> 3) & 0x07;
                match_data ^= lz77_customkey[key_index];

                output[out_pos++] = match_data & 0xFF;
                output[out_pos++] = (match_data >> 8) & 0xFF;
                read_pos += 2;
            }
            bit_pos <<= 1;
        }
    }

    free(hash_table);
    *output_size = out_pos;
    return 0;
}

#ifdef _WIN32
/**
 * add file or directory to the list
 */
int add_entry_wide(const char *relative_path, const wchar_t *full_path_w, struct stat *statbuf) {
    FILE *fp;

    if (file_count >= MAX_FILES) {
        printf("ERROR: Too many files (max %d)\n", MAX_FILES);
        return -1;
    }

    // copy and normalize the path
    files[file_count].name = malloc(strlen(relative_path) + 1);
    strcpy(files[file_count].name, relative_path);
    normalize_path(files[file_count].name);
    files[file_count].name_size = strlen(files[file_count].name);

    if (S_ISDIR(statbuf->st_mode)) {
        // directory entry
        files[file_count].type = NOP_DATA_DIRECTORY;
        files[file_count].offset = 0;
        files[file_count].encoded_size = 0;
        files[file_count].decoded_size = 0x71; // directory key
        files[file_count].data = NULL;
        files[file_count].compressed_data = NULL;
        printf("Added directory: %s\n", DISPLAY(files[file_count].name));
    } else {
        // file entry
        files[file_count].type = get_file_type(files[file_count].name);
        files[file_count].offset = 0; // will be set during writing
        files[file_count].decoded_size = statbuf->st_size;

        // read file data using wide character path
        files[file_count].data = malloc(statbuf->st_size);
        fp = _wfopen(full_path_w, L"rb");
        if (fp) {
            size_t bytes_read = fread(files[file_count].data, 1, statbuf->st_size, fp);
            fclose(fp);
            if (bytes_read != statbuf->st_size) {
                printf("Failed to read all data from: %s\n", DISPLAY(files[file_count].name));
                free(files[file_count].name);
                free(files[file_count].data);
                return -1;
            }

            // compress if needed
            if (files[file_count].type == NOP_DATA_SONNORI_LZ77) {
                files[file_count].compressed_data = malloc(statbuf->st_size * 2);  // extra space for compression
                int compressed_size;

                printf("Compressing %s (%u bytes)...\n", DISPLAY(files[file_count].name), (unsigned int)statbuf->st_size);
                if (compress_sonnori_lz77(files[file_count].data, statbuf->st_size,
                                        files[file_count].compressed_data, &compressed_size) == 0) {
                    files[file_count].encoded_size = compressed_size;
                    printf("Added file: %s (%u->%u bytes, type=%d, compressed)\n",
                           DISPLAY(files[file_count].name), (unsigned int)statbuf->st_size,
                           compressed_size, files[file_count].type);
                } else {
                    // compression failed, use raw
                    free(files[file_count].compressed_data);
                    files[file_count].compressed_data = NULL;
                    files[file_count].encoded_size = statbuf->st_size;
                    files[file_count].type = NOP_DATA_RAW;
                    printf("Added file: %s (%u bytes, type=%d, compression failed->raw)\n",
                           DISPLAY(files[file_count].name), (unsigned int)statbuf->st_size, files[file_count].type);
                }
            } else {
                // raw file - store as-is without masking
                files[file_count].encoded_size = statbuf->st_size;
                files[file_count].compressed_data = NULL;
                printf("Added file: %s (%u bytes, type=%d)\n",
                       DISPLAY(files[file_count].name), (unsigned int)statbuf->st_size, files[file_count].type);
            }
        } else {
            printf("Failed to open file: %s\n", DISPLAY(files[file_count].name));
            free(files[file_count].name);
            free(files[file_count].data);
            return -1;
        }
    }

    file_count++;
    return 0;
}
#endif

/**
 * add file or directory to the list
 */
int add_entry(const char *relative_path, const char *full_path, struct stat *statbuf) {
    FILE *fp;

    if (file_count >= MAX_FILES) {
        printf("ERROR: Too many files (max %d)\n", MAX_FILES);
        return -1;
    }

    // copy and normalize the path
    files[file_count].name = malloc(strlen(relative_path) + 1);
    strcpy(files[file_count].name, relative_path);
    normalize_path(files[file_count].name);
    files[file_count].name_size = strlen(files[file_count].name);

    if (S_ISDIR(statbuf->st_mode)) {
        // directory entry
        files[file_count].type = NOP_DATA_DIRECTORY;
        files[file_count].offset = 0;
        files[file_count].encoded_size = 0;
        files[file_count].decoded_size = 0x71; // directory key
        files[file_count].data = NULL;
        files[file_count].compressed_data = NULL;
        printf("Added directory: %s\n", DISPLAY(files[file_count].name));
    } else {
        // file entry
        files[file_count].type = get_file_type(files[file_count].name);
        files[file_count].offset = 0; // will be set during writing
        files[file_count].decoded_size = statbuf->st_size;

        // read file data
        files[file_count].data = malloc(statbuf->st_size);
#ifdef _WIN32
        fp = win_fopen(full_path, "rb");
#else
        fp = fopen(full_path, "rb");
#endif
        if (fp) {
            size_t bytes_read = fread(files[file_count].data, 1, statbuf->st_size, fp);
            fclose(fp);
            if (bytes_read != statbuf->st_size) {
                printf("Failed to read all data from: %s\n", DISPLAY(files[file_count].name));
                free(files[file_count].name);
                free(files[file_count].data);
                return -1;
            }

            // compress if needed
            if (files[file_count].type == NOP_DATA_SONNORI_LZ77) {
                files[file_count].compressed_data = malloc(statbuf->st_size * 2);  // extra space for compression
                int compressed_size;

                printf("Compressing %s (%u bytes)...\n", DISPLAY(files[file_count].name), (unsigned int)statbuf->st_size);
                if (compress_sonnori_lz77(files[file_count].data, statbuf->st_size,
                                        files[file_count].compressed_data, &compressed_size) == 0) {
                    files[file_count].encoded_size = compressed_size;
                    printf("Added file: %s (%u->%u bytes, type=%d, compressed)\n",
                           DISPLAY(files[file_count].name), (unsigned int)statbuf->st_size,
                           compressed_size, files[file_count].type);
                } else {
                    // compression failed, use raw
                    free(files[file_count].compressed_data);
                    files[file_count].compressed_data = NULL;
                    files[file_count].encoded_size = statbuf->st_size;
                    files[file_count].type = NOP_DATA_RAW;
                    printf("Added file: %s (%u bytes, type=%d, compression failed->raw)\n",
                           DISPLAY(files[file_count].name), (unsigned int)statbuf->st_size, files[file_count].type);
                }
            } else {
                // raw file - store as-is without masking
                files[file_count].encoded_size = statbuf->st_size;
                files[file_count].compressed_data = NULL;
                printf("Added file: %s (%u bytes, type=%d)\n",
                       DISPLAY(files[file_count].name), (unsigned int)statbuf->st_size, files[file_count].type);
            }
        } else {
            printf("Failed to open file: %s\n", DISPLAY(files[file_count].name));
            free(files[file_count].name);
            free(files[file_count].data);
            return -1;
        }
    }

    file_count++;
    return 0;
}

#ifdef _WIN32
/**
 * wide character directory scanning for Korean filenames
 */
int scan_directory_wide(const wchar_t *filesystem_path_w, const char *nop_path) {
    struct stat statbuf;
    char* mb_nop_path;

    // convert wide path to multibyte for stat() call
    char* filesystem_path = wchar_to_euc_kr(filesystem_path_w);
    if (!filesystem_path) return 0;

    // add directory entry for the directory being scanned using wide character stat
    struct _stat wstatbuf;
    if (_wstat(filesystem_path_w, &wstatbuf) == 0 && S_ISDIR(wstatbuf.st_mode)) {
        // convert _stat to stat structure
        statbuf.st_dev = wstatbuf.st_dev;
        statbuf.st_ino = wstatbuf.st_ino;
        statbuf.st_mode = wstatbuf.st_mode;
        statbuf.st_nlink = wstatbuf.st_nlink;
        statbuf.st_uid = wstatbuf.st_uid;
        statbuf.st_gid = wstatbuf.st_gid;
        statbuf.st_rdev = wstatbuf.st_rdev;
        statbuf.st_size = wstatbuf.st_size;
        statbuf.st_atime = wstatbuf.st_atime;
        statbuf.st_mtime = wstatbuf.st_mtime;
        statbuf.st_ctime = wstatbuf.st_ctime;

        if (add_entry(nop_path, filesystem_path, &statbuf) != 0) {
            free(filesystem_path);
            return -1;
        }
    }

    // append \* for FindFirstFile
    size_t wpath_len = wcslen(filesystem_path_w);
    wchar_t* search_path = malloc((wpath_len + 3) * sizeof(wchar_t));
    if (!search_path) {
        free(filesystem_path);
        return -1;
    }
    wcscpy(search_path, filesystem_path_w);
    wcscat(search_path, L"\\*");

    WIN32_FIND_DATAW find_data;
    HANDLE hFind = FindFirstFileW(search_path, &find_data);
    free(search_path);

    if (hFind == INVALID_HANDLE_VALUE) {
        free(filesystem_path);
        return 0;
    }

    do {
        // skip . and ..
        if (wcscmp(find_data.cFileName, L".") == 0 || wcscmp(find_data.cFileName, L"..") == 0) {
            continue;
        }

        // build the wide character full path
        size_t wname_len = wcslen(find_data.cFileName);
        wchar_t* wfull_path = malloc((wpath_len + wname_len + 2) * sizeof(wchar_t));
        if (!wfull_path) continue;

        wcscpy(wfull_path, filesystem_path_w);
        wcscat(wfull_path, L"\\");
        wcscat(wfull_path, find_data.cFileName);

        // convert filename for NOP path
        char* mb_filename = wchar_to_euc_kr(find_data.cFileName);
        if (!mb_filename) {
            free(wfull_path);
            continue;
        }

        // build NOP path
        size_t nop_path_len = strlen(nop_path);
        size_t mb_filename_len = strlen(mb_filename);
        char* entry_nop_path = malloc(nop_path_len + mb_filename_len + 2);
        if (!entry_nop_path) {
            free(wfull_path);
            free(mb_filename);
            continue;
        }
        sprintf(entry_nop_path, "%s/%s", nop_path, mb_filename);

        // use direct wide character stat
        struct _stat wstatbuf;
        if (_wstat(wfull_path, &wstatbuf) == 0) {
            // convert _stat to stat structure
            statbuf.st_dev = wstatbuf.st_dev;
            statbuf.st_ino = wstatbuf.st_ino;
            statbuf.st_mode = wstatbuf.st_mode;
            statbuf.st_nlink = wstatbuf.st_nlink;
            statbuf.st_uid = wstatbuf.st_uid;
            statbuf.st_gid = wstatbuf.st_gid;
            statbuf.st_rdev = wstatbuf.st_rdev;
            statbuf.st_size = wstatbuf.st_size;
            statbuf.st_atime = wstatbuf.st_atime;
            statbuf.st_mtime = wstatbuf.st_mtime;
            statbuf.st_ctime = wstatbuf.st_ctime;

            if (S_ISDIR(statbuf.st_mode)) {
                // recursively scan subdirectory - directory entry will be added by recursive call
                if (scan_directory_wide(wfull_path, entry_nop_path) != 0) {
                    free(wfull_path);
                    free(mb_filename);
                    free(entry_nop_path);
                    FindClose(hFind);
                    free(filesystem_path);
                    return -1;
                }
            } else if (S_ISREG(statbuf.st_mode)) {
                // add regular file using wide character path for Korean support
                if (add_entry_wide(entry_nop_path, wfull_path, &statbuf) != 0) {
                    free(wfull_path);
                    free(mb_filename);
                    free(entry_nop_path);
                    FindClose(hFind);
                    free(filesystem_path);
                    return -1;
                }
            }
        }

        free(wfull_path);
        free(mb_filename);
        free(entry_nop_path);
    } while (FindNextFileW(hFind, &find_data));

    FindClose(hFind);
    free(filesystem_path);
    return 0;
}

/**
 * windows Unicode-aware directory scanning for Korean filenames
 */
int scan_directory_win(const char *filesystem_path, const char *nop_path) {
    struct stat statbuf;
    char entry_filesystem_path[1024];
    char entry_nop_path[1024];

    // add directory entry using Unicode-aware stat
    if (win_stat(filesystem_path, &statbuf) == 0 && S_ISDIR(statbuf.st_mode)) {
        if (add_entry(nop_path, filesystem_path, &statbuf) != 0) {
            return -1;
        }
    }

    // convert path to wide character for Windows API
    wchar_t* wpath = euc_kr_to_wchar(filesystem_path);
    if (!wpath) {
        printf("WARNING: Failed to convert path to Unicode: %s\n", filesystem_path);
        return 0;
    }

    // append \* for FindFirstFile
    size_t wpath_len = wcslen(wpath);
    wchar_t* search_path = malloc((wpath_len + 3) * sizeof(wchar_t));
    if (!search_path) {
        free(wpath);
        return -1;
    }
    wcscpy(search_path, wpath);
    wcscat(search_path, L"\\*");
    free(wpath);

    WIN32_FIND_DATAW find_data;
    HANDLE hFind = FindFirstFileW(search_path, &find_data);
    free(search_path);

    if (hFind == INVALID_HANDLE_VALUE) {
        // directory access failed - this might be due to Korean characters
        // try to continue processing
        return 0;
    }

    do {
        // skip . and ..
        if (wcscmp(find_data.cFileName, L".") == 0 || wcscmp(find_data.cFileName, L"..") == 0) {
            continue;
        }

        // build the wide character path for direct use
        wchar_t* wparent = euc_kr_to_wchar(filesystem_path);
        if (!wparent) continue;

        size_t wparent_len = wcslen(wparent);
        size_t wname_len = wcslen(find_data.cFileName);
        wchar_t* wfull_path = malloc((wparent_len + wname_len + 2) * sizeof(wchar_t));
        if (!wfull_path) {
            free(wparent);
            continue;
        }

        wcscpy(wfull_path, wparent);
        wcscat(wfull_path, L"\\");
        wcscat(wfull_path, find_data.cFileName);
        free(wparent);

        // convert filename for NOP path (try best-effort conversion)
        char* mb_filename = wchar_to_euc_kr(find_data.cFileName);
        if (!mb_filename) {
            free(wfull_path);
            continue;
        }

        // convert wide path back to multibyte for filesystem operations
        char* mb_full_path = wchar_to_euc_kr(wfull_path);
        if (!mb_full_path) {
            free(wfull_path);
            free(mb_filename);
            continue;
        }

        snprintf(entry_nop_path, sizeof(entry_nop_path), "%s/%s", nop_path, mb_filename);

        // use direct wide character stat
        struct _stat wstatbuf;
        if (_wstat(wfull_path, &wstatbuf) == 0) {
            // convert _stat to stat structure
            statbuf.st_dev = wstatbuf.st_dev;
            statbuf.st_ino = wstatbuf.st_ino;
            statbuf.st_mode = wstatbuf.st_mode;
            statbuf.st_nlink = wstatbuf.st_nlink;
            statbuf.st_uid = wstatbuf.st_uid;
            statbuf.st_gid = wstatbuf.st_gid;
            statbuf.st_rdev = wstatbuf.st_rdev;
            statbuf.st_size = wstatbuf.st_size;
            statbuf.st_atime = wstatbuf.st_atime;
            statbuf.st_mtime = wstatbuf.st_mtime;
            statbuf.st_ctime = wstatbuf.st_ctime;

            if (S_ISDIR(statbuf.st_mode)) {
                // recursively scan subdirectory - directory entry will be added by recursive call
                if (scan_directory_wide(wfull_path, entry_nop_path) != 0) {
                    free(wfull_path);
                    free(mb_filename);
                    free(mb_full_path);
                    FindClose(hFind);
                    return -1;
                }
            } else if (S_ISREG(statbuf.st_mode)) {
                // add regular file using wide character path for Korean support
                if (add_entry_wide(entry_nop_path, wfull_path, &statbuf) != 0) {
                    free(wfull_path);
                    free(mb_filename);
                    free(mb_full_path);
                    FindClose(hFind);
                    return -1;
                }
            }
        }

        free(wfull_path);
        free(mb_filename);
        free(mb_full_path);
    } while (FindNextFileW(hFind, &find_data));

    FindClose(hFind);
    return 0;
}
#endif

/**
 * recursively scan directory
 */
int scan_directory(const char *filesystem_path, const char *nop_path) {
#ifdef _WIN32
    // convert to wide characters and use Unicode-aware scanning
    wchar_t* wpath = euc_kr_to_wchar(filesystem_path);
    if (wpath) {
        int result = scan_directory_wide(wpath, nop_path);
        free(wpath);
        return result;
    } else {
        // fallback to multibyte version
        return scan_directory_win(filesystem_path, nop_path);
    }
#else
    DIR *dir;
    struct dirent *entry;
    struct stat statbuf;
    char entry_filesystem_path[1024];
    char entry_nop_path[1024];

    // Add directory entry
    if (stat(filesystem_path, &statbuf) == 0 && S_ISDIR(statbuf.st_mode)) {
        if (add_entry(nop_path, filesystem_path, &statbuf) != 0) {
            return -1;
        }
    }

    dir = opendir(filesystem_path);
    if (!dir) {
        printf("WARNING: Failed to open directory: %s\n", filesystem_path);
        return 0; // continue processing other directories
    }

    while ((entry = readdir(dir)) != NULL) {
        if (strcmp(entry->d_name, ".") == 0 || strcmp(entry->d_name, "..") == 0) {
            continue;
        }

        snprintf(entry_filesystem_path, sizeof(entry_filesystem_path), "%s/%s", filesystem_path, entry->d_name);
        snprintf(entry_nop_path, sizeof(entry_nop_path), "%s/%s", nop_path, entry->d_name);

        if (stat(entry_filesystem_path, &statbuf) == 0) {
            if (S_ISDIR(statbuf.st_mode)) {
                // recursively scan subdirectory
                if (scan_directory(entry_filesystem_path, entry_nop_path) != 0) {
                    closedir(dir);
                    return -1;
                }
            } else if (S_ISREG(statbuf.st_mode)) {
                // add regular file
                if (add_entry(entry_nop_path, entry_filesystem_path, &statbuf) != 0) {
                    closedir(dir);
                    return -1;
                }
            }
        }
    }

    closedir(dir);
    return 0;
#endif
}

/**
 * write NOP file
 */
int write_nop_file(const char *output_filename) {
    FILE *fp;
    unsigned int current_offset = 0;
    unsigned int file_table_offset;
    unsigned char key = 0x71;
    unsigned char magic = 0x12;
    int i, j;

    fp = fopen(output_filename, "wb");
    if (!fp) {
        printf("Failed to create output file: %s\n", output_filename);
        return -1;
    }

    printf("Writing %d entries to %s...\n", file_count, output_filename);

    // write initial separator marker
    unsigned char initial_separator = 0xAB;
    fwrite(&initial_separator, 1, 1, fp);
    current_offset = 1;

    // write file data first (only for non-directories)
    for (i = 0; i < file_count; i++) {
        if (files[i].type == NOP_DATA_DIRECTORY) {
            files[i].offset = 0;  // directories have no file data
        } else {
            files[i].offset = current_offset;
            unsigned char *data_to_write;
            size_t size_to_write;

            if (files[i].compressed_data) {
                data_to_write = files[i].compressed_data;
                size_to_write = files[i].encoded_size;
            } else {
                data_to_write = files[i].data;
                size_to_write = files[i].encoded_size;
            }

            if (data_to_write) {
                size_t bytes_written = fwrite(data_to_write, 1, size_to_write, fp);
                if (bytes_written != size_to_write) {
                    printf("Failed to write data for: %s\n", DISPLAY(files[i].name));
                    fclose(fp);
                    return -1;
                }
                current_offset += size_to_write;

                // add separator marker after files (except the last file with data)
                // count remaining files with data
                int remaining_files_with_data = 0;
                for (int j = i + 1; j < file_count; j++) {
                    if (files[j].type != NOP_DATA_DIRECTORY && files[j].data) {
                        remaining_files_with_data++;
                    }
                }

                if (remaining_files_with_data > 0) {
                    unsigned char separator = 0xAB;
                    fwrite(&separator, 1, 1, fp);
                    current_offset += 1;
                }
            }
        }
    }

    // remember file table position
    file_table_offset = current_offset;

    // write file table
    for (i = 0; i < file_count; i++) {
        // write name_size
        fwrite(&files[i].name_size, 1, 1, fp);

        // write type
        fwrite(&files[i].type, 1, 1, fp);

        // write offset
        fwrite(&files[i].offset, 4, 1, fp);

        // write encoded_size
        fwrite(&files[i].encoded_size, 4, 1, fp);

        // write decoded_size (XORed with key for non-directories)
        unsigned int xored_size;
        if (files[i].type == NOP_DATA_DIRECTORY) {
            xored_size = files[i].decoded_size; // key value (0x71) for directories
        } else {
            xored_size = files[i].decoded_size ^ key;
        }
        fwrite(&xored_size, 4, 1, fp);

        // write encrypted filename
        for (j = 0; j < files[i].name_size; j++) {
            unsigned char encrypted_char = files[i].name[j] ^ key;
            fwrite(&encrypted_char, 1, 1, fp);
        }

        // write null terminator (unencrypted)
        unsigned char null_terminator = 0;
        fwrite(&null_terminator, 1, 1, fp);
    }

    // write footer
    fwrite(&file_table_offset, 4, 1, fp); // offset to file table
    fwrite(&file_count, 4, 1, fp);        // number of files
    fwrite(&magic, 1, 1, fp);             // magic byte 0x12

    fclose(fp);
    printf("Successfully created %s with %d entries\n", output_filename, file_count);
    return 0;
}

/**
 * cleanup allocated memory
 */
void cleanup() {
    int i;
    for (i = 0; i < file_count; i++) {
        if (files[i].name) free(files[i].name);
        if (files[i].data) free(files[i].data);
        if (files[i].compressed_data) free(files[i].compressed_data);
    }
    if (buff1) free(buff1);
    if (buff2) free(buff2);
}

int main(int argc, char *argv[]) {
    int i;

    if (argc < 2) {
        printf("Usage: %s <directory1> [directory2] ...\n", argv[0]);
        printf("Creates whiteday000.nop from the specified directories\n");
        printf("Example: %s data script\n", argv[0]);
        return 1;
    }

    // set locale for proper Korean character handling
#ifdef _WIN32
    // set console to UTF-8 for proper display in logs
    SetConsoleOutputCP(CP_UTF8);
    SetConsoleCP(CP_UTF8);

    // set the thread locale to Korean so CP_ACP maps to Korean (needed for EUC-KR file handling)
    SetThreadLocale(MAKELCID(MAKELANGID(LANG_KOREAN, SUBLANG_KOREAN), SORT_DEFAULT));
    setlocale(LC_ALL, "Korean");
#else
    setlocale(LC_ALL, "");
#endif

    // allocate buffers
    buff1 = malloc(BUFFER_SIZE);
    buff2 = malloc(BUFFER_SIZE);
    if (!buff1 || !buff2) {
        printf("Failed to allocate memory buffers\n");
        cleanup();
        return 1;
    }

    printf("NOPPack (Compressed) - White Day Asset Packer\n");
    printf("Processing %d directories...\n", argc - 1);

    // process each directory argument
    for (i = 1; i < argc; i++) {
        struct stat statbuf;

        printf("Scanning directory: %s\n", argv[i]);

        // check if directory exists
#ifdef _WIN32
        if (win_stat(argv[i], &statbuf) != 0 || !S_ISDIR(statbuf.st_mode)) {
#else
        if (stat(argv[i], &statbuf) != 0 || !S_ISDIR(statbuf.st_mode)) {
#endif
            printf("Error: %s is not a valid directory\n", argv[i]);
            cleanup();
            return 1;
        }

        // get the directory name for relative paths
        char *dir_name = strrchr(argv[i], '/');
        if (!dir_name) dir_name = strrchr(argv[i], '\\');
        if (!dir_name) dir_name = argv[i];
        else dir_name++; // skip the separator

        // scan the directory using the directory name as the base path in NOP
        if (scan_directory(argv[i], dir_name) != 0) {
            cleanup();
            return 1;
        }
    }

    if (file_count == 0) {
        printf("No files found in directories\n");
        cleanup();
        return 1;
    }

    // write NOP file
    if (write_nop_file("whiteday000.nop") != 0) {
        cleanup();
        return 1;
    }

    cleanup();
    printf("Packing complete!\n");
    return 0;
}
