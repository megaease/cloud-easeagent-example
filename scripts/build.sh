#!/usr/bin/env bash

pushd `dirname $0` > /dev/null
SCRIPT_PATH=`pwd -P`
cd $SCRIPT_PATH/
BIN_PATH=$SCRIPT_PATH/../bin

mkdir -p $BIN_PATH/java
cd ../java-user-list
mvn clean install -Dmaven.test.skip && cp target/java-user-list-1.0.0.jar $BIN_PATH/java/
cp resources/scripts/* $BIN_PATH/java/
cd $SCRIPT_PATH
cp mysql/db.sql $BIN_PATH/java/

mkdir -p $BIN_PATH/java-mysql
cd ../java-user-list-mysql
mvn clean install -Dmaven.test.skip && cp target/java-user-list-mysql-1.0.0.jar $BIN_PATH/java-mysql/
cp resources/scripts/* $BIN_PATH/java-mysql/
cd $SCRIPT_PATH
cp mysql/db.sql $BIN_PATH/java-mysql/

mkdir -p $BIN_PATH/go
cd ../go-admin
mkdir -p $BIN_PATH/go/linux_amd64
go build && mv go-admin $BIN_PATH/go/ && CGO_ENABLED=0 GOOS=linux GOARCH=amd64 go build && mv go-admin $BIN_PATH/go/linux_amd64/
cp resources/scripts/* $BIN_PATH/go/
cd $SCRIPT_PATH

mkdir -p $BIN_PATH/go-admin-redis
cd ../go-admin-redis
mkdir -p $BIN_PATH/go-admin-redis/linux_amd64
go build && mv go-admin-redis $BIN_PATH/go-admin-redis/ && CGO_ENABLED=0 GOOS=linux GOARCH=amd64 go build && mv go-admin-redis $BIN_PATH/go-admin-redis/linux_amd64/
cp resources/scripts/* $BIN_PATH/go-admin-redis/
cd $SCRIPT_PATH

mkdir -p $BIN_PATH/php
cd ../php-frontend
rm -rf vendor
rm -rf composer.lock && composer install
cp frontend.php $BIN_PATH/php/ && cp -r vendor $BIN_PATH/php/
cp resources/scripts/* $BIN_PATH/php/
cd $SCRIPT_PATH

mkdir -p $BIN_PATH/php-user-list
cd ../php-user-list-frontend
rm -rf vendor
rm -rf composer.lock && composer install
cp frontend.php $BIN_PATH/php-user-list/ && cp -r vendor $BIN_PATH/php-user-list/
cp resources/scripts/* $BIN_PATH/php-user-list/
cd $SCRIPT_PATH

cp -r mysql $BIN_PATH/
cp -r redis $BIN_PATH/