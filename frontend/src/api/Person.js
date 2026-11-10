import api from "./Axios";
export const getPerson = async (pageIndex = 1, pageSize = 30) => {
  const response = await api.get('Person', {
    params: {
      PageIndex: pageIndex,
      PageSize: pageSize,
    }
  });
  return response.data;
};

export const getPersonPropertyNames = async () => {
  try {
    const response = await api.get('Person/GetPersonPropertyNames');
    return response.data;
  } catch (error) {
    throw error;
  }
};

export const getPersonIds = async () => {
  try {
    const response = await api.get('Person/GetPersonIds');
    return response.data;
  } catch (error) {
    throw error;
  }
};  

export const createPerson = async (person) => {
  const response = await api.post("Person/CreatePerson", person);
  return response.data;
}