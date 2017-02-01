#!/bin/bash

# variables
target=${1:-local}
projectFile="project.json"

# run tests on local
if [ "${target}" == "local" ]; then
    for path in test/*/"${projectFile}"; do
        dotnet test $path
    done
fi

# run tests on appveyor
if [ "${target}" == "appveyor" ]; then
    for path in test/*; do
        ".\OpenCover\OpenCover.4.6.519\tools\OpenCover.Console.exe" \
        -oldstyle \
        -target:"C:\Program Files\dotnet\dotnet.exe" \
        -filter:"+[Tossit.*]* -[*.Tests]* -[*]*.Api.*" \
        -targetargs:"test ${path}" \
        -mergeoutput -output:coverage.xml -register:user
    done
fi