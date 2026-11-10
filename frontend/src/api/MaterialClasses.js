import api from "./Axios";
export const getMaterialClasses = async (pageIndex = 1, pageSize = 30) => {
  const response = await api.get('MaterialClasses', {
    params: {
      PageIndex: pageIndex,
      PageSize: pageSize,
    },
  });
  return response.data; // { items: [...], totalItems: 48 }
};
export const createMaterialClass = async (materialClassData) => {
  const response = await api.post('MaterialClasses', materialClassData);
  return response.data;
};

export const getMaterialClassPropertyNames = async () => {
  try {
    const response = await api.get('MaterialClasses/GetMaterialClassPropertyNames');
    return response.data;
  } catch (error) {
    throw error;
  }
};

export const getMaterialClassIds = async () => {
  const response = await api.get('MaterialClasses/GetMaterialClassIds');
  return response.data;
}