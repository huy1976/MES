import api from "./Axios";
export const getManufacturingOrders = async (pageIndex = 1, pageSize = 30) => {
  const response = await api.get('ManufacturingOrders', {
    params: {
      pageIndex,
      pageSize
    }
  });
  return response.data;
};
export const createManufacturingOrder = async (orderData) => {
  const response = await api.post('ManufacturingOrders', orderData);
  return response.data;
};