import api from "./Axios";

export const Login = async (username, password) => {
  const response = await api.post("Auth/Login", {
    username,
    password
  });

  const data = response.data;

  // Lấy access token từ dữ liệu backend trả về
  const accessToken =
    data?.accessToken ||
    data?.AccessToken ||
    
    data?.token ||
    data?.Token;

  // Lưu JWT để những API khác, như chatbot, có thể sử dụng
  if (accessToken) {
    localStorage.setItem("accessToken", accessToken);
  } else {
    console.warn(
      "Đăng nhập thành công nhưng không tìm thấy access token trong response:",
      data
    );
  }

  return data;
};