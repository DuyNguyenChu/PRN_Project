import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { API_URL } from '~/api/api';
// (MỚI) Import hook và validators
import { useFormValidation } from '../validator/useFormValidation';
import { required, maxLength, minLength, isNumber } from '../validator/validators';

// (MỚI) SVG Icons cho con mắt
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

export default function Profile() {
    const PROFILE_API_URL = `${API_URL}/User/me`; // API cập nhật (PUT/PATCH) - Giả định cùng URL
    const CHANGE_PASSWORD_API_URL = `${API_URL}/User/me/change-password`;

    // --- (SỬA) State và Validation cho Form Profile ---
    const profileInitialState = {
        firstName: '',
        lastName: '',
        gender: null,
        email: '',
        phoneNumber: '',
    };
    const profileValidationRules = {
        firstName: [required, maxLength(50)], // Ví dụ maxLength 50
        lastName: [required, maxLength(50)],
        phoneNumber: [maxLength(10), isNumber], // Ví dụ maxLength 15
        // gender và email không cần validation ở đây
    };
    const profileForm = useFormValidation(profileInitialState, profileValidationRules);
    // --- Kết thúc sửa ---

    // --- (SỬA) State và Validation cho Form Password ---
    const passwordInitialState = {
        oldPassword: '',
        newPassword: '',
        confirmNewPassword: '',
    };
    // Custom validator cho password match
    const passwordsMatch = (value, values) => {
        if (value !== values.newPassword) {
            return 'Xác nhận mật khẩu không khớp.';
        }
        return null;
    };
    const passwordValidationRules = {
        oldPassword: [required, minLength(10)],
        newPassword: [required, minLength(10)], // Ví dụ minLength(6) nếu có
        confirmNewPassword: [required, (value) => passwordsMatch(value, passwordForm.values), minLength(10)], // Pass values vào validator
    };
    // Sử dụng hook lần 2
    const passwordForm = useFormValidation(passwordInitialState, passwordValidationRules);
    // --- Kết thúc sửa ---

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [token, setToken] = useState('');

    // (MỚI) State cho show/hide password
    const [showOldPassword, setShowOldPassword] = useState(false);
    const [showNewPassword, setShowNewPassword] = useState(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);

    const [isUpdatingProfile, setIsUpdatingProfile] = useState(false);
    const [isUpdatingPassword, setIsUpdatingPassword] = useState(false);

    const [notifyState, setNotifyState] = useState({ show: false, message: '', isSuccess: false });
    // --- Kết thúc State ---

    // 1. Lấy token
    useEffect(() => {
        const userDataString = localStorage.getItem('userData');
        try {
            const userData = JSON.parse(userDataString);
            const accessToken = userData?.resources?.accessToken;
            if (!accessToken) {
                setError('Không tìm thấy token. Vui lòng đăng nhập lại.');
                setLoading(false); // Dừng loading nếu không có token
            } else {
                setToken(accessToken);
            }
        } catch (e) {
            setError('Lỗi đọc dữ liệu người dùng. Vui lòng đăng nhập lại.');
            setLoading(false);
        }
    }, []);

    // 2. Fetch Profile Data khi có token
    useEffect(() => {
        if (!token) return; // Không gọi API nếu không có token

        const fetchProfile = async () => {
            setLoading(true);
            setError(null); // Reset lỗi
            try {
                const res = await axios.get(PROFILE_API_URL, {
                    headers: { Authorization: `Bearer ${token}` },
                });
                const userData = res.data.resources;
                if (userData) {
                    profileForm.setValues({
                        // Gọi hàm setValues trả về từ hook
                        firstName: userData.firstName || '',
                        lastName: userData.lastName || '',
                        gender: userData.gender !== undefined ? String(userData.gender) : null,
                        email: userData.email || '',
                        phoneNumber: userData.phoneNumber || '',
                    });
                } else {
                    throw new Error('Không nhận được dữ liệu người dùng.');
                }
            } catch (err) {
                setError('Lỗi tải thông tin cá nhân: ' + (err.response?.data?.message || err.message));
            } finally {
                setLoading(false);
            }
        };

        fetchProfile();
    }, [token, profileForm.setValues]); // Chạy lại khi token thay đổi

    /// --- Handlers ---
    // (SỬA) Dùng handleChange của profileForm
    const handleProfileChange = profileForm.handleChange;
    // (SỬA) Dùng handleChange của passwordForm
    const handlePasswordChange = passwordForm.handleChange;

    // (SỬA) Cập nhật Profile
    const handleUpdateProfile = async (e) => {
        e.preventDefault();

        if (!profileForm.validateForm()) return;
        setIsUpdatingProfile(true); // Bắt đầu loading
        setError(null); // Reset lỗi cũ (nếu có)
        try {
            // Chuẩn bị payload theo API request
            const payload = {
                firstName: profileForm.values.firstName,
                lastName: profileForm.values.lastName,
                gender: profileForm.values.gender !== null ? Number(profileForm.values.gender) : 0,
                phoneNumber: profileForm.values.phoneNumber,
                // Không gửi email, dateOfBirth nếu API không yêu cầu
            };

            // Gọi API PUT (hoặc PATCH tùy backend)
            await axios.put(PROFILE_API_URL, payload, {
                headers: { Authorization: `Bearer ${token}` },
            });

            setNotifyState({ show: true, message: 'Cập nhật thông tin thành công!', isSuccess: true });
            // Optional: Fetch lại profile nếu cần cập nhật state ngay lập tức
            // fetchProfile();
        } catch (err) {
            const errorMsg = 'Lỗi cập nhật thông tin: ' + (err.response?.data?.message || err.message);
            setError(errorMsg); // Hiển thị lỗi chung ở đầu form
            setNotifyState({ show: true, message: 'Cập nhật thông tin thất bại!', isSuccess: false });
        } finally {
            setIsUpdatingProfile(false); // Kết thúc loading
        }
    };

    // (SỬA) Cập nhật Mật khẩu
    const handleUpdatePassword = async (e) => {
        e.preventDefault();
        if (!passwordForm.validateForm()) return;
        setIsUpdatingPassword(true);
        setError(null);
        try {
            // (SỬA) Lấy values từ passwordForm
            const payload = {
                oldPassword: passwordForm.values.oldPassword,
                newPassword: passwordForm.values.newPassword,
                confirmNewPassword: passwordForm.values.confirmNewPassword,
            };
            await axios.put(CHANGE_PASSWORD_API_URL, payload, { headers: { Authorization: `Bearer ${token}` } });
            setNotifyState({ show: true, message: 'Đổi mật khẩu thành công!', isSuccess: true });
            // (SỬA) Dùng setValues của passwordForm để reset
            passwordForm.setValues(passwordInitialState); // Reset về trạng thái ban đầu
        } catch (err) {
            const errorMsg = 'Lỗi đổi mật khẩu: ' + (err.response?.data?.message || err.message);
            setNotifyState({ show: true, message: errorMsg, isSuccess: false });
        } finally {
            setIsUpdatingPassword(false);
        }
    };

    // (MỚI) Hàm toggle show/hide password
    const togglePasswordVisibility = (field) => {
        if (field === 'old') {
            setShowOldPassword((prev) => !prev);
        } else if (field === 'new') {
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

    // Hàm đóng modal thông báo
    const handleCloseNotify = () => setNotifyState({ show: false, message: '', isSuccess: false });
    // --- Kết thúc Handlers ---
    return (
        <div className="container-fluid pt-4 px-4">
            <div className="row g-4">
                <div className="col-sm-12 col-xl-12">
                    <div className="bg-light rounded h-100 p-4">
                        {/* Tabs */}
                        <nav>
                            {' '}
                            {/* ... giữ nguyên ... */}
                            <div className="nav nav-tabs" id="nav-tab" role="tablist">
                                <button
                                    className="nav-link active"
                                    id="nav-profile-tab"
                                    data-bs-toggle="tab"
                                    data-bs-target="#nav-profile"
                                    type="button"
                                    role="tab"
                                    aria-controls="nav-profile"
                                    aria-selected="true"
                                >
                                    {' '}
                                    Profile{' '}
                                </button>
                                <button
                                    className="nav-link"
                                    id="nav-contact-tab"
                                    data-bs-toggle="tab"
                                    data-bs-target="#nav-contact"
                                    type="button"
                                    role="tab"
                                    aria-controls="nav-contact"
                                    aria-selected="false"
                                >
                                    {' '}
                                    Đổi mật khẩu{' '}
                                </button>
                            </div>
                        </nav>

                        {/* Tab Content */}
                        <div className="tab-content pt-3" id="nav-tabContent">
                            {/* --- Profile Tab --- */}
                            <div
                                className="tab-pane fade show active"
                                id="nav-profile"
                                role="tabpanel"
                                aria-labelledby="nav-profile-tab"
                            >
                                <h6 className="mb-4">Thông tin người dùng</h6>
                                {loading && <p>Đang tải thông tin...</p>}
                                {error && <div className="alert alert-danger">{error}</div>}
                                {!loading && !error && (
                                    <form onSubmit={handleUpdateProfile}>
                                        <fieldset disabled={loading || isUpdatingProfile}>
                                            {/* Họ */}
                                            <div className="form-floating mb-3">
                                                <input
                                                    type="text"
                                                    // (SỬA) className và hiển thị lỗi
                                                    className={`form-control ${
                                                        profileForm.errors.firstName ? 'is-invalid' : ''
                                                    }`}
                                                    id="firstName"
                                                    name="firstName"
                                                    placeholder="Họ"
                                                    // (SỬA) value và onChange
                                                    value={profileForm.values.firstName}
                                                    onChange={handleProfileChange}
                                                    required
                                                />
                                                <label htmlFor="firstName">Họ</label>
                                                {/* (SỬA) Hiển thị lỗi */}
                                                {profileForm.errors.firstName && (
                                                    <div className="invalid-feedback">
                                                        {profileForm.errors.firstName}
                                                    </div>
                                                )}
                                            </div>
                                            {/* Tên */}
                                            <div className="form-floating mb-3">
                                                <input
                                                    type="text"
                                                    className={`form-control ${
                                                        profileForm.errors.lastName ? 'is-invalid' : ''
                                                    }`}
                                                    id="lastName"
                                                    name="lastName"
                                                    placeholder="Tên"
                                                    value={profileForm.values.lastName}
                                                    onChange={handleProfileChange}
                                                    required
                                                />
                                                <label htmlFor="lastName">Tên</label>
                                                {profileForm.errors.lastName && (
                                                    <div className="invalid-feedback">
                                                        {profileForm.errors.lastName}
                                                    </div>
                                                )}
                                            </div>
                                            {/* Giới tính */}
                                            <div className="mb-3">
                                                <label className="form-label me-3">Giới tính:</label>
                                                <div className="btn-group" role="group">
                                                    <input
                                                        type="radio"
                                                        className="btn-check"
                                                        name="gender"
                                                        id="genderMale"
                                                        value="1"
                                                        checked={profileForm.values.gender === '1'}
                                                        onChange={handleProfileChange}
                                                        autoComplete="off"
                                                    />
                                                    <label className="btn btn-outline-secondary" htmlFor="genderMale">
                                                        Nam
                                                    </label>
                                                    <input
                                                        type="radio"
                                                        className="btn-check"
                                                        name="gender"
                                                        id="genderFemale"
                                                        value="0"
                                                        checked={profileForm.values.gender === '0'}
                                                        onChange={handleProfileChange}
                                                        autoComplete="off"
                                                    />
                                                    <label className="btn btn-outline-secondary" htmlFor="genderFemale">
                                                        Nữ
                                                    </label>
                                                </div>
                                                {/* Không cần hiển thị lỗi cho radio */}
                                            </div>
                                            {/* Email */}
                                            <div className="form-floating mb-3">
                                                <input
                                                    type="email"
                                                    className="form-control"
                                                    id="email"
                                                    name="email"
                                                    placeholder="name@example.com"
                                                    value={profileForm.values.email} // Lấy từ profileForm
                                                    readOnly
                                                    style={{ backgroundColor: '#e9ecef' }}
                                                />
                                                <label htmlFor="email">Email</label>
                                            </div>
                                            {/* Số điện thoại */}
                                            <div className="form-floating mb-3">
                                                <input
                                                    type="tel"
                                                    className={`form-control ${
                                                        profileForm.errors.phoneNumber ? 'is-invalid' : ''
                                                    }`}
                                                    id="phoneNumber"
                                                    name="phoneNumber"
                                                    placeholder="Số điện thoại"
                                                    value={profileForm.values.phoneNumber}
                                                    onChange={handleProfileChange}
                                                />
                                                <label htmlFor="phoneNumber">Số điện thoại</label>
                                                {profileForm.errors.phoneNumber && (
                                                    <div className="invalid-feedback">
                                                        {profileForm.errors.phoneNumber}
                                                    </div>
                                                )}
                                            </div>
                                            {/* Nút Submit */}
                                            <button
                                                type="submit"
                                                className="btn btn-primary"
                                                // (SỬA) Dùng isSubmitDisabled của profileForm
                                                disabled={loading || isUpdatingProfile || profileForm.isSubmitDisabled}
                                            >
                                                {isUpdatingProfile ? (
                                                    /* ... spinner ... */ <>
                                                        <span
                                                            className="spinner-border spinner-border-sm"
                                                            role="status"
                                                            aria-hidden="true"
                                                        ></span>
                                                        <span className="ms-1">Đang cập nhật...</span>
                                                    </>
                                                ) : (
                                                    'Cập nhật thông tin'
                                                )}
                                            </button>
                                        </fieldset>
                                    </form>
                                )}
                            </div>

                            {/* --- Change Password Tab --- */}
                            <div
                                className="tab-pane fade"
                                id="nav-contact"
                                role="tabpanel"
                                aria-labelledby="nav-contact-tab"
                            >
                                <h6 className="mb-4">Đổi mật khẩu</h6>
                                <form onSubmit={handleUpdatePassword}>
                                    <fieldset disabled={loading || isUpdatingPassword}>
                                        {/* Mật khẩu cũ */}
                                        <div className="form-floating mb-3 position-relative">
                                            <input
                                                type={showOldPassword ? 'text' : 'password'}
                                                // (SỬA) className và hiển thị lỗi
                                                className={`form-control ${
                                                    passwordForm.errors.oldPassword ? 'is-invalid' : ''
                                                }`}
                                                id="oldPassword"
                                                name="oldPassword"
                                                placeholder="Mật khẩu cũ"
                                                // (SỬA) value và onChange
                                                value={passwordForm.values.oldPassword}
                                                onChange={handlePasswordChange}
                                            />
                                            <label htmlFor="oldPassword">Mật khẩu cũ</label>
                                            {/* (MỚI) Icon con mắt */}
                                            <span style={eyeIconStyle} onClick={() => togglePasswordVisibility('old')}>
                                                {showOldPassword ? <EyeSlashIcon /> : <EyeIcon />}
                                            </span>
                                            {/* (SỬA) Hiển thị lỗi */}
                                            {passwordForm.errors.oldPassword && (
                                                <div className="invalid-feedback">
                                                    {passwordForm.errors.oldPassword}
                                                </div>
                                            )}
                                        </div>
                                        {/* Mật khẩu mới */}
                                        <div className="form-floating mb-3">
                                            <input
                                                type={showNewPassword ? 'text' : 'password'}
                                                className={`form-control ${
                                                    passwordForm.errors.newPassword ? 'is-invalid' : ''
                                                }`}
                                                id="newPassword"
                                                name="newPassword"
                                                placeholder="Mật khẩu mới"
                                                value={passwordForm.values.newPassword}
                                                onChange={handlePasswordChange}
                                            />
                                            <label htmlFor="newPassword">Mật khẩu mới</label>
                                            <span style={eyeIconStyle} onClick={() => togglePasswordVisibility('new')}>
                                                {showNewPassword ? <EyeSlashIcon /> : <EyeIcon />}
                                            </span>
                                            {passwordForm.errors.newPassword && (
                                                <div className="invalid-feedback">
                                                    {passwordForm.errors.newPassword}
                                                </div>
                                            )}
                                        </div>
                                        {/* Xác nhận mật khẩu mới */}
                                        <div className="form-floating mb-3">
                                            <input
                                                type={showConfirmPassword ? 'text' : 'password'}
                                                className={`form-control ${
                                                    passwordForm.errors.confirmNewPassword ? 'is-invalid' : ''
                                                }`}
                                                id="confirmNewPassword"
                                                name="confirmNewPassword"
                                                placeholder="Xác nhận mật khẩu"
                                                value={passwordForm.values.confirmNewPassword}
                                                onChange={handlePasswordChange}
                                            />
                                            <label htmlFor="confirmNewPassword">Xác nhận mật khẩu</label>
                                            <span
                                                style={eyeIconStyle}
                                                onClick={() => togglePasswordVisibility('confirm')}
                                            >
                                                {showConfirmPassword ? <EyeSlashIcon /> : <EyeIcon />}
                                            </span>
                                            {passwordForm.errors.confirmNewPassword && (
                                                <div className="invalid-feedback">
                                                    {passwordForm.errors.confirmNewPassword}
                                                </div>
                                            )}
                                        </div>
                                        {/* Nút Submit */}
                                        <button
                                            type="submit"
                                            className="btn btn-primary"
                                            // (SỬA) Dùng isSubmitDisabled của passwordForm
                                            disabled={loading || isUpdatingPassword || passwordForm.isSubmitDisabled}
                                        >
                                            {isUpdatingPassword ? (
                                                /* ... spinner ... */ <>
                                                    <span
                                                        className="spinner-border spinner-border-sm"
                                                        role="status"
                                                        aria-hidden="true"
                                                    ></span>
                                                    <span className="ms-1">Đang cập nhật...</span>
                                                </>
                                            ) : (
                                                'Cập nhật mật khẩu'
                                            )}
                                        </button>
                                    </fieldset>
                                </form>
                            </div>
                        </div>

                        {/* Modal Thông báo */}
                        {notifyState.show /* ... giữ nguyên ... */ && (
                            <div className="modal fade show d-block" style={{ background: 'rgba(0,0,0,0.4)' }}>
                                <div className="modal-dialog modal-dialog-centered">
                                    <div className="modal-content">
                                        <div className="modal-header">
                                            <h5
                                                className={`modal-title ${
                                                    notifyState.isSuccess ? 'text-success' : 'text-danger'
                                                }`}
                                            >
                                                {notifyState.isSuccess ? 'Thành công' : 'Thất bại'}
                                            </h5>
                                        </div>
                                        <div className="modal-body">
                                            <p>{notifyState.message}</p>
                                        </div>
                                        <div className="modal-footer">
                                            <button className="btn btn-primary" onClick={handleCloseNotify}>
                                                Đóng
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
}
