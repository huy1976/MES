import api from "./Axios";
export const getPersonGroups = async (pageIndex = 1, pageSize = 30) => {
  const response = await api.get('PersonnelClass/GetPersonnelClasses', {
    params: {
      pageIndex,
      pageSize
    }
  });
  return response.data;
};

export const getPersonnelClassIds = async () => {
  try {
    const response = await api.get('PersonnelClass/GetPersonnelClassIds');  
    return response.data;
  } catch (error) {
    throw error;
  }
};  

export const getPersonnelClassPropertyNames = async () => {
  try {
    const response = await api.get('PersonnelClass/GetPersonnelClassPropertyNames');
    return response.data;
  } catch (error) {
    throw error;
  }
};

export const createPersonnelClass = async (personnelClassData) => {
  try {
    const response = await api.post('PersonnelClass/CreatePersonnelClass', personnelClassData);
    return response.data;
  } catch (error) {
    throw error;
  }
};  

