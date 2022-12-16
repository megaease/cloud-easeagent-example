#!/bin/bash
pushd `dirname $0` > /dev/null
SCRIPT_PATH=`pwd -P`
cd $SCRIPT_PATH/

docker build -f ./Dockerfile . -t megaease/php-apm-user-list-frontend-example:1.0.0