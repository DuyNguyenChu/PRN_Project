import React, { useState, useEffect, useMemo } from 'react';
import axios from 'axios';
// import Autocomplete from 'react-google-autocomplete';
import { API_URL } from '~/api/api';
import { useFormValidation } from '../validator/useFormValidation';
import { required, maxLength } from '../validator/validators';

const getLoggedInUserId = () => {
    try {
        const userDataString = localStorage.getItem('userData');
        const userData = JSON.parse(userDataString);
        return userData?.resources?.id || 0;
    } catch (e) {
        console.error('Lỗi đọc ID người dùng từ localStorage:', e);
        return 0;
    }
};

// SỬA: Nhận thêm prop isReadOnly
function TripRequestFormPopup({
    item,
    onClose,
    apiUrl,
    token,
    onSuccess,
    showConfirmModal,
    showNotifyModal,
    isReadOnly = false,
}) {
    const isUpdate = !!item;
    const loggedInUserId = useMemo(() => getLoggedInUserId(), []);
    const [loadingTripData, setLoadingTripData] = useState(false);
    const [fetchError, setFetchError] = useState(null);

    const [fromResults, setFromResults] = useState([]);
    const [toResults, setToResults] = useState([]);

    const initialState = useMemo(
        () => ({
            fromLocation: item?.fromLocation || '',
            fromLatitude: item?.fromLatitude || 0,
            fromLongitude: item?.fromLongitude || 0,
            toLocation: item?.toLocation || '',
            toLatitude: item?.toLatitude || 0,
            toLongitude: item?.toLongitude || 0,
            description: item?.description || '',
            createdBy: item?.createdBy || 0,
            requesterId: item?.requesterId || 0,
        }),
        [item],
    );

    const validationRules = {
        fromLocation: [required, maxLength(255)],
        toLocation: [required, maxLength(255)],
        description: [maxLength(500)],
    };

    const { values, errors, setValues, handleChange, validateForm, isSubmitDisabled } = useFormValidation(
        initialState,
        validationRules,
    );

    const handleSearchLocation = async (fieldPrefix) => {
        if (isReadOnly) return; // SỬA: Không tìm kiếm nếu chỉ xem
        const query = values[`${fieldPrefix}Location`];
        if (query.length < 3) return;

        try {
            const response = await fetch(
                `https://nominatim.openstreetmap.org/search?q=${encodeURIComponent(
                    query,
                )}&format=json&limit=5&addressdetails=1`,
            );
            if (!response.ok) throw new Error('Network response was not ok');
            const data = await response.json();
            if (fieldPrefix === 'from') setFromResults(data);
            else setToResults(data);
        } catch (error) {
            console.error('Lỗi khi tìm kiếm Nominatim:', error);
            showNotifyModal('Không thể tìm thấy địa chỉ. Lỗi: ' + error.message, false);
        }
    };

    const handleSelectResult = (fieldPrefix, result) => {
        if (isReadOnly) return; // SỬA: Không cho chọn nếu chỉ xem

        setValues((prev) => ({
            ...prev,
            [`${fieldPrefix}Location`]: result.display_name,
            [`${fieldPrefix}Latitude`]: parseFloat(result.lat),
            [`${fieldPrefix}Longitude`]: parseFloat(result.lon),
        }));

        if (fieldPrefix === 'from') setFromResults([]);
        else setToResults([]);
    };

    // Fetch dữ liệu (Giữ nguyên)
    useEffect(() => {
        if (isUpdate && item?.id && token) {
            const fetchTripData = async () => {
                setLoadingTripData(true);
                setFetchError(null);
                try {
                    const res = await axios.get(`${apiUrl}/${item.id}`, {
                        headers: { Authorization: `Bearer ${token}` },
                    });
                    const tripData = res.data.resources;
                    if (tripData) {
                        setValues({
                            fromLocation: tripData.fromLocation || '',
                            fromLatitude: tripData.fromLatitude || 0,
                            fromLongitude: tripData.fromLongitude || 0,
                            toLocation: tripData.toLocation || '',
                            toLatitude: tripData.toLatitude || 0,
                            toLongitude: tripData.toLongitude || 0,
                            description: tripData.description || '',
                            createdBy: tripData.createdBy || 0,
                            requesterId: tripData.requesterId || 0,
                        });
                    } else {
                        throw new Error('Không tìm thấy dữ liệu chuyến đi.');
                    }
                } catch (err) {
                    setFetchError('Lỗi tải thông tin chuyến đi: ' + (err.response?.data?.message || err.message));
                } finally {
                    setLoadingTripData(false);
                }
            };
            fetchTripData();
        } else if (!isUpdate) {
            setValues({
                ...initialState,
                createdBy: loggedInUserId,
            });
        }
    }, [isUpdate, item, apiUrl, token, setValues, initialState, loggedInUserId]);

    // Các hàm handle Google Maps (Đã comment out)
    const handlePlaceSelected = (place, fieldPrefix) => {
        // ... (Giữ nguyên)
    };
    const handleLocationChange = (e) => {
        // ... (Giữ nguyên)
    };

    // handleSubmit (Giữ nguyên)
    const handleSubmit = () => {
        if (isReadOnly) return; // SỬA: Không submit nếu chỉ xem
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
                    method = 'put';
                    payload = {
                        id: item.id,
                        fromLocation: values.fromLocation,
                        fromLatitude: values.fromLatitude,
                        fromLongitude: values.fromLongitude,
                        toLocation: values.toLocation,
                        toLatitude: values.toLatitude,
                        toLongitude: values.toLongitude,
                        description: values.description,
                        createdBy: values.createdBy,
                        requesterId: values.requesterId,
                        updatedBy: loggedInUserId,
                    };
                } else {
                    payload = {
                        fromLocation: values.fromLocation,
                        fromLatitude: values.fromLatitude,
                        fromLongitude: values.fromLongitude,
                        toLocation: values.toLocation,
                        toLatitude: values.toLatitude,
                        toLongitude: values.toLongitude,
                        description: values.description,
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

    if (fetchError) {
        // ... (Giữ nguyên JSX báo lỗi)
    }

    return (
        <div className="popup-overlay">
            <div className="popup-content p-4 rounded shadow bg-white" style={{ maxWidth: '800px', width: '100%' }}>
                <h5>{isUpdate ? (isReadOnly ? 'Chi tiết Yêu cầu' : 'Cập nhật Yêu cầu') : 'Tạo Yêu cầu Chuyến đi'}</h5>

                {loadingTripData ? (
                    <div className="text-center p-5">Đang tải dữ liệu...</div>
                ) : (
                    <>
                        <div className="row g-3 mt-2">
                            <div className="col-md-6">
                                <div className="form-group">
                                    <label>Từ địa điểm</label>
                                    <div className="input-group">
                                        <input
                                            type="text"
                                            className={`form-control ${errors.fromLocation ? 'is-invalid' : ''}`}
                                            name="fromLocation"
                                            value={values.fromLocation}
                                            onChange={handleChange}
                                            placeholder="Nhập địa chỉ rồi nhấn Tìm"
                                            disabled={isReadOnly} // SỬA: Thêm disabled
                                        />
                                        <button
                                            className="btn btn-outline-secondary"
                                            type="button"
                                            onClick={() => handleSearchLocation('from')}
                                            disabled={isReadOnly} // SỬA: Thêm disabled
                                        >
                                            Tìm
                                        </button>
                                    </div>
                                    {errors.fromLocation && (
                                        <div className="text-danger mt-1">{errors.fromLocation}</div>
                                    )}

                                    {fromResults.length > 0 && (
                                        <ul
                                            className="list-group mt-2"
                                            style={{ maxHeight: '150px', overflowY: 'auto' }}
                                        >
                                            {fromResults.map((result) => (
                                                <li
                                                    key={result.place_id}
                                                    className="list-group-item list-group-item-action"
                                                    style={{ cursor: isReadOnly ? 'default' : 'pointer' }} // SỬA
                                                    onClick={isReadOnly ? null : () => handleSelectResult('from', result)} // SỬA
                                                >
                                                    {result.display_name}
                                                </li>
                                            ))}
                                        </ul>
                                    )}
                                </div>
                            </div>

                            <div className="col-md-6">
                                <div className="form-group">
                                    <label>Đến địa điểm</label>
                                    <div className="input-group">
                                        <input
                                            type="text"
                                            className={`form-control ${errors.toLocation ? 'is-invalid' : ''}`}
                                            name="toLocation"
                                            value={values.toLocation}
                                            onChange={handleChange}
                                            placeholder="Nhập địa chỉ rồi nhấn Tìm"
                                            disabled={isReadOnly} // SỬA: Thêm disabled
                                        />
                                        <button
                                            className="btn btn-outline-secondary"
                                            type="button"
                                            onClick={() => handleSearchLocation('to')}
                                            disabled={isReadOnly} // SỬA: Thêm disabled
                                        >
                                            Tìm
                                        </button>
                                    </div>
                                    {errors.toLocation && <div className="text-danger mt-1">{errors.toLocation}</div>}

                                    {toResults.length > 0 && (
                                        <ul
                                            className="list-group mt-2"
                                            style={{ maxHeight: '150px', overflowY: 'auto' }}
                                        >
                                            {toResults.map((result) => (
                                                <li
                                                    key={result.place_id}
                                                    className="list-group-item list-group-item-action"
                                                    style={{ cursor: isReadOnly ? 'default' : 'pointer' }} // SỬA
                                                    onClick={isReadOnly ? null : () => handleSelectResult('to', result)} // SỬA
                                                >
                                                    {result.display_name}
                                                </li>
                                            ))}
                                        </ul>
                                    )}
                                </div>
                            </div>
                        </div>

                        <div className="row g-3 mt-2">
                            <div className="col-6 text-muted small">
                                From Lat/Lng: {values.fromLatitude.toFixed(4)}, {values.fromLongitude.toFixed(4)}
                            </div>
                            <div className="col-6 text-muted small">
                                To Lat/Lng: {values.toLatitude.toFixed(4)}, {values.toLongitude.toFixed(4)}
                            </div>
                        </div>

                        <div className="row g-3 mt-2">
                            <div className="col-12">
                                <div className="form-group">
                                    <label>Mô tả (Tùy chọn)</label>
                                    <textarea
                                        className={`form-control ${errors.description ? 'is-invalid' : ''}`}
                                        name="description"
                                        rows="3"
                                        value={values.description}
                                        onChange={handleChange}
                                        disabled={isReadOnly} // SỬA: Thêm disabled
                                    ></textarea>
                                    {errors.description && <div className="text-danger mt-1">{errors.description}</div>}
                                </div>
                            </div>
                        </div>

                        {/* SỬA: Nút bấm dựa trên isReadOnly */}
                        <div className="text-end mt-4">
                            {isReadOnly ? (
                                <button className="btn btn-primary" onClick={onClose}>
                                    OK
                                </button>
                            ) : (
                                <>
                                    <button className="btn btn-secondary me-2" onClick={onClose}>
                                        Hủy
                                    </button>
                                    <button
                                        className="btn btn-primary"
                                        onClick={handleSubmit}
                                        disabled={isSubmitDisabled}
                                    >
                                        {isUpdate ? 'Lưu thay đổi' : 'Tạo yêu cầu'}
                                    </button>
                                </>
                            )}
                        </div>
                    </>
                )}
            </div>
        </div>
    );
}

export default TripRequestFormPopup;