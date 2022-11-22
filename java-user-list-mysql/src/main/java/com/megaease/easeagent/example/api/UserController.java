package com.megaease.easeagent.example.api;

import com.megaease.easeagent.example.model.User;
import com.megaease.easeagent.example.service.UserService;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import javax.annotation.Resource;
import java.util.Date;
import java.util.List;

@RestController
@Slf4j
public class UserController {
    @Value("${app.name}")
    private String appName;

    @Value("${app.age}")
    private String appAge;

    @Resource
    private UserService userService;

    @RequestMapping("/user/list")
    public List<User> userList() {
        log.info("{}-{} userList invoke", this.appName, this.appAge);
        return this.userService.getUsers();
    }

    @RequestMapping("/user/add/{name}")
    public User add(@PathVariable("name") String name) {
        if (name == null || name.length() <= 0) {
            name = "easeagent-" + System.currentTimeMillis();
        }
        return this.userService.addUser(name, new Date());
    }

    @RequestMapping("/user/add")
    public User add() {
        String name = "easeagent-" + System.currentTimeMillis();
        return this.userService.addUser(name, new Date());
    }
}
