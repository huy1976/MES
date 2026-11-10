import api from "./Axios";
export const getEquipmentClasses = async (pageIndex = 1, pageSize = 30) => {
  const response = await api.get('EquipmentClasses', {
    params: {
      pageIndex,
      pageSize
    }
  });
  return response.data;
};
export const createEquipmentClass = async (equipmentClassData) => {
  const response = await api.post('EquipmentClasses', equipmentClassData);
  return response.data;
}

export const getEquipmentClassIds = async () => {
  try {
    const response = await api.get('EquipmentClasses/GetEquipmentClassIds');
    return response.data;
  } catch (error) {
    throw error;
  }
}

export const getEquipmentClassPropertyNames = async () => {
  try {
    const response = await api.get('EquipmentClasses/GetEquipmentClassPropertyNames');
    return response.data;
  } catch (error) {
    throw error;
  }
};  