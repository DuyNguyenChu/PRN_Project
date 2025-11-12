// TripExpenseFormPopup.js
import React, { useState, useEffect, useMemo } from 'react';
import axios from 'axios';
import { API_URL } from '~/api/api';
import { useFormValidation } from '../validator/useFormValidation'; // Sửa lại đường dẫn nếu cần
import { required, isNumber, maxLength } from '../validator/validators'; // Sửa lại đường dẫn
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';

// Lấy ID người dùng
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

function TripExpenseFormPopup({ item, onClose, apiUrl, token, onSuccess, showConfirmModal, showNotifyModal }) {
    const isUpdate = !!item;
    const loggedInUserId = useMemo(() => getLoggedInUserId(), []);

    // State cho dropdown
    const [tripOptions, setTripOptions] = useState([]);
    const [expenseTypeOptions, setExpenseTypeOptions] = useState([]);
    const [loadingDropdown, setLoadingDropdown] = useState(true);

    // State loading data khi update
    const [loadingData, setLoadingData] = useState(false);
    const [fetchError, setFetchError] = useState(null);

    // --- State & Validation ---
    const initialState = {
        tripId: item?.TripId || '', // Sửa: TripId (viết hoa) từ bảng
        expenseTypeId: '',
        amount: 0,
        expenseDate: new Date(),
        notes: '',
    };

    const validationRules = {
        tripId: [required],
        expenseTypeId: [required],
        amount: [required, (value) => (value > 0 ? null : 'Số tiền phải lớn hơn 0')],
        expenseDate: [required],
        notes: [maxLength(500)],
    };

    const { values, errors, setValues, handleChange, validateForm, isSubmitDisabled } = useFormValidation(
        initialState,
        validationRules,
    );

    // --- Data Fetching ---

    // Fetch dropdown (Trips & ExpenseTypes)
    useEffect(() => {
        const fetchDropdownData = async () => {
            setLoadingDropdown(true);
            try {
                const [tripRes, typeRes] = await Promise.all([
                    axios.get(`${API_URL}/Trip`, { headers: { Authorization: `Bearer ${token}` } }),
                    axios.get(`${API_URL}/ExpenseType`, { headers: { Authorization: `Bearer ${token}` } }),
                ]);
                setTripOptions(tripRes.data.resources || []);
                setExpenseTypeOptions(typeRes.data.resources || []);
            } catch (err) {
                showNotifyModal('Lỗi tải dữ liệu Dropdown: ' + (err.response?.data?.message || err.message), false);
            } finally {
                setLoadingDropdown(false);
            }
        };
        fetchDropdownData();
    }, [token, showNotifyModal]);

    // Fetch dữ liệu chi tiết khi CẬP NHẬT
    useEffect(() => {
        if (isUpdate && item?.Id && token) {
            const fetchExpenseData = async () => {
                setLoadingData(true);
                setFetchError(null);
                try {
                    // Giả sử API GET /TripExpense/{id} trả về chi tiết
                    const res = await axios.get(`${apiUrl}/${item.Id}`, {
                        headers: { Authorization: `Bearer ${token}` },
                    });
                    
                    const data = res.data.resources; // Giả sử dữ liệu trả về ở đây
                    
                    if (data) {
                        setValues({
                            tripId: data.tripId,
                            expenseTypeId: data.expenseTypeId,
                            amount: data.amount,
                            expenseDate: new Date(data.expenseDate), // Chuyển sang Date object
                            notes: data.notes || '',
                        });
                    } else {
                        throw new Error("Không tìm thấy dữ liệu chi phí.");
                    }
                } catch (err) {
                    setFetchError('Lỗi tải thông tin chi phí: ' + (err.response?.data?.message || err.message));
                } finally {
                    setLoadingData(false);
                }
            };
            fetchExpenseData();
        } else if (!isUpdate) {
             setValues(initialState);
        }
    }, [isUpdate, item, apiUrl, token, setValues]); // Bỏ initialState

    // --- Handlers ---
    const handleDateChange = (date, name) => {
        handleChange({ target: { name, value: date } });
    };

    const handleSelectChange = (e) => {
        const { name, value } = e.target;
        handleChange({ target: { name, value: value ? Number(value) : '' } });
    };

    const handleSubmit = () => {
        if (!validateForm()) return;

        onClose();
        showConfirmModal(isUpdate ? 'Bạn có chắc chắn muốn cập nhật?' : 'Bạn có chắc chắn muốn thêm mới?', async () => {
            try {
                let payload;
                let url = apiUrl;
                let method = isUpdate ? 'put' : 'post';
                
                // Chuẩn bị payload chung
                const commonPayload = {
                    expenseTypeId: values.expenseTypeId,
                    amount: values.amount,
                    expenseDate: values.expenseDate.toISOString(), // Gửi ISO string
                    notes: values.notes,
                };

                if (isUpdate) {
                    // Chế độ UPDATE
                    payload = {
                        ...commonPayload,
                        id: item.Id, // Sửa: Id (viết hoa) từ bảng
                        updatedBy: loggedInUserId,
                    };
                } else {
                    // Chế độ ADD NEW
                    payload = {
                        ...commonPayload,
                        tripId: values.tripId,
                        createdBy: loggedInUserId,
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
            <div className="popup-content p-4 rounded shadow bg-white" style={{ maxWidth: '600px', width: '100%' }}>
                <h5>{isUpdate ? 'Cập nhật chi phí' : 'Thêm mới chi phí'}</h5>
                
                <div className="row g-3 mt-2">
                    <div className="col-12">
                        <label>Chuyến đi</label>
                        <select
                            className={`form-select ${errors.tripId ? 'is-invalid' : ''}`}
                            name="tripId"
                            value={values.tripId}
                            onChange={handleSelectChange}
                            disabled={isUpdate} // KHÔNG cho sửa Chuyến đi khi update
                        >
                            <option value="">-- Chọn chuyến đi --</option>
                            {tripOptions.map((trip) => (
                                <option key={trip.id} value={trip.id}>
                                    {trip.id} (Từ {trip.fromLocation} đến {trip.toLocation})
                                </option>
                            ))}
                        </select>
                        {errors.tripId && <div className="text-danger mt-1">{errors.tripId}</div>}
                    </div>
                    <div className="col-12">
                        <label>Loại chi phí</label>
                        <select
                            className={`form-select ${errors.expenseTypeId ? 'is-invalid' : ''}`}
                            name="expenseTypeId"
                            value={values.expenseTypeId}
                            onChange={handleSelectChange}
                        >
                            <option value="">-- Chọn loại chi phí --</option>
                            {expenseTypeOptions.map((type) => (
                                <option key={type.id} value={type.id}>
                                    {type.name}
                                </option>
                            ))}
                        </select>
                        {errors.expenseTypeId && <div className="text-danger mt-1">{errors.expenseTypeId}</div>}
                    </div>
                    <div className="col-md-6">
                        <label>Số tiền</label>
                        <input
                            type="number"
                            className={`form-control ${errors.amount ? 'is-invalid' : ''}`}
                            name="amount"
                            value={values.amount}
                            onChange={handleChange}
                        />
                        {errors.amount && <div className="text-danger mt-1">{errors.amount}</div>}
                    </div>
                    <div className="col-md-6">
                        <label>Ngày chi phí</label>
                        <DatePicker 
                            selected={values.expenseDate} 
                            onChange={(date) => handleDateChange(date, 'expenseDate')} 
                            className={`form-control ${errors.expenseDate ? 'is-invalid' : ''}`}
                            dateFormat="dd/MM/yyyy" 
                        />
                        {errors.expenseDate && <div className="text-danger mt-1">{errors.expenseDate}</div>}
                    </div>
                    <div className="col-12">
                        <label>Ghi chú</label>
                        <textarea
                            className={`form-control ${errors.notes ? 'is-invalid' : ''}`}
                            name="notes"
                            rows="3"
                            value={values.notes}
                            onChange={handleChange}
                        ></textarea>
                        {errors.notes && <div className="text-danger mt-1">{errors.notes}</div>}
                    </div>
                </div>

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

export default TripExpenseFormPopup;