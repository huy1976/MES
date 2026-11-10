import api from "./Axios";
export const getOperations = async (pageIndex = 1, pageSize = 30) => {
  const response = await api.get('OperationsDefinition/GetOperationsDefinitions', {
    params: {
      $top: pageSize,
      $skip: (pageIndex - 1) * pageSize
    }
  });
  return response.data;
};