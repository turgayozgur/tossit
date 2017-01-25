#!/bin/bash

# variables
revision=${1}
projectFile="project.json"

# Version
revision=$(printf %04d $revision)

# pack
dotnet pack ./src/Tossit.RabbitMQ -c Debug -o ./artifacts --version-suffix=$revision --no-build
dotnet pack ./src/Tossit.WorkQueue -c Debug -o ./artifacts --version-suffix=$revision --no-build