// src/api/Dashboard.js
import api from "./Axios";

export const GetMOReport = async (timeRange) => {
  try {
    // Bắt đầu với endpoint cơ bản
    let url = 'Dashboard/GetMOReport';
    
    // Kiểm tra nếu timeRange có giá trị (không phải null, undefined, hoặc chuỗi rỗng)
    if (timeRange !== null && timeRange !== undefined && timeRange !== '') {
      // Thêm query parameter vào URL
      url = `${url}?timeRange=${timeRange}`;
    }
    
    // Gọi API với URL đã được xây dựng
    const response = await api.get(url);
    
    return response.data; // → ["Cus01", "Cus02", ...]
  } catch (error) {
    console.error('Lỗi lấy danh sách Tổng quan:', error.response || error);
    throw error;
  }
};