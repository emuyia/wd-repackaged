param([string]$FilePath)

$textExtensions = @('.scp', '.txt', '.ini', '.wdb', '.xml', '.json', '.md')

$codeCmd = $null
if (Get-Command 'code-insiders' -ErrorAction SilentlyContinue) {
    $codeCmd = 'code-insiders'
} elseif (Get-Command 'code' -ErrorAction SilentlyContinue) {
    $codeCmd = 'code'
} else {
    Write-Host "Neither 'code' nor 'code-insiders' found in PATH" -ForegroundColor Red
    exit 1
}

$absPath = Resolve-Path $FilePath -ErrorAction Stop
$repoRoot = (Get-Item "$PSScriptRoot/../..").FullName

# Extract relative path from nop folder
$relPath = $absPath.Path.Substring($repoRoot.Length + 1)  # e.g. "nop\whiteday120\script\file.scp"

# Parse NOP folder and map to official_assets
if ($relPath -match '^nop\\(whiteday\d+)\\(.+)$') {
    $originalBase = "nop\official_assets\whiteday"
    $subPath = $Matches[2]
} elseif ($relPath -match '^nop\\(mod_beanbag\d+)\\(.+)$') {
    $originalBase = "nop\official_assets\mod_beanbag"
    $subPath = $Matches[2]
} else {
    Write-Host "File is not in a recognized NOP folder" -ForegroundColor Red
    exit 1
}

$originalPath = Join-Path $repoRoot "$originalBase\$subPath"

# Check if original exists
if (-not (Test-Path $originalPath)) {
    Write-Host "NEW FILE - No original exists at: $originalPath" -ForegroundColor Yellow
    exit 0
}

# Determine text or binary
$ext = [System.IO.Path]::GetExtension($absPath.Path).ToLower()
if ($textExtensions -contains $ext) {
    # Open VS Code diff
    & $codeCmd --diff $originalPath $absPath.Path
} else {
    # Open both files in default app
    Start-Process $originalPath
    Start-Process $absPath.Path
}
