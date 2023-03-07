param (
    [Parameter(Mandatory=$true)]
    [int]$length,
    
    [Parameter(Mandatory=$true)]
    [int]$count,

    [string]$output
)

$charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`!@#`$%^&*()-_=+[]{}\\<|>;:`"',.?/" 

$passwords = for ($passwordIndex = 1; $passwordIndex -le $count; $passwordIndex++) {
    $localGenerator = New-Object System.Security.Cryptography.RNGCryptoServiceProvider  # Generate a new random number generator using local entropy for each password
    $randomBytes = New-Object byte[] ($length * 4) # Generate a new byte array to store the random bytes for each password
    $localGenerator.GetBytes($randomBytes) # Fill the byte array with random bytes from the local entropy generator for each password
    $password = New-Object System.Text.StringBuilder # Initialize a StringBuilder to store the password for each password
    
    # Loop through the byte array and select characters from the character set string for each password
    for ($charIndex = 0; $charIndex -lt $length; $charIndex++) {
        $index = [System.BitConverter]::ToInt32($randomBytes, $charIndex * 4) % $charset.Length
        [void]$password.Append($charset[$index])
    }
    $passwords += $password.ToString();
    $password.ToString() | Out-Host
}


if ($output) {
    $output = Join-Path (Get-Location) $output
    if (-not (Test-Path $output)) {
        New-Item -Path $output -ItemType File | Out-Null
    }
    $passwords | Set-Content $output
    Write-Host "Passwords saved to $output"
}