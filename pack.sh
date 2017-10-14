#!/bin/bash
set -e

# Version
revision=$(printf %04d $APPVEYOR_BUILD_NUMBER)

# pack
for path in src/*; do
    dotnet pack ${path} -c Release -o "..\..\artifacts" --version-suffix="beta-${revision}" --no-build
done