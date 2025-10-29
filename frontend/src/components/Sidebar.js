import React from 'react';
import { NavLink, useLocation } from 'react-router-dom';

export default function Sidebar() {
    const location = useLocation();

    // ✅ Kiểm tra route hiện tại để set active cho dropdown cha
    const isElementsActive = ['/buttons', '/typography', '/elements'].includes(location.pathname);
    const isPagesActive = ['/login', '/register', '/error', '/blank'].includes(location.pathname);
    const isUserManagementActive = ['/userStatus'].includes(location.pathname);
    const isSystemActive = ['/action'].includes(location.pathname);

    return (
        <nav className="navbar bg-light navbar-light">
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
                    <h6 className="mb-0">Jhon Doe</h6>
                    <span>Admin</span>
                </div>
            </div>
            <div className="navbar-nav w-100">
                <NavLink to="/dashboard" className="nav-item nav-link">
                    <i className="fa fa-tachometer-alt me-2"></i>Dashboard
                </NavLink>
                <div className="nav-item dropdown">
                    <a
                        href="#"
                        className={`nav-link dropdown-toggle ${isElementsActive ? 'active' : ''}`}
                        data-bs-toggle="dropdown"
                    >
                        <i className="fa fa-laptop me-2"></i>Elements
                    </a>
                    <div className="dropdown-menu bg-transparent border-0">
                        <NavLink to="/buttons" className="dropdown-item">
                            Buttons
                        </NavLink>
                        <NavLink to="/typography" className="dropdown-item">
                            Typography
                        </NavLink>
                        <NavLink to="/elements" className="dropdown-item">
                            Other Elements
                        </NavLink>
                    </div>
                </div>
                <NavLink to="/widget" className="nav-item nav-link">
                    <i className="fa fa-th me-2"></i>Widgets
                </NavLink>
                <NavLink to="/forms" className="nav-item nav-link">
                    <i className="fa fa-keyboard me-2"></i>Forms
                </NavLink>
                <NavLink to="/tables" className="nav-item nav-link">
                    <i className="fa fa-table me-2"></i>Tables
                </NavLink>
                <NavLink to="/charts" className="nav-item nav-link">
                    <i className="fa fa-chart-bar me-2"></i>Charts
                </NavLink>
                <div className="nav-item dropdown">
                    <a
                        href="#"
                        className={`nav-link dropdown-toggle ${isPagesActive ? 'active' : ''}`}
                        data-bs-toggle="dropdown"
                    >
                        <i className="far fa-file-alt me-2"></i>Pages
                    </a>
                    <div className="dropdown-menu bg-transparent border-0">
                        <NavLink to="/login" className="dropdown-item">
                            Sign In
                        </NavLink>
                        <NavLink to="/register" className="dropdown-item">
                            Sign Up
                        </NavLink>
                        <NavLink to="/error" className="dropdown-item">
                            404 Error
                        </NavLink>
                        <NavLink to="/blank" className="dropdown-item">
                            Blank Page
                        </NavLink>
                    </div>
                </div>
                <div className="nav-item dropdown">
                    <a
                        href="#"
                        className={`nav-link dropdown-toggle ${isUserManagementActive ? 'active' : ''}`}
                        data-bs-toggle="dropdown"
                    >
                        <i className="fa fa-user me-2"></i>Người dùng
                    </a>
                    <div className="dropdown-menu bg-transparent border-0">
                        <NavLink to="/userStatus" className="dropdown-item">
                            Trạng thái người dùng
                        </NavLink>
                        <NavLink to="/role" className="dropdown-item">
                            Vai trò người dùng
                        </NavLink>
                        <NavLink to="/user" className="dropdown-item">
                            Danh sách người dùng
                        </NavLink>
                    </div>
                </div>
                <div className="nav-item dropdown">
                    <a
                        href="#"
                        className={`nav-link dropdown-toggle ${isSystemActive ? 'active' : ''}`}
                        data-bs-toggle="dropdown"
                    >
                        <i className="fa fa-laptop me-2"></i>Hệ thống
                    </a>
                    <div className="dropdown-menu bg-transparent border-0">
                        <NavLink to="/action" className="dropdown-item">
                            Hành động
                        </NavLink>
                        <NavLink to="/menu" className="dropdown-item">
                            Danh mục hệ thống
                        </NavLink>
                        <NavLink to="/error" className="dropdown-item">
                            404 Error
                        </NavLink>
                        <NavLink to="/blank" className="dropdown-item">
                            Blank Page
                        </NavLink>
                    </div>
                </div>
                <div className="nav-item dropdown">
                    <a
                        href="#"
                        className={`nav-link dropdown-toggle ${isSystemActive ? 'active' : ''}`}
                        data-bs-toggle="dropdown"
                    >
                        <i className="far fa-laptop me-2"></i>Quản Lý xe
                    </a>
                    <div className="dropdown-menu bg-transparent border-0">
                        <NavLink to="/Vehicle" className="dropdown-item">
                            Xe
                        </NavLink>
                        <NavLink to="/VehicleRegistration" className="dropdown-item">
                            Đăng ký xe
                        </NavLink>
                        <NavLink to="/VehicleBranch" className="dropdown-item">
                            Chi nhánh xe
                        </NavLink>
                        <NavLink to="/VehicleStatus" className="dropdown-item">
                            Tình trạng xe
                        </NavLink>
                        <NavLink to="/VehicleModel" className="dropdown-item">
                            Mẫu xe
                        </NavLink>
                    </div>
                </div>
            </div>
        </nav>
    );
}
