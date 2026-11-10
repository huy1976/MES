import { useState, useEffect } from "react";
import api from "../api/Axios"; // import instance axios hoặc cấu hình fetch

/**
 * Hook lấy quyền của người dùng theo pageId
 * @param {number} pageId - ID của trang cần kiểm tra quyền
 */
export function useAuthPermissions(pageId) {
  const [permissions, setPermissions] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!pageId) return; // nếu không truyền pageId thì không gọi API

    async function fetchPermissions() {
      try {
        setLoading(true);

        const response = await api.get("/Auth/GetPermissions"); // axios instance

        const data = response.data; // nếu dùng fetch thì là await response.json()

        // Lọc quyền cho pageId được truyền vào
        const pagePermissions = data.permissions?.filter(
          (p) => p.pageId === pageId
        );

        setPermissions(pagePermissions || []);
      } catch (err) {
        console.error("Failed to fetch permissions:", err);
        setError(err);
      } finally {
        setLoading(false);
      }
    }

    fetchPermissions();
  }, [pageId]);

  return { permissions, loading, error };
}
