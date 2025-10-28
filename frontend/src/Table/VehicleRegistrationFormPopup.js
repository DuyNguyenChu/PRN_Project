// import React, { useState } from 'react';
import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useFormValidation } from '../validator/useFormValidation'; // Sửa lại đường dẫn nếu cần
import { required, maxLength } from '../validator/validators'; // Sửa lại đường dẫn nếu cần

function VehicleRegistrationFormPopup({ item, onClose, apiUrl, token, onSuccess, showConfirmModal, showNotifyModal }) {
    const initialState = {
        // name: item?.name || '',
        vehicleId: item?.vehicleId || '',
        registrationNumber: item?.registrationNumber || '',
        status: item?.status || 0,
        issueDate: item?.issueDate ? item.issueDate.split('T')[0] : new Date().toISOString().split('T')[0],
        expiryDate: item?.expiryDate ? item.expiryDate.split('T')[0] : new Date().toISOString().split('T')[0],
    };
    const validationRules = {
        // name: [required, maxLength(255)], // Tên: Bắt buộc và tối đa 50 ký tự
        // Thêm quy tắc cho các trường khác nếu có
        vehicleId: [required],
        registrationNumber: [required, maxLength(50)],
    };

    // Bước 3: Sử dụng hook
    const { values, errors, handleChange, validateForm, isSubmitDisabled } = useFormValidation(
        initialState,
        validationRules,
    );
    const [id, setId] = useState(item?.id || null);
    const createdDate = item?.createdDate || new Date().toLocaleString();

    const handleSubmit = () => {
        // Bước 4: Validate toàn bộ form trước khi thực hiện logic submit
        const isFormValid = validateForm();
        if (!isFormValid) {
            return; // Nếu form không hợp lệ, dừng lại
        }

        // Nếu hợp lệ, tiếp tục logic cũ
        onClose();
        showConfirmModal(item ? 'Bạn có chắc chắn muốn cập nhật?' : 'Bạn có chắc chắn muốn thêm mới?', async () => {
            try {
                // Dùng `values` từ hook
                const payload = {
                    // name: values.name,
                    vehicleId: Number(values.vehicleId),
                    registrationNumber: values.registrationNumber,
                    issueDate: new Date(values.issueDate).toISOString(),
                    expiryDate: new Date(values.expiryDate).toISOString(),
                    status: values.status || 0,
                };
                if (item) {
                    await axios.put(
                        `${apiUrl}/${id}`,
                        { ...payload, id },
                        { headers: { Authorization: `Bearer ${token}` } },
                    );
                    showNotifyModal('Cập nhật thành công!');
                } else {
                    await axios.post(apiUrl, payload, { headers: { Authorization: `Bearer ${token}` } });
                    showNotifyModal('Thêm mới thành công!');
                }
                onSuccess();
            } catch (err) {
                showNotifyModal('Thao tác thất bại: ' + (err.response?.data?.message || err.message), false);
            }
        });
    };

    const [vehicle, setVehicle] = useState([]);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await axios.get('http://localhost:5180/api/VehicleModel', {
                    headers: { Authorization: `Bearer ${token}` },
                });

                setVehicle(response.data.resources || response.data);
            } catch (err) {
                showNotifyModal('Không thể tải dữ liệu dropdown: ' + err.message, false);
            }
        };
        fetchData();
    }, [token]);

    useEffect(() => {
        if (vehicle.length > 0 && item?.vehicleId) {
            handleChange({
                target: { name: 'vehicleId', value: item.vehicleId },
            });
        }
    }, [vehicle, item]);

    return (
        <div className="popup-overlay">
            <div className="popup-content p-4 rounded shadow bg-white">
                <h5>{item ? 'Cập nhật Đăng ký xe' : 'Thêm Đăng ký xe'}</h5>

                <div className="form-group mt-3">
                    <label>Tên xe</label>
                    {/* <select
                        className="form-select"
                        name="vehicleId"
                        value={values.vehicleId ? String(values.vehicleId) : ''} // 🔹 ép về string
                        onChange={(e) =>
                            handleChange({
                                target: { name: 'vehicleId', value: Number(e.target.value) },
                            })
                        }
                    >
                        <option value="">-- Chọn loại xe --</option>
                        {vehicle.map((type) => (
                            <option key={type.id} value={type.id}>
                                {type.name}
                            </option>
                        ))}
                    </select> */}
                    <select
                        className="form-select"
                        name="vehicleId"
                        value={values.vehicleId ? String(values.vehicleId) : ''} // ép về string
                        onChange={(e) =>
                            handleChange({
                                target: { name: 'vehicleId', value: Number(e.target.value) },
                            })
                        }
                    >
                        <option value="">-- Chọn xe --</option>
                        {vehicle.map((v) => (
                            <option key={v.id} value={v.id}>
                                {v.name}
                            </option>
                        ))}
                    </select>

                    {errors.vehicleId && <div className="text-danger mt-1">{errors.vehicleId}</div>}
                </div>

                <div className="form-group mt-3">
                    <label>Số đăng ký</label>
                    <input
                        type="text"
                        className={`form-control ${errors.registrationNumber ? 'is-invalid' : ''}`} // Thêm class is-invalid khi có lỗi
                        name="registrationNumber" // Rất quan trọng: Thêm thuộc tính `name`
                        value={values.registrationNumber} // Sử dụng `values.name`
                        onChange={handleChange} // Sử dụng `handleChange`
                    />
                    {/* Bước 5: Hiển thị lỗi */}
                    {errors.registrationNumber && <div className="text-danger mt-1">{errors.registrationNumber}</div>}
                </div>

                <div className="form-group mt-3">
                    <label>Ngày phát hành</label>
                    <input
                        type="date"
                        className={`form-control ${errors.issueDate ? 'is-invalid' : ''}`}
                        name="issueDate"
                        value={values.issueDate}
                        onChange={handleChange}
                    />
                    {errors.issueDate && <div className="text-danger mt-1">{errors.issueDate}</div>}
                </div>

                <div className="form-group mt-3">
                    <label>Ngày hết hạn</label>
                    <input
                        type="date"
                        className={`form-control ${errors.expiryDate ? 'is-invalid' : ''}`}
                        name="expiryDate"
                        value={values.expiryDate}
                        onChange={handleChange}
                    />
                    {errors.expiryDate && <div className="text-danger mt-1">{errors.expiryDate}</div>}
                </div>

                <div className="form-group mt-3">
                    <label>Ngày tạo</label>
                    <input type="text" className="form-control" value={createdDate} disabled />
                </div>

                <div className="text-end mt-4">
                    <button className="btn btn-secondary me-2" onClick={onClose}>
                        Hủy
                    </button>
                    <button
                        className="btn btn-primary"
                        onClick={handleSubmit}
                        disabled={isSubmitDisabled} // Bước 6: Vô hiệu hóa nút khi có lỗi
                    >
                        {item ? 'Lưu thay đổi' : 'Thêm mới'}
                    </button>
                </div>
            </div>
        </div>
    );
}

export default VehicleRegistrationFormPopup;
