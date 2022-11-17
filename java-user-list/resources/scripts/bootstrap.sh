#!/bin/bash
pushd `dirname $0` > /dev/null
SCRIPT_PATH=`pwd -P`
cd $SCRIPT_PATH/
# DB_HOST=127.0.0.1
# DB_PORT=3306
# JAVA_OPTS=-javaagent:./easeagent.jar
java ${JAVA_OPTS} -jar ./jdbc-demo-1.0.0.jar
