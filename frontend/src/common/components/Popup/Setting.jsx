import React, { useState, useEffect, useCallback } from 'react';
import './Setting.css'; 
import { getAccessToken } from './../../../api/Axios';
import { useAuth } from '../../components/Login/AuthContext';

// --- GIẢ LẬP: Hàm giải mã Token (Giữ nguyên) ---
const decodeToken = (token) => {
    if (!token) return null;
    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return {
            userName: payload.name || 'Unknown User',
            role: payload.role || 'Unknown Role',
        };
    } catch (error) {
        console.error('Lỗi giải mã token:', error);
    }
    return null;
};



// --- MAIN COMPONENT ---
const SettingPopup = ({ isOpen, onClose }) => {
    const [userInfo, setUserInfo] = useState(null);
    const { logout } = useAuth();
    // Sử dụng useCallback cho hàm handleLogout để tránh re-render không cần thiết
    const handleLogout = useCallback(() => {
        console.log('Đang thực hiện đăng xuất...');
        // 1. Xóa Token khỏi localStorage/Cookies
        localStorage.removeItem('authToken'); 
        logout();
        // 2. Chuyển hướng người dùng về trang Đăng nhập
        window.location.href = '/login'; 
    }, [logout]);

    // 1. Lấy thông tin người dùng từ Token
    useEffect(() => {
        if (isOpen) {
            const token = getAccessToken();
            const user = decodeToken(token);
            setUserInfo(user);
        }
    }, [isOpen]); // CHỈ chạy khi popup được MỞ

    if (!isOpen) return null;

    return (
        // Modal Overlay (phần nền mờ)
        <div className="modal-overlay" onClick={onClose}>
            {/* Modal Content (phần hộp chứa nội dung) */}
            <div className="modal-content" onClick={(e) => e.stopPropagation()}> 
                
                <div className="modal-header">
                    {/* Đã xóa icon để giống giao diện gốc */}
                    <h3>Thông tin Tài khoản</h3>
                    <button className="close-btn" onClick={onClose}>✕</button>
                </div>
                
                <div className="modal-body">
                    {userInfo ? (
                        <div className="user-info-detail">
                            
                            {/* Cải thiện 1: Nhất quán Tiếng Việt và nhấn mạnh giá trị */}
                            <div className="info-item">
                                <span className="label">👤 Tên người dùng:</span>
                                {/* Đã sửa lỗi: Loại bỏ label trùng lặp trong thẻ value */}
                                <span className="value">👤 Tên người dùng: {userInfo.userName}</span>
                            </div>
                            
                            {/* Cải thiện 2: Nhất quán Tiếng Việt và nhấn mạnh giá trị */}
                            <div className="info-item">
                                <span className="label">🛡️ Vai trò:</span> 
                                {/* Đã sửa lỗi: Loại bỏ label trùng lặp trong thẻ value */}
                                <span className="value">🛡️ Vai trò: {userInfo.role}</span>
                            </div>
                            
                        </div>
                    ) : (
                        <p className="loading-error">Không thể tải thông tin người dùng hoặc chưa đăng nhập.</p>
                    )}
                </div>

                <div className="modal-footer">
                    {/* Cải thiện 3: Nhất quán Tiếng Việt */}
                    <button 
                        className="logout-button" 
                        onClick={handleLogout}
                    >
                        Đăng xuất
                    </button>
                </div>
            </div>
        </div>
    );
};

export default SettingPopup;