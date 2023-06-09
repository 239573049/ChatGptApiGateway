# ChatGptApiGateway

## 介绍
用于代理ChatGpt api，部署到国外服务器无须翻墙即可使用ChatGpt Api服务

## 软件架构
.NET 7强力驱动，Yarp开源代理
只需要几行代码即可使用

docker 构建镜像

```shell
docker build -t registry.cn-shenzhen.aliyuncs.com/tokengo/chatgpt-gateway -f src/Gateway/Dockerfile .
```

## 简单使用Docker

```shell
docker run -d -p 1800:80 --name gateway registry.cn-shenzhen.aliyuncs.com/tokengo/chatgpt-gateway
```

### 管理token自行提供文件管理

```shell
docker run -d -p 1800:80 -v ./token.json:/app/token.json --name gateway registry.cn-shenzhen.aliyuncs.com/tokengo/chatgpt-gateway
```

`token.json`内容格式

```json
[
    {
        "Token":"adasadsadsads", // url授权校验的token
        "Expire":"2023-5-01 00:00:00", // token过期时间
        "ChatGptToken":"adsadadsadsads" // 如果填写了ChatGptToken的值当请求的Url并没有携带token的时候将默认使用当前值
    }
]
```

默认的`docker compose`文件

```yaml
services:
  chatgpt:
    image: registry.cn-shenzhen.aliyuncs.com/tokengo/chatgpt-gateway
    container_name: chatgpt
    volumes:
      - ./token.json:/app/token.json
    ports:
      - 1800:80

```





## 使用代理

将默认的`https://api.openai.com`替换`http://ip:端口/.......?token=<填写添加的token>`这样就可以做授权访问了，如果未添加token默认是不需要使用token即可访问代理服务的

技术交流群：737776595
