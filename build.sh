#!/bin/bash
set -e

configuration=${1-"Debug"}

dotnet restore

# src
for path in src/*/*.csproj;
do
    [ $APPVEYOR ] \
        && { dotnet build $path --no-restore -f net451 -c $configuration; \
             dotnet build $path --no-restore -f netstandard2.0 -c $configuration; } \
        || dotnet build $path --no-restore -f netstandard2.0
done

# test
for path in test/*/*.csproj;
do
    dotnet build $path -c $configuration --no-dependencies
done

# samples
for path in samples/*/*.csproj;
do
    dotnet build -c $configuration $path
done