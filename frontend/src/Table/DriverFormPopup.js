// DriverFormPopup.js
import React, { useState, useEffect, useMemo } from 'react';
import axios from 'axios';
import { API_URL } from '~/api/api';
import { useFormValidation } from '../validator/useFormValidation'; // Sửa lại đường dẫn nếu cần
import { required, maxLength, emailValidator, isNumber, phoneValidator } from '../validator/validators'; // Sửa lại đường dẫn
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';

// --- (CÁC HÀM VALIDATION - Giữ nguyên) ---
const regex = (pattern, message) => (value) => 
    (value && pattern.test(value)) ? null : message;
const licenseNumberValidator = regex(
    /^([A-Z]{1,2}\d{6,8}|\d{12})$/,
    'Số GPLX không hợp lệ (vd: B2123456 hoặc 012345678901)'
);
const socialInsuranceValidator = regex(
    /^\d{10}$/,
    'Số BHXH phải là 10 chữ số'
);
const identityNumberValidator = regex(
    /^(\d{9}|\d{12})$/,
    'CCCD (12 số) hoặc CMND (9 số) không hợp lệ'
);
// --- (KẾT THÚC) VALIDATION ---

// Lấy ID người dùng (giữ nguyên)
const getLoggedInUserId = () => {
    try {
        const userDataString = localStorage.getItem('userData');
        const userData = JSON.parse(userDataString);
        return userData?.resources?.id || 0;
    } catch (e) {
        console.error('Lỗi đọc ID người dùng:', e);
        return 0;
    }
};

function DriverFormPopup({ item, onClose, apiUrl, token, onSuccess, showConfirmModal, showNotifyModal }) {
    const isUpdate = !!item;
    const loggedInUserId = useMemo(() => getLoggedInUserId(), []);

    // State cho dropdown
    const [statusOptions, setStatusOptions] = useState([]);
    const [licenseClassOptions, setLicenseClassOptions] = useState([]);
    const [loadingDropdown, setLoadingDropdown] = useState(true);

    // State loading data khi update
    const [loadingData, setLoadingData] = useState(false);
    const [fetchError, setFetchError] = useState(null);

    // --- State & Validation (HỢP NHẤT - Giữ nguyên) ---
    const initialState = {
        experienceYears: 0,
        baseSalary: 0,
        licenseNumber: '',
        licenseClass: 'B2',
        licenseExpiryDate: new Date(),
        socialInsuranceNumber: '',
        emergencyContactName: '',
        emergencyContactPhone: '',
        driverStatusId: '',
        username: '',
        password: '',
        firstName: '',
        lastName: '',
        email: '',
        gender: 1,
        dateOfBirth: new Date(),
        phoneNumber: '',
        identityNumber: '',
    };

    const validationRulesCreate = {
        // (Giữ nguyên các rules)
        username: [required, maxLength(100)],
        password: [required],
        firstName: [required, maxLength(100)],
        lastName: [required, maxLength(100)],
        email: [required, emailValidator, maxLength(255)],
        phoneNumber: [required, isNumber, maxLength(20), phoneValidator],
        identityNumber: [required, identityNumberValidator],
        licenseNumber: [required, licenseNumberValidator],
        socialInsuranceNumber: [required, socialInsuranceValidator],
        licenseClass: [required],
        licenseExpiryDate: [required],
        driverStatusId: [required],
        experienceYears: [required, isNumber],
        baseSalary: [required, isNumber],
        emergencyContactPhone: [phoneValidator, maxLength(20)],
    };

    const validationRulesUpdate = {
        baseSalary: [required, isNumber],
        driverStatusId: [required],
    };

    const { values, errors, setValues, handleChange, validateForm, isSubmitDisabled } = useFormValidation(
        initialState,
        isUpdate ? validationRulesUpdate : validationRulesCreate,
    );
    // --- Kết thúc State & Validation ---

    // --- Data Fetching ---

    // Fetch dropdown (giữ nguyên)
    useEffect(() => {
        const fetchDropdownData = async () => {
            setLoadingDropdown(true);
            try {
                const [statusRes, licenseRes] = await Promise.all([
                    axios.get(`${API_URL}/driver-status`, { headers: { Authorization: `Bearer ${token}` } }),
                    axios.get(`${API_URL}/Driver/license-class`, { headers: { Authorization: `Bearer ${token}` } }),
                ]);
                setStatusOptions(statusRes.data.resources || []);
                setLicenseClassOptions(licenseRes.data.resources || []);
            } catch (err) {
                showNotifyModal('Lỗi tải Trạng thái/Hạng bằng lái: ' + (err.response?.data?.message || err.message), false);
            } finally {
                setLoadingDropdown(false);
            }
        };
        fetchDropdownData();
    }, [token, showNotifyModal]);

    // SỬA: Logic fetch/fill dữ liệu khi Update
    useEffect(() => {
        // SỬA: Kiểm tra item.id (camelCase) HOẶC item.Id (PascalCase)
        const driverId = item?.id || item?.Id;

        if (isUpdate && driverId && token) {
            
            // --- BƯỚC 1: Fill form ngay lập tức với dữ liệu từ `item` (bảng) ---
            // SỬA: Ưu tiên camelCase (item.fullName) rồi mới đến PascalCase (item.FullName)
            const fullName = item.fullName || item.FullName || '';
            const nameParts = fullName ? fullName.split(' ') : [''];
            const lastName = nameParts.pop() || '';
            const firstName = nameParts.join(' ') || '';

            setValues({
                ...initialState,
                // SỬA: Kiểm tra cả 2 kiểu viết
                experienceYears: item.experienceYears || item.ExperienceYears || 0,
                licenseNumber: item.licenseNumber || item.LicenseNumber || '',
                licenseClass: item.licenseClass || item.LicenseClass || 'B2',
                driverStatusId: item.driverStatusId || item.DriverStatusId || '',
                firstName: firstName,
                lastName: lastName,
                email: item.email || item.Email || '',
                phoneNumber: item.phoneNumber || item.PhoneNumber || '',
            });
            
            // --- BƯỚC 2: Gọi API GET /{id} để lấy dữ liệu chi tiết ---
            const fetchDriverDetail = async () => {
                setLoadingData(true);
                setFetchError(null);
                try {
                    // SỬA: Dùng driverId đã lấy được
                    const res = await axios.get(`${apiUrl}/${driverId}`, {
                        headers: { Authorization: `Bearer ${token}` },
                    });
                    
                    // Dùng API response JSON (bạn vừa cung cấp)
                    const driverData = res.data.resources; 
                    
                    if (driverData) {
                        // Cập nhật *thêm* các trường còn thiếu
                        // API response của bạn dùng camelCase, nên ta truy cập trực tiếp
                        setValues(prev => ({
                            ...prev,
                            baseSalary: driverData.baseSalary || 0,
                            driverStatusId: driverData.driverStatusId || prev.driverStatusId,
                            
                            experienceYears: driverData.experienceYears || prev.experienceYears,
                            licenseNumber: driverData.licenseNumber || prev.licenseNumber,
                            licenseClass: driverData.licenseClass || prev.licenseClass,
                            licenseExpiryDate: driverData.licenseExpiryDate ? new Date(driverData.licenseExpiryDate) : (prev.licenseExpiryDate || new Date()),
                            socialInsuranceNumber: driverData.socialInsuranceNumber || '',
                            emergencyContactName: driverData.emergencyContactName || '',
                            emergencyContactPhone: driverData.emergencyContactPhone || '',
                            username: driverData.username || prev.username,
                            firstName: driverData.firstName || prev.firstName, // Lấy từ API chi tiết
                            lastName: driverData.lastName || prev.lastName, // Lấy từ API chi tiết
                            email: driverData.email || prev.email,
                            gender: driverData.gender !== undefined ? driverData.gender : prev.gender,
                            dateOfBirth: driverData.dateOfBirth ? new Date(driverData.dateOfBirth) : (prev.dateOfBirth || new Date()),
                            phoneNumber: driverData.phoneNumber || prev.phoneNumber,
                            identityNumber: driverData.identityNumber || '', // API của bạn không có trường này, nhưng ta vẫn giữ
                        }));
                    } else {
                        throw new Error("Không tìm thấy dữ liệu chi tiết của tài xế.");
                    }
                } catch (err) {
                    setFetchError('Lỗi tải thông tin chi tiết tài xế: ' + (err.response?.data?.message || err.message));
                    showNotifyModal('Lỗi tải chi tiết (vd: Lương): ' + (err.response?.data?.message || err.message), false);
                } finally {
                    setLoadingData(false);
                }
            };
            fetchDriverDetail(); // Chạy BƯỚC 2

        } else if (!isUpdate) {
             setValues(initialState); // Reset về form rỗng
        }
    // SỬA: Dependencies
    }, [isUpdate, item, apiUrl, token, setValues, showNotifyModal]); 

    // --- Handlers (Giữ nguyên) ---

    // Hàm helper để disable trường
    const isFieldDisabled = (fieldName) => {
        if (!isUpdate) {
            return false;
        }
        const editableFields = ['baseSalary', 'driverStatusId'];
        return !editableFields.includes(fieldName);
    };

    const handleDateChange = (date, name) => {
        handleChange({ target: { name, value: date } });
    };
    const handleSelectChange = (e) => {
        const { name, value } = e.target;
        handleChange({ target: { name, value: value ? Number(value) : '' } });
    };
    const handleSelectStringChange = (e) => {
        handleChange(e);
    };

    // handleSubmit (Giữ nguyên)
    const handleSubmit = () => {
        let isFormValid;
        if (isUpdate) {
            isFormValid = validateForm(['baseSalary', 'driverStatusId']);
        } else {
            isFormValid = validateForm();
        }
        if (!isFormValid) return;

        onClose();
        showConfirmModal(isUpdate ? 'Bạn có chắc chắn muốn cập nhật?' : 'Bạn có chắc chắn muốn thêm mới?', async () => {
            try {
                let payload;
                let url = apiUrl;
                let method = isUpdate ? 'put' : 'post';
                const driverId = item?.id || item?.Id; // SỬA: Lấy ID

                if (isUpdate) {
                    payload = {
                        id: driverId, // SỬA
                        baseSalary: values.baseSalary,
                        driverStatusId: values.driverStatusId,
                        updatedBy: loggedInUserId,
                    };
                } else {
                    payload = {
                        ...values,
                        createdBy: loggedInUserId,
                        dateOfBirth: values.dateOfBirth.toISOString(),
                        licenseExpiryDate: values.licenseExpiryDate.toISOString(),
                    };
                }

                await axios[method](url, payload, { headers: { Authorization: `Bearer ${token}` } });
                showNotifyModal(isUpdate ? 'Cập nhật thành công!' : 'Thêm mới thành công!', true);
                onSuccess();
            } catch (err) {
                showNotifyModal('Thao tác thất bại: ' + (err.response?.data?.message || err.message), false);
            }
        });
    };

    // --- Render (Giữ nguyên toàn bộ JSX) ---
    if (loadingDropdown) { 
        return (
            <div className="popup-overlay"><div className="popup-content p-4">
                <h5>Đang tải dữ liệu Dropdown...</h5>
            </div></div>
        );
    }
    
    return (
        <div className="popup-overlay">
            <div className="popup-content p-4 rounded shadow bg-white" style={{ maxWidth: '900px', width: '100%' }}>
                <h5>{isUpdate ? 'Cập nhật tài xế' : 'Thêm mới tài xế'}</h5>
                
                {fetchError && (
                    <div className="alert alert-danger" role="alert">
                        {fetchError}
                    </div>
                )}
                {loadingData && (
                    <div className="text-center p-3">
                        <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                        Đang tải chi tiết...
                    </div>
                )}

                <div className="row g-3 mt-2" style={{ maxHeight: '70vh', overflowY: 'auto' }}>
                    {/* Cột 1 */}
                    <div className="col-md-6">
                        <h6 className="mb-3">Thông tin đăng nhập</h6>
                        <div className="form-group">
                            <label>Username</label>
                            <input type="text" className={`form-control ${errors.username ? 'is-invalid' : ''}`} name="username" value={values.username} onChange={handleChange} disabled={isFieldDisabled('username')} />
                            {errors.username && <div className="text-danger mt-1">{errors.username}</div>}
                        </div>
                        <div className="form-group mt-3">
                            <label>Mật khẩu</label>
                            <input type="password" className={`form-control ${errors.password ? 'is-invalid' : ''}`} name="password" value={values.password} onChange={handleChange} disabled={isFieldDisabled('password')} placeholder={isUpdate ? '********' : ''} />
                            {errors.password && <div className="text-danger mt-1">{errors.password}</div>}
                        </div>
                        
                        <h6 className="mb-3 mt-4">Thông tin cá nhân</h6>
                        <div className="form-group">
                            <label>Họ</label>
                            <input type="text" className={`form-control ${errors.firstName ? 'is-invalid' : ''}`} name="firstName" value={values.firstName} onChange={handleChange} disabled={isFieldDisabled('firstName')} />
                            {errors.firstName && <div className="text-danger mt-1">{errors.firstName}</div>}
                        </div>
                        <div className="form-group mt-3">
                            <label>Tên</label>
                            <input type="text" className={`form-control ${errors.lastName ? 'is-invalid' : ''}`} name="lastName" value={values.lastName} onChange={handleChange} disabled={isFieldDisabled('lastName')} />
                            {errors.lastName && <div className="text-danger mt-1">{errors.lastName}</div>}
                        </div>
                        <div className="form-group mt-3">
                            <label>Email</label>
                            <input type="email" className={`form-control ${errors.email ? 'is-invalid' : ''}`} name="email" value={values.email} onChange={handleChange} disabled={isFieldDisabled('email')} />
                            {errors.email && <div className="text-danger mt-1">{errors.email}</div>}
                        </div>
                        <div className="form-group mt-3">
                            <label>Ngày sinh</label>
                            <DatePicker selected={values.dateOfBirth} onChange={(date) => handleDateChange(date, 'dateOfBirth')} className="form-control" dateFormat="dd/MM/yyyy" disabled={isFieldDisabled('dateOfBirth')} />
                        </div>
                        <div className="form-group mt-3">
                            <label>Giới tính</label>
                            <select className="form-select" name="gender" value={values.gender} onChange={handleSelectChange} disabled={isFieldDisabled('gender')}>
                                <option value={1}>Nam</option>
                                <option value={0}>Nữ</option>
                                <option value={2}>Khác</option>
                            </select>
                        </div>
                        <div className="form-group mt-3">
                            <label>Số điện thoại</label>
                            <input type="text" className={`form-control ${errors.phoneNumber ? 'is-invalid' : ''}`} name="phoneNumber" value={values.phoneNumber} onChange={handleChange} disabled={isFieldDisabled('phoneNumber')} />
                            {errors.phoneNumber && <div className="text-danger mt-1">{errors.phoneNumber}</div>}
                        </div>
                        <div className="form-group mt-3">
                            <label>Số CCCD/CMND</label>
                            <input type="text" className={`form-control ${errors.identityNumber ? 'is-invalid' : ''}`} name="identityNumber" value={values.identityNumber} onChange={handleChange} disabled={isFieldDisabled('identityNumber')} />
                            {errors.identityNumber && <div className="text-danger mt-1">{errors.identityNumber}</div>}
                        </div>
                    </div>
                    
                    {/* Cột 2 */}
                    <div className="col-md-6">
                        <h6 className="mb-3">Thông tin tài xế</h6>
                        <div className="form-group">
                            <label>Số bằng lái</label>
                            <input type="text" className={`form-control ${errors.licenseNumber ? 'is-invalid' : ''}`} name="licenseNumber" value={values.licenseNumber} onChange={handleChange} disabled={isFieldDisabled('licenseNumber')} />
                            {errors.licenseNumber && <div className="text-danger mt-1">{errors.licenseNumber}</div>}
                        </div>
                        <div className="form-group mt-3">
                            <label>Hạng bằng lái</label>
                            <select className={`form-select ${errors.licenseClass ? 'is-invalid' : ''}`} name="licenseClass" value={values.licenseClass} onChange={handleSelectStringChange} disabled={isFieldDisabled('licenseClass')}>
                                {licenseClassOptions.map(cls => <option key={cls.id} value={cls.id}>{cls.name}</option>)}
                            </select>
                            {errors.licenseClass && <div className="text-danger mt-1">{errors.licenseClass}</div>}
                        </div>
                         <div className="form-group mt-3">
                            <label>Ngày hết hạn bằng lái</label>
                            <DatePicker selected={values.licenseExpiryDate} onChange={(date) => handleDateChange(date, 'licenseExpiryDate')} className={`form-control ${errors.licenseExpiryDate ? 'is-invalid' : ''}`} dateFormat="dd/MM/yyyy" disabled={isFieldDisabled('licenseExpiryDate')} />
                            {errors.licenseExpiryDate && <div className="text-danger mt-1">{errors.licenseExpiryDate}</div>}
                        </div>
                        <div className="form-group mt-3">
                            <label>Thâm niên (năm)</label>
                            <input type="number" className={`form-control ${errors.experienceYears ? 'is-invalid' : ''}`} name="experienceYears" value={values.experienceYears} onChange={handleChange} disabled={isFieldDisabled('experienceYears')} />
                            {errors.experienceYears && <div className="text-danger mt-1">{errors.experienceYears}</div>}
                        </div>
                        <div className="form-group mt-3">
                            <label>Lương cơ bản</label>
                            <input type="number" className={`form-control ${errors.baseSalary ? 'is-invalid' : ''}`} name="baseSalary" value={values.baseSalary} onChange={handleChange} disabled={isFieldDisabled('baseSalary')} />
                            {errors.baseSalary && <div className="text-danger mt-1">{errors.baseSalary}</div>}
                        </div>
                         <div className="form-group mt-3">
                            <label>Trạng thái tài xế</label>
                            <select className={`form-select ${errors.driverStatusId ? 'is-invalid' : ''}`} name="driverStatusId" value={values.driverStatusId} onChange={handleSelectChange} disabled={isFieldDisabled('driverStatusId')}>
                                <option value="">-- Chọn trạng thái --</option>
                                {statusOptions.map(status => <option key={status.id} value={status.id}>{status.name}</option>)}
                            </select>
                            {errors.driverStatusId && <div className="text-danger mt-1">{errors.driverStatusId}</div>}
                        </div>
                         <div className="form-group mt-3">
                            <label>Số BHXH</label>
                            <input type="text" className={`form-control ${errors.socialInsuranceNumber ? 'is-invalid' : ''}`} name="socialInsuranceNumber" value={values.socialInsuranceNumber} onChange={handleChange} disabled={isFieldDisabled('socialInsuranceNumber')} />
                            {errors.socialInsuranceNumber && <div className="text-danger mt-1">{errors.socialInsuranceNumber}</div>}
                        </div>
                        <div className="form-group mt-3">
                            <label>Liên hệ khẩn cấp (Tên)</label>
                            <input type="text" className="form-control" name="emergencyContactName" value={values.emergencyContactName} onChange={handleChange} disabled={isFieldDisabled('emergencyContactName')} />
                        </div>
                         <div className="form-group mt-3">
                            <label>Liên hệ khẩn cấp (SĐT)</label>
                            <input type="text" className={`form-control ${errors.emergencyContactPhone ? 'is-invalid' : ''}`} name="emergencyContactPhone" value={values.emergencyContactPhone} onChange={handleChange} disabled={isFieldDisabled('emergencyContactPhone')} />
                            {errors.emergencyContactPhone && <div className="text-danger mt-1">{errors.emergencyContactPhone}</div>}
                        </div>
                    </div>
                </div>

                {/* Nút bấm */}
                <div className="text-end mt-4">
                    <button className="btn btn-secondary me-2" onClick={onClose}>
                        Hủy
                    </button>
                    <button className="btn btn-primary" onClick={handleSubmit} disabled={isSubmitDisabled || loadingData}>
                        {isUpdate ? 'Lưu thay đổi' : 'Thêm mới'}
                    </button>
                </div>
            </div>
        </div>
    );
}

export default DriverFormPopup;