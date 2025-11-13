// DriverFormPopup.js
import React, { useState, useEffect, useMemo } from 'react';
import axios from 'axios';
import { API_URL } from '~/api/api';
import { useFormValidation } from '../validator/useFormValidation'; // Sửa lại đường dẫn nếu cần
import { required, maxLength, emailValidator, isNumber } from '../validator/validators'; // Sửa lại đườngẫn
import DatePicker from 'react-datepicker'; // THÊM
import 'react-datepicker/dist/react-datepicker.css'; // THÊM

// Lấy ID người dùng (từ TripRequestFormPopup)
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

// Hardcode Hạng bằng lái xe (từ Driver.js)
const LICENSE_CLASSES = [
    { id: 'A1', name: 'A1' }, { id: 'B1', name: 'B1' }, { id: 'B2', name: 'B2' },
    { id: 'C', name: 'C' }, { id: 'D', name: 'D' }, { id: 'E', name: 'E' }, { id: 'F', name: 'F' },
];

function DriverFormPopup({ item, onClose, apiUrl, token, onSuccess, showConfirmModal, showNotifyModal }) {
    const isUpdate = !!item;
    const loggedInUserId = useMemo(() => getLoggedInUserId(), []);

    // State cho dropdown
    const [statusOptions, setStatusOptions] = useState([]);
    const [loadingDropdown, setLoadingDropdown] = useState(true);

    // State loading data khi update
    const [loadingData, setLoadingData] = useState(false);
    const [fetchError, setFetchError] = useState(null);

    // --- State & Validation (TÁCH BIỆT) ---

    // 1. Dùng cho chế độ THÊM MỚI
    const initialStateCreate = {
        experienceYears: 0,
        baseSalary: 0,
        licenseNumber: '',
        licenseClass: 'B2', // Mặc định
        licenseExpiryDate: new Date(),
        socialInsuranceNumber: '',
        emergencyContactName: '',
        emergencyContactPhone: '',
        driverStatusId: '',
        username: '',
        password: '', // API yêu cầu, nhưng nên bỏ nếu API tự tạo
        firstName: '',
        lastName: '',
        email: '',
        gender: 1, // 1 = Nam
        dateOfBirth: new Date(),
        phoneNumber: '',
        identityNumber: '',
    };

    // 2. Dùng cho chế độ CẬP NHẬT
    const initialStateUpdate = {
        baseSalary: 0,
        driverStatusId: '',
    };

    // 3. Rules cho THÊM MỚI
    const validationRulesCreate = {
        username: [required, maxLength(100)],
        password: [required], // Giả sử có validator này
        firstName: [required, maxLength(100)],
        lastName: [required, maxLength(100)],
        email: [required, emailValidator, maxLength(255)],
        phoneNumber: [required, isNumber, maxLength(20)],
        identityNumber: [required, isNumber, maxLength(20)],
        licenseNumber: [required, maxLength(50)],
        licenseClass: [required],
        licenseExpiryDate: [required],
        driverStatusId: [required],
        experienceYears: [required, isNumber],
        baseSalary: [required, isNumber],
    };

    // 4. Rules cho CẬP NHẬT
    const validationRulesUpdate = {
        baseSalary: [required, isNumber],
        driverStatusId: [required],
    };

    // 5. Khởi tạo hook validation
    const { values, errors, setValues, handleChange, validateForm, isSubmitDisabled } = useFormValidation(
        isUpdate ? initialStateUpdate : initialStateCreate,
        isUpdate ? validationRulesUpdate : validationRulesCreate,
    );

    // --- Data Fetching ---

    // Fetch dropdown (chỉ cần DriverStatus)
    useEffect(() => {
        const fetchDropdownData = async () => {
            setLoadingDropdown(true);
            try {
                const statusRes = await axios.get(`${API_URL}/DriverStatus`, { headers: { Authorization: `Bearer ${token}` } });
                setStatusOptions(statusRes.data?.resources || []);
            } catch (err) {
                showNotifyModal('Lỗi tải Trạng thái: ' + (err.response?.data?.message || err.message), false);
            } finally {
                setLoadingDropdown(false);
            }
        };
        fetchDropdownData();
    }, [token, showNotifyModal]);

    // Fetch dữ liệu chi tiết khi CẬP NHẬT
    useEffect(() => {
        if (isUpdate && item?.Id && token) {
            const fetchDriverData = async () => {
                setLoadingData(true);
                setFetchError(null);
                try {
                    // API trả về DriverAggregate không có baseSalary
                    // Ta phải gọi 1 API GET chi tiết (ví dụ: /api/Driver/1)
                    // Giả sử API này trả về { baseSalary: 1000, driverStatusId: 1 }
                    const res = await axios.get(`${apiUrl}/${item.Id}`, { // Giả sử item.Id (viết hoa)
                        headers: { Authorization: `Bearer ${token}` },
                    });
                    
                    const driverData = res.data.resources; // Giả sử dữ liệu trả về ở đây
                    
                    if (driverData) {
                        setValues({
                            baseSalary: driverData.baseSalary || 0,
                            driverStatusId: driverData.driverStatusId || '',
                        });
                    } else {
                        throw new Error("Không tìm thấy dữ liệu tài xế.");
                    }
                } catch (err) {
                    setFetchError('Lỗi tải thông tin tài xế: ' + (err.response?.data?.message || err.message));
                } finally {
                    setLoadingData(false);
                }
            };
            fetchDriverData();
        } else if (!isUpdate) {
             setValues(initialStateCreate);
        }
    }, [isUpdate, item, apiUrl, token, setValues]); // Bỏ initialState...

    // --- Handlers ---

    // Xử lý DatePicker
    const handleDateChange = (date, name) => {
        handleChange({ target: { name, value: date } });
    };

    // Xử lý Select (để đảm bảo là Number)
    const handleSelectChange = (e) => {
        const { name, value } = e.target;
        handleChange({ target: { name, value: value ? Number(value) : '' } });
    };
    
    // Xử lý Select (cho string)
    const handleSelectStringChange = (e) => {
        handleChange(e); // Dùng handleChange gốc
    };

    const handleSubmit = () => {
        if (!validateForm()) return;

        onClose();
        showConfirmModal(isUpdate ? 'Bạn có chắc chắn muốn cập nhật?' : 'Bạn có chắc chắn muốn thêm mới?', async () => {
            try {
                let payload;
                let url = apiUrl;
                let method = isUpdate ? 'put' : 'post';

                if (isUpdate) {
                    // Chế độ UPDATE: Chỉ gửi 3 trường
                    payload = {
                        id: item.Id, // Giả sử Id viết hoa từ bảng
                        baseSalary: values.baseSalary,
                        driverStatusId: values.driverStatusId,
                        updatedBy: loggedInUserId,
                    };
                } else {
                    // Chế độ ADD NEW: Gửi tất cả giá trị
                    payload = {
                        ...values,
                        createdBy: loggedInUserId,
                        // Đảm bảo Date là string ISO
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

    // --- Render Loading/Error ---
    if (loadingDropdown || loadingData) {
        return (
            <div className="popup-overlay"><div className="popup-content p-4">
                <h5>Đang tải dữ liệu...</h5>
            </div></div>
        );
    }

    if (fetchError) {
        return (
            <div className="popup-overlay"><div className="popup-content p-4">
                <h5 className="text-danger">Lỗi</h5>
                <p>{fetchError}</p>
                 <div className="text-end mt-4">
                    <button className="btn btn-secondary" onClick={onClose}>Đóng</button>
                 </div>
            </div></div>
        );
    }
    
    // --- Render Form ---
    return (
        <div className="popup-overlay">
            <div className="popup-content p-4 rounded shadow bg-white" style={{ maxWidth: '900px', width: '100%' }}>
                <h5>{isUpdate ? 'Cập nhật tài xế' : 'Thêm mới tài xế'}</h5>
                
                {/* Tách biệt 2 form */}
                {isUpdate ? (
                    // FORM CẬP NHẬT (Chỉ 2 trường)
                    <div className="row g-3 mt-2">
                        <div className="col-md-6">
                            <label>Lương cơ bản</label>
                            <input
                                type="number"
                                className={`form-control ${errors.baseSalary ? 'is-invalid' : ''}`}
                                name="baseSalary"
                                value={values.baseSalary}
                                onChange={handleChange}
                            />
                            {errors.baseSalary && <div className="text-danger mt-1">{errors.baseSalary}</div>}
                        </div>
                        <div className="col-md-6">
                            <label>Trạng thái tài xế</label>
                            <select
                                className={`form-select ${errors.driverStatusId ? 'is-invalid' : ''}`}
                                name="driverStatusId"
                                value={values.driverStatusId}
                                onChange={handleSelectChange} // Dùng handler riêng
                            >
                                <option value="">-- Chọn trạng thái --</option>
                                {statusOptions.map((status) => (
                                    <option key={status.id} value={status.id}>
                                        {status.name}
                                    </option>
                                ))}
                            </select>
                            {errors.driverStatusId && <div className="text-danger mt-1">{errors.driverStatusId}</div>}
                        </div>
                    </div>
                ) : (
                    // FORM THÊM MỚI (Form lớn)
                    <div className="row g-3 mt-2" style={{ maxHeight: '70vh', overflowY: 'auto' }}>
                        {/* Cột 1 */}
                        <div className="col-md-6">
                            <h6 className="mb-3">Thông tin đăng nhập</h6>
                            <div className="form-group">
                                <label>Username</label>
                                <input type="text" className={`form-control ${errors.username ? 'is-invalid' : ''}`} name="username" value={values.username} onChange={handleChange} />
                                {errors.username && <div className="text-danger mt-1">{errors.username}</div>}
                            </div>
                            <div className="form-group mt-3">
                                <label>Mật khẩu</label>
                                <input type="password" className={`form-control ${errors.password ? 'is-invalid' : ''}`} name="password" value={values.password} onChange={handleChange} />
                                {errors.password && <div className="text-danger mt-1">{errors.password}</div>}
                            </div>
                            
                            <h6 className="mb-3 mt-4">Thông tin cá nhân</h6>
                            <div className="form-group">
                                <label>Họ</label>
                                <input type="text" className={`form-control ${errors.firstName ? 'is-invalid' : ''}`} name="firstName" value={values.firstName} onChange={handleChange} />
                                {errors.firstName && <div className="text-danger mt-1">{errors.firstName}</div>}
                            </div>
                            <div className="form-group mt-3">
                                <label>Tên</label>
                                <input type="text" className={`form-control ${errors.lastName ? 'is-invalid' : ''}`} name="lastName" value={values.lastName} onChange={handleChange} />
                                {errors.lastName && <div className="text-danger mt-1">{errors.lastName}</div>}
                            </div>
                            <div className="form-group mt-3">
                                <label>Email</label>
                                <input type="email" className={`form-control ${errors.email ? 'is-invalid' : ''}`} name="email" value={values.email} onChange={handleChange} />
                                {errors.email && <div className="text-danger mt-1">{errors.email}</div>}
                            </div>
                            <div className="form-group mt-3">
                                <label>Ngày sinh</label>
                                <DatePicker selected={values.dateOfBirth} onChange={(date) => handleDateChange(date, 'dateOfBirth')} className="form-control" dateFormat="dd/MM/yyyy" />
                            </div>
                            <div className="form-group mt-3">
                                <label>Giới tính</label>
                                <select className="form-select" name="gender" value={values.gender} onChange={handleSelectChange}>
                                    <option value={1}>Nam</option>
                                    <option value={0}>Nữ</option>
                                    <option value={2}>Khác</option>
                                </select>
                            </div>
                            <div className="form-group mt-3">
                                <label>Số điện thoại</label>
                                <input type="text" className={`form-control ${errors.phoneNumber ? 'is-invalid' : ''}`} name="phoneNumber" value={values.phoneNumber} onChange={handleChange} />
                                {errors.phoneNumber && <div className="text-danger mt-1">{errors.phoneNumber}</div>}
                            </div>
                            <div className="form-group mt-3">
                                <label>Số CCCD/CMND</label>
                                <input type="text" className={`form-control ${errors.identityNumber ? 'is-invalid' : ''}`} name="identityNumber" value={values.identityNumber} onChange={handleChange} />
                                {errors.identityNumber && <div className="text-danger mt-1">{errors.identityNumber}</div>}
                            </div>
                        </div>
                        
                        {/* Cột 2 */}
                        <div className="col-md-6">
                            <h6 className="mb-3">Thông tin tài xế</h6>
                            <div className="form-group">
                                <label>Số bằng lái</label>
                                <input type="text" className={`form-control ${errors.licenseNumber ? 'is-invalid' : ''}`} name="licenseNumber" value={values.licenseNumber} onChange={handleChange} />
                                {errors.licenseNumber && <div className="text-danger mt-1">{errors.licenseNumber}</div>}
                            </div>
                            <div className="form-group mt-3">
                                <label>Hạng bằng lái</label>
                                <select className={`form-select ${errors.licenseClass ? 'is-invalid' : ''}`} name="licenseClass" value={values.licenseClass} onChange={handleSelectStringChange}>
                                    {LICENSE_CLASSES.map(cls => <option key={cls.id} value={cls.id}>{cls.name}</option>)}
                                </select>
                                {errors.licenseClass && <div className="text-danger mt-1">{errors.licenseClass}</div>}
                            </div>
                             <div className="form-group mt-3">
                                <label>Ngày hết hạn bằng lái</label>
                                <DatePicker selected={values.licenseExpiryDate} onChange={(date) => handleDateChange(date, 'licenseExpiryDate')} className={`form-control ${errors.licenseExpiryDate ? 'is-invalid' : ''}`} dateFormat="dd/MM/yyyy" />
                                {errors.licenseExpiryDate && <div className="text-danger mt-1">{errors.licenseExpiryDate}</div>}
                            </div>
                            <div className="form-group mt-3">
                                <label>Thâm niên (năm)</label>
                                <input type="number" className={`form-control ${errors.experienceYears ? 'is-invalid' : ''}`} name="experienceYears" value={values.experienceYears} onChange={handleChange} />
                                {errors.experienceYears && <div className="text-danger mt-1">{errors.experienceYears}</div>}
                            </div>
                            <div className="form-group mt-3">
                                <label>Lương cơ bản</label>
                                <input type="number" className={`form-control ${errors.baseSalary ? 'is-invalid' : ''}`} name="baseSalary" value={values.baseSalary} onChange={handleChange} />
                                {errors.baseSalary && <div className="text-danger mt-1">{errors.baseSalary}</div>}
                            </div>
                             <div className="form-group mt-3">
                                <label>Trạng thái tài xế</label>
                                <select className={`form-select ${errors.driverStatusId ? 'is-invalid' : ''}`} name="driverStatusId" value={values.driverStatusId} onChange={handleSelectChange}>
                                    <option value="">-- Chọn trạng thái --</option>
                                    {statusOptions.map(status => <option key={status.id} value={status.id}>{status.name}</option>)}
                                </select>
                                {errors.driverStatusId && <div className="text-danger mt-1">{errors.driverStatusId}</div>}
                            </div>
                             <div className="form-group mt-3">
                                <label>Số BHXH</label>
                                <input type="text" className="form-control" name="socialInsuranceNumber" value={values.socialInsuranceNumber} onChange={handleChange} />
                            </div>
                            <div className="form-group mt-3">
                                <label>Liên hệ khẩn cấp (Tên)</label>
                                <input type="text" className="form-control" name="emergencyContactName" value={values.emergencyContactName} onChange={handleChange} />
                            </div>
                             <div className="form-group mt-3">
                                <label>Liên hệ khẩn cấp (SĐT)</label>
                                <input type="text" className="form-control" name="emergencyContactPhone" value={values.emergencyContactPhone} onChange={handleChange} />
                            </div>
                        </div>
                    </div>
                )}

                {/* Nút bấm */}
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

export default DriverFormPopup;