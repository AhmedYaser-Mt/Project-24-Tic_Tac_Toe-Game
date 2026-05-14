@echo off
setlocal EnableExtensions EnableDelayedExpansion
title Universal Git Upload Tool

echo =========================================
echo        Universal Git Upload Tool
echo =========================================
echo.

:: -------------------------------------------------
:: 1) Ensure Git repository exists
:: -------------------------------------------------
if not exist ".git" (
    echo No Git repository found.
    echo Initializing a new repository...
    git init
    if errorlevel 1 (
        echo Failed to initialize Git repository.
        pause
        exit /b 1
    )
)

:: -------------------------------------------------
:: 2) Force branch to main
:: -------------------------------------------------
set "currentBranch=main"

git checkout main >nul 2>nul
if errorlevel 1 (
    git checkout -b main >nul 2>nul
)

echo Using branch: !currentBranch!
echo.

:: -------------------------------------------------
:: 3) Detect project type and create .gitignore
:: -------------------------------------------------
if not exist ".gitignore" (

    set "isCSharp=0"
    for %%f in (*.csproj *.sln) do set "isCSharp=1"

    set "isCpp=0"
    for %%f in (*.vcxproj *.cpp *.h) do set "isCpp=1"

    if "!isCSharp!"=="1" (
        echo Detected: C# .NET project
        echo Creating .gitignore for C# .NET Framework...
        (
            echo # ---- Batch files
            echo *.bat
            echo.
            echo # ---- Build output
            echo bin/
            echo obj/
            echo.
            echo # ---- Visual Studio settings
            echo .vs/
            echo *.user
            echo *.suo
            echo *.cache
            echo *.userosscache
            echo *.sln.docstates
            echo.
            echo # ---- NuGet packages
            echo packages/
            echo *.nupkg
            echo.
            echo # ---- Compiled output
            echo *.exe
            echo *.dll
            echo *.pdb
            echo.
            echo # ---- Logs and temp files
            echo *.log
            echo Thumbs.db
            echo Desktop.ini
        ) > .gitignore

    ) else if "!isCpp!"=="1" (
        echo Detected: C++ project
        echo Creating .gitignore for C++...
        (
            echo # ---- Batch files
            echo *.bat
            echo.
            echo # ---- Build output
            echo bin/
            echo obj/
            echo Debug/
            echo Release/
            echo x64/
            echo x86/
            echo.
            echo # ---- Visual Studio settings
            echo .vs/
            echo *.user
            echo *.suo
            echo *.cache
            echo *.sdf
            echo *.opensdf
            echo *.VC.db
            echo *.VC.opendb
            echo ipch/
            echo.
            echo # ---- Compiled output
            echo *.exe
            echo *.dll
            echo *.pdb
            echo *.ilk
            echo *.obj
            echo *.lib
            echo *.exp
            echo.
            echo # ---- Logs and temp files
            echo *.log
            echo Thumbs.db
            echo Desktop.ini
        ) > .gitignore

    ) else (
        echo Could not detect project type. Creating general .gitignore...
        (
            echo # ---- Batch files
            echo *.bat
            echo.
            echo # ---- Common build/temp files
            echo bin/
            echo obj/
            echo .vs/
            echo *.user
            echo *.exe
            echo *.dll
            echo *.pdb
            echo *.log
            echo Thumbs.db
            echo Desktop.ini
        ) > .gitignore
    )

    echo .gitignore created.
    echo.
)

:: -------------------------------------------------
:: 4) Show repository status
:: -------------------------------------------------
echo Repository status:
git status
echo.

:: -------------------------------------------------
:: 5) Add files and show staged files
:: -------------------------------------------------
echo Adding files...
git add .
if errorlevel 1 (
    echo Failed to stage files.
    pause
    exit /b 1
)

echo.
echo Staged files:
git diff --cached --name-only
echo.

git diff --cached --quiet
if errorlevel 1 (
    set "commitMsg="
    set /p "commitMsg=Enter commit message: "
    if not defined commitMsg (
        echo Commit message cannot be empty.
        pause
        exit /b 1
    )

    git commit -m "!commitMsg!"
    if errorlevel 1 (
        echo Commit failed.
        pause
        exit /b 1
    )
) else (
    echo Nothing new to commit.
)

:: -------------------------------------------------
:: 6) Ask for GitHub repository URL
:: -------------------------------------------------
set "repoURL="
set /p "repoURL=Enter GitHub repository URL: "
if not defined repoURL (
    echo Repository URL cannot be empty.
    pause
    exit /b 1
)

:: -------------------------------------------------
:: 7) Check internet connection
:: -------------------------------------------------
echo.
echo Checking internet connection...
ping github.com -n 1 >nul
if errorlevel 1 (
    echo No internet connection detected.
    pause
    exit /b 1
)
echo Internet connection OK.
echo.

:: -------------------------------------------------
:: 8) Configure remote safely
:: -------------------------------------------------
git remote remove origin >nul 2>nul
git remote add origin "!repoURL!"
if errorlevel 1 (
    echo Failed to set remote origin.
    pause
    exit /b 1
)

:: -------------------------------------------------
:: 9) Handle README conflict if exists remotely
:: -------------------------------------------------
echo Checking for README conflicts...
git pull origin main --allow-unrelated-histories --no-edit >nul 2>nul

:: -------------------------------------------------
:: 10) Push to GitHub (main only)
:: -------------------------------------------------
echo Pushing to remote (main)...
git push -u origin main --force-with-lease

if errorlevel 1 (
    echo.
    echo Push failed. Trying normal push...
    git push -u origin main
    if errorlevel 1 (
        echo Push failed again.
        pause
        exit /b 1
    )
)

echo.
echo Done successfully.
pause
exit /b 0