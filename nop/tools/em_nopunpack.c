#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <sys/stat.h>
#include <direct.h>
#include <locale.h>

#ifdef _WIN32
#include <windows.h>
#include <wchar.h>
#include <io.h>
#include <fcntl.h>
#endif

#define BUFFER_SIZE (1024 * 1024 * 256)

/**
 * BIG-SIZE buffer
 **/
unsigned char *buff1;
unsigned char *buff2;

/**
 * SONNORI Lz77 Xor Key
 **/
unsigned short lz77_customkey[] = {
	0xFF21, 0x834F, 0x675F, 0x0034, 0xF237, 0x815F, 0x4765, 0x0233
};

#ifdef _WIN32
/**
 * Convert UTF-16 to multibyte for Korean filename handling
 */
char* wchar_to_multibyte(const wchar_t* wstr) {
    if (!wstr) return NULL;

    // Use EUC-KR codepage (949) explicitly to match LEProc Korean locale
    int mb_len = WideCharToMultiByte(949, 0, wstr, -1, NULL, 0, NULL, NULL);
    if (mb_len == 0) {
        // Fallback to system default codepage if EUC-KR fails
        mb_len = WideCharToMultiByte(CP_ACP, 0, wstr, -1, NULL, 0, NULL, NULL);
        if (mb_len == 0) return NULL;

        char* mb_str = malloc(mb_len);
        if (!mb_str) return NULL;

        WideCharToMultiByte(CP_ACP, 0, wstr, -1, mb_str, mb_len, NULL, NULL);
        return mb_str;
    }

    char* mb_str = malloc(mb_len);
    if (!mb_str) return NULL;

    WideCharToMultiByte(CP_ACP, 0, wstr, -1, mb_str, mb_len, NULL, NULL);
    return mb_str;
}

/**
 * Convert multibyte to UTF-16 for Windows API calls
 */
wchar_t* multibyte_to_wchar(const char* str) {
    if (!str) return NULL;

    // Use EUC-KR codepage (949) first to match LEProc Korean locale
    int wlen = MultiByteToWideChar(949, 0, str, -1, NULL, 0);
    if (wlen == 0) {
        // Fallback to system default codepage if EUC-KR fails
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
 * Windows-compatible fopen that handles Korean filenames
 */
FILE* win_fopen(const char* filename, const char* mode) {
    wchar_t* wfilename = multibyte_to_wchar(filename);
    wchar_t* wmode = multibyte_to_wchar(mode);

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
 * Windows-compatible mkdir that handles Korean directory names
 */
int win_mkdir(const char* dirname) {
    wchar_t* wdirname = multibyte_to_wchar(dirname);
    if (!wdirname) return -1;

    int result = _wmkdir(wdirname);
    free(wdirname);
    return result;
}
#endif

//
// NOP file unpack for testing
//
void nop_unpack(const char *nopFile, const char *outputDir)
{
	enum e_noptype {
		NOP_DATA_RAW = 0x00,
		NOP_DATA_LZ77 = 0x01,
		NOP_DATA_DIRECTORY = 0x02,
		NOP_DATA_SONNORI_LZ77 = 0x03
	};
	FILE *fp, *out_fp;
	int off, num, i, j;
	unsigned char key;
	char full_path[512];

#ifdef _WIN32
	fp = win_fopen(nopFile, "rb");
#else
	fp = fopen(nopFile, "rb");
#endif
	if (!fp) {
		printf("Failed to open: %s\n", nopFile);
		return;
	}

	/* the last byte value of NOP file must be 0x12 */
	fseek(fp, -1, SEEK_END);
	if (fgetc(fp) != 0x12) {
		printf("* %-15s : Invalid NOP file (missing magic byte).\n", nopFile);
		fclose(fp);
		return;
	}

	/* NOP file header */
	fseek(fp, -9, SEEK_END);
	fread(&off, 4, 1, fp);
	fread(&num, 4, 1, fp);
	printf("* %-15s : Contains %d entries.\n", nopFile, num);

	for (i = 0; i < num; ++i) {
		unsigned char name[256];
		unsigned char name_size;
		unsigned char type;
		int offset;
		int encode_size;
		int decode_size;

		/* File information */
		fseek(fp, off, SEEK_SET);
		fread(&name_size, 1, 1, fp);
		fread(&type, 1, 1, fp);
		fread(&offset, 4, 1, fp);
		fread(&encode_size, 4, 1, fp);
		fread(&decode_size, 4, 1, fp);
		fread(name, 1, name_size + 1, fp);
		off += name_size + 15;

		/* Directory handling */
		if (type == NOP_DATA_DIRECTORY) {
			key = (char)decode_size;
		}
		else {
			decode_size ^= key;
		}

		/* File path decrypt */
		for (j = 0; j < name_size; ++j)
			name[j] ^= key;

		/* Skip if too large */
		if (decode_size > BUFFER_SIZE) {
			printf(" -> SKIP: %s (too large)\n", name);
			continue;
		}

		/* Build output path */
		snprintf(full_path, sizeof(full_path), "%s/%s", outputDir, name);

		/* Progress */
		printf("* %3d/%d : %-60s", i+1, num, name);

		switch (type) {
		case NOP_DATA_RAW:
		{
			fseek(fp, offset, SEEK_SET);
			fread(buff1, 1, encode_size, fp);
#ifdef _WIN32
			out_fp = win_fopen(full_path, "wb");
#else
			out_fp = fopen(full_path, "wb");
#endif
			if (!out_fp) {
				printf(" -> FAIL: Cannot create file\n");
				break;
			}
			fwrite(buff1, 1, decode_size, out_fp);
			fclose(out_fp);
			printf(" -> OK (RAW)\n");
			break;
		}
		case NOP_DATA_LZ77:
		{
			int bmask, bcnt = 0, size = 0, off_lz, len;
			unsigned short Lz77Info;
			fseek(fp, offset, SEEK_SET);
			fread(buff1, 1, encode_size, fp);
			/* Lz77 uncompress */
			for (j = 0; j < encode_size; bcnt = (bcnt + 1) & 0x07)
			{
				if (!bcnt) {
					bmask = buff1[j++];
				}
				else {
					bmask >>= 1;
				}
				if (bmask & 0x01) {
					Lz77Info = *(unsigned short *)&buff1[j], j += 2;
					off_lz = Lz77Info & 0x0FFF;
					len = (Lz77Info >> 12) + 2;
					memcpy(&buff2[size], &buff2[size - off_lz], len);
					size += len;
				}
				else {
					buff2[size++] = buff1[j++];
				}
			}
			if (size != decode_size) {
				printf(" -> FAIL: Size mismatch %d != %d (LZ77)\n", size, decode_size);
				break;
			}
#ifdef _WIN32
			out_fp = win_fopen(full_path, "wb");
#else
			out_fp = fopen(full_path, "wb");
#endif
			if (!out_fp) {
				printf(" -> FAIL: Cannot create file\n");
				break;
			}
			fwrite(buff2, 1, decode_size, out_fp);
			fclose(out_fp);
			printf(" -> OK (LZ77)\n");
			break;
		}
		case NOP_DATA_DIRECTORY:
		{
#ifdef _WIN32
			win_mkdir(full_path);
#else
			_mkdir(full_path);
#endif
			printf(" -> OK (DIR)\n");
			break;
		}
		case NOP_DATA_SONNORI_LZ77:
		{
			int bmask, bsrcmask, bcnt = 0, size = 0, off_lz, len;
			unsigned short Lz77Info;
			fseek(fp, offset, SEEK_SET);
			fread(buff1, 1, encode_size, fp);
			/* SONNORI Lz77 uncompress */
				for (j = 0; j < encode_size; bcnt = (bcnt + 1) & 0x07)
			{
				if (!bcnt) {
					bmask = bsrcmask = buff1[j++];
					bmask ^= 0xC8;
				}
				else {
					bmask >>= 1;
				}
				if (bmask & 0x01) {
					if (j + 1 >= encode_size) {
						break;
					}
					Lz77Info = *(unsigned short *)&buff1[j], j += 2;
					Lz77Info ^= lz77_customkey[(bsrcmask >> 3) & 0x07];
					off_lz = Lz77Info & 0x0FFF;
					len = (Lz77Info >> 12) + 2;
					if (off_lz > size) {
						break;
					}
					memcpy(&buff2[size], &buff2[size - off_lz], len);
					size += len;
				}
				else {
					if (j >= encode_size) {
						break;
					}
					buff2[size++] = buff1[j++];
				}
				if (size > decode_size) {
					break;
				}
			}
			if (size != decode_size) {
				printf(" -> FAIL: Size mismatch %d != %d (SONNORI)\n", size, decode_size);
				break;
			}
#ifdef _WIN32
			out_fp = win_fopen(full_path, "wb");
#else
			out_fp = fopen(full_path, "wb");
#endif
			if (!out_fp) {
				printf(" -> FAIL: Cannot create file\n");
				break;
			}
			fwrite(buff2, 1, decode_size, out_fp);
			fclose(out_fp);
			printf(" -> OK (SONNORI)\n");
			break;
		}
		default: // unknown type
			printf(" -> FAIL: Unknown type %d\n", type);
			break;
		}
	}
	fclose(fp);
	return;
}

int main(int argc, char *argv[])
{
	if (argc != 3) {
		printf("Usage: %s <nop_file> <output_directory>\n", argv[0]);
		printf("Example: %s test.nop test_output\n", argv[0]);
		return 1;
	}

	// Set locale for proper Korean character handling
#ifdef _WIN32
	// Set console codepage to handle Korean characters
	SetConsoleOutputCP(949); // EUC-KR codepage
	SetConsoleCP(949);
	setlocale(LC_ALL, "Korean");
#else
	setlocale(LC_ALL, "");
#endif

	// Create output directory
#ifdef _WIN32
	win_mkdir(argv[2]);
#else
	_mkdir(argv[2]);
#endif


	/* Allocate buffers */
	if (!(buff1 = (unsigned char *)malloc(BUFFER_SIZE)) || !(buff2 = (unsigned char *)malloc(BUFFER_SIZE))) {
		printf("Memory allocation failed.\n");
		return 1;
	}

	printf("NOPUnpack Test - Unpacking %s to %s\n", argv[1], argv[2]);
	nop_unpack(argv[1], argv[2]);

	/* Cleanup */
	free(buff2);
	free(buff1);
	printf("Unpacking complete.\n");
	return 0;
}
