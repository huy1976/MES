import React, { useState } from 'react';
import './Loginscreen.css';
import BKlogo from './../../../assets/bk_logo.png';
import { Login as loginApi } from './../../../api/Login';
import { useAuth } from './../Login/AuthContext';

const Login = ({ onLoginSuccess }) => {
  const [credentials, setCredentials] = useState({ username: '', password: '' });
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const { setAuthToken } = useAuth();

  const handleChange = (e) => {
    const { name, value } = e.target;
    setCredentials(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    const { username, password } = credentials;

    if (!username.trim() || !password.trim()) {
      setError('Tên đăng nhập và mật khẩu không được để trống.');
      return;
    }

    if (password.length < 3) {
      setError('Mật khẩu phải có ít nhất 3 ký tự.');
      return;
    }

    setIsLoading(true);

    try {
      const responseData = await loginApi(username, password);
      const token = responseData.accessToken;

      if (token) {
        setAuthToken(token);
        onLoginSuccess?.(responseData);
      } else {
        setError('Đăng nhập không thành công. Thiếu Token.');
      }
    } catch (apiError) {
      let errorMessage = 'Đã xảy ra lỗi hệ thống, vui lòng thử lại.';
      if (apiError.response?.data?.message) {
        errorMessage = apiError.response.data.message;
      } else if (apiError.message) {
        errorMessage = `Lỗi: ${apiError.message}`;
      }
      setError(errorMessage);
      console.error('Login API Error:', apiError);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="login-wrapper">
      <div className="login-card">
        <div className="logo-container">
          <img src={BKlogo} alt="BK Logo" className="bk-logo" />
        </div>

        <h2 className="login-title">Welcome Back</h2>
        <p className="login-subtitle">Đăng nhập để tiếp tục</p>

        <form onSubmit={handleSubmit} noValidate>
          {error && <div className="error-message">{error}</div>}

          <div className="input-group modern">
            <span className="input-icon"><i className="fas fa-user"></i></span>
            <input
              type="text"
              name="username"
              value={credentials.username}
              onChange={handleChange}
              placeholder="UserName"
              autoComplete="username"
            />
          </div>

          <div className="input-group modern">
            <span className="input-icon"><i className="fas fa-lock"></i></span>
            <input
              type="password"
              name="password"
              value={credentials.password}
              onChange={handleChange}
              placeholder="Mật khẩu"
              autoComplete="current-password"
            />
          </div>

          <button type="submit" className="login-btn" disabled={isLoading}>
            {isLoading ? 'Đang xử lý...' : 'Đăng nhập'}
          </button>
        </form>

        <a href="/forgot-password" className="forgot-link">Quên mật khẩu?</a>
      </div>
    </div>
  );
};

export default Login;
