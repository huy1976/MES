import api from "./Axios";


export const getEquipmentCapabilities = async (pageIndex = 1, pageSize = 30) => {
  const response = await api.get('Equipments/GetEquipmentCapabilities', {
    params: {
      pageIndex,
      pageSize
    }
  });
  return response.data;
};

export const getEquipmentCapabilityUnits = async () => {
  try {
    const response = await api.get('Equipments/GetEquipmentCapabilityUnits');
    return response.data;
    } catch (error) {
        throw error;
    }
}   

export const getEquipmentCapabilityPropertyNames = async () => {
  try {
    const response = await api.get('Equipments/GetEquipmentCapabilityPropertyNames');
    return response.data;
    } catch (error) {
        throw error;
    }   
};

export const getEquipmentCapabilityPropertyUnits = async () => {
  try {
    const response = await api.get('Equipments/GetEquipmentCapabilityPropertyUnits');
    return response.data;
    } catch (error) {
        throw error;
    }   
};

export const createEquipmentCapability = async (equipmentCapability) => {
  const response = await api.post("Equipments/CreateEquipmentCapability", equipmentCapability);
  return response.data;
};