# Project Starter PowerShell Script
# This script helps developers quickly set up new projects with various frameworks

function Check-Prerequisites {
    # Check if Node.js is installed
    try {
        $nodeVersion = node -v
        Write-Host "Node.js is installed: $nodeVersion" -ForegroundColor Green
    }
    catch {
        Write-Host "Node.js is not installed!" -ForegroundColor Red
        Write-Host "Please install Node.js from https://nodejs.org/" -ForegroundColor Yellow
        exit
    }

    # Check if npm is installed
    try {
        $npmVersion = npm -v
        Write-Host "npm is installed: $npmVersion" -ForegroundColor Green
    }
    catch {
        Write-Host "npm is not installed!" -ForegroundColor Red
        Write-Host "Please install npm (it usually comes with Node.js)" -ForegroundColor Yellow
        exit
    }
}

function Show-Banner {
    Write-Host "===============================================" -ForegroundColor Cyan
    Write-Host "               PROJECT STARTER                 " -ForegroundColor Cyan
    Write-Host "===============================================" -ForegroundColor Cyan
    Write-Host "Get your project up and running in seconds!" -ForegroundColor Yellow
    Write-Host ""
}

function Get-ProjectName {
    $projectName = Read-Host "Enter your project name"
    # Replace spaces with hyphens and convert to lowercase
    $sanitizedProjectName = $projectName -replace '\s+', '-' | ForEach-Object { $_.ToLower() }
    return $sanitizedProjectName
}

function Get-Template {
    Write-Host "Available templates:" -ForegroundColor Green
    
    $templates = @(
        "React",
        "React + Vite",
        "Vite (Vanilla)",
        "Svelte",
        "SvelteKit",
        "Vue",
        "Next.js",
        "Three.js (using Vite)",
        "Express",
        "Angular",
        "Tailwind Standalone"
    )
    
    for ($i = 0; $i -lt $templates.Count; $i++) {
        Write-Host "$($i+1). $($templates[$i])"
    }
    
    $choice = Read-Host "`nSelect template (1-$($templates.Count))"
    $index = [int]$choice - 1
    
    if ($index -lt 0 -or $index -ge $templates.Count) {
        Write-Host "Invalid selection. Exiting." -ForegroundColor Red
        exit
    }
    
    return $templates[$index]
}

function Get-Enhancements {
    Write-Host "`nAvailable enhancements (optional):" -ForegroundColor Green
    
    $enhancements = @(
        "Tailwind CSS",
        "ESLint + Prettier",
        "Sass",
        "Playwright (E2E testing)",
        "Vitest (Unit testing)",
        "TypeScript"
    )
    
    for ($i = 0; $i -lt $enhancements.Count; $i++) {
        Write-Host "$($i+1). $($enhancements[$i])"
    }
    
    $choices = Read-Host "`nSelect enhancements (comma-separated numbers, or press Enter for none)"
    
    if ([string]::IsNullOrWhiteSpace($choices)) {
        return @()
    }
    
    $selectedEnhancements = @()
    $indices = $choices -split ',' | ForEach-Object { [int]$_.Trim() - 1 }
    
    foreach ($idx in $indices) {
        if ($idx -ge 0 -and $idx -lt $enhancements.Count) {
            $selectedEnhancements += $enhancements[$idx]
        }
    }
    
    return $selectedEnhancements
}

function Setup-Project {
    param (
        [string]$projectName,
        [string]$template,
        [array]$enhancements
    )
    
    # Display current directory information
    $currentLocation = Get-Location
    
    Write-Host "`nProject configuration:" -ForegroundColor Cyan
    Write-Host "- Location: $currentLocation" -ForegroundColor Cyan
    Write-Host "- Name: $projectName" -ForegroundColor Cyan
    Write-Host "- Template: $template" -ForegroundColor Cyan
    if ($enhancements.Count -gt 0) {
        Write-Host "- Enhancements: $($enhancements -join ', ')" -ForegroundColor Cyan
    }
    else {
        Write-Host "- Enhancements: None" -ForegroundColor Cyan
    }
    
    $confirm = Read-Host "`nProceed with setup? (y/n)"
    if ($confirm.ToLower() -ne "y") {
        Write-Host "Setup cancelled. Exiting." -ForegroundColor Yellow
        exit
    }
    
    # Change to current directory
    $currentDir = Get-Location
    
    # Create project based on template
    Write-Host "`nSetting up project..." -ForegroundColor Green
    
    # Store the start command based on template
    $startCommand = "npm run dev" # Default for most modern frameworks
    
    switch ($template) {
        "React" {
            Write-Host "Running: npx create-react-app $projectName" -ForegroundColor Yellow
            npx create-react-app $projectName
            $startCommand = "npm start"
        }
        "React + Vite" {
            Write-Host "Running: npm create vite@latest $projectName -- --template react" -ForegroundColor Yellow
            npm create vite@latest $projectName -- --template react
            Set-Location "$currentDir\$projectName"
            npm install
        }
        "Vite (Vanilla)" {
            Write-Host "Running: npm create vite@latest $projectName -- --template vanilla" -ForegroundColor Yellow
            npm create vite@latest $projectName -- --template vanilla
            Set-Location "$currentDir\$projectName"
            npm install
        }
        "Svelte" {
            Write-Host "Running: npx degit sveltejs/template $projectName" -ForegroundColor Yellow
            npx degit sveltejs/template $projectName
            Set-Location "$currentDir\$projectName"
            npm install
        }
        "SvelteKit" {
            Write-Host "Running: npm create svelte@latest $projectName" -ForegroundColor Yellow
            npm create svelte@latest $projectName
            Set-Location "$currentDir\$projectName"
            npm install
        }
        "Vue" {
            Write-Host "Running: npm create vue@latest $projectName" -ForegroundColor Yellow
            npm create vue@latest $projectName
            Set-Location "$currentDir\$projectName"
            npm install
        }
        "Next.js" {
            Write-Host "Running: npx create-next-app@latest $projectName" -ForegroundColor Yellow
            npx create-next-app@latest $projectName
            $startCommand = "npm run dev"
        }
        "Three.js (using Vite)" {
            Write-Host "Running: npm create vite@latest $projectName -- --template vanilla" -ForegroundColor Yellow
            npm create vite@latest $projectName -- --template vanilla
            Set-Location "$currentDir\$projectName"
            npm install
            npm install three
        }
        "Express" {
            Write-Host "Running: npx express-generator $projectName" -ForegroundColor Yellow
            npx express-generator $projectName
            Set-Location "$currentDir\$projectName"
            npm install
            $startCommand = "npm start"
        }
        "Angular" {
            Write-Host "Running: npx @angular/cli new $projectName" -ForegroundColor Yellow
            npx @angular/cli new $projectName
            $startCommand = "ng serve"
        }
        "Tailwind Standalone" {
            Write-Host "Creating a basic HTML project with Tailwind CSS..." -ForegroundColor Yellow
            New-Item -Path $projectName -ItemType Directory
            Set-Location "$currentDir\$projectName"
            
            # Create index.html with Tailwind CDN
            $htmlContent = @"
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>$projectName</title>
    <script src="https://cdn.tailwindcss.com"></script>
</head>
<body class="bg-gray-100 min-h-screen">
    <div class="container mx-auto px-4 py-8">
        <h1 class="text-3xl font-bold text-center mb-8">Welcome to $projectName</h1>
        
        <div class="bg-white p-6 rounded-lg shadow-md">
            <p class="text-gray-700">This is a starter template with Tailwind CSS.</p>
        </div>
    </div>
</body>
</html>
"@
            Set-Content -Path "index.html" -Value $htmlContent
            
            # Create a simple JavaScript file
            $jsContent = @"
// Your JavaScript code here
console.log('$projectName is ready!');
"@
            New-Item -Path "js" -ItemType Directory
            Set-Content -Path "js/main.js" -Value $jsContent
            
            # For Tailwind standalone, we'll use a simple HTTP server
            $startCommand = "Start-Process 'http://localhost:8000' ; python -m http.server"
        }
    }
    
    # Apply enhancements if any
    if ($enhancements.Count -gt 0) {
        # Make sure we're in the project directory
        if ($template -ne "Tailwind Standalone") {
            Set-Location "$currentDir\$projectName"
        }
        
        Write-Host "`nApplying enhancements..." -ForegroundColor Green
        
        foreach ($enhancement in $enhancements) {
            switch ($enhancement) {
                "Tailwind CSS" {
                    Write-Host "Installing Tailwind CSS..." -ForegroundColor Yellow
                    npm install -D tailwindcss postcss autoprefixer
                    npx tailwindcss init -p
                    
                    # Create or update tailwind.config.js
                    $tailwindConfig = @"
/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{js,jsx,ts,tsx,svelte,vue,html}",
    "./index.html",
  ],
  theme: {
    extend: {},
  },
  plugins: [],
}
"@
                    Set-Content -Path "tailwind.config.js" -Value $tailwindConfig
                    
                    # Create or update CSS with Tailwind directives
                    if (Test-Path "src") {
                        $tailwindCSS = @"
@tailwind base;
@tailwind components;
@tailwind utilities;
"@
                        if (Test-Path "src/index.css") {
                            Set-Content -Path "src/index.css" -Value $tailwindCSS
                        }
                        elseif (Test-Path "src/style.css") {
                            Set-Content -Path "src/style.css" -Value $tailwindCSS
                        }
                        else {
                            # Create CSS file if it doesn't exist
                            New-Item -Path "src" -ItemType Directory -Force
                            Set-Content -Path "src/index.css" -Value $tailwindCSS
                        }
                    }
                }
                "ESLint + Prettier" {
                    Write-Host "Installing ESLint and Prettier..." -ForegroundColor Yellow
                    npm install -D eslint prettier eslint-config-prettier eslint-plugin-prettier
                    
                    # Create ESLint config
                    $eslintConfig = @"
module.exports = {
  "env": {
    "browser": true,
    "es2021": true,
    "node": true
  },
  "extends": [
    "eslint:recommended",
    "plugin:prettier/recommended"
  ],
  "parserOptions": {
    "ecmaVersion": "latest",
    "sourceType": "module"
  },
  "rules": {
    "prettier/prettier": "error"
  }
}
"@
                    Set-Content -Path ".eslintrc.js" -Value $eslintConfig
                    
                    # Create Prettier config
                    $prettierConfig = @"
{
  "semi": true,
  "singleQuote": true,
  "tabWidth": 2,
  "trailingComma": "es5"
}
"@
                    Set-Content -Path ".prettierrc" -Value $prettierConfig
                }
                "Sass" {
                    Write-Host "Installing Sass..." -ForegroundColor Yellow
                    npm install -D sass
                }
                "Playwright (E2E testing)" {
                    Write-Host "Installing Playwright..." -ForegroundColor Yellow
                    npm init playwright@latest -y
                }
                "Vitest (Unit testing)" {
                    Write-Host "Installing Vitest..." -ForegroundColor Yellow
                    npm install -D vitest
                    
                    # Create Vitest config
                    $vitestConfig = @"
import { defineConfig } from 'vitest/config'

export default defineConfig({
  test: {
    globals: true,
    environment: 'jsdom',
  },
})
"@
                    Set-Content -Path "vitest.config.js" -Value $vitestConfig
                }
                "TypeScript" {
                    Write-Host "Installing TypeScript..." -ForegroundColor Yellow
                    npm install -D typescript @types/node
                    
                    # Create tsconfig.json
                    $tsConfig = @"
{
  "compilerOptions": {
    "target": "es2022",
    "module": "esnext",
    "moduleResolution": "node",
    "esModuleInterop": true,
    "forceConsistentCasingInFileNames": true,
    "strict": true,
    "skipLibCheck": true
  }
}
"@
                    Set-Content -Path "tsconfig.json" -Value $tsConfig
                }
            }
        }
    }
    
    # Return to the original directory
    Set-Location $currentDir
    
    Write-Host "`n🚀 Project setup complete!" -ForegroundColor Green
    
    # Ask user if they want to start the development server
    $startDev = Read-Host "Would you like to start the development server? (y/n)"
    
    if ($startDev.ToLower() -eq "y") {
        Write-Host "`nStarting development server..." -ForegroundColor Green
        Write-Host "You can stop the server by pressing Ctrl+C" -ForegroundColor Yellow
        
        # Navigate to project directory
        Set-Location (Join-Path $currentDir $projectName)
        
        # Start the development server
        try {
            Invoke-Expression $startCommand
        }
        catch {
            Write-Host "Error starting development server: $_" -ForegroundColor Red
            Write-Host "You can manually start it by running:" -ForegroundColor Yellow
            Write-Host "  cd $projectName" -ForegroundColor White
            Write-Host "  $startCommand" -ForegroundColor White
        }
    }
    else {
        Write-Host "To start development:" -ForegroundColor Green
        Write-Host "  cd $projectName" -ForegroundColor White
        Write-Host "  $startCommand" -ForegroundColor White
    }
    
    Write-Host "`nHappy coding! 💻" -ForegroundColor Cyan
}

# Function to get working directory
function Get-WorkingDirectory {
    Write-Host "Working Directory:" -ForegroundColor Green
    Write-Host "1. Current Directory: $(Get-Location)"
    Write-Host "2. Custom Directory"
    
    $choice = Read-Host "`nWhere would you like to create your project? (1-2)"
    
    switch ($choice) {
        "1" {
            return (Get-Location).Path
        }
        "2" {
            $customPath = $null
            while (-not $customPath -or -not (Test-Path $customPath)) {
                $customPath = Read-Host "Enter the full path to your projects directory"
                if (-not (Test-Path $customPath)) {
                    $createDir = Read-Host "Directory does not exist. Would you like to create it? (y/n)"
                    if ($createDir.ToLower() -eq "y") {
                        try {
                            New-Item -Path $customPath -ItemType Directory -Force | Out-Null
                            Write-Host "Directory created: $customPath" -ForegroundColor Green
                        }
                        catch {
                            Write-Host "Failed to create directory. Please try again." -ForegroundColor Red
                            $customPath = $null
                        }
                    }
                    else {
                        $customPath = $null
                    }
                }
            }
            return $customPath
        }
        default {
            Write-Host "Invalid choice. Using current directory." -ForegroundColor Yellow
            return (Get-Location).Path
        }
    }
}

# Main execution starts here
Clear-Host
Check-Prerequisites
Show-Banner

# Get working directory first
$workingDirectory = Get-WorkingDirectory
Set-Location $workingDirectory
Write-Host "Working in: $workingDirectory" -ForegroundColor Cyan
Write-Host ""

$projectName = Get-ProjectName
$template = Get-Template
$enhancements = Get-Enhancements
Setup-Project -projectName $projectName -template $template -enhancements $enhancements