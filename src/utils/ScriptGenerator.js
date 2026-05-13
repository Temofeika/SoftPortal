export const generatePowerShellScript = (selectedApps) => {
  if (selectedApps.length === 0) return '';

  const header = `# Ninite-like Installation Script
# Generated on ${new Date().toLocaleString()}
# Run this script as Administrator to install selected applications.

Write-Host "Starting installation of ${selectedApps.length} applications..." -ForegroundColor Cyan

# Ensure winget is available
if (!(Get-Command winget -ErrorAction SilentlyContinue)) {
    Write-Error "Winget not found. Please install Windows Package Manager."
    exit
}

$apps = @(
${selectedApps.map(app => `    "${app.id}"`).join(',\n')}
)

foreach ($appId in $apps) {
    Write-Host "Installing $appId..." -ForegroundColor Yellow
    winget install --id $appId --silent --accept-package-agreements --accept-source-agreements
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Successfully installed $appId" -ForegroundColor Green
    } else {
        Write-Host "Failed to install $appId (Error code: $LASTEXITCODE)" -ForegroundColor Red
    }
}

Write-Host "Installation process completed!" -ForegroundColor Cyan
Pause
`;

  return header;
};
