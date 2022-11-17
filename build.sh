#!/usr/bin/env bash

pushd `dirname $0` > /dev/null
SCRIPT_PATH=`pwd -P`
cd $SCRIPT_PATH/

mkdir -p $SCRIPT_PATH/bin/java
cd java-user-list
mvn clean install -Dmaven.test.skip && cp target/java-user-list-1.0.0.jar $SCRIPT_PATH/bin/java/
cp resources/scripts/bootstrap.sh $SCRIPT_PATH/bin/java/
cd $SCRIPT_PATH

mkdir -p $SCRIPT_PATH/bin/go
cd go-admin
go build && mv go-admin $SCRIPT_PATH/bin/go/ && cd $SCRIPT_PATH
cp resources/scripts/bootstrap.sh $SCRIPT_PATH/bin/go/

mkdir -p $SCRIPT_PATH/bin/php
cd php-frontend
