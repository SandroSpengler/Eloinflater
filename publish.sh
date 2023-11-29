#!/bin/bash

PROJECT_NAME=$1

echo ${PROJECT_NAME}

if [ $# -eq 0 ]; then
    printf "no arguments supplied valid arguments are: \\n api \\n dataminer \\n"
fi

if [[ -n "$1" ]] && [[ "${1#*.}" == "api" ]]; then

    echo "building eloinflater_api"

    docker build \
        -f "./Api/Dockerfile" . \
        -t sandrospengler/eloinflater_api \
        --force-rm \
        --build-arg "BUILD_CONFIGURATION=Release"

    docker push \
        sandrospengler/eloinflater_api
fi

if [[ -n "$1" ]] && [[ "${1#*.}" == "dataminer" ]]; then

    echo "building eloinflater_dataminer"

    docker build \
        -f "./Dataminer/Dockerfile" . \
        -t sandrospengler/eloinflater_dataminer \
        --force-rm \
        --build-arg "BUILD_CONFIGURATION=Release"

    docker push \
        sandrospengler/eloinflater_dataminer
fi
