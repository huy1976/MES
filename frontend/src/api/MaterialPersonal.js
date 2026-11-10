import api from "./Axios";
export const TIME_RANGE = {
  DAY: 0,
  WEEK: 1,
  MONTH: 2,
};
/**
 * Hàm lấy dữ liệu kế hoạch vật tư và nhân sự
 * @param {number} timeRange - 0: Ngày, 1: Tuần, 2: Tháng
 */
export const getPlanMaterials = async (timeRange) => {
  try {
    const response = await api.get('PlanMaterialAndPersonnel/GetPlanMaterials', {
      params: {
        timeRange: timeRange
      }
    });
    return response.data; 
  } catch (error) {
    console.error("Lỗi khi gọi API PlanMaterials:", error);
    throw error;
  }
};
export const getSelectedPlanMaterials = async (Month, Year) => {
  try {
    const response = await api.get('PlanMaterialAndPersonnel/GetPlanMaterials', {
      params: {
        timeRange: 3,   
        customMonth: Month, 
        customYear: Year
      }
    });
    return response.data; 
  } catch (error) {
    console.error("Lỗi khi gọi API SelectedPlanMaterials:", error);
    throw error;
  }
};
export const getSelectedPlanPersonals = async (Month, Year) => {
  try {
    const response = await api.get('PlanMaterialAndPersonnel/GetPlanPersonnels', {
      params: {
        timeRange: 3,   
        customMonth: Month, 
        customYear: Year
      }
    });
    return response.data; 
  } catch (error) {
    console.error("Lỗi khi gọi API SelectedPlanPersonals:", error);
    throw error;
  }
};

export const getPlanPersonals = async (timeRange) => {
  try {
    const response = await api.get('PlanMaterialAndPersonnel/GetPlanPersonnels', {
      params: {
        timeRange: timeRange
      }
    });
    return response.data; 
  } catch (error) {
    console.error("Lỗi khi gọi API PlanPersonnels:", error);
    throw error;
  }
};