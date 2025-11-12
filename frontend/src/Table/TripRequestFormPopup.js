import React, { useState, useEffect, useMemo } from 'react';
import axios from 'axios';
// import Autocomplete from 'react-google-autocomplete'; // <-- Import thư viện bản đồ
import { API_URL } from '~/api/api';
import { useFormValidation } from '../validator/useFormValidation'; // Sửa lại đường dẫn nếu cần
import { required, maxLength } from '../validator/validators'; // Sửa lại đường dẫn nếu cần

/**
 * Lấy ID của người dùng đang đăng nhập từ localStorage.
 * !! QUAN TRỌNG: Hãy kiểm tra lại đường dẫn 'userData.resources.id'
 * !! cho chính xác với cấu trúc localStorage của bạn.
 */
const getLoggedInUserId = () => {
    try {
        const userDataString = localStorage.getItem('userData');
        const userData = JSON.parse(userDataString);
        // Giả sử ID người dùng nằm ở đây, Vd: { resources: { id: 123, ... } }
        return userData?.resources?.id || 0;
    } catch (e) {
        console.error('Lỗi đọc ID người dùng từ localStorage:', e);
        return 0;
    }
};

/**
 * Lấy Google Maps API Key từ file .env
 * Bạn cần tạo file .env ở gốc dự án và thêm vào:
 * REACT_APP_GOOGLE_MAPS_API_KEY=AIzaSy... (key của bạn)
 */
// const GOOGLE_MAPS_API_KEY = process.env.REACT_APP_GOOGLE_MAPS_API_KEY;

// if (!GOOGLE_MAPS_API_KEY) {
//     console.error('Vui lòng cung cấp REACT_APP_GOOGLE_MAPS_API_KEY trong file .env');
// }

function TripRequestFormPopup({ item, onClose, apiUrl, token, onSuccess, showConfirmModal, showNotifyModal }) {
    // Xác định chế độ (true: Cập nhật, false: Thêm mới)
    const isUpdate = !!item;

    // Lấy ID người dùng (chỉ 1 lần)
    const loggedInUserId = useMemo(() => getLoggedInUserId(), []);

    // State loading user data khi update
    const [loadingTripData, setLoadingTripData] = useState(false);
    // State lưu lỗi khi fetch user data
    const [fetchError, setFetchError] = useState(null);

    const [fromResults, setFromResults] = useState([]);
    const [toResults, setToResults] = useState([]);

    // --- State & Validation ---

    // 1. initialState
    const initialState = useMemo(
        () => ({
            fromLocation: item?.fromLocation || '',
            fromLatitude: item?.fromLatitude || 0,
            fromLongitude: item?.fromLongitude || 0,
            toLocation: item?.toLocation || '',
            toLatitude: item?.toLatitude || 0,
            toLongitude: item?.toLongitude || 0,
            description: item?.description || '',
            // Các trường ID (sẽ được điền khi fetch hoặc submit)
            createdBy: item?.createdBy || 0,
            requesterId: item?.requesterId || 0,
        }),
        [item],
    );

    // 2. validationRules
    const validationRules = {
        fromLocation: [required, maxLength(255)],
        toLocation: [required, maxLength(255)],
        description: [maxLength(500)],
        // Latitude/Longitude không cần validate vì người dùng không nhập tay
    };

    // 3. Sử dụng hook
    const { values, errors, setValues, handleChange, validateForm, isSubmitDisabled } = useFormValidation(
        initialState,
        validationRules,
    );

    /**
     * Hàm tìm kiếm địa chỉ bằng API Nominatim (miễn phí)
     * @param {'from' | 'to'} fieldPrefix - 'from' hoặc 'to'
     */
    const handleSearchLocation = async (fieldPrefix) => {
        const query = values[`${fieldPrefix}Location`]; // Lấy text từ input
        if (query.length < 3) return; // Không tìm nếu quá ngắn

        try {
            const response = await fetch(
                `https://nominatim.openstreetmap.org/search?q=${encodeURIComponent(
                    query,
                )}&format=json&limit=5&addressdetails=1`,
            );
            if (!response.ok) throw new Error('Network response was not ok');

            const data = await response.json();

            if (fieldPrefix === 'from') {
                setFromResults(data);
            } else {
                setToResults(data);
            }
        } catch (error) {
            console.error('Lỗi khi tìm kiếm Nominatim:', error);
            showNotifyModal('Không thể tìm thấy địa chỉ. Lỗi: ' + error.message, false);
        }
    };

    /**
     * Hàm khi người dùng CHỌN một kết quả từ danh sách
     * @param {'from' | 'to'} fieldPrefix
     * @param {object} result - Kết quả từ Nominatim
     */
    const handleSelectResult = (fieldPrefix, result) => {
        // Cập nhật form
        setValues((prev) => ({
            ...prev,
            [`${fieldPrefix}Location`]: result.display_name,
            [`${fieldPrefix}Latitude`]: parseFloat(result.lat),
            [`${fieldPrefix}Longitude`]: parseFloat(result.lon),
        }));

        // Xóa danh sách kết quả
        if (fieldPrefix === 'from') {
            setFromResults([]);
        } else {
            setToResults([]);
        }
    };

    // --- Data Fetching ---

    // (MỚI) Fetch dữ liệu Trip khi ở chế độ Update
    useEffect(() => {
        // Chỉ fetch khi là Update, có item.id, và đã có token
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
                        // Dùng setValues để cập nhật form với dữ liệu đầy đủ
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
            // Nếu là Thêm mới, đảm bảo form dùng initialState (và gán createdBy)
            setValues({
                ...initialState,
                createdBy: loggedInUserId, // Gán người tạo là user đang login
            });
        }
        // Thêm dependencies
    }, [isUpdate, item, apiUrl, token, setValues, initialState, loggedInUserId]);

    // --- Handlers ---

    /**
     * Xử lý khi chọn một địa điểm từ Google Autocomplete
     * @param {object} place - Đối tượng 'place' từ Google
     * @param {'from' | 'to'} fieldPrefix - 'from' hoặc 'to'
     */
    const handlePlaceSelected = (place, fieldPrefix) => {
        if (!place || !place.geometry) {
            // Người dùng nhấn Enter mà không chọn
            // Xóa tọa độ nếu địa chỉ bị xóa
            if (!values[`${fieldPrefix}Location`]) {
                setValues((prev) => ({
                    ...prev,
                    [`${fieldPrefix}Latitude`]: 0,
                    [`${fieldPrefix}Longitude`]: 0,
                }));
            }
            return;
        }

        const lat = place.geometry.location.lat();
        const lng = place.geometry.location.lng();
        const locationName = place.formatted_address;

        // Cập nhật state của form
        setValues((prev) => ({
            ...prev,
            [`${fieldPrefix}Location`]: locationName,
            [`${fieldPrefix}Latitude`]: lat,
            [`${fieldPrefix}Longitude`]: lng,
        }));
    };

    /**
     * Xử lý khi người dùng TỰ GÕ (không chọn) vào ô Autocomplete
     * Chúng ta cần cập nhật `fromLocation` / `toLocation`
     */
    const handleLocationChange = (e) => {
        const { name, value } = e.target;
        // Dùng handleChange của hook để cập nhật
        handleChange(e);

        // Nếu người dùng xóa sạch text, ta cũng reset tọa độ
        if (value === '') {
            const fieldPrefix = name === 'fromLocation' ? 'from' : 'to';
            setValues((prev) => ({
                ...prev,
                [`${fieldPrefix}Latitude`]: 0,
                [`${fieldPrefix}Longitude`]: 0,
            }));
        }
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
                    // Chế độ UPDATE
                    method = 'put';
                    // Giả sử API update cần ID trong payload (giống UserForm)
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
                        updatedBy: loggedInUserId, // Gán người cập nhật
                    };
                } else {
                    // Chế độ ADD NEW
                    payload = {
                        fromLocation: values.fromLocation,
                        fromLatitude: values.fromLatitude,
                        fromLongitude: values.fromLongitude,
                        toLocation: values.toLocation,
                        toLatitude: values.toLatitude,
                        toLongitude: values.toLongitude,
                        description: values.description,
                        createdBy: loggedInUserId, // Gán người tạo
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

    if (fetchError) {
        return (
            <div className="popup-overlay">
                <div className="popup-content p-4 rounded shadow bg-white">
                    <h5 className="text-danger">Lỗi</h5>
                    <p>{fetchError}</p>
                    <div className="text-end mt-4">
                        <button className="btn btn-secondary" onClick={onClose}>
                            Đóng
                        </button>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="popup-overlay">
            <div className="popup-content p-4 rounded shadow bg-white" style={{ maxWidth: '800px', width: '100%' }}>
                <h5>{isUpdate ? 'Cập nhật Yêu cầu' : 'Tạo Yêu cầu Chuyến đi'}</h5>

                {loadingTripData ? (
                    <div className="text-center p-5">Đang tải dữ liệu...</div>
                ) : (
                    <>
                        {/* Hàng 1: Từ đâu / Đến đâu */}
                        <div className="row g-3 mt-2">
                            {/* Cột 1: TỪ ĐÂU */}
                            <div className="col-md-6">
                                <div className="form-group">
                                    <label>Từ địa điểm</label>
                                    <div className="input-group">
                                        <input
                                            type="text"
                                            className={`form-control ${errors.fromLocation ? 'is-invalid' : ''}`}
                                            name="fromLocation"
                                            value={values.fromLocation}
                                            onChange={handleChange} // Cập nhật text khi gõ
                                            placeholder="Nhập địa chỉ rồi nhấn Tìm"
                                        />
                                        <button
                                            className="btn btn-outline-secondary"
                                            type="button"
                                            onClick={() => handleSearchLocation('from')}
                                        >
                                            Tìm
                                        </button>
                                    </div>
                                    {errors.fromLocation && (
                                        <div className="text-danger mt-1">{errors.fromLocation}</div>
                                    )}

                                    {/* Hiển thị danh sách kết quả tìm kiếm */}
                                    {fromResults.length > 0 && (
                                        <ul
                                            className="list-group mt-2"
                                            style={{ maxHeight: '150px', overflowY: 'auto' }}
                                        >
                                            {fromResults.map((result) => (
                                                <li
                                                    key={result.place_id}
                                                    className="list-group-item list-group-item-action"
                                                    style={{ cursor: 'pointer' }}
                                                    onClick={() => handleSelectResult('from', result)}
                                                >
                                                    {result.display_name}
                                                </li>
                                            ))}
                                        </ul>
                                    )}
                                </div>
                            </div>

                            {/* Cột 2: ĐẾN ĐÂU */}
                            <div className="col-md-6">
                                <div className="form-group">
                                    <label>Đến địa điểm</label>
                                    <div className="input-group">
                                        <input
                                            type="text"
                                            className={`form-control ${errors.toLocation ? 'is-invalid' : ''}`}
                                            name="toLocation"
                                            value={values.toLocation}
                                            onChange={handleChange} // Cập nhật text khi gõ
                                            placeholder="Nhập địa chỉ rồi nhấn Tìm"
                                        />
                                        <button
                                            className="btn btn-outline-secondary"
                                            type="button"
                                            onClick={() => handleSearchLocation('to')} // <-- Sửa 'from' thành 'to'
                                        >
                                            Tìm
                                        </button>
                                    </div>
                                    {errors.toLocation && <div className="text-danger mt-1">{errors.toLocation}</div>}

                                    {/* Hiển thị danh sách kết quả tìm kiếm 'to' */}
                                    {toResults.length > 0 && ( // <-- Sửa fromResults thành toResults
                                        <ul
                                            className="list-group mt-2"
                                            style={{ maxHeight: '150px', overflowY: 'auto' }}
                                        >
                                            {toResults.map(
                                                (
                                                    result, // <-- Sửa fromResults thành toResults
                                                ) => (
                                                    <li
                                                        key={result.place_id}
                                                        className="list-group-item list-group-item-action"
                                                        style={{ cursor: 'pointer' }}
                                                        onClick={() => handleSelectResult('to', result)} // <-- Sửa 'from' thành 'to'
                                                    >
                                                        {result.display_name}
                                                    </li>
                                                ),
                                            )}
                                        </ul>
                                    )}
                                </div>
                            </div>
                        </div>

                        {/* Hàng 2: Tọa độ (ẩn) - Chủ yếu để debug, có thể xóa đi */}
                        <div className="row g-3 mt-2">
                            <div className="col-6 text-muted small">
                                From Lat/Lng: {values.fromLatitude.toFixed(4)}, {values.fromLongitude.toFixed(4)}
                            </div>
                            <div className="col-6 text-muted small">
                                To Lat/Lng: {values.toLatitude.toFixed(4)}, {values.toLongitude.toFixed(4)}
                            </div>
                        </div>

                        {/* Hàng 3: Mô tả */}
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
                                    ></textarea>
                                    {errors.description && <div className="text-danger mt-1">{errors.description}</div>}
                                </div>
                            </div>
                        </div>

                        {/* Nút bấm */}
                        <div className="text-end mt-4">
                            <button className="btn btn-secondary me-2" onClick={onClose}>
                                Hủy
                            </button>
                            <button className="btn btn-primary" onClick={handleSubmit} disabled={isSubmitDisabled}>
                                {isUpdate ? 'Lưu thay đổi' : 'Tạo yêu cầu'}
                            </button>
                        </div>
                    </>
                )}
            </div>
        </div>
    );
}

export default TripRequestFormPopup;
