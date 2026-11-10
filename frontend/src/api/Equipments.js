import api from "./Axios";
export const getEquipments = async (pageIndex = 1, pageSize = 30) => {
  const response = await api.get('Equipments', {
    params: {
      pageIndex,
      pageSize
    }
  });
  return response.data;
};
export const getEquipmentPropertyNames = async () => {
  try {
    const response = await api.get('Equipments/GetEquipmentPropertyNames');
    return response.data;
  } catch (error) {
    throw error;
  }
};

export const getEquipmentIds = async () => {
  try {
    const response = await api.get('Equipments/GetEquipmentIds'); 
    return response.data;
  }
  catch (error) {
    throw error;
  }
};

export const updateEquipmentStatus = async () => {
  try {
    const response = await api.patch(
      'Equipments/UpdateEquipmentStatuses',
      {}
    );
    return response.data;
  } catch (error) {
    throw error;
  }
};

export const createEquipment = async (equipment) => {
  const response = await api.post("Equipments/CreateEquipment", equipment);
  return response.data;
};

