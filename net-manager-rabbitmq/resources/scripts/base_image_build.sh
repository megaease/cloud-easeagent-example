#!/bin/bash
pushd `dirname $0` > /dev/null
SCRIPT_PATH=`pwd -P`
cd $SCRIPT_PATH/

docker build -f ./Dockerfile . -t megaease/net-apm-manager-example:1.0.0