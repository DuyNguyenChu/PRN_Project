import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { API_URL } from '~/api/api';
import { useFormValidation } from '../validator/useFormValidation'; // Sửa lại đường dẫn nếu cần
import { required, maxLength, emailValidator, minLength, isNumber } from '../validator/validators'; // Sửa lại đường dẫn nếu cần

/**
 * Helper function để định dạng ngày ISO sang YYYY-MM-DD cho input type="date"
 */
const formatDateForInput = (isoDate) => {
    if (!isoDate) return '';
    try {
        // Cắt chuỗi để lấy phần YYYY-MM-DD
        return new Date(isoDate).toISOString().split('T')[0];
    } catch (e) {
        console.error('Ngày không hợp lệ:', isoDate, e);
        return '';
    }
};

function UserFormPopup({ item, onClose, apiUrl, token, onSuccess, showConfirmModal, showNotifyModal }) {
    // Xác định chế độ (true: Cập nhật, false: Thêm mới)
    const isUpdate = !!item;

    // State để lưu danh sách options cho dropdown
    const [roleOptions, setRoleOptions] = useState([]);
    const [statusOptions, setStatusOptions] = useState([]);
    const [loadingDropdown, setLoadingDropdown] = useState(true);

    const apiUrl1 = `${API_URL}`;

    // --- State & Validation ---

    // 1. Cập nhật initialState để khớp với model User
    const initialState = {
        username: item?.username || '',
        firstName: item?.firstName || '',
        lastName: item?.lastName || '',
        email: item?.email || '',
        phoneNumber: item?.phoneNumber || '',
        gender: item?.gender || 1, // Mặc định là 1 (Nam), hoặc null/'' tùy logic
        userStatus: item?.userStatus?.id || '', // Chỉ lưu ID
        roles: item?.roles?.map((role) => role.id) || [], // Mảng các ID
        passwordHash: 'Abc@123456',
    };

    // 2. Cập nhật validationRules
    const validationRules = {
        email: [required, emailValidator, maxLength(255)],
        firstName: [required, maxLength(100)],
        lastName: [required, maxLength(100)],
        phoneNumber: [maxLength(20), isNumber], // Bỏ 'required' nếu nó bị disable khi thêm mới
        userStatus: [required],
        roles: [(value) => (value && value.length > 0 ? null : 'Vui lòng chọn ít nhất một vai trò')], // Custom validator cho mảng
    };

    // 3. Sử dụng hook
    const { values, errors, handleChange, validateForm, isSubmitDisabled } = useFormValidation(
        initialState,
        validationRules,
    );

    // Ngày tạo (chỉ hiển thị)
    const createdDate = item?.createdDate ? new Date(item.createdDate).toLocaleString() : new Date().toLocaleString();

    // --- Data Fetching cho Dropdowns ---

    useEffect(() => {
        const fetchDropdownData = async () => {
            setLoadingDropdown(true);
            try {
                // !!! THAY THẾ API URL THẬT SỰ Ở ĐÂY !!!
                const [rolesRes, statusRes] = await Promise.all([
                    axios.get(`${apiUrl1}/Role`, { headers: { Authorization: `Bearer ${token}` } }), // URL giả định
                    axios.get(`${apiUrl1}/UserStatus`, { headers: { Authorization: `Bearer ${token}` } }), // URL giả định
                ]);

                // Giả sử API trả về mảng trong `resources`
                setRoleOptions(rolesRes.data?.resources || []);
                setStatusOptions(statusRes.data?.resources || []);
            } catch (err) {
                showNotifyModal('Lỗi tải dữ liệu cho dropdown: ' + (err.response?.data?.message || err.message), false);
            } finally {
                setLoadingDropdown(false);
            }
        };

        fetchDropdownData();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [token]); // Chỉ chạy 1 lần khi component mount

    // --- Handlers ---

    /**
     * Xử lý riêng cho 'select-multiple' (Roles)
     * vì handleChange mặc định của hook có thể không xử lý đúng mảng
     */
    const handleRoleChange = (e) => {
        const selectedIds = Array.from(e.target.selectedOptions, (option) => Number(option.value));
        // Tạo một "fake event" để hook validation có thể hiểu
        const fakeEvent = {
            target: {
                name: 'roles',
                value: selectedIds,
            },
        };
        handleChange(fakeEvent);
    };

    /**
     * Xử lý riêng cho 'select' (UserStatus) để đảm bảo giá trị là Number
     */
    const handleStatusChange = (e) => {
        const fakeEvent = {
            target: {
                name: 'userStatus',
                value: e.target.value ? Number(e.target.value) : '',
            },
        };
        handleChange(fakeEvent);
    };

    const handleSubmit = () => {
        const isFormValid = validateForm();
        if (!isFormValid) {
            return;
        }

        onClose();
        showConfirmModal(isUpdate ? 'Bạn có chắc chắn muốn cập nhật?' : 'Bạn có chắc chắn muốn thêm mới?', async () => {
            try {
                let payload;
                let url = apiUrl;
                let method = 'post';

                if (isUpdate) {
                    // Chế độ UPDATE: Chỉ gửi 2 trường được phép sửa
                    payload = {
                        id: item.id,
                        roles: values.roles,
                        userStatusId: values.userStatus,
                    };
                    url = `${apiUrl}`; // API để update thường có ID
                    method = 'put';
                } else {
                    // Chế độ ADD NEW: Gửi tất cả giá trị từ form
                    payload = { ...values };
                    // Chuyển đổi ID status và roles về đúng định dạng API yêu cầu (nếu cần)
                    // Ví dụ: API cần object thay vì ID
                    // payload.userStatus = { id: values.userStatus };
                    // payload.roles = values.roles.map(id => ({ id: id }));
                }

                await axios[method](url, payload, { headers: { Authorization: `Bearer ${token}` } });

                showNotifyModal(isUpdate ? 'Cập nhật thành công!' : 'Thêm mới thành công!');
                onSuccess();
            } catch (err) {
                showNotifyModal('Thao tác thất bại: ' + (err.response?.data?.message || err.message), false);
            }
        });
    };

    // --- Logic disable trường theo yêu cầu ---
    // Update: disable tất cả TRỪ userStatus và roles
    // Add new: disable firstName, lastName, phoneNumber (theo yêu cầu)
    const isFieldDisabled = (fieldName) => {
        if (isUpdate) {
            // Chế độ CẬP NHẬT
            return !['userStatus', 'roles'].includes(fieldName);
        } else {
            // Chế độ THÊM MỚI
            return ['phoneNumber', 'username', 'gender'].includes(fieldName);
        }
    };

    if (loadingDropdown) {
        return (
            <div className="popup-overlay">
                <div className="popup-content p-4 rounded shadow bg-white">
                    <h5>Đang tải dữ liệu...</h5>
                </div>
            </div>
        );
    }

    return (
        <div className="popup-overlay">
            <div className="popup-content p-4 rounded shadow bg-white" style={{ maxWidth: '800px', width: '100%' }}>
                <h5>{isUpdate ? 'Cập nhật người dùng' : 'Thêm mới người dùng'}</h5>
                {/* Sử dụng grid để chia layout */}
                <div className="row g-3 mt-2">
                    {/* Cột 1 */}
                    <div className="col-md-6">
                        <div className="form-group">
                            <label>Username</label>
                            <input
                                type="email"
                                className={`form-control ${errors.username ? 'is-invalid' : ''}`}
                                name="username"
                                value={values.username}
                                onChange={handleChange}
                                disabled={isFieldDisabled('username')}
                            />
                            {errors.username && <div className="text-danger mt-1">{errors.username}</div>}
                        </div>

                        <div className="form-group mt-3">
                            <label>Họ</label>
                            <input
                                type="text"
                                className={`form-control ${errors.firstName ? 'is-invalid' : ''}`}
                                name="firstName"
                                value={values.firstName}
                                onChange={handleChange}
                                disabled={isFieldDisabled('firstName')}
                            />
                            {errors.firstName && <div className="text-danger mt-1">{errors.firstName}</div>}
                        </div>

                        <div className="form-group mt-3">
                            <label>Số điện thoại</label>
                            <input
                                type="text"
                                className={`form-control ${errors.phoneNumber ? 'is-invalid' : ''}`}
                                name="phoneNumber"
                                value={values.phoneNumber}
                                onChange={handleChange}
                                disabled={isFieldDisabled('phoneNumber')}
                            />
                            {errors.phoneNumber && <div className="text-danger mt-1">{errors.phoneNumber}</div>}
                        </div>

                        <div className="form-group mt-3">
                            <label>Giới tính</label>
                            <select
                                className="form-select"
                                name="gender"
                                value={values.gender}
                                onChange={handleChange}
                                disabled={isFieldDisabled('gender')}
                            >
                                <option value={1}>Nam</option>
                                <option value={0}>Nữ</option>
                                <option value={2}>Khác</option>
                            </select>
                        </div>

                        <div className="form-group mt-3">
                            <label>Trạng thái người dùng</label>
                            <select
                                className={`form-select ${errors.userStatus ? 'is-invalid' : ''}`}
                                name="userStatus"
                                value={values.userStatus}
                                onChange={handleStatusChange} // Dùng handler riêng
                                disabled={isFieldDisabled('userStatus')} // Sẽ luôn là false
                            >
                                <option value="">-- Chọn trạng thái --</option>
                                {statusOptions.map((status) => (
                                    <option key={status.id} value={status.id}>
                                        {status.name}
                                    </option>
                                ))}
                            </select>
                            {errors.userStatus && <div className="text-danger mt-1">{errors.userStatus}</div>}
                        </div>
                    </div>

                    {/* Cột 2 */}
                    <div className="col-md-6">
                        <div className="form-group">
                            <label>Email</label>
                            <input
                                type="email"
                                className={`form-control ${errors.email ? 'is-invalid' : ''}`}
                                name="email"
                                value={values.email}
                                onChange={handleChange}
                                disabled={isFieldDisabled('email')}
                            />
                            {errors.email && <div className="text-danger mt-1">{errors.email}</div>}
                        </div>

                        <div className="form-group mt-3">
                            <label>Tên</label>
                            <input
                                type="text"
                                className={`form-control ${errors.lastName ? 'is-invalid' : ''}`}
                                name="lastName"
                                value={values.lastName}
                                onChange={handleChange}
                                disabled={isFieldDisabled('lastName')}
                            />
                            {errors.lastName && <div className="text-danger mt-1">{errors.lastName}</div>}
                        </div>

                        <div className="form-group mt-3">
                            <label>Ngày tạo</label>
                            <input type="text" className="form-control" value={createdDate} disabled />
                        </div>

                        <div className="form-group mt-3">
                            <label>Vai trò (Chọn nhiều)</label>
                            <select
                                className={`form-select ${errors.roles ? 'is-invalid' : ''}`}
                                name="roles"
                                multiple // Cho phép chọn nhiều
                                value={values.roles} // value là một mảng ID
                                onChange={handleRoleChange} // Dùng handler riêng
                                disabled={isFieldDisabled('roles')} // Sẽ luôn là false
                                style={{ height: '150px' }} // Tăng chiều cao
                            >
                                {roleOptions.map((role) => (
                                    <option key={role.id} value={role.id}>
                                        {role.name}
                                    </option>
                                ))}
                            </select>
                            {errors.roles && <div className="text-danger mt-1">{errors.roles}</div>}
                        </div>
                    </div>
                </div>

                <div className="text-end mt-4">
                    <button className="btn btn-secondary me-2" onClick={onClose}>
                        Hủy
                    </button>
                    <button className="btn btn-primary" onClick={handleSubmit} disabled={isSubmitDisabled}>
                        {isUpdate ? 'Lưu thay đổi' : 'Thêm mới'}
                    </button>
                </div>
            </div>
        </div>
    );
}

export default UserFormPopup;
