#!/bin/bash

# variables
projectFile="project.json"

# run test
for path in test/*/"${projectFile}"; do
    dotnet test $path
done