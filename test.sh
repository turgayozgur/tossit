#!/bin/bash
set -e

# run tests on local
if [ ! $APPVEYOR ]; then
    for path in test/*/*.csproj; do
        dotnet test $path
    done
else
    # run tests on appveyor
    for path in test/*; do
        (cd $path;
        "..\\..\OpenCover\OpenCover.4.6.519\tools\OpenCover.Console.exe" \
        -oldstyle \
        -target:"C:\Program Files\dotnet\dotnet.exe" \
        -filter:"+[Tossit.*]* -[*.Tests]* -[*]*.Api.*" \
        -targetargs:"test" \
        -mergeoutput -output:"..\\..\coverage.xml" -register:user)
    done
fi