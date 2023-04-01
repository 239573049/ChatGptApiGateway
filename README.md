# ChatGptApiGateway

## 介绍
用于代理ChatGpt api，部署到国外服务器无须翻墙即可使用ChatGpt Api服务

## 软件架构
.NET 7强力驱动，Yarp开源代理
只需要几行代码即可使用

docker 构建镜像

```shell
docker build -t registry.cn-shenzhen.aliyuncs.com/tokengo/chatgpt-gateway -f Gateway/Dockerfile .
```

## 使用管理界面

`http://ip:端口/index.html`进入管理界面



技术交流群：737776595
