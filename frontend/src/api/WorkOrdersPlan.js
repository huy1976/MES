import api from "./Axios";
export const getManufacturingOrders = async (pageIndex = 1, pageSize = 100) => {
  const response = await api.get('WorkOrders/GetWorkOrders', {
    params: {
      pageIndex,
      pageSize
    }
  });
  return response.data;
};
export const getWorkOrdersByStatus2 = async () => {
  try {
    const response = await api.get("WorkOrders/GetWorkOrdersByStatus/2");
    return response.data;
  } catch (error) {
  }
};
/**
 * Gửi yêu cầu lập lịch sản xuất
 * @param {Array} selectedOrders - Danh sách object đơn hàng (chứa field 'id')
 * @param {Number} algorithmType - 2 (Genetic) hoặc 3 (Tabu)
 */
export const scheduleWorkOrders = async (selectedOrders, algorithmType) => {
  const workOrderIds = selectedOrders.map(order => order.id);
  const payload = {
    workOrderIds: workOrderIds,   // Mảng [1, 2, 5, ...]
    algorithmType: algorithmType  // 2 hoặc 3
  };
  console.log("Payload gửi lên API:", payload);
  try {
    const response = await api.post('Scheduling/ResultSchedulingForWorkOrders', payload);
    return response.data;
  } catch (error) {
    throw error;
  }
};
export const getCurrentProductionSchedule = async () => {
  try {
    const response = await api.get('Scheduling/CurrentProductionSchedule');
    return response.data;
  } catch (error) {
    console.error('Lỗi khi tải lịch sản xuất hiện tại:', error);
    throw error;
  }
};
export const rescheduleUrgentOrder = async (urgentOrders, algorithmType) => {
  const payload = {
    urgentWorkOrderIds: urgentOrders.map(x => Number(x.id)).filter(Boolean),
    reschedulableWorkOrderIds: [],
    algorithmType
  };

  const response = await api.post('Scheduling/RescheduleUrgentOrder', payload);
  return response.data;
};
export const confirmListWorkOrders = async (scheduledWorkOrders) => {
  try {
    const requestBody = {
      scheduledWorkOrderDTOs: scheduledWorkOrders
    };
    const response = await api.patch('WorkOrders/ConfirmListWorkOrders', requestBody);
    return response.data;
  } catch (error) {
    console.error('Lỗi khi gọi API confirmListWorkOrders (PATCH):', error);
    throw error;
  }
};