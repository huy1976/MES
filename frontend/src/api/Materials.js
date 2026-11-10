import api from "./Axios";
export const getMaterials = async (pageIndex = 1, pageSize = 31) => {
  const response = await api.get('MaterialDefinitions/GetMaterialDefinitions', {
    params: {
      PageIndex: pageIndex,
      PageSize: pageSize,
    },
  });
  return response.data;
};

export const createMaterial = async (material) => {
  const response = await api.post("MaterialDefinitions/CreateMaterialDefinition", material);
  return response.data;
};