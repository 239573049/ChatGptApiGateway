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

## 使用授权

新增`appsettings.json`配置 ，添加`Tokens`节点，如果`Tokens`节点存在token即启动授权，参考案例的token，请务必格式正确，`Tokens[0].Token`是使用的token，`Tokens.[0].Expire`是这个token过期时间

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "Tokens": [
    {
      "Token": "datatoken",
      "Expire": "2023-3-30 23:00"
    }
  ],
  "AllowedHosts": "*",
  "ReverseProxy": {
    "Routes": {
      "route1": {
        "ClusterId": "cluster1",
        "Match": {
          "Path": "{**catch-all}"
        }
      }
    },
    "Clusters": {
      "cluster1": {
        "Destinations": {
          "destination1": {
            "Address": "https://api.openai.com/"
          }
        }
      }
    }
  }
}
```

## 授权使用方法

使用方法`http://ip:端口/api?token=datatoken` ，需要在所有的请求中添加url参数`?token=datatoken`使用案例。当token过期以后状态码将显示401。

## docker compose部署

需要准备俩个文件`docker-compose.yml`，`appsettings.json`俩个文件

`docker-compose.yml`默认内容 `1080`是外网访问的端口可以根据需求自行修改，80不需要改，

```yaml
services:
  chatgpt:
    image: registry.cn-shenzhen.aliyuncs.com/tokengo/chatgpt-gateway
    container_name: chatgpt
    volumes:
      - ./appsettings.json/:/app/appsettings.json
    ports:
      - 1080:80
```

`appsettings.json`默认内容，可以自行添加token，请注意当前文件要和`docker-compose.yml`文件同一级

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "Tokens": [
    {
      "Token": "token",
      "Expire": "2023-3-30 23:00"
    }
  ],
  "AllowedHosts": "*",
  "ReverseProxy": {
    "Routes": {
      "route1": {
        "ClusterId": "cluster1",
        "Match": {
          "Path": "{**catch-all}"
        }
      }
    },
    "Clusters": {
      "cluster1": {
        "Destinations": {
          "destination1": {
            "Address": "https://api.openai.com/"
          }
        }
      }
    }
  }
}
```

在`docker-compose.yml`目录中执行 `docker compose up -d` 即可，更新配置需要重启服务,可以使用`docker compose restart`重启

## 无授权简洁使用

```
services:
  chatgpt:
    image: registry.cn-shenzhen.aliyuncs.com/tokengo/chatgpt-gateway
    container_name: chatgpt
    ports:
      - 1080:80
```

直接使用`compose`启动服务

技术交流群：737776595
