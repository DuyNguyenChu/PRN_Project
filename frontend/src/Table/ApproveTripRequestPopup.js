// ApproveTripRequestPopup.js
import React, { useState, useEffect } from 'react';
import axios from 'axios';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import { API_URL } from '~/api/api';
import { useFormValidation } from '../validator/useFormValidation'; // Sửa đường dẫn nếu cần
import { required } from '../validator/validators'; // Sửa đường dẫn nếu cần

// Lấy ID người dùng (Tương tự file TripRequestFormPopup)
const getLoggedInUserId = () => {
    try {
        const userDataString = localStorage.getItem('userData');
        const userData = JSON.parse(userDataString);
        return userData?.resources?.id || 0;
    } catch (e) { return 0; }
};

export default function ApproveTripRequestPopup({ item, apiUrl, token, onClose, onSuccess, showNotifyModal, showConfirmModal }) {
    
    const [driverOptions, setDriverOptions] = useState([]);
    const [vehicleOptions, setVehicleOptions] = useState([]);
    const [loadingDropdown, setLoadingDropdown] = useState(true);

    const initialState = {
        driverId: '',
        vehicleId: '',
    };

    const validationRules = {
        driverId: [required],
        vehicleId: [required],
    };

    const { values, errors, setValues, handleChange, validateForm, isSubmitDisabled } = useFormValidation(
        initialState,
        validationRules
    );

    // Fetch Drivers and Vehicles
    useEffect(() => {
        const fetchDropdownData = async () => {
            setLoadingDropdown(true);
            try {
                const [driverRes, vehicleRes] = await Promise.all([
                    axios.get(`${API_URL}/Driver/available-driver`, { headers: { Authorization: `Bearer ${token}` } }),
                    axios.get(`${API_URL}/Vehicle/available-vehicle`, { headers: { Authorization: `Bearer ${token}` } }),
                ]);
                setDriverOptions(driverRes.data.resources || []);
                setVehicleOptions(vehicleRes.data.resources || []);
            } catch (err) {
                showNotifyModal('Lỗi tải dữ liệu Tài xế/Xe: ' + (err.response?.data?.message || err.message), false);
            } finally {
                setLoadingDropdown(false);
            }
        };
        fetchDropdownData();
    }, [token, showNotifyModal]);

    const handleSelectChange = (e) => {
        const { name, value } = e.target;
        handleChange({ target: { name, value: value ? Number(value) : '' } });
    };

    const handleSubmit = () => {
        if (!validateForm()) return;

        onClose(); // Đóng popup này trước
        showConfirmModal(
            `Bạn có chắc chắn muốn DUYỆT yêu cầu này?`,
            async () => {
                try {
                    const loggedInUserId = getLoggedInUserId();
                    const payload = {
                        id: item.id, // ID của TripRequest
                        driverId: values.driverId,
                        vehicleId: values.vehicleId,
                        scheduledStartTime: values.scheduledStartTime.toISOString(),
                        scheduledEndTime: values.scheduledEndTime.toISOString(),
                        notes: values.notes,
                        approvalBy: loggedInUserId,
                    };
                    
                    await axios.put(`${apiUrl}/approve`, payload, { headers: { Authorization: `Bearer ${token}` } });
                    showNotifyModal('Duyệt yêu cầu thành công!', true);
                    onSuccess(); // Tải lại bảng
                } catch (err) {
                    showNotifyModal('Duyệt yêu cầu thất bại: ' + (err.response?.data?.message || err.message), false);
                }
            },
            () => onClose() // Nếu người dùng nhấn "Hủy" trong confirm modal
        );
    };


    return (
        <div className="popup-overlay">
            <div className="popup-content p-4 rounded shadow bg-white" style={{ maxWidth: '600px', width: '100%' }}>
                <h5>Duyệt Yêu cầu (ID: {item.id})</h5>
                <p>Từ: {item.fromLocation} <br/> Đến: {item.toLocation}</p>
                <hr/>

                {loadingDropdown ? (
                    <p>Đang tải danh sách Tài xế và Xe...</p>
                ) : (
                    <div className="row g-3 mt-2">
                        <div className="col-md-6">
                            <label>Tài xế</label>
                            <select
                                className={`form-select ${errors.driverId ? 'is-invalid' : ''}`}
                                name="driverId"
                                value={values.driverId}
                                onChange={handleSelectChange}
                            >
                                <option value="">-- Chọn tài xế --</option>
                                {driverOptions.map((driver) => (
                                    <option key={driver.id} value={driver.id}>{driver.fullName}</option> 
                                ))}
                            </select>
                            {errors.driverId && <div className="text-danger mt-1">{errors.driverId}</div>}
                        </div>
                        <div className="col-md-6">
                            <label>Xe</label>
                            <select
                                className={`form-select ${errors.vehicleId ? 'is-invalid' : ''}`}
                                name="vehicleId"
                                value={values.vehicleId}
                                onChange={handleSelectChange}
                            >
                                <option value="">-- Chọn xe --</option>
                                 {vehicleOptions.map((vehicle) => (
                                    <option key={vehicle.id} value={vehicle.id}>{vehicle.name}</option> 
                                ))}
                            </select>
                            {errors.vehicleId && <div className="text-danger mt-1">{errors.vehicleId}</div>}
                        </div>
                    </div>
                )}
                
                <div className="text-end mt-4">
                    <button className="btn btn-secondary me-2" onClick={onClose}>
                        Hủy
                    </button>
                    <button className="btn btn-success" onClick={handleSubmit} disabled={loadingDropdown || isSubmitDisabled}>
                        Duyệt
                    </button>
                </div>
            </div>
        </div>
    );
}