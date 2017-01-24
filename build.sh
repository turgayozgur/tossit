#!/bin/bash

# variables
srcFramework=$1
projectFile="project.json"

# restore all
dotnet restore

# src
[ ! -z "${srcFramework}" ] && dotnet build src/*/"${projectFile}" -f "${srcFramework}" || dotnet build src/*/"${projectFile}"

# samples
dotnet build samples/*/"${projectFile}"

# test
dotnet build test/*/"${projectFile}" --no-dependencies