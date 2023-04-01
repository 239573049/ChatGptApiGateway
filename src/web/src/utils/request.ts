import axios from "axios";
const api = axios.create({
  baseURL:
    process.env.NODE_ENV === "development" ? "http://localhost:53969" : "",
  headers: {
    "Content-Type": "application/json",
    Authorization: window.localStorage.getItem("token"),
  },
});

// 添加响应拦截器
api.interceptors.response.use(
  (response) => {
    // 对响应数据做些什么
    return response;
  },
  (error) => {
    if(error.response.status === 401){
        window.location.href = "/";
    }
    // 对响应错误做些什么
    return Promise.reject(error);
  }
);

export default api;
