#!/bin/bash
set -e

# src
for path in src/*/*.csproj;
do
    dotnet restore $path
    dotnet build $path -f netstandard1.6

    # appveyor only:
    [ $APPVEYOR ] \
        && { dotnet build $path -f net451 -c Release; \
             dotnet build $path -f netstandard1.6 -c Release; }
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