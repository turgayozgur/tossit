#!/bin/bash
set -e

# src
for path in src/*/*.csproj;
do
    [ $APPVEYOR ] \
        && { dotnet restore -c Release; \
             dotnet build $path --no-restore -f net451 -c Release; \
             dotnet build $path --no-restore -f netstandard2.0 -c Release; } \
        || { dotnet restore; \
             dotnet build $path --no-restore -f netstandard2.0 }
done

# test
for path in test/*/*.csproj;
do
    dotnet restore $path
    dotnet build $path --no-dependencies
done

# samples
for path in samples/*/*.csproj;
do
    dotnet restore $path
    dotnet build $path
done