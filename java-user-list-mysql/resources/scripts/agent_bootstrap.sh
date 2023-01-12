#!/bin/bash
pushd `dirname $0` > /dev/null
SCRIPT_PATH=`pwd -P`
cd $SCRIPT_PATH/
JAVA_OPTS=-javaagent:$SCRIPT_PATH/easeagent.jar ./bootstrap.sh

