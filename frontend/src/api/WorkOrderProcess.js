import api from "./Axios";

export const getWorkOrderProcess = async () => {
  try {
    const response = await api.get("WorkOrders/WorkOrderProgressViewQuery");
    return response.data;
  } catch (error) {
    console.error("Error fetching work orders:", error);
    return null;
  }
};
export const getReviewNewScheduleTimeForWo = async (woId, duration) => {
  try {
    const response = await api.get("WorkOrders/ReviewNewScheduleTimeForWo", {
      params: { 
        WoId: woId,
        Duration: duration // Thêm tham số Duration vào đây
      }
    });
    return response.data;
  } catch (error) {
    console.error("Error fetching review schedule time:", error);
    return null;
  }
};

// --- Hàm mới bạn cần ---
export const patchWorkOrderStatus = async (payload) => {
  try {
    const response = await api.patch("WorkOrders/UpdateWorkOrderStatus", payload);
    return response.data;
  } catch (error) {
    console.error("Lỗi khi cập nhật trạng thái lệnh sản xuất:", error);
    throw error;
  }
};
export const postWorkOrderProgressRecord = async (payload) => {
  try {
    const response = await api.post("WorkOrders/CreateWorkOrderProgressRecord", payload);
    return response.data;
  } catch (error) {
    console.error("Lỗi khi xuất bản ghi tiến trình lệnh sản xuất:", error);
    throw error;
  }
};

export const postWorkOrderErrorRecord = async (payload) => {
  try {
    const response = await api.post("WorkOrders/CreateWorkOrderErrorRecord", payload);
    return response.data;
  } catch (error) {
    console.error("Lỗi khi xuất bản ghi lỗi lệnh sản xuất:", error);
    throw error;
  }
};

export const getWorkOrderProgressHistory = async (woId) => {
   try {
    const response = await api.get(`WorkOrders/GetWorkOrderProgressRecordsByWorkOrderId/${woId}`);
    return response.data;
  } catch (error) {
    console.error("Error fetching work order progress history:", error);
    return null;
   }
}

export const getWorkOrderErrorHistory = async (woId) => {
   try {
    const response = await api.get(`WorkOrders/GetWorkOrderErrorRecordsByWorkOrderId/${woId}`);  
    return response.data;
  } catch (error) {
    console.error("Error fetching work order error history:", error);
    return null;
   }
} 