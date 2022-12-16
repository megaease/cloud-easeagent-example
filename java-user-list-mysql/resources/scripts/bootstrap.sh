#!/bin/bash
pushd `dirname $0` > /dev/null
SCRIPT_PATH=`pwd -P`
cd $SCRIPT_PATH/
# DB_HOST=127.0.0.1
# DB_PORT=3306
# DB_USER=easeagent_example
# DB_PASSWORD=demo_password
# DB_NAME=db_demo
# JAVA_OPTS=-javaagent:./easeagent.jar

java ${JAVA_OPTS} -jar ./java-user-list-mysql-1.0.0.jar
