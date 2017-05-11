#!/bin/bash
set -e

# variables
revision=$APPVEYOR_BUILD_NUMBER

# Version
revision=$(printf %04d $revision)

# pack
for path in src/*; do
    dotnet pack ${path} -c Release -o "..\..\artifacts" --version-suffix=$revision --no-build
done