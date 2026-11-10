import api from "./Axios";

export const getManufacturingOrdersByStatus1 = async () => {
  try {
    const response = await api.get("ManufacturingOrders/GetManufacturingOrdersByStatus1");
    return response.data;
  } catch (error) {
    console.error("Lỗi khi tải đơn hàng sản xuất (trạng thái 1):", error);
    throw error;
  }
};

/**
 * Tạo (mô phỏng) danh sách Work Orders cho một Manufacturing Order.
 * @param {string} manufacturingOrderId ID của đơn hàng sản xuất (MO)
 * @returns {Promise<any>} Dữ liệu trả về từ API sau khi tạo
 */
export const createWorkOrdersForManufacturingOrder = async (manufacturingOrderId) => {
  try {
    const requestBody = {
      manufacturingOrderId: manufacturingOrderId,
    };
    const response = await api.post("WorkOrders/CreateListWorkOrders", requestBody);
    return response.data;
  } catch (error) {
    console.error(`Lỗi khi tạo Work Orders cho MO ${manufacturingOrderId}:`, error);
    throw error;
  }
};

export const getQuantityWorkOrders = async () => {
  try {
    const response = await api.get("WorkOrders/GetQuantityWorkOrders");
    return response.data;
  } catch (error) {
    throw error;
  }
};

// === HÀM POST MỚI ĐƯỢC THÊM VÀO ===

/**
 * Gửi (Duyệt) một danh sách các Work Orders đã tạo để lưu vào CSDL.
 * @param {Array<Object>} workOrderDTOs - Một mảng các đối tượng Work Order (LSX) đã được tạo.
 * @returns {Promise<any>} Dữ liệu trả về từ API (thường là xác nhận hoặc danh sách đã lưu).
 */
export const addListWorkOrders = async (workOrderDTOs) => {
  try {
    const requestBody = {
      workOrderDTOs: workOrderDTOs
    };
    const response = await api.post("WorkOrders/AddListWorkOrders", requestBody);
    return response.data;
  } catch (error) {
    console.error("Lỗi khi gửi danh sách Work Orders (AddListWorkOrders):", error);
    throw error;
  }
};
