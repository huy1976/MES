import api from "./Axios";
export const getProductionParameters = async (pageIndex = 1, pageSize = 30) => {
  const response = await api.get('ProductionParameter/GetProductionParameters', {
    params: {
      pageIndex,
      pageSize
    }
  });
  return response.data;
};
export const getProductionParameterPropertyNames = async () => {
  const response = await api.get('ProductionParameter/GetProductionParameterPropertyNames');
  return response.data;
}
export const createProductionParameter = async (parameterData) => {
  const response = await api.post('ProductionParameter/CreateProductionParameter', parameterData);
  return response.data;
}

export const getProductionParameterIds = async () => {
  const response = await api.get('ProductionParameter/GetProductionParameterIds');
  return response.data;
}
