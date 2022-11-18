#!/bin/bash
pushd `dirname $0` > /dev/null
SCRIPT_PATH=`pwd -P`
cd $SCRIPT_PATH/
JAVA_OPTS=-javaagent:./easeagent.jar ./bootstrap.sh

