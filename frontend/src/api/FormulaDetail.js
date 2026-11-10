import api from "./Axios";
export const getFormualaDetail = async (operationId) => {
  const response = await api.get(
    `OperationsDefinition/GetOperationsDefinitionById/${operationId}`
  );
  return response.data; 
};