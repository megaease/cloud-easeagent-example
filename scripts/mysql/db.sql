CREATE DATABASE `db_demo`  DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci ;

CREATE TABLE `db_demo`.`t_user` (
  `user_id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(45) NOT NULL,
  `create_time` DATETIME(3) NOT NULL,
  PRIMARY KEY (`user_id`));

use db_demo;
INSERT INTO t_user (name, create_time) values ('admin', now());
INSERT INTO t_user (name, create_time) values ('ZARD', now());


CREATE USER IF NOT EXISTS 'easeagent_example'@'%' IDENTIFIED BY 'demo_password';
GRANT ALL privileges ON `db_demo`.* TO 'easeagent_example'@`%`;
FLUSH PRIVILEGES;
