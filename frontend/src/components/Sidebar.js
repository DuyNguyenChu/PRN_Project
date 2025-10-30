import React, { useState, useEffect } from 'react';
import { NavLink, useLocation } from 'react-router-dom';
import axios from 'axios';
import { API_URL } from '~/api/api';

// --- (SỬA) ĐỊNH NGHĨA API URL CỦA BẠN TẠI ĐÂY ---
// Tôi đang giả định API này giống hệt API bạn dùng trong Menu.js
const MENU_API_URL = `${API_URL}/User/menus`;
const PERMISSIONS_API_URL = `${API_URL}/User/permissions`;
// ------------------------------------------------

/**
 * Component con, tự render 1 link hoặc 1 dropdown
 * (Component này được gọi đệ quy, nhưng API của bạn chỉ có 2 cấp)
 */
function SidebarMenuItem({ item, location }) {
    // Logic 1: Menu này có con không?
    const hasChildren = item.child && item.child.length > 0;

    // Logic 2: Menu này có phải là dropdown không?
    // (Là dropdown nếu có con HOẶC nếu url = #)
    const isDropdown = hasChildren || item.url === '#';

    if (isDropdown) {
        // --- TRƯỜNG HỢP 1: RENDER DROPDOWN ---

        // Logic 3: Tính toán 'active' cho dropdown
        // (Active nếu route hiện tại là 1 trong các 'url' của con)
        let isActive = false;
        if (hasChildren) {
            const childPaths = item.child.map((child) => child.url);
            isActive = childPaths.includes(location.pathname);
        }

        return (
            <div className="nav-item dropdown">
                <a
                    href="#"
                    // (SỬA) Tự động active
                    className={`nav-link dropdown-toggle ${isActive ? 'active' : ''}`}
                    data-bs-toggle="dropdown"
                >
                    {/* (SỬA) Thêm icon default nếu API không trả về */}
                    <i className={`${item.icon || 'fa fa-folder'} me-2`}></i>
                    <span>{item.name}</span>
                </a>
                <div className={`dropdown-menu bg-transparent border-0 ${isActive ? 'show' : ''}`}>
                    {/* (SỬA) Tự động render các con */}
                    {item.child
                        .sort((a, b) => a.sortOrder - b.sortOrder) // Sắp xếp con
                        .map((childItem) => (
                            <NavLink key={childItem.id} to={childItem.url} className="dropdown-item">
                                {childItem.name}
                            </NavLink>
                        ))}
                </div>
            </div>
        );
    } else {
        // --- TRƯỜNG HỢP 2: RENDER LINK ĐƠN ---
        return (
            <NavLink to={item.url} className="nav-item nav-link">
                <i className={`${item.icon || 'fa fa-link'} me-2`}></i>
                <span>{item.name}</span>
            </NavLink>
        );
    }
}

// --- Component Sidebar Chính ---
export default function Sidebar() {
    const location = useLocation();

    // (SỬA) Thêm State để lưu menu
    const [menus, setMenus] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const userDataString = localStorage.getItem('userData');
    const userData = JSON.parse(userDataString);

    // (SỬA) Thêm useEffect để gọi API
    useEffect(() => {
        const fetchMenus = async () => {
            setLoading(true);
            try {
                const token = userData?.resources?.accessToken;
                if (!token) {
                    throw new Error('Không tìm thấy token.');
                }

                // 3. Gọi API
                const res = await axios.get(MENU_API_URL, {
                    headers: {
                        'Content-Type': 'application/json',
                        Authorization: `Bearer ${token}`,
                    },
                });
                localStorage.setItem('menus', JSON.stringify(res.data.resources || []));

                setMenus(res.data.resources || []);
            } catch (err) {
                console.error('Lỗi tải sidebar menu:', err);
                setError(err.message || 'Lỗi tải menu.');
            } finally {
                setLoading(false);
            }
        };

        // (MỚI) Hàm lấy Permissions và lưu vào localStorage
        const fetchUserPermissions = async () => {
            try {
                // 1. Lấy token
                const userDataString = localStorage.getItem('userData');
                const userData = JSON.parse(userDataString);
                const token = userData?.resources?.accessToken;
                if (!token) {
                    throw new Error('Không tìm thấy token cho permissions.');
                }

                // 2. Gọi API Permissions (Giả định là GET)
                const res = await axios.get(PERMISSIONS_API_URL, {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });

                // 3. Lưu vào localStorage
                const permissionsData = res.data.resources || [];
                localStorage.setItem('Permissions', JSON.stringify(permissionsData));
            } catch (err) {
                localStorage.setItem('Permissions', '[]'); // Lưu mảng rỗng nếu lỗi
            }
        };

        fetchMenus();
        fetchUserPermissions();
    }, []); // Chỉ chạy 1 lần khi component mount

    return (
        <nav className="navbar bg-light navbar-light">
            <style>
                {`
                .navbar-nav .nav-link {
                    /* Cho phép text được xuống dòng */
                    white-space: normal; 
                    word-wrap: break-word; 

                    /* Đảm bảo icon và text căn chỉnh đẹp khi xuống dòng */
                    display: flex;
                    align-items: center; /* Căn giữa icon và khối text */
                }
                .navbar-nav .nav-link i {
                    /* Giữ icon cố định, không bị ảnh hưởng bởi text */
                    flex-shrink: 0;
                }
                .navbar-nav .nav-link span {
                    /* Cho phép khối text tự xử lý xuống dòng */
                    display: inline-block;
                    line-height: 1.25; /* Giảm khoảng cách dòng nếu text bị ngắt */
                }
                `}
            </style>
            {/* ... (Phần Brand và User Profile giữ nguyên) ... */}
            <NavLink to="/dashboard" className="navbar-brand mx-4 mb-3">
                <h3 className="text-primary">
                    <i className="fa fa-hashtag me-2"></i>G - CAR
                </h3>
            </NavLink>
            <div className="d-flex align-items-center ms-4 mb-4">
                <div className="position-relative">
                    <img
                        className="rounded-circle"
                        src="/assets/img/user.jpg"
                        alt=""
                        style={{ width: '40px', height: '40px' }}
                    />
                    <div className="bg-success rounded-circle border border-2 border-white position-absolute end-0 bottom-0 p-1"></div>
                </div>
                <div className="ms-3">
                    <h6 className="mb-0">{userData.resources.userInfo.fullName}</h6>
                    <span>
                        {
                            // 1. Dùng .map() để tạo mảng tên: [{...}] -> ["Admin", "User"]
                            userData.resources.userInfo.roles
                                .map((role) => role.name)

                                // 2. Dùng .join() để biến mảng thành chuỗi: ["Admin", "User"] -> "Admin, User"
                                .join(', ')
                        }
                    </span>
                </div>
            </div>

            {/* (SỬA) Thay thế toàn bộ nav-links bằng logic động */}
            <div className="navbar-nav w-100">
                {loading && (
                    <div className="nav-item nav-link text-muted">
                        <i className="fa fa-spinner fa-spin me-2"></i>Đang tải...
                    </div>
                )}

                {error && (
                    <div className="nav-item nav-link text-danger">
                        <i className="fa fa-exclamation-triangle me-2"></i>
                        {error}
                    </div>
                )}

                {!loading &&
                    !error &&
                    menus
                        .sort((a, b) => a.sortOrder - b.sortOrder) // Sắp xếp menu cấp 1
                        .map((item) => <SidebarMenuItem key={item.id} item={item} location={location} />)}
            </div>
        </nav>
    );
}
