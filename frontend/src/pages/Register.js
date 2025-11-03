import React, { useState } from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import axios from 'axios';
import { API_URL } from '~/api/api';

// Import hook và các validators đã có
import { useFormValidation } from '../validator/useFormValidation'; // Sửa lại đường dẫn nếu cần
import { required, maxLength, emailValidator, minLength, phoneValidator } from '../validator/validators'; // Sửa lại đường dẫn nếu cần

// --- (MỚI) Định nghĩa API URL ---
const REGISTER_API_URL = `${API_URL}/Auth/register`; // !! THAY THẾ BẰNG API ĐĂNG KÝ CỦA BẠN !!

const EyeIcon = () => (
    <svg
        xmlns="http://www.w3.org/2000/svg"
        width="16"
        height="16"
        fill="currentColor"
        className="bi bi-eye-fill"
        viewBox="0 0 16 16"
    >
        <path d="M10.5 8a2.5 2.5 0 1 1-5 0 2.5 2.5 0 0 1 5 0z" />
        <path d="M0 8s3-5.5 8-5.5S16 8 16 8s-3 5.5-8 5.5S0 8 0 8zm8 3.5a3.5 3.5 0 1 0 0-7 3.5 3.5 0 0 0 0 7z" />
    </svg>
);

const EyeSlashIcon = () => (
    <svg
        xmlns="http://www.w3.org/2000/svg"
        width="16"
        height="16"
        fill="currentColor"
        className="bi bi-eye-slash-fill"
        viewBox="0 0 16 16"
    >
        <path d="m10.79 12.912-1.614-1.615a3.5 3.5 0 0 1-4.474-4.474l-2.06-2.06C.938 6.278 0 8 0 8s3 5.5 8 5.5a7.029 7.029 0 0 0 2.79-.588zM5.21 3.088A7.028 7.028 0 0 1 8 2.5c5 0 8 5.5 8 5.5s-.939 1.721-2.641 3.238l-2.062-2.062a3.5 3.5 0 0 0-4.474-4.474L5.21 3.089z" />
        <path d="M5.525 7.646a2.5 2.5 0 0 0 2.829 2.829l-2.83-2.829zm4.95.708-2.829-2.83a2.5 2.5 0 0 1 2.829 2.829zm3.171 6-12-12 .708-.708 12 12-.708.708z" />
    </svg>
);

export default function Register() {
    const [showNewPassword, setShowNewPassword] = useState(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);

    // (MỚI) State cho API
    const [isLoading, setIsLoading] = useState(false);
    const [apiMessage, setApiMessage] = useState({ type: '', text: '' }); // 'success' hoặc 'error'

    // (MỚI) Hook điều hướng
    const navigate = useNavigate();

    // --- (MỚI) Tích hợp Hook Validation ---
    const initialState = {
        email: '',
        password: '',
        confirmPassword: '',
        firstName: '',
        lastName: '',
        phoneNumber: '',
    };

    const validationRules = {
        email: [required, emailValidator],
        password: [required, minLength(10)], // Yêu cầu tối thiểu 6 ký tự
        confirmPassword: [required], // Sẽ kiểm tra khớp ở handleSubmit
        firstName: [required, maxLength(50)],
        lastName: [required, maxLength(50)],
        phoneNumber: [required, phoneValidator],
    };

    const { values, errors, handleChange, validateForm, isSubmitDisabled } = useFormValidation(
        initialState,
        validationRules,
    );

    const togglePasswordVisibility = (field) => {
        if (field === 'new') {
            setShowNewPassword((prev) => !prev);
        } else if (field === 'confirm') {
            setShowConfirmPassword((prev) => !prev);
        }
    };

    const eyeIconStyle = {
        position: 'absolute',
        top: '50%',
        right: '1rem', // Khoảng cách từ lề phải
        transform: 'translateY(-50%)',
        cursor: 'pointer',
        zIndex: 3, // Đảm bảo icon nằm trên input
    };

    // --- (MỚI) Hàm Submit ---
    const handleSubmit = async (e) => {
        e.preventDefault();
        setApiMessage({ type: '', text: '' }); // Xóa thông báo cũ

        // 1. Chạy validation cơ bản của hook
        const isFormHookValid = validateForm();
        if (!isFormHookValid) {
            return; // Dừng nếu các trường (required, format) chưa hợp lệ
        }

        // 2. Kiểm tra thủ công (vì hook không hỗ trợ)
        if (values.password !== values.confirmPassword) {
            setApiMessage({ type: 'error', text: 'Mật khẩu xác nhận không khớp.' });
            // Bạn có thể set lỗi cho trường confirmPassword nếu hook có hỗ trợ setErrors
            // passwordForm.setErrors(prev => ({...prev, confirmPassword: 'Mật khẩu không khớp'}))
            return;
        }

        // 3. Tiến hành gọi API
        setIsLoading(true);
        try {
            const payload = {
                userName: values.email, // API yêu cầu userName = email
                email: values.email,
                password: values.password,
                confirmPassword: values.confirmPassword,
                firstName: values.firstName,
                lastName: values.lastName,
                phoneNumber: values.phoneNumber,
            };

            await axios.post(REGISTER_API_URL, payload);

            // Thành công
            setApiMessage({ type: 'success', text: 'Đăng ký thành công! Đang chuyển hướng đến trang đăng nhập...' });

            // Chờ 2 giây rồi chuyển hướng
            setTimeout(() => {
                navigate('/login');
            }, 2000);
        } catch (err) {
            // Thất bại
            const errorMsg = err.response?.data?.message || 'Đã xảy ra lỗi. Vui lòng thử lại.';
            setApiMessage({ type: 'error', text: errorMsg });
            setIsLoading(false);
        }
        // Không set isLoading(false) ở đây nếu thành công, vì đang chuyển hướng
    };

    return (
        <div className="container-fluid">
            <div className="row h-100 align-items-center justify-content-center" style={{ minHeight: '100vh' }}>
                <div className="col-12 col-sm-8 col-md-6 col-lg-5 col-xl-4">
                    <div className="bg-light rounded p-4 p-sm-5 my-4 mx-3">
                        <div className="d-flex align-items-center justify-content-between mb-3">
                            <a href="index.html" className="">
                                <h3 className="text-primary">
                                    <i className="fa fa-car me-2"></i>G-CAR
                                </h3>
                            </a>
                            <h3>Đăng ký</h3>
                        </div>
                        {/* (MỚI) Form và Thông báo API */}
                        <form onSubmit={handleSubmit} noValidate>
                            {apiMessage.text && (
                                <div
                                    className={`alert ${
                                        apiMessage.type === 'success' ? 'alert-success' : 'alert-danger'
                                    }`}
                                    role="alert"
                                >
                                    {apiMessage.text}
                                </div>
                            )}

                            {/* Email / Username */}
                            <div className="form-floating mb-3">
                                <input
                                    type="email"
                                    className={`form-control ${errors.email ? 'is-invalid' : ''}`}
                                    id="email"
                                    name="email"
                                    placeholder="name@example.com"
                                    value={values.email}
                                    onChange={handleChange}
                                />
                                <label htmlFor="email">Email</label>
                                {errors.email && <div className="invalid-feedback">{errors.email}</div>}
                            </div>

                            {/* Mật khẩu */}
                            <div className="form-floating mb-4 position-relative">
                                <input
                                    type={showNewPassword ? 'text' : 'password'}
                                    className={`form-control ${errors.password ? 'is-invalid' : ''}`}
                                    id="password"
                                    name="password"
                                    placeholder="Password"
                                    value={values.password}
                                    onChange={handleChange}
                                />
                                <label htmlFor="password">Mật khẩu</label>
                                <span style={eyeIconStyle} onClick={() => togglePasswordVisibility('new')}>
                                    {showNewPassword ? <EyeSlashIcon /> : <EyeIcon />}
                                </span>
                                {errors.password && <div className="invalid-feedback">{errors.password}</div>}
                            </div>

                            {/* Xác nhận Mật khẩu */}
                            <div className="form-floating mb-4 position-relative">
                                <input
                                    type={showConfirmPassword ? 'text' : 'password'}
                                    className={`form-control ${errors.confirmPassword ? 'is-invalid' : ''}`}
                                    id="confirmPassword"
                                    name="confirmPassword"
                                    placeholder="Confirm Password"
                                    value={values.confirmPassword}
                                    onChange={handleChange}
                                />
                                <label htmlFor="confirmPassword">Xác nhận mật khẩu</label>
                                <span style={eyeIconStyle} onClick={() => togglePasswordVisibility('confirm')}>
                                    {showConfirmPassword ? <EyeSlashIcon /> : <EyeIcon />}
                                </span>
                                {errors.confirmPassword && (
                                    <div className="invalid-feedback">{errors.confirmPassword}</div>
                                )}
                            </div>

                            {/* Họ */}
                            <div className="form-floating mb-3">
                                <input
                                    type="text"
                                    className={`form-control ${errors.firstName ? 'is-invalid' : ''}`}
                                    id="firstName"
                                    name="firstName"
                                    placeholder="Họ"
                                    value={values.firstName}
                                    onChange={handleChange}
                                />
                                <label htmlFor="firstName">Họ</label>
                                {errors.firstName && <div className="invalid-feedback">{errors.firstName}</div>}
                            </div>

                            {/* Tên */}
                            <div className="form-floating mb-3">
                                <input
                                    type="text"
                                    className={`form-control ${errors.lastName ? 'is-invalid' : ''}`}
                                    id="lastName"
                                    name="lastName"
                                    placeholder="Tên"
                                    value={values.lastName}
                                    onChange={handleChange}
                                />
                                <label htmlFor="lastName">Tên</label>
                                {errors.lastName && <div className="invalid-feedback">{errors.lastName}</div>}
                            </div>

                            {/* Số điện thoại */}
                            <div className="form-floating mb-3">
                                <input
                                    type="tel"
                                    className={`form-control ${errors.phoneNumber ? 'is-invalid' : ''}`}
                                    id="phoneNumber"
                                    name="phoneNumber"
                                    placeholder="Số điện thoại"
                                    value={values.phoneNumber}
                                    onChange={handleChange}
                                />
                                <label htmlFor="phoneNumber">Số điện thoại</label>
                                {errors.phoneNumber && <div className="invalid-feedback">{errors.phoneNumber}</div>}
                            </div>

                            {/* Bỏ Checkbox "Ghi nhớ tôi" và "Quên mật khẩu" trong trang Đăng ký */}
                            {/* <div className="d-flex align-items-center justify-content-between mb-4">
                                <div className="form-check">...</div>
                                <a href="">Quên mật khẩu</a>
                            </div> 
                            */}

                            <button
                                type="submit"
                                className="btn btn-primary py-3 w-100 mb-4"
                                disabled={isLoading || isSubmitDisabled}
                            >
                                {isLoading ? (
                                    <>
                                        <span
                                            className="spinner-border spinner-border-sm"
                                            role="status"
                                            aria-hidden="true"
                                        ></span>
                                        <span className="ms-1">Đang xử lý...</span>
                                    </>
                                ) : (
                                    'Đăng ký'
                                )}
                            </button>
                        </form>
                        <p className="text-center mb-0">
                            Đã có tài khoản? <NavLink to="/login">Đăng nhập</NavLink>
                        </p>
                    </div>
                </div>
            </div>
        </div>
    );
}
