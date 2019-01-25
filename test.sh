#!/bin/bash
set -e

# run tests on local
if [ ! $APPVEYOR ]; then
    dotnet test --no-restore --no-build
else
    # run tests on appveyor
    for path in test/*; do
        (cd $path;
        "..\\..\OpenCover\OpenCover.4.6.519\tools\OpenCover.Console.exe" \
        -oldstyle \
        -target:"C:\Program Files\dotnet\dotnet.exe" \
        -filter:"+[Tossit.*]* -[*.Tests]* -[*]*.Api.*" \
        -targetargs:"test --no-restore --no-build -c Release" \
        -mergeoutput -output:"..\\..\coverage.xml" -register:user)
    done
fi