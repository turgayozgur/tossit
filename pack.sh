#!/bin/bash

# variables
revision=${1}
projectFile="project.json"

# Version
revision=$(printf %04d $revision)

# pack
for path in src/*; do
    dotnet pack ${path} -c Debug -o ./artifacts --version-suffix=$revision --no-build
done