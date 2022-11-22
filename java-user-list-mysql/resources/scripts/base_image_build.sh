#!/bin/bash
pushd `dirname $0` > /dev/null
SCRIPT_PATH=`pwd -P`
cd $SCRIPT_PATH/

docker build -f ./Dockerfile . -t megaease/java-apm-user-list-example:1.0.0