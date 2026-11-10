import api from "./Axios";
export const getQualityStandards = async (pageIndex = 1, pageSize = 30) => {
  const response = await api.get('QualityStandard/GetQualityStandards', {
    params: {
      pageIndex,
      pageSize
    }
  });
  return response.data;
};
export const getQualityStandardPropertyNames = async () => {
  const response = await api.get('QualityStandard/GetQualityStandardPropertyNames');
  return response.data;
}
export const createQualityStandard = async (standardData) => {
  const response = await api.post('QualityStandard/CreateQualityStandard', standardData);
  return response.data;
}
export const getMaterialDefinitionIdsByFinishedProductNVL = async () => {
  // Fix cứng "NVL" vào đường dẫn API
  const response = await api.get('MaterialDefinitions/GetMaterialDefinitionIdsByClassId/NVL');
  return response.data;
}
export const getMaterialDefinitionIdsByFinishedProductTP = async () => {
  // Fix cứng "NVL" vào đường dẫn API
  const response = await api.get('MaterialDefinitions/GetMaterialDefinitionIdsByClassId/TP');
  return response.data;
}
export const createMaterialInfoQualityStandard = async (materialInfoStandardData) => {
  const response = await api.post('QualityStandard/AddMaterialIntoQualityStandard', materialInfoStandardData);
  return response.data;
}