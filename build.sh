#!/bin/bash
set -e

configuration=${1-"Debug"}

dotnet restore

if [ $APPVEYOR ]; then
    dotnet build --no-restore -c $configuration;
else
    # src
    for path in src/*/*.csproj;
    do
        dotnet build $path --no-restore -f netstandard2.0 -c $configuration
    done
    
    # test
    for path in test/*/*.csproj;
    do
        dotnet build $path -c $configuration --no-restore
    done
    
    # samples
    for path in samples/*/*.csproj;
    do
        dotnet build $path -c $configuration --no-restore
    done
fi
