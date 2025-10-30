import React from 'react';
import { NavLink,useNavigate } from 'react-router-dom';
import { API_URL } from '~/api/api';

export default function Header({ onToggleSidebar }) {
    const navigate = useNavigate();

    // Hàm xử lý đăng xuất
    const handleLogout = async (event) => {
        // Ngăn chặn hành vi mặc định của thẻ <a>
        event.preventDefault();

        const userDataString = localStorage.getItem('userData');

        try {
            const userData = JSON.parse(userDataString); // Kiểm tra dữ liệu người dùng
            const refreshToken = userData?.resources.refreshToken;

            if (!refreshToken) {
                throw new Error('Không tìm thấy refreshToken.');
            }

            // Gọi API để thông báo cho server về việc đăng xuất (ví dụ: hủy token)
            // Bạn có thể cần thêm header Authorization nếu API yêu cầu
            const response = await fetch(`${API_URL}/Auth/logout`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    // 'Authorization': `Bearer ${token}` // Gửi token nếu cần
                },
                body: JSON.stringify({ refreshToken: refreshToken }),
            });

            // Nếu API trả về lỗi, ném ra một lỗi để catch xử lý
            if (!response.ok) {
                throw new Error('Đăng xuất thất bại từ phía server.');
            }

            // Chỉ xóa localStorage và chuyển hướng khi API chạy thành công
            localStorage.clear();
            navigate('/login');
        } catch (error) {
            console.error('Lỗi khi đăng xuất:', error);
            // Có thể hiển thị thông báo lỗi cho người dùng ở đây nếu cần
            // Quan trọng: không xóa localStorage nếu API thất bại
        }
    };

    return (
        <nav className="navbar navbar-expand bg-light navbar-light sticky-top px-4 py-0">
            <a href="index.html" className="navbar-brand d-flex d-lg-none me-4">
                <h2 className="text-primary mb-0">
                    <i className="fa fa-hashtag"></i>
                </h2>
            </a>
            <a href="#" className="sidebar-toggler flex-shrink-0" onClick={onToggleSidebar}>
                <i className="fa fa-bars"></i>
            </a>
            <form className="d-none d-md-flex ms-4">
                <input className="form-control border-0" type="search" placeholder="Search" />
            </form>
            <div className="navbar-nav align-items-center ms-auto">
                <div className="nav-item dropdown">
                    <a href="#" className="nav-link dropdown-toggle" data-bs-toggle="dropdown">
                        <i className="fa fa-envelope me-lg-2"></i>
                        <span className="d-none d-lg-inline-flex">Message</span>
                    </a>
                    <div className="dropdown-menu dropdown-menu-end bg-light border-0 rounded-0 rounded-bottom m-0">
                        <a href="#" className="dropdown-item">
                            <div className="d-flex align-items-center">
                                <img
                                    className="rounded-circle"
                                    src="/assets/img/user.jpg"
                                    alt=""
                                    style={{ width: '40px', height: '40px' }}
                                />
                                <div className="ms-2">
                                    <h6 className="fw-normal mb-0">Jhon send you a message</h6>
                                    <small>15 minutes ago</small>
                                </div>
                            </div>
                        </a>
                        <hr className="dropdown-divider" />
                        <a href="#" className="dropdown-item">
                            <div className="d-flex align-items-center">
                                <img
                                    className="rounded-circle"
                                    src="/assets/img/user.jpg"
                                    alt=""
                                    style={{ width: '40px', height: '40px' }}
                                />
                                <div className="ms-2">
                                    <h6 className="fw-normal mb-0">Jhon send you a message</h6>
                                    <small>15 minutes ago</small>
                                </div>
                            </div>
                        </a>
                        <hr className="dropdown-divider" />
                        <a href="#" className="dropdown-item">
                            <div className="d-flex align-items-center">
                                <img
                                    className="rounded-circle"
                                    src="/assets/img/user.jpg"
                                    alt=""
                                    style={{ width: '40px', height: '40px' }}
                                />
                                <div className="ms-2">
                                    <h6 className="fw-normal mb-0">Jhon send you a message</h6>
                                    <small>15 minutes ago</small>
                                </div>
                            </div>
                        </a>
                        <hr className="dropdown-divider" />
                        <a href="#" className="dropdown-item text-center">
                            See all message
                        </a>
                    </div>
                </div>
                <div className="nav-item dropdown">
                    <a href="#" className="nav-link dropdown-toggle" data-bs-toggle="dropdown">
                        <i className="fa fa-bell me-lg-2"></i>
                        <span className="d-none d-lg-inline-flex">Notificatin</span>
                    </a>
                    <div className="dropdown-menu dropdown-menu-end bg-light border-0 rounded-0 rounded-bottom m-0">
                        <a href="#" className="dropdown-item">
                            <h6 className="fw-normal mb-0">Profile updated</h6>
                            <small>15 minutes ago</small>
                        </a>
                        <hr className="dropdown-divider" />
                        <a href="#" className="dropdown-item">
                            <h6 className="fw-normal mb-0">New user added</h6>
                            <small>15 minutes ago</small>
                        </a>
                        <hr className="dropdown-divider" />
                        <a href="#" className="dropdown-item">
                            <h6 className="fw-normal mb-0">Password changed</h6>
                            <small>15 minutes ago</small>
                        </a>
                        <hr className="dropdown-divider" />
                        <a href="#" className="dropdown-item text-center">
                            See all notifications
                        </a>
                    </div>
                </div>
                <div className="nav-item dropdown">
                    <a href="#" className="nav-link dropdown-toggle" data-bs-toggle="dropdown">
                        <img
                            className="rounded-circle me-lg-2"
                            src="/assets/img/user.jpg"
                            alt=""
                            style={{ width: '40px', height: '40px' }}
                        />
                        <span className="d-none d-lg-inline-flex">John Doe</span>
                    </a>
                    <div className="dropdown-menu dropdown-menu-end bg-light border-0 rounded-0 rounded-bottom m-0">
                        <NavLink to="/profile" className="dropdown-item">
                            My Profile
                        </NavLink>
                        <a href="#" className="dropdown-item">
                            Settings
                        </a>
                        <a href="#" className="dropdown-item" onClick={handleLogout}>
                            Log Out
                        </a>
                    </div>
                </div>
            </div>
        </nav>
    );
}
