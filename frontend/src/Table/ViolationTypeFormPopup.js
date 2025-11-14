import React from 'react';
import axios from 'axios';
import { useFormValidation } from '../validator/useFormValidation';
import { required, maxLength } from '../validator/validators';

function ViolationTypeFormPopup({ item, onClose, apiUrl, token, onSuccess, showConfirmModal, showNotifyModal }) {
    // THAY ĐỔI: Thêm color
    const initialState = {
        name: item?.name || '',
        description: item?.description || '',
        color: item?.color || '#000000', // THÊM LẠI TRƯỜNG COLOR
    };

    const validationRules = {
        name: [required, maxLength(255)],
        description: [maxLength(500)],
    };

    const { values, errors, handleChange, validateForm, isSubmitDisabled } = useFormValidation(
        initialState,
        validationRules,
    );
    const id = item?.id || null;
    const createdDate = item?.createdDate
        ? new Date(item.createdDate).toLocaleString('vi-VN')
        : new Date().toLocaleString('vi-VN');

    const handleSubmit = () => {
        const isFormValid = validateForm();
        if (!isFormValid) {
            return;
        }

        onClose();
        showConfirmModal(item ? 'Bạn có chắc chắn muốn cập nhật?' : 'Bạn có chắc chắn muốn thêm mới?', async () => {
            try {
                // THAY ĐỔI: Thêm color vào payload
                const payload = {
                    name: values.name,
                    description: values.description,
                    color: values.color, // THÊM LẠI TRƯỜNG COLOR
                };
                if (item) {
                    await axios.put(`${apiUrl}`, { ...payload, id }, { headers: { Authorization: `Bearer ${token}` } });
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

    return (
        <div className="popup-overlay">
            <div className="popup-content p-4 rounded shadow bg-white">
                <h5>{item ? 'Cập nhật loại vi phạm' : 'Thêm mới loại vi phạm'}</h5>

                <div className="form-group mt-3">
                    <label>Tên loại vi phạm</label>
                    <input
                        type="text"
                        className={`form-control ${errors.name ? 'is-invalid' : ''}`}
                        name="name"
                        value={values.name}
                        onChange={handleChange}
                    />
                    {errors.name && <div className="text-danger mt-1">{errors.name}</div>}
                </div>

                <div className="form-group mt-3">
                    <label>Mô tả</label>
                    <textarea
                        className={`form-control ${errors.description ? 'is-invalid' : ''}`}
                        name="description"
                        value={values.description}
                        onChange={handleChange}
                        rows="3"
                    />
                    {errors.description && <div className="text-danger mt-1">{errors.description}</div>}
                </div>

                {/* THÊM LẠI TRƯỜNG MÀU SẮC */}
                <div className="form-group mt-3">
                    <label>Màu sắc</label>
                    <input
                        type="color"
                        className="form-control form-control-color"
                        name="color"
                        value={values.color}
                        onChange={handleChange}
                    />
                </div>

                <div className="form-group mt-3">
                    <label>Ngày tạo</label>
                    <input type="text" className="form-control" value={createdDate} disabled />
                </div>

                <div className="text-end mt-4">
                    <button className="btn btn-secondary me-2" onClick={onClose}>
                        Hủy
                    </button>
                    <button className="btn btn-primary" onClick={handleSubmit} disabled={isSubmitDisabled}>
                        {item ? 'Lưu thay đổi' : 'Thêm mới'}
                    </button>
                </div>
            </div>
        </div>
    );
}

export default ViolationTypeFormPopup;