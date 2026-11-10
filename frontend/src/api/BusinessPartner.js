// src/api/BusinessPartner.js
import api from "./Axios";
export const getCustomerIds = async () => {
  try {
    const response = await api.get('BusinessPartner/GetCustomerIds');
    return response.data; // → ["Cus01", "Cus02", ...]
  } catch (error) {
    console.error('Lỗi lấy danh sách khách hàng:', error.response || error);
    throw error;
  }
};