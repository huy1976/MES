import api from "./Axios";

export const createPersonCapability = async (personCapability) => {
 const response = await api.post("Person/CreatePersonnelCapability", personCapability);
 return response.data;
};

export const getPersonelCapability = async (pageIndex = 1, pageSize = 30) => {
  const response = await api.get('Person/GetPersonnelCapabilities', {
    params: {
        PageIndex: pageIndex,
        PageSize: pageSize,
    }
  });
  return response.data;
}

export const getPersonCapabilityPropertyNames = async () => {
  try {
    const response = await api.get('Person/GetPersonnelCapabilityPropertyNames');
    return response.data;
  }catch (error) {
    throw error;
  }
};  