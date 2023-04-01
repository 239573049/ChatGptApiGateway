import React, { useState } from "react";
import api from "../../utils/request";
import "./index.css";
import { Notification } from '@douyinfe/semi-ui';

const Login: React.FC = () => {
  const [username, setUsername] = useState("");

  const handleUsernameChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setUsername(event.target.value);
  };

  const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    api.post('api/v1/Authorizes?username='+username).then(res=>{
      console.log(res.data);
      if(res.data.code === 200){
        window.localStorage.setItem('token',res.data.value)
        window.open('/admin','_self')
      }else{
        Notification.error({
          title: 'error',
          content: res.data.message,
          duration: 3,
          theme: 'light',
      })
      }
    })
  };

  return (
    <div className="login-container">
      <form onSubmit={handleSubmit}>
        <h1>ApiGateway登录界面</h1>
        <div className="form-group">
          <label htmlFor="username">Username:</label>
          <input
            type="text"
            id="username"
            value={username}
            onChange={handleUsernameChange}
          />
        </div>
        <button type="submit">进入系统</button>
      </form>
    </div>
  );
};

export default Login;