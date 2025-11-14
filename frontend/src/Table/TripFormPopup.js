// TripFormPopup.js
// (Nội dung file giống hệt file TripFormPopup.js ở câu trả lời trước)
import React, { useState, useEffect, useMemo } from 'react';
import axios from 'axios';
import { API_URL } from '~/api/api';
import { useFormValidation } from '../validator/useFormValidation'; // Sửa lại đường dẫn nếu cần
import { required, maxLength } from '../validator/validators'; // Sửa lại đường dẫn

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

function TripFormPopup({ item, onClose, apiUrl, token, onSuccess, showConfirmModal, showNotifyModal }) {
    const isUpdate = !!item;
    const loggedInUserId = useMemo(() => getLoggedInUserId(), []);

    // State cho dropdown
    const [vehicleOptions, setVehicleOptions] = useState([]);
    const [driverOptions, setDriverOptions] = useState([]);
    const [loadingDropdown, setLoadingDropdown] = useState(true);

    // State loading data khi update
    const [loadingData, setLoadingData] = useState(false);
    const [fetchError, setFetchError] = useState(null);
    
    // State cho Nominatim Search
    const [fromResults, setFromResults] = useState([]);
    const [toResults, setToResults] = useState([]);

    // --- State & Validation ---
    const initialState = {
        vehicleId: '',
        driverId: '',
        fromLocation: '',
        fromLatitude: 0,
        fromLongitude: 0,
        toLocation: '',
        toLatitude: 0,
        toLongitude: 0,
    };

    const validationRules = {
        vehicleId: [required],
        driverId: [required],
        fromLocation: [required],
        toLocation: [required],
    };

    const { values, errors, setValues, handleChange, validateForm, isSubmitDisabled } = useFormValidation(
        initialState,
        validationRules,
    );

    // --- Data Fetching ---
    useEffect(() => {
        const fetchDropdownData = async () => {
            setLoadingDropdown(true);
            try {
                const [vehicleRes, driverRes] = await Promise.all([
                    axios.get(`${API_URL}/Vehicle`, { headers: { Authorization: `Bearer ${token}` } }),
                    axios.get(`${API_URL}/Driver`, { headers: { Authorization: `Bearer ${token}` } }),
                ]);
                setVehicleOptions(vehicleRes.data.resources || []);
                setDriverOptions(driverRes.data.resources || []);
            } catch (err) {
                showNotifyModal('Lỗi tải dữ liệu Xe/Tài xế: ' + (err.response?.data?.message || err.message), false);
            } finally {
                setLoadingDropdown(false);
            }
        };
        fetchDropdownData();
    }, [token, showNotifyModal]);

    useEffect(() => {
        const tripId = item?.id || item?.Id; // Sửa: Dùng cả 2
        if (isUpdate && tripId && token) {
            const fetchTripData = async () => {
                setLoadingData(true);
                setFetchError(null);
                try {
                    const res = await axios.get(`${apiUrl}/${tripId}`, { // Sửa
                        headers: { Authorization: `Bearer ${token}` },
                    });
                    
                    const data = res.data.resources || res.data;
                    
                    if (data) {
                        setValues({
                            // Sửa: Dùng camelCase từ API Detail
                            vehicleId: data.vehicle?.id || '',
                            driverId: data.driver?.id || '',
                            fromLocation: data.fromLocation?.name || '',
                            fromLatitude: data.fromLocation?.latitude || 0,
                            fromLongitude: data.fromLocation?.longitude || 0,
                            toLocation: data.toLocation?.name || '',
                            toLatitude: data.toLocation?.latitude || 0,
                            toLongitude: data.toLocation?.longitude || 0,
                        });
                    } else {
                        throw new Error("Không tìm thấy dữ liệu chuyến đi.");
                    }
                } catch (err) {
                    setFetchError('Lỗi tải thông tin chuyến đi: ' + (err.response?.data?.message || err.message));
                } finally {
                    setLoadingData(false);
                }
            };
            fetchTripData();
        } else if (!isUpdate) {
             setValues(initialState);
        }
    }, [isUpdate, item, apiUrl, token, setValues, showNotifyModal]); // Sửa

    // --- Handlers ---
    const handleSelectChange = (e) => {
        const { name, value } = e.target;
        handleChange({ target: { name, value: value ? Number(value) : '' } });
    };

    // (Các hàm Nominatim giữ nguyên)
    const handleSearchLocation = async (fieldPrefix) => {
        const query = values[`${fieldPrefix}Location`];
        if (query.length < 3) return;

        try {
            const response = await fetch(
                `https://nominatim.openstreetmap.org/search?q=${encodeURIComponent(query)}&format=json&limit=5&addressdetails=1`
            );
            if (!response.ok) throw new Error('Network response was not ok');
            const data = await response.json();
            
            if (fieldPrefix === 'from') setFromResults(data);
            else setToResults(data);

        } catch (error) {
            showNotifyModal('Không thể tìm thấy địa chỉ: ' + error.message, false);
        }
    };
    const handleSelectResult = (fieldPrefix, result) => {
        setValues(prev => ({
            ...prev,
            [`${fieldPrefix}Location`]: result.display_name,
            [`${fieldPrefix}Latitude`]: parseFloat(result.lat),
            [`${fieldPrefix}Longitude`]: parseFloat(result.lon),
        }));
        if (fieldPrefix === 'from') setFromResults([]);
        else setToResults([]);
    };
    
    // Bắt sự kiện gõ vào ô địa chỉ
    const handleLocationChange = (e) => {
        const { name, value } = e.target;
        handleChange(e);
        if (value === '') {
            const prefix = name === 'fromLocation' ? 'from' : 'to';
            setValues(prev => ({
                ...prev,
                [`${prefix}Latitude`]: 0,
                [`${prefix}Longitude`]: 0,
            }));
            if (prefix === 'from') setFromResults([]);
            else setToResults([]);
        }
    };

    const handleSubmit = () => {
        if (!validateForm()) return;

        onClose();
        showConfirmModal(isUpdate ? 'Bạn có chắc chắn muốn cập nhật?' : 'Bạn có chắc chắn muốn thêm mới?', async () => {
            try {
                let payload;
                let url = apiUrl;
                let method = isUpdate ? 'put' : 'post';
                const tripId = item?.id || item?.Id;
                
                const commonPayload = {
                    vehicleId: values.vehicleId,
                    driverId: values.driverId,
                    fromLocation: values.fromLocation,
                    fromLatitude: values.fromLatitude,
                    fromLongitude: values.fromLongitude,
                    toLocation: values.toLocation,
                    toLatitude: values.toLatitude,
                    toLongitude: values.toLongitude,
                };

                if (isUpdate) {
                    payload = {
                        ...commonPayload,
                        id: tripId,
                        updatedBy: loggedInUserId,
                    };
                } else {
                    payload = {
                        ...commonPayload,
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


    // --- Render ---
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
    
    return (
        <div className="popup-overlay">
            <div className="popup-content p-4 rounded shadow bg-white" style={{ maxWidth: '800px', width: '100%' }}>
                <h5>{isUpdate ? 'Cập nhật chuyến đi' : 'Tạo chuyến đi'}</h5>
                
                <div className="row g-3 mt-2">
                    <div className="col-md-6">
                        <label>Chọn xe</label>
                        <select
                            className={`form-select ${errors.vehicleId ? 'is-invalid' : ''}`}
                            name="vehicleId"
                            value={values.vehicleId}
                            onChange={handleSelectChange}
                        >
                            <option value="">-- Chọn xe --</option>
                            {vehicleOptions.map((v) => (
                                <option key={v.id} value={v.id}>{v.name}</option>
                            ))}
                        </select>
                        {errors.vehicleId && <div className="text-danger mt-1">{errors.vehicleId}</div>}
                    </div>
                    <div className="col-md-6">
                        <label>Chọn tài xế</label>
                        <select
                            className={`form-select ${errors.driverId ? 'is-invalid' : ''}`}
                            name="driverId"
                            value={values.driverId}
                            onChange={handleSelectChange}
                        >
                            <option value="">-- Chọn tài xế --</option>
                            {driverOptions.map((d) => (
                                <option key={d.id} value={d.id}>{d.fullName}</option>
                            ))}
                        </select>
                        {errors.driverId && <div className="text-danger mt-1">{errors.driverId}</div>}
                    </div>

                    {/* Từ địa điểm */}
                    <div className="col-md-6">
                         <div className="form-group">
                            <label>Từ địa điểm</label>
                            <div className="input-group">
                                <input
                                    type="text"
                                    className={`form-control ${errors.fromLocation ? 'is-invalid' : ''}`}
                                    name="fromLocation"
                                    value={values.fromLocation}
                                    onChange={handleLocationChange}
                                    placeholder="Nhập địa chỉ rồi nhấn Tìm"
                                />
                                <button className="btn btn-outline-secondary" type="button" onClick={() => handleSearchLocation('from')}>
                                    Tìm
                                </button>
                            </div>
                            {errors.fromLocation && <div className="text-danger mt-1">{errors.fromLocation}</div>}
                            {fromResults.length > 0 && (
                                <ul className="list-group mt-2" style={{ maxHeight: '150px', overflowY: 'auto', position: 'absolute', zIndex: 1000 }}>
                                    {fromResults.map((result) => (
                                        <li key={result.place_id} className="list-group-item list-group-item-action"
                                            style={{ cursor: 'pointer' }} onClick={() => handleSelectResult('from', result)}>
                                            {result.display_name}
                                        </li>
                                    ))}
                                </ul>
                            )}
                        </div>
                    </div>

                    {/* Đến địa điểm */}
                    <div className="col-md-6">
                         <div className="form-group">
                            <label>Đến địa điểm</label>
                            <div className="input-group">
                                <input
                                    type="text"
                                    className={`form-control ${errors.toLocation ? 'is-invalid' : ''}`}
                                    name="toLocation"
                                    value={values.toLocation}
                                    onChange={handleLocationChange}
                                    placeholder="Nhập địa chỉ rồi nhấn Tìm"
                                />
                                <button className="btn btn-outline-secondary" type="button" onClick={() => handleSearchLocation('to')}>
                                    Tìm
                                </button>
                            </div>
                            {errors.toLocation && <div className="text-danger mt-1">{errors.toLocation}</div>}
                            {toResults.length > 0 && (
                                <ul className="list-group mt-2" style={{ maxHeight: '150px', overflowY: 'auto', position: 'absolute', zIndex: 1000 }}>
                                    {toResults.map((result) => (
                                        <li key={result.place_id} className="list-group-item list-group-item-action"
                                            style={{ cursor: 'pointer' }} onClick={() => handleSelectResult('to', result)}>
                                            {result.display_name}
                                        </li>
                                    ))}
                                </ul>
                            )}
                        </div>
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

export default TripFormPopup;