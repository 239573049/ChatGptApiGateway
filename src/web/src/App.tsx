import './App.css';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import Admin from './pages/admin';
import Login from './pages/login';
import '@douyinfe/semi-ui/dist/css/semi.min.css';

function App() {
  return (
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Login />} />
          <Route path="/admin" element={<Admin />} />
        </Routes>
      </BrowserRouter>
  );
}

export default App;
