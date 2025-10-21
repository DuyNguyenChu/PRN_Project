import React, { useState } from 'react';
import axios from 'axios';
import { useFormValidation } from '../validator/useFormValidation'; // Sửa lại đường dẫn nếu cần
import { required, maxLength } from '../validator/validators'; // Sửa lại đường dẫn nếu cần

function UserStatusFormPopup({ item, onClose, apiUrl, token, onSuccess, showConfirmModal, showNotifyModal }) {
    const initialState = {
        name: item?.name || '',
        color: item?.color || '#000000',
    };

    const validationRules = {
        name: [required, maxLength(255)], // Tên: Bắt buộc và tối đa 50 ký tự
        // Thêm quy tắc cho các trường khác nếu có
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
                const payload = { name: values.name, color: values.color };
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
                <h5>{item ? 'Cập nhật trạng thái' : 'Thêm mới trạng thái'}</h5>

                <div className="form-group mt-3">
                    <label>Tên trạng thái</label>
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

export default UserStatusFormPopup;
