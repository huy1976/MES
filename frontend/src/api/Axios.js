import axios from 'axios';
import { isTokenExpired } from './jwtUtils'; // hàm check token hết hạn

let accessToken = null;

const api = axios.create({
  baseURL: 'https://localhost:7173/api/',
    // baseURL: 'https://mesmicroserivce-bwghbeerfrd6hwe3.japaneast-01.azurewebsites.net/api/',
  timeout: 600000,
  headers: { 'Content-Type': 'application/json' },
  withCredentials: true,
});
const refreshApi = axios.create({
  baseURL: 'https://localhost:7173/api/',
    // baseURL: 'https://mesmicroserivce-bwghbeerfrd6hwe3.japaneast-01.azurewebsites.net/api/',
  withCredentials: true,
  timeout: 60000
});

export const getAccessToken = () => accessToken;
export const setAccessTokenInAxios = (token) => {
  accessToken = token;
  if (token) {
    api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  } else {
    delete api.defaults.headers.common['Authorization'];
  }
};

// Queue refresh token
let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
  failedQueue.forEach(p => {
    if (error) p.reject(error);
    else p.resolve(token);
  });
  failedQueue = [];
};

// Hàm lấy thời gian hết hạn của token
const getTokenExpiryDate = (token) => {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    if (payload.exp) {
      return new Date(payload.exp * 1000); // chuyển giây -> mili giây
    }
  } catch (err) {
    console.error('Invalid token format', err);
  }
  return null;
};

// Interceptor request: kiểm tra token trước request
api.interceptors.request.use(
  async (config) => {
    // Log ngày hết hạn accessToken
    if (accessToken) {
      const expiryDate = getTokenExpiryDate(accessToken);
      if (expiryDate) {
        console.log('Access token expires at:', expiryDate.toLocaleString());
      }
    }

    // Nếu token sắp hết hạn (<1 phút)
    if (accessToken && isTokenExpired(accessToken, 60)) {
      if (!isRefreshing) {
        isRefreshing = true;
        try {
          console.log('1 Refreshing access token...');
          const res = await refreshApi.post('/Auth/Refresh');
          console.log('Refresh token response:', res.data);
          const newToken = res.data.accessToken;
          console.log('New access token:', newToken);
          setAccessTokenInAxios(newToken);
          processQueue(null, newToken);
        } catch (err) {
          processQueue(err, null);
          window.location.href = '/login';
          return Promise.reject(err);
        } finally {
          isRefreshing = false;
        }
      } else {
        await new Promise((resolve, reject) => failedQueue.push({ resolve, reject }));
      }
    }

    if (accessToken) config.headers['Authorization'] = `Bearer ${accessToken}`;
    return config;
  },
  (error) => Promise.reject(error)
);

api.interceptors.response.use(
  response => response,
  async error => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      try {
        console.log('Chuẩn bị refresh token do 401 response');
        const res = await refreshApi.post('/Auth/Refresh');
        console.log('Refresh token response:', res.data);
        const newToken = res.data.accessToken;
        console.log('New access token:', newToken);
        setAccessTokenInAxios(newToken);
        console.log('Retrying original request with new token');
        return api(originalRequest);
      } catch (err) {
        window.location.href = '/login';
        return Promise.reject(err);
      }
    }
    return Promise.reject(error);
  }
);

export default api;
