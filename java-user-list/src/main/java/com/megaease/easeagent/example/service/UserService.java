/*
 * Copyright (c) 2021, MegaEase
 * All rights reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package com.megaease.easeagent.example.service;

import com.megaease.easeagent.example.model.User;
import com.megaease.easeagent.example.server.JdkHttpServer;
import org.apache.http.HttpEntity;
import org.apache.http.ParseException;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.methods.CloseableHttpResponse;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.CloseableHttpClient;
import org.apache.http.impl.client.HttpClients;
import org.apache.http.util.EntityUtils;
import org.springframework.stereotype.Component;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

@Component
public class UserService {
    static {
        JdkHttpServer.INSTANCE.start();
    }

    private final CloseableHttpClient httpclient = HttpClients.createDefault();

    public List<User> getUsers() {
        List<User> users = new ArrayList<>();
        User user1 = new User();
        user1.setName("admin");
        user1.setCreateTime(new Date());
        user1.setUserId(1);
        users.add(user1);
        User user2 = new User();
        user2.setName("ZARD");
        user2.setCreateTime(new Date());
        user2.setUserId(2);
        users.add(user2);
        for (User user : users) {
            user.setMessage(callHttpServer());
        }
        return users;
    }

    private String callHttpServer() {
        String context = null;
        try {
            // 创建httpget.
            HttpGet httpget = new HttpGet(JdkHttpServer.INSTANCE.getUrl());
            CloseableHttpResponse response = httpclient.execute(httpget);
            try {
                // 获取响应实体
                HttpEntity entity = response.getEntity();
                if (entity != null) {
                    context = EntityUtils.toString(entity);
                }
            } catch (Exception e) {
                throw e;
            } finally {
                response.close();
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
        return context;
    }

    public User addUser(String name, Date createTime) {
        User user = new User();
        user.setName(name);
        user.setCreateTime(createTime);
        return user;
    }
}
