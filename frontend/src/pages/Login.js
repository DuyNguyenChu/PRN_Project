import React, { useState, useEffect } from 'react';
import { NavLink, useNavigate, useLocation } from 'react-router-dom';
import { API_URL } from '~/api/api';
import { useFormValidation } from '../validator/useFormValidation'; // Sửa lại đường dẫn nếu cần
import { required } from '../validator/validators'; // Sửa lại đường dẫn nếu cần

// --- LOGIN COMPONENT ---
export default function Login() {
    // Định nghĩa các quy tắc validate
    const validationRules = {
        username: [required],
        password: [required],
    };

    // Sử dụng custom hook
    const { values, errors, handleChange, validateForm, isSubmitDisabled } = useFormValidation(
        { username: '', password: '' },
        validationRules,
    );

    // State riêng để hiển thị lỗi từ API
    const [apiError, setApiError] = useState('');

    // State để kiểm soát việc hiển thị mật khẩu
    const [showPassword, setShowPassword] = useState(false);

    // Hook để điều hướng trang
    const navigate = useNavigate();
    const location = useLocation();

    useEffect(() => {
        if (location.state?.message) {
            setApiError(location.state.message);
            window.history.replaceState({}, document.title);
        }
    }, [location]);

    // Hàm xử lý khi người dùng nhấn nút đăng nhập
    const handleLogin = async (event) => {
        event.preventDefault();
        setApiError(''); // Xóa lỗi API cũ

        // Bước 1: Validate form trước khi gửi
        const isFormValid = validateForm();
        if (!isFormValid) {
            return; // Dừng lại nếu form không hợp lệ
        }

        // Bước 2: Gọi API
        try {
            const response = await fetch(`${API_URL}/Auth/admin/login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ username: values.username, password: values.password }),
            });

            const data = await response.json();

            if (response.ok) {
                localStorage.setItem('userData', JSON.stringify(data));
                navigate('/dashboard');
            } else {
                setApiError(data.message || 'Tài khoản hoặc mật khẩu không đúng');
            }
        } catch (err) {
            setApiError('Đã có lỗi xảy ra. Vui lòng thử lại sau.');
            console.error('Login API call failed:', err);
        }
    };

    return (
        <div className="container-fluid">
            <div className="row h-100 align-items-center justify-content-center" style={{ minHeight: '100vh' }}>
                <div className="col-12 col-sm-8 col-md-6 col-lg-5 col-xl-4">
                    <div className="bg-light rounded p-4 p-sm-5 my-4 mx-3">
                        <div className="d-flex align-items-center justify-content-between mb-3">
                            <h3 className="text-primary">
                                <i className="fa fa-car me-2"></i>G-CAR
                            </h3>
                            <h3>Đăng nhập</h3>
                        </div>

                        <form onSubmit={handleLogin}>
                            <div className="form-floating mb-3">
                                <input
                                    type="text"
                                    className={`form-control ${errors.username ? 'is-invalid' : ''}`}
                                    id="floatingInput"
                                    name="username" // Thêm name để hook hoạt động
                                    placeholder="Username"
                                    value={values.username}
                                    onChange={handleChange}
                                />
                                <label htmlFor="floatingInput">Tên đăng nhập</label>
                                {errors.username && (
                                    <div className="text-danger mt-1" style={{ fontSize: '0.875em' }}>
                                        {errors.username}
                                    </div>
                                )}
                            </div>

                            <div className="form-floating mb-4 position-relative">
                                <input
                                    type={showPassword ? 'text' : 'password'}
                                    className={`form-control ${errors.password ? 'is-invalid' : ''}`}
                                    id="floatingPassword"
                                    name="password" // Thêm name để hook hoạt động
                                    placeholder="Password"
                                    value={values.password}
                                    onChange={handleChange}
                                />
                                <label htmlFor="floatingPassword">Mật khẩu</label>
                                <i
                                    className={`fa ${showPassword ? 'fa-eye-slash' : 'fa-eye'}`}
                                    onClick={() => setShowPassword(!showPassword)}
                                    style={{
                                        position: 'absolute',
                                        top: '50%',
                                        right: '15px',
                                        transform: 'translateY(-50%)',
                                        cursor: 'pointer',
                                        color: '#555',
                                    }}
                                ></i>
                                {errors.password && (
                                    <div className="text-danger mt-1" style={{ fontSize: '0.875em' }}>
                                        {errors.password}
                                    </div>
                                )}
                            </div>

                            {/* Hiển thị thông báo lỗi từ API */}
                            {apiError && <p className="text-danger text-center mb-4">{apiError}</p>}

                            <div className="d-flex align-items-center justify-content-between mb-4">
                                <div className="form-check">
                                    <input type="checkbox" className="form-check-input" id="exampleCheck1" />
                                    <label className="form-check-label" htmlFor="exampleCheck1">
                                        Ghi nhớ tôi
                                    </label>
                                </div>
                                <a href="#">Quên mật khẩu?</a>
                            </div>
                            <button
                                type="submit"
                                className="btn btn-primary py-3 w-100 mb-4"
                                disabled={isSubmitDisabled}
                            >
                                Đăng nhập
                            </button>
                        </form>

                        <p className="text-center mb-0">
                            Chưa có tài khoản? <NavLink to="/register">Đăng ký</NavLink>
                        </p>
                    </div>
                </div>
            </div>
        </div>
    );
}
