import React, { useState, useEffect} from 'react';
import axios from 'axios';
import { useFormValidation } from '../validator/useFormValidation'; // Sửa lại đường dẫn nếu cần
import { required, maxLength } from '../validator/validators'; // Sửa lại đường dẫn nếu cần

// (Component này KHÔNG THAY ĐỔI, nó vẫn nhận props như cũ)
function MenuFormPopup({
    item,
    allMenus,
    menuType,
    apiUrl,
    token,
    onClose,
    onSuccess,
    showConfirmModal,
    actionApiUrl,
    showNotifyModal,
    isReadOnly,
}) {
    const isUpdate = !!item;

    const initialState = {
        name: item?.name || '',
        icon: item?.icon || '',
        url: item?.url || '',
        sortOrder: item?.sortOrder || 0,
        parentId: item?.parentId || null,
        className: item?.className || '',
    };

    const validationRules = {
        name: [required, maxLength(100)],
        sortOrder: [required],
        url: [maxLength(500)],
        icon: [maxLength(100)],
    };

    const { values, errors, handleChange, setValues, validateForm } = useFormValidation(
        initialState,
        validationRules,
    );

    const [isSubmitting, setIsSubmitting] = useState(false);

    // --- (MỚI) State cho Actions và Admin Only ---
    const [allActions, setAllActions] = useState([]); // Danh sách actions (VD: {id: 1, name: "Xem"})
    const [selectedActionIds, setSelectedActionIds] = useState(new Set());
    const [isAdminOnly, setIsAdminOnly] = useState(false);
    const [loadingData, setLoadingData] = useState(true); // Loading Actions VÀ Item chi tiết

    // (SỬA LỖI KEY PROP)
    const renderMenuOptions = (parentId, depth) => {
        const prefix = '— '.repeat(depth);
        return (
            allMenus
                // (SỬA LỖI HIỂN THỊ CON)
                .filter((menu) => {
                    if (parentId === null) return !menu.parentId;
                    return menu.parentId === parentId;
                })
                .sort((a, b) => a.sortOrder - b.sortOrder)
                .flatMap((menu) => [
                    <option key={menu.id} value={menu.id} disabled={isUpdate && item.id === menu.id}>
                        {prefix} {menu.name}
                    </option>,
                    ...renderMenuOptions(menu.id, depth + 1),
                ])
        );
    };

    useEffect(() => {
        const loadDependencies = async () => {
            setLoadingData(true);
            try {
                // 1. Luôn tải danh sách Actions
                const actionRes = await axios.get(actionApiUrl, {
                    headers: { Authorization: `Bearer ${token}` },
                });
                setAllActions(actionRes.data.resources || []);

                // 2. Nếu là "Sửa", tải chi tiết Menu
                if ((isUpdate || isReadOnly) && item?.id) {
                    const itemRes = await axios.get(`${apiUrl}/${item.id}`, {
                        headers: { Authorization: `Bearer ${token}` },
                    });
                    const fullItem = itemRes.data.resources;

                    // Cập nhật lại form với dữ liệu đầy đủ
                    setValues({
                        name: fullItem.name || '',
                        icon: fullItem.icon || '',
                        url: fullItem.url || '',
                        sortOrder: fullItem.sortOrder || 0,
                        parentId: fullItem.parentId || null,
                        className: fullItem.className || '',
                    });

                    // Cập nhật state cho Checkboxes
                    setIsAdminOnly(fullItem.isAdminOnly || false);
                    setSelectedActionIds(new Set(fullItem.actionIds || []));
                }
            } catch (err) {
                showNotifyModal('Lỗi tải dữ liệu cho form: ' + (err.response?.data?.message || err.message), false);
            } finally {
                setLoadingData(false);
            }
        };

        if (token && actionApiUrl) {
            loadDependencies();
        }
    }, [item, apiUrl, actionApiUrl, token, isUpdate, setValues, showNotifyModal, isReadOnly]);

    // (MỚI) Hàm xử lý check/uncheck Action
    const handleActionToggle = (actionId) => {
        if (isReadOnly) return;
        setSelectedActionIds((prevSet) => {
            const newSet = new Set(prevSet);
            if (newSet.has(actionId)) {
                newSet.delete(actionId);
            } else {
                newSet.add(actionId);
            }
            return newSet;
        });
    };

    const handleSubmit = () => {
        if (isReadOnly) {
            onClose();
            return;
        }

        if (!validateForm()) return;
        const title = isUpdate ? 'Bạn có chắc chắn muốn cập nhật?' : 'Bạn có chắc chắn muốn thêm mới?';

        // Dùng showConfirmModal được truyền từ MenuPage (giờ là hàm nội bộ)
        showConfirmModal(title, async () => {
            setIsSubmitting(true);
            try {
                const payload = {
                    ...values,
                    id: item?.id || 0, // ID cho update
                    menuType: menuType, // Lấy từ props
                    parentId: values.parentId || null,
                    sortOrder: Number(values.sortOrder) || 0,
                    className: values.className || null, // Gửi null nếu rỗng

                    // (MỚI) Thêm các trường mới
                    isAdminOnly: isAdminOnly,
                    actionIds: Array.from(selectedActionIds),
                };

                if (isUpdate) {
                    await axios.put(`${apiUrl}`, payload, {
                        headers: { Authorization: `Bearer ${token}` },
                    });
                    // Dùng showNotifyModal được truyền từ MenuPage
                    showNotifyModal('Cập nhật thành công!', true);
                } else {
                    await axios.post(apiUrl, payload, {
                        headers: { Authorization: `Bearer ${token}` },
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

    const isSubmitDisabledCalculated = !isReadOnly && Object.values(errors).some((error) => error !== null);

    return (
        <div className="popup-overlay">
            <div className="popup-content p-0" style={{ width: '700px' }}>
                <div className="modal-content">
                    <div className="modal-header">
                        <h2 className="fw-bold">
                            {isReadOnly ? 'Chi tiết Menu' : isUpdate ? 'Cập nhật Menu' : 'Thêm mới Menu'}
                        </h2>
                        <button className="btn btn-icon btn-sm" onClick={onClose} disabled={isSubmitting}>
                            <svg
                                xmlns="http://www.w3.org/2000/svg"
                                width="16"
                                height="16"
                                fill="currentColor"
                                className="bi bi-x-lg"
                                viewBox="0 0 16 16"
                            >
                                <path d="M2.146 2.854a.5.5 0 1 1 .708-.708L8 7.293l5.146-5.147a.5.5 0 0 1 .708.708L8.707 8l5.147 5.146a.5.5 0 0 1-.708.708L8 8.707l-5.146 5.147a.5.5 0 0 1-.708-.708L7.293 8 2.146 2.854Z" />
                            </svg>
                        </button>
                    </div>

                    <div className="modal-body p-4">
                        <div className="scroll-y" style={{ maxHeight: '60vh', overflowY: 'auto', padding: '0 1rem' }}>
                            <form id="menu_form">
                                <fieldset disabled={isReadOnly}>
                                    {' '}
                                    {/* (SỬA) Dùng fieldset để disable hàng loạt */}
                                    <div className="row">
                                        <div className="col-md-6">
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
                                        </div>
                                        <div className="col-md-6">
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
                                                {errors.parentId && (
                                                    <div className="invalid-feedback">{errors.parentId}</div>
                                                )}
                                            </div>
                                        </div>
                                    </div>
                                    <div className="row">
                                        <div className="col-md-6">
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
                                        </div>
                                        <div className="col-md-6">
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
                                        </div>
                                    </div>
                                    <div className="row">
                                        <div className="col-md-6">
                                            <div className="form-group mb-3">
                                                <label className="form-label fw-bold">Thứ tự</label>
                                                <input
                                                    type="number"
                                                    className={`form-control ${errors.sortOrder ? 'is-invalid' : ''}`}
                                                    name="sortOrder"
                                                    value={values.sortOrder}
                                                    onChange={handleChange}
                                                />
                                                {errors.sortOrder && (
                                                    <div className="invalid-feedback">{errors.sortOrder}</div>
                                                )}
                                            </div>
                                        </div>
                                        <div className="col-md-6">
                                            <div className="form-group mb-3">
                                                <label className="form-label fw-bold">Class (CSS)</label>
                                                <input
                                                    type="text"
                                                    className={`form-control ${errors.className ? 'is-invalid' : ''}`}
                                                    name="className"
                                                    value={values.className}
                                                    onChange={handleChange}
                                                />
                                                {errors.className && (
                                                    <div className="invalid-feedback">{errors.className}</div>
                                                )}
                                            </div>
                                        </div>
                                    </div>
                                    {/* --- Hành Động và Admin Only --- */}
                                    <div className="form-group mb-3">
                                        <label className="form-label fw-bold">Hành động cho Menu</label>
                                        <div className="d-flex flex-wrap">
                                            {allActions.length > 0 ? (
                                                allActions.map((action) => (
                                                    <div
                                                        key={action.id}
                                                        className="form-check form-check-custom form-check-solid me-4 mb-2"
                                                    >
                                                        <input
                                                            className="form-check-input"
                                                            type="checkbox"
                                                            id={`action_${action.id}`}
                                                            checked={selectedActionIds.has(action.id)}
                                                            onChange={() => handleActionToggle(action.id)}
                                                            // Disabled nếu readOnly
                                                            disabled={isReadOnly}
                                                        />
                                                        <label
                                                            className="form-check-label"
                                                            htmlFor={`action_${action.id}`}
                                                        >
                                                            {action.name}
                                                        </label>
                                                    </div>
                                                ))
                                            ) : (
                                                <span className="text-muted">Không tìm thấy hành động nào.</span>
                                            )}
                                        </div>
                                        {errors.actionIds && (
                                            <div className="text-danger mt-1 fs-7">{errors.actionIds}</div>
                                        )}
                                    </div>
                                    <div className="form-group mb-3">
                                        <div className="form-check form-check-custom form-check-solid">
                                            <input
                                                className="form-check-input"
                                                type="checkbox"
                                                id="isAdminOnlyCheck"
                                                checked={isAdminOnly}
                                                onChange={(e) => setIsAdminOnly(e.target.checked)}
                                                // Disabled nếu readOnly
                                                disabled={isReadOnly}
                                            />
                                            <label className="form-check-label fw-bold" htmlFor="isAdminOnlyCheck">
                                                Chỉ dành cho Admin
                                            </label>
                                        </div>
                                        {errors.isAdminOnly && (
                                            <div className="text-danger mt-1 fs-7">{errors.isAdminOnly}</div>
                                        )}
                                    </div>
                                </fieldset>{' '}
                                {/* Kết thúc fieldset */}
                            </form>
                        </div>
                    </div>

                    <div className="modal-footer flex-center p-4">
                        {isReadOnly ? (
                            <button className="btn btn-primary" onClick={onClose}>
                                OK
                            </button>
                        ) : (
                            <>
                                <button className="btn btn-secondary me-3" onClick={onClose} disabled={isSubmitting}>
                                    Hủy
                                </button>
                                <button
                                    className="btn btn-primary"
                                    onClick={handleSubmit}
                                    disabled={isSubmitDisabledCalculated || loadingData || isSubmitting}
                                >
                                    {isSubmitting ? 'Đang lưu...' : 'Lưu'}
                                </button>
                            </>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
}

export default MenuFormPopup;
