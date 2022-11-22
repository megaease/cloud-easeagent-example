#!/bin/bash
pushd `dirname $0` > /dev/null
SCRIPT_PATH=`pwd -P`
cd $SCRIPT_PATH/
EASEAGENT_SDK_CONFIG_FILE=$SCRIPT_PATH/agent.yml ./bootstrap.sh

