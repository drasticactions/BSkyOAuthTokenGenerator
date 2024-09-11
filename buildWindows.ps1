# Delete artifacts directory if it exists
if (Test-Path -Path "artifacts") {
    Remove-Item -Path "artifacts" -Recurse -Force
}

if (Test-Path -Path "build") {
    Remove-Item -Path "build" -Recurse -Force
}

# Create artifacts directory

New-Item -ItemType Directory -Path "artifacts"

# Create build directory

New-Item -ItemType Directory -Path "build"

# Publish the project

dotnet publish .\src\BSkyOAuthTokenGenerator\BSkyOAuthTokenGenerator.csproj -c Release -o build

# Delete non DLL and EXEs from the build directory

Get-ChildItem -Path .\build\* -Exclude *.dll,*.exe | Remove-Item -Recurse -Force

# Zip the contents of the build directory

Compress-Archive -Path .\build\* -DestinationPath .\artifacts\BSkyOAuthTokenGenerator.zip