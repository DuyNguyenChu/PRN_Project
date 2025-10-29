import React, { useState, useEffect, useCallback, useRef } from 'react';
import axios from 'axios';
import { useFormValidation } from '../validator/useFormValidation'; // Sửa lại đường dẫn nếu cần
import { required, maxLength } from '../validator/validators'; // Sửa lại đường dẫn nếu cần

// (Component này KHÔNG THAY ĐỔI, nó vẫn nhận props như cũ)
function MenuFormPopup({ item, allMenus, menuType, apiUrl, token, onClose, onSuccess, showConfirmModal, showNotifyModal }) {
    const isUpdate = !!item;

    const initialState = {
        name: item?.name || '',
        icon: item?.icon || '',
        url: item?.url || '',
        sortOrder: item?.sortOrder || 0,
        parentId: item?.parentId || null,
    };

    const validationRules = {
        name: [required, maxLength(100)],
        sortOrder: [required],
        url: [maxLength(500)],
        icon: [maxLength(100)],
    };

    const { values, errors, handleChange, validateForm, isSubmitDisabled } = useFormValidation(
        initialState,
        validationRules
    );

    const [isSubmitting, setIsSubmitting] = useState(false);

    // (SỬA LỖI KEY PROP)
    const renderMenuOptions = (parentId, depth) => {
        const prefix = '— '.repeat(depth);
        return allMenus
            // (SỬA LỖI HIỂN THỊ CON)
            .filter(menu => {
                if (parentId === null) return !menu.parentId;
                return menu.parentId == parentId;
            })
            .sort((a, b) => a.sortOrder - b.sortOrder) 
            .flatMap(menu => [
                <option 
                    key={menu.id} 
                    value={menu.id} 
                    disabled={isUpdate && item.id === menu.id}
                >
                    {prefix} {menu.name}
                </option>,
                ...renderMenuOptions(menu.id, depth + 1)
            ]);
    };

    const handleSubmit = () => {
        if (!validateForm()) return;

        const title = isUpdate ? 'Bạn có chắc chắn muốn cập nhật?' : 'Bạn có chắc chắn muốn thêm mới?';
        
        // Dùng showConfirmModal được truyền từ MenuPage (giờ là hàm nội bộ)
        showConfirmModal(title, async () => {
            setIsSubmitting(true);
            try {
                const payload = {
                    ...values,
                    id: item?.id || 0,
                    menuType: menuType,
                    parentId: values.parentId || null,
                    sortOrder: Number(values.sortOrder) || 0,
                };

                if (isUpdate) {
                    await axios.put(`${apiUrl}`, payload, { 
                        headers: { Authorization: `Bearer ${token}` } 
                    });
                    // Dùng showNotifyModal được truyền từ MenuPage
                    showNotifyModal('Cập nhật thành công!', true);
                } else {
                    await axios.post(apiUrl, payload, { 
                        headers: { Authorization: `Bearer ${token}` } 
                    });
                    // Dùng showNotifyModal được truyền từ MenuPage
                    showNotifyModal('Thêm mới thành công!', true);
                }
                onClose();
                onSuccess();
            } catch (err) {
                // Dùng showNotifyModal được truyền từ MenuPage
                showNotifyModal('Thao tác thất bại: ' + (err.response?.data?.message || err.message), false);
            } finally {
                setIsSubmitting(false);
            }
        });
    };

    return (
        <div className="popup-overlay">
            <div className="popup-content p-0" style={{ width: '700px' }}>
                <div className="modal-content">
                    <div className="modal-header">
                        <h2 className="fw-bold">
                            {isUpdate ? 'Cập nhật Menu' : 'Thêm mới Menu'}
                        </h2>
                        <button className="btn btn-icon btn-sm" onClick={onClose} disabled={isSubmitting}>
                             <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-x-lg" viewBox="0 0 16 16">
                                <path d="M2.146 2.854a.5.5 0 1 1 .708-.708L8 7.293l5.146-5.147a.5.5 0 0 1 .708.708L8.707 8l5.147 5.146a.5.5 0 0 1-.708.708L8 8.707l-5.146 5.147a.5.5 0 0 1-.708-.708L7.293 8 2.146 2.854Z"/>
                            </svg>
                        </button>
                    </div>

                    <div className="modal-body p-4">
                         <div className="scroll-y" style={{ maxHeight: '60vh', overflowY: 'auto', padding: '0 1rem' }}>
                            <form id="menu_form">
                                {/* ... (Nội dung form: name, parentId, icon, url, order) ... */}
                                <div className="form-group mb-3">
                                    <label className="form-label fw-bold">Tên menu</label>
                                    <input
                                        type="text"
                                        className={`form-control ${errors.name ? 'is-invalid' : ''}`}
                                        name="name"
                                        value={values.name}
                                        onChange={handleChange}
                                    />
                                    {errors.name && <div className="invalid-feedback">{errors.name}</div>}
                                </div>
                                
                                <div className="form-group mb-3">
                                    <label className="form-label fw-bold">Menu cha</label>
                                    <select
                                        className="form-select"
                                        name="parentId"
                                        value={values.parentId || ''}
                                        onChange={handleChange}
                                    >
                                        <option value="">(Là menu gốc)</option>
                                        {renderMenuOptions(null, 0)} 
                                    </select>
                                </div>

                                <div className="form-group mb-3">
                                    <label className="form-label fw-bold">Icon</label>
                                    <input
                                        type="text"
                                        className={`form-control ${errors.icon ? 'is-invalid' : ''}`}
                                        name="icon"
                                        value={values.icon}
                                        onChange={handleChange}
                                        placeholder="Vd: bi bi-speedometer"
                                    />
                                    {errors.icon && <div className="invalid-feedback">{errors.icon}</div>}
                                </div>

                                <div className="form-group mb-3">
                                    <label className="form-label fw-bold">Đường dẫn (URL)</label>
                                    <input
                                        type="text"
                                        className={`form-control ${errors.url ? 'is-invalid' : ''}`}
                                        name="url"
                                        value={values.url}
                                        onChange={handleChange}
                                        placeholder="Vd: /admin/dashboard"
                                    />
                                    {errors.url && <div className="invalid-feedback">{errors.url}</div>}
                                </div>

                                <div className="form-group mb-3">
                                    <label className="form-label fw-bold">Thứ tự</label>
                                    <input
                                        type="number"
                                        className={`form-control ${errors.sortOrder ? 'is-invalid' : ''}`}
                                        name="sortOrder"
                                        value={values.sortOrder}
                                        onChange={handleChange}
                                    />
                                    {errors.sortOrder && <div className="invalid-feedback">{errors.sortOrder}</div>}
                                </div>
                            </form>
                        </div>
                    </div>
                    
                    <div className="modal-footer flex-center p-4">
                        <button className="btn btn-secondary me-3" onClick={onClose} disabled={isSubmitting}>
                            Hủy
                        </button>
                        <button
                            className="btn btn-primary"
                            onClick={handleSubmit}
                            disabled={isSubmitDisabled || isSubmitting}
                        >
                            {isSubmitting ? 'Đang lưu...' : 'Lưu'}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default MenuFormPopup;