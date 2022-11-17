package com.megaease.easeagent.example;

import org.springframework.boot.CommandLineRunner;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.SpringBootConfiguration;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.context.annotation.Bean;
import tk.mybatis.spring.annotation.MapperScan;

@SpringBootConfiguration
@SpringBootApplication(scanBasePackages = "com.megaease.easeagent.example")
@MapperScan(basePackages = {
        "com.megaease.easeagent.example.dao"
})
public class JdbcApp {
    public static void main(String[] args) {
        SpringApplication.run(JdbcApp.class, args);
    }

    @Bean
    public CommandLineRunner commandLineRunner() {
        return args -> {
            System.out.println("commandLineRunner run1");
        };
    }

}
