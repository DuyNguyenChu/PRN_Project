// import React, { useState } from 'react';
import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useFormValidation } from '../validator/useFormValidation'; // Sửa lại đường dẫn nếu cần
import { required, maxLength } from '../validator/validators'; // Sửa lại đường dẫn nếu cần

function VehicleFormPopup({ item, onClose, apiUrl, token, onSuccess, showConfirmModal, showNotifyModal }) {
    const initialState = {
        name: item?.name || '',
        vehicleTypeId: item?.vehicleTypeId || '',
        vehicleStatusId: item?.vehicleStatusId || '',
        vehicleBranchId: item?.vehicleBranchId || '',
        vehicleModelId: item?.vehicleModelId || '',
        color: item?.color || '#000000',
    };
    const validationRules = {
        name: [required, maxLength(255)], // Tên: Bắt buộc và tối đa 50 ký tự
        // Thêm quy tắc cho các trường khác nếu có
        vehicleTypeId: [required],
        vehicleStatusId: [required],
        vehicleBranchId: [required],
        vehicleModelId: [required],
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
                    name: values.name,
                    color: values.color,
                    vehicleTypeId: Number(values.vehicleTypeId),
                    vehicleStatusId: Number(values.vehicleStatusId),
                    vehicleBranchId: Number(values.vehicleBranchId),
                    vehicleModelId: Number(values.vehicleModelId),
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

    const [vehicleTypes, setVehicleTypes] = useState([]);
    const [vehicleStatuses, setVehicleStatuses] = useState([]);
    const [vehicleBranches, setVehicleBranches] = useState([]);
    const [vehicleModels, setVehicleModels] = useState([]);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const [types, statuses, branches, models] = await Promise.all([
                    // const [statuses, branches, models] = await Promise.all([
                    axios.get('http://localhost:5180/api/vehicle-types', {
                        headers: { Authorization: `Bearer ${token}` },
                    }),
                    axios.get('http://localhost:5180/api/VehicleStatus', {
                        headers: { Authorization: `Bearer ${token}` },
                    }),
                    axios.get('http://localhost:5180/api/VehicleBranch', {
                        headers: { Authorization: `Bearer ${token}` },
                    }),
                    axios.get('http://localhost:5180/api/VehicleModel', {
                        headers: { Authorization: `Bearer ${token}` },
                    }),
                ]);
                setVehicleTypes(types.data);
                setVehicleStatuses(statuses.data.resources);
                setVehicleBranches(branches.data.resources);
                setVehicleModels(models.data.resources);
            } catch (err) {
                showNotifyModal('Không thể tải dữ liệu dropdown: ' + err.message, false);
            }
        };
        fetchData();
    }, [token]);

    return (
        <div className="popup-overlay">
            <div className="popup-content p-4 rounded shadow bg-white">
                <h5>{item ? 'Cập nhật Xe' : 'Thêm Xe'}</h5>

                <div className="form-group mt-3">
                    <label>Tên xe</label>
                    <input
                        type="text"
                        className={`form-control ${errors.name ? 'is-invalid' : ''}`} // Thêm class is-invalid khi có lỗi
                        name="name" // Rất quan trọng: Thêm thuộc tính `name`
                        value={values.name} // Sử dụng `values.name`
                        onChange={handleChange} // Sử dụng `handleChange`
                    />
                    {/* Bước 5: Hiển thị lỗi */}
                    {errors.name && <div className="text-danger mt-1">{errors.name}</div>}
                </div>

                <div className="form-group mt-3">
                    <label>Loại xe</label>
                    <select
                        className="form-select"
                        name="vehicleTypeId"
                        value={values.vehicleTypeId ?? ''}
                        onChange={(e) =>
                            handleChange({
                                target: { name: 'vehicleTypeId', value: Number(e.target.value) },
                            })
                        }
                    >
                        <option value="">-- Chọn loại xe --</option>
                        {vehicleTypes.map((type) => (
                            <option key={type.id} value={type.id}>
                                {type.name}
                            </option>
                        ))}
                    </select>
                    {errors.vehicleTypeId && <div className="text-danger mt-1">{errors.vehicleTypeId}</div>}
                </div>

                <div className="form-group mt-3">
                    <label>trạng thái xe</label>
                    <select
                        className="form-select"
                        name="vehicleStatusId"
                        value={values.vehicleStatusId ?? ''}
                        onChange={(e) =>
                            handleChange({
                                target: { name: 'vehicleStatusId', value: Number(e.target.value) },
                            })
                        }
                    >
                        <option value="">-- Chọn trạng thái xe --</option>
                        {vehicleStatuses.map((type) => (
                            <option key={type.id} value={type.id}>
                                {type.name}
                            </option>
                        ))}
                    </select>
                    {errors.vehicleStatusId && <div className="text-danger mt-1">{errors.vehicleStatusId}</div>}
                </div>

                <div className="form-group mt-3">
                    <label>chi nhánh xe</label>
                    <select
                        className="form-select"
                        name="vehicleBranchId"
                        value={values.vehicleBranchId ?? ''}
                        onChange={(e) =>
                            handleChange({
                                target: { name: 'vehicleBranchId', value: Number(e.target.value) },
                            })
                        }
                    >
                        <option value="">-- Chọn chi nhánh xe --</option>
                        {vehicleBranches.map((type) => (
                            <option key={type.id} value={type.id}>
                                {type.name}
                            </option>
                        ))}
                    </select>
                    {errors.vehicleBranchId && <div className="text-danger mt-1">{errors.vehicleBranchId}</div>}
                </div>

                <div className="form-group mt-3">
                    <label>mẫu xe</label>
                    <select
                        className="form-select"
                        name="vehicleModelId"
                        value={values.vehicleModelId || ''}
                        onChange={(e) =>
                            handleChange({
                                target: { name: 'vehicleModelId', value: Number(e.target.value) },
                            })
                        }
                    >
                        <option value="">-- Chọn mẫu xe --</option>
                        {vehicleModels.map((type) => (
                            <option key={type.id} value={type.id}>
                                {type.name}
                            </option>
                        ))}
                    </select>
                    {errors.vehicleModelId && <div className="text-danger mt-1">{errors.vehicleModelId}</div>}
                </div>

                <div className="form-group mt-3">
                    <label>Màu sắc</label>
                    <input
                        type="color"
                        className="form-control form-control-color"
                        name="color" // Rất quan trọng: Thêm thuộc tính `name`
                        value={values.color} // Sử dụng `values.color`
                        onChange={handleChange} // Sử dụng `handleChange`
                    />
                    {/* Không có lỗi cho trường này */}
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

export default VehicleFormPopup;
