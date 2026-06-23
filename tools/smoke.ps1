# smoke.ps1 — autonomous "Load with Mods" smoke test via godot.log parsing.
# Launches StS2 windowed, waits for the mod loader to finish, checks whether our mod
# initialized (or errored), then closes the game. No bridge / no manual clicking.
#
# Usage: pwsh -File tools/smoke.ps1 [-ModId DundunDudu] [-TimeoutSec 120]
param(
  [string]$Game = "D:\SteamLibrary\steamapps\common\Slay the Spire 2",
  [string]$ModId = "DundunDudu",
  [int]$TimeoutSec = 120
)
$ErrorActionPreference = "Continue"
$ud  = "$env:APPDATA\SlayTheSpire2"
$logPath = "$ud\logs\godot.log"
$exe = "$Game\SlayTheSpire2.exe"

# Safety: never kill a game the user is actively playing.
if (Get-Process SlayTheSpire2 -ErrorAction SilentlyContinue) {
  Write-Output "ABORT: SlayTheSpire2 is already running — skipping smoke test to avoid disrupting your session."
  exit 2
}

# Standalone Steam init: the exe needs an appID when not launched through Steam's UI.
# This is the fix the game's own error message recommends; reversible (delete the file).
$appidFile = "$Game\steam_appid.txt"
if (-not (Test-Path $appidFile)) {
  Set-Content -Path $appidFile -Value "2868840" -NoNewline -Encoding ascii
  Write-Output "Wrote steam_appid.txt (2868840) for standalone Steam init."
}

Write-Output "Launching (windowed): $exe"
$proc = Start-Process -FilePath $exe -ArgumentList @("--rendering-driver","d3d12","--windowed","--resolution","1280x720") -PassThru
$deadline = (Get-Date).AddSeconds($TimeoutSec)
$done = $false
while ((Get-Date) -lt $deadline) {
  Start-Sleep -Seconds 5
  if (Test-Path $logPath) {
    $txt = Get-Content -Raw $logPath -ErrorAction SilentlyContinue
    if ($txt -match "Finished mod initialization for 'BaseLib'") {
      # loader has run BaseLib; give it a moment, then consider done if our mod resolved or errored
      if ($txt -match [regex]::Escape("($ModId).") -or ($txt -match $ModId -and $txt -match "\[ERROR\]")) { $done = $true; break }
      # else keep waiting a little for the local-mods pass / main menu
      if ($txt -match "Main ?Menu|MAIN_MENU|reached main menu") { $done = $true; break }
    }
  }
}
Start-Sleep -Seconds 4
$loaded   = $false
$ourLines = @(); $finished = @(); $errors = @()
if (Test-Path $logPath) {
  $raw = Get-Content -Raw $logPath -ErrorAction SilentlyContinue
  $loaded = $raw -match [regex]::Escape("($ModId).")
  $ourLines = @(Select-String -Path $logPath -Pattern $ModId -ErrorAction SilentlyContinue | ForEach-Object { $_.Line })
  $finished = @(Select-String -Path $logPath -Pattern 'Finished mod initialization' -ErrorAction SilentlyContinue | ForEach-Object { $_.Line })
  $errors   = @(Select-String -Path $logPath -Pattern '\[ERROR\]|Unhandled exception|failed to load' -ErrorAction SilentlyContinue | Select-Object -First 15 | ForEach-Object { $_.Line })
}

# Close the game.
Get-Process SlayTheSpire2 -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue

Write-Output ""
Write-Output "==================== SMOKE VERDICT ===================="
Write-Output ("ModId            : {0}" -f $ModId)
Write-Output ("Reached load-done: {0}" -f $done)
Write-Output ("Our mod inited   : {0}" -f $loaded)
Write-Output "---- our-mod log lines ----"
if ($ourLines.Count) { $ourLines } else { Write-Output "(none — our mod was not seen by the loader)" }
Write-Output "---- all finished inits ----"
$finished
Write-Output "---- errors ----"
if ($errors.Count) { $errors } else { Write-Output "(none)" }
Write-Output "======================================================="
