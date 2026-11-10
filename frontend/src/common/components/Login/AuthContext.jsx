import React, { createContext, useState, useContext } from 'react';
import { setAccessTokenInAxios } from '../../../api/Axios';
import { isTokenExpired } from '../../../api/jwtUtils'; // hàm kiểm tra token

const AuthContext = createContext(null);

export const useAuth = () => useContext(AuthContext);

export const AuthProvider = ({ children }) => {
  const [accessToken, setAccessToken] = useState(null);

  // Cập nhật Access Token và đồng bộ Axios
  const setAuthToken = (token) => {
    setAccessToken(token);
    setAccessTokenInAxios(token);
  };

  // Kiểm tra token còn hạn hay không
  const isAuthenticated = () => {
    return accessToken && !isTokenExpired(accessToken, 60); // buffer 60 giây
  };

  // Logout: xóa token và refresh token server
  const logout = async () => {
    setAccessToken(null);
    setAccessTokenInAxios(null);
    localStorage.removeItem('authToken');

    try {
      await fetch('https://localhost:7173/api/Auth/Logout', {
        method: 'POST',
        credentials: 'include', // gửi cookie refresh token
      });
    } catch (err) {
      console.error('Logout error:', err);
    } finally {
      // redirect về login
      window.location.href = '/login';
    }
  };

  const contextValue = {
    accessToken,
    setAuthToken,
    logout,
    isAuthenticated: isAuthenticated(),
  };

  return (
    <AuthContext.Provider value={contextValue}>
      {children}
    </AuthContext.Provider>
  );
};
