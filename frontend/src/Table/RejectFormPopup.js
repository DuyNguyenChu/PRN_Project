import React from 'react';
import axios from 'axios';
import { useFormValidation } from '../validator/useFormValidation'; // Sửa lại đường dẫn nếu cần
// THAY ĐỔI: Chỉ import 'required'
import { required } from '../validator/validators'; 

function RejectFormPopup({ item, onClose, apiUrl, token, onSuccess, showConfirmModal, showNotifyModal }) {
    
    const initialState = {
        rejectReason: '',
    };

    // THAY ĐỔI: Chỉ giữ lại 'required'
    const validationRules = {
        rejectReason: [required],
    };

    const { values, errors, handleChange, validateForm, isSubmitDisabled } = useFormValidation(
        initialState,
        validationRules,
    );

    const handleSubmit = () => {
        const isFormValid = validateForm();
        if (!isFormValid) return;

        onClose(); // Đóng popup lý do
        showConfirmModal('Bạn có chắc chắn muốn từ chối nhật ký này?', async () => {
            try {
                const payload = {
                    id: item.id,
                    rejectReason: values.rejectReason,
                };
                
                await axios.put(`${apiUrl}/reject`, payload, { headers: { Authorization: `Bearer ${token}` } });
                showNotifyModal('Đã từ chối thành công!');
                onSuccess(); // Tải lại bảng
            } catch (err) {
                showNotifyModal('Thao tác thất bại: ' + (err.response?.data?.message || err.message), false);
            }
        });
    };

    return (
        <div className="popup-overlay">
            <div className="popup-content p-4 rounded shadow bg-white">
                <h5>Lý do từ chối</h5>
                
                <div className="form-group mt-3">
                    <label>Lý do</label>
                    <textarea
                        className={`form-control ${errors.rejectReason ? 'is-invalid' : ''}`}
                        name="rejectReason"
                        value={values.rejectReason}
                        onChange={handleChange}
                        rows="4"
                        placeholder="Nhập lý do từ chối..."
                    />
                    {errors.rejectReason && <div className="text-danger mt-1">{errors.rejectReason}</div>}
                </div>

                <div className="text-end mt-4">
                    <button className="btn btn-secondary me-2" onClick={onClose}>
                        Hủy
                    </button>
                    <button
                        className="btn btn-primary"
                        onClick={handleSubmit}
                        disabled={isSubmitDisabled} 
                    >
                        Xác nhận
                    </button>
                </div>
            </div>
        </div>
    );
}

export default RejectFormPopup;