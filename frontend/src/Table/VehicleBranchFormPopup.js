// import React, { useState } from 'react';
import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useFormValidation } from '../validator/useFormValidation'; // Sửa lại đường dẫn nếu cần
import { required, maxLength } from '../validator/validators'; // Sửa lại đường dẫn nếu cần

function VehicleBranchFormPopup({ item, onClose, apiUrl, token, onSuccess, showConfirmModal, showNotifyModal }) {
    const initialState = {
        name: item?.name || '',
        description: item?.description || '',
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
    const LastModifiedDate = item.LastModifiedDate ?? new Date().toLocaleString();

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
                    description: values.description,
                    lastModifiedDate: new Date().toISOString(),
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

    return (
        <div className="popup-overlay">
            <div className="popup-content p-4 rounded shadow bg-white">
                <h5>{item ? 'Cập nhật Xe' : 'Thêm Xe'}</h5>

                <div className="form-group mt-3">
                    <label>Tên Chi nhánh xe</label>
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
                    <label>Mô tả</label>
                    <input
                        type="text"
                        className={`form-control ${errors.description ? 'is-invalid' : ''}`} // Thêm class is-invalid khi có lỗi
                        name="description" // Rất quan trọng: Thêm thuộc tính `name`
                        value={values.description} // Sử dụng `values.name`
                        onChange={handleChange} // Sử dụng `handleChange`
                    />
                    {/* Bước 5: Hiển thị lỗi */}
                    {errors.description && <div className="text-danger mt-1">{errors.description}</div>}
                </div>

                <div className="form-group mt-3">
                    <label>Ngày sửa đổi cuối cùng</label>
                    <input type="text" className="form-control" value={LastModifiedDate} disabled />
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

export default VehicleBranchFormPopup;
