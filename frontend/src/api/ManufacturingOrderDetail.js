import api from "./Axios";
const ManufacturingOrderDetail = {
  getOrderDetail: (orderId) => {
    return api.get(`ManufacturingOrders/${orderId}`);
  },
};
export default ManufacturingOrderDetail;
