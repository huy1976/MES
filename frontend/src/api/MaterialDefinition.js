// src/api/MaterialDefinitions.js
import api from "./Axios";
export const getMaterialDefinitionIds = async () => {
  try {
    const response = await api.get('MaterialDefinitions/GetMaterialDefinitionIdsByFinishedProduct');
    return response.data; // → ["Al_0.45_1p", "Cu_0.19_2p", ...]
  } catch (error) {
    throw error;
  }
};
export const getMaterialNameandUnit = async (materialId) => {
  const response = await api.get(
    `MaterialDefinitions/GetMaterialDefinitionById/${materialId}`
  );
  return response.data; // ← trả về { name: "Nhôm 0.45mm 1 pha", primaryUnit: "kg", ... }
};
export const getMaterialById = async (materialId) => {
  const response = await api.get(
    `MaterialDefinitions/GetMaterialDefinitionByMaterialId/${materialId}`
  );
  return response.data; 
};
export const getQualityStandardByMaterialId = async (materialId) => {
  const response = await api.get(
    `QualityStandard/GetQualityStandardsByMaterialId/${materialId}`
  );
  return response.data;
};

export const getMaterialDefinitionPropertyNames = async () => {
  try {
    const response = await api.get('MaterialDefinitions/GetMaterialDefinitionPropertyNames');
    return response.data;
  } catch (error) {
    throw error;
  }
};

export const getMaterialDefinitionIdList = async () => {
  try {
    const response = await api.get('MaterialDefinitions/GetMaterialDefinitionIds');  
    return response.data;
  } catch (error) {
    throw error;
  }       
};

export const createMaterialUnit = async (materialId, data) => {
  try {
    const response = await api.post(`MaterialDefinitions/CreateMaterialUnit?materialDefinitionId=${materialId}`, data);  
    return response.data;
  } catch (error) {     
    throw error;
  }       
}   