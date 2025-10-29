import React, { useState, useEffect, useRef, useCallback } from 'react';
import axios from 'axios';
import { useFormValidation } from '../validator/useFormValidation'; // Sửa lại đường dẫn nếu cần
import { required, maxLength } from '../validator/validators'; // Sửa lại đườngZN

// --- DỮ LIỆU GIẢ ĐỊNH ---
// Đây là cấu trúc dữ liệu cho API /api/v1/permissions/all
// Bạn CẦN thay thế API call trong useEffect bằng API thật
const FAKE_PERMISSIONS_DATA = [
    {
        groupName: "Quản lý Người dùng",
        permissions: [
            { id: 1, name: "Xem", code: "USER_VIEW" },
            { id: 2, name: "Thêm", code: "USER_CREATE" },
            { id: 3, name: "Sửa", code: "USER_UPDATE" },
            { id: 4, name: "Xóa", code: "USER_DELETE" },
        ]
    },
    {
        groupName: "Quản lý Vai trò",
        permissions: [
            { id: 5, name: "Xem", code: "ROLE_VIEW" },
            { id: 6, name: "Thêm", code: "ROLE_CREATE" },
            { id: 7, name: "Sửa", code: "ROLE_UPDATE" },
            { id: 8, name: "Xóa", code: "ROLE_DELETE" },
        ]
    },
    {
        groupName: "Quản lý Sản phẩm",
        permissions: [
            { id: 9, name: "Xem", code: "PRODUCT_VIEW" },
            { id: 10, name: "Thêm", code: "PRODUCT_CREATE" },
        ]
    }
];
// -----------------------

function RoleFormPopup({ item, onClose, apiUrl, onSuccess, showConfirmModal, showNotifyModal }) {
    const isUpdate = !!item;
    
    // Lấy token từ localStorage
    const userDataString = localStorage.getItem('userData'); 
    
    const initialState = {
        name: item?.name || '',
        description: item?.description || '',
    };

    const validationRules = {
        name: [required, maxLength(255)],
        description: [maxLength(500)],
    };

    const { values, errors, handleChange, validateForm, isSubmitDisabled } = useFormValidation(
        initialState,
        validationRules,
    );

    // State cho Permissions
    const [allPermissions, setAllPermissions] = useState([]); // Danh sách tất cả quyền
    const [selectedPermissions, setSelectedPermissions] = useState(new Set(item?.permissons || [])); // Các quyền đã chọn (dạng Set)
    const [loadingPermissions, setLoadingPermissions] = useState(true);
    const [isSubmitting, setIsSubmitting] = useState(false); // Trạng thái loading nút Save

    const selectAllCheckboxRef = useRef(null); // Ref cho checkbox "Chọn tất cả"

    // Tính toán tổng số quyền (chỉ 1 lần)
    const totalPermissionsCount = allPermissions.reduce((acc, group) => acc + group.permissions.length, 0);

    // 1. Fetch tất cả permissions khi component mount
    useEffect(() => {
        const fetchAllPermissions = async () => {
            setLoadingPermissions(true);
            try {
                // TODO: THAY THẾ API URL THẬT SỰ Ở ĐÂY
                // const userData = JSON.parse(userDataString);
                // const token = userData?.resources?.accessToken;
                // const res = await axios.get('/api/v1/permissions/all', { 
                //     headers: { Authorization: `Bearer ${token}` } 
                // });
                // setAllPermissions(res.data.resources || []);

                // ---- BẮT ĐẦU DÙNG DATA GIẢ ----
                await new Promise(resolve => setTimeout(resolve, 500)); // Giả lập loading
                setAllPermissions(FAKE_PERMISSIONS_DATA);
                // ---- KẾT THÚC DÙNG DATA GIẢ ----

            } catch (err) {
                showNotifyModal('Lỗi tải danh sách quyền: ' + (err.response?.data?.message || err.message), false);
            } finally {
                setLoadingPermissions(false);
            }
        };

        fetchAllPermissions();
    }, []); // Chỉ chạy 1 lần

    // 2. Cập nhật trạng thái checkbox "Chọn tất cả"
    useEffect(() => {
        if (!selectAllCheckboxRef.current || loadingPermissions || totalPermissionsCount === 0) return;

        const selectedCount = selectedPermissions.size;

        if (selectedCount === 0) {
            selectAllCheckboxRef.current.checked = false;
            selectAllCheckboxRef.current.indeterminate = false;
        } else if (selectedCount === totalPermissionsCount) {
            selectAllCheckboxRef.current.checked = true;
            selectAllCheckboxRef.current.indeterminate = false;
        } else {
            selectAllCheckboxRef.current.checked = false;
            selectAllCheckboxRef.current.indeterminate = true;
        }
    }, [selectedPermissions, loadingPermissions, totalPermissionsCount]);

    // 3. Xử lý khi click 1 checkbox quyền
    const handlePermissionChange = (permissionCode) => {
        setSelectedPermissions(prevSet => {
            const newSet = new Set(prevSet);
            if (newSet.has(permissionCode)) {
                newSet.delete(permissionCode);
            } else {
                newSet.add(permissionCode);
            }
            return newSet;
        });
    };

    // 4. Xử lý khi click "Chọn tất cả"
    const handleSelectAll = (e) => {
        if (e.target.checked) {
            // Chọn tất cả
            const allCodes = allPermissions.flatMap(group => 
                group.permissions.map(p => p.code)
            );
            setSelectedPermissions(new Set(allCodes));
        } else {
            // Bỏ chọn tất cả
            setSelectedPermissions(new Set());
        }
    };

    // 5. Xử lý Submit
    const handleSubmit = () => {
        if (!validateForm()) return;

        const title = isUpdate ? 'Bạn có chắc chắn muốn cập nhật?' : 'Bạn có chắc chắn muốn thêm mới?';
        
        showConfirmModal(title, async () => {
            setIsSubmitting(true);
            try {
                const userData = JSON.parse(userDataString);
                const token = userData?.resources?.accessToken;
                if (!token) {
                    throw new Error('Không tìm thấy token. Vui lòng đăng nhập lại.');
                }

                // Chuẩn bị payload
                const payload = {
                    name: values.name,
                    description: values.description,
                    permissions: Array.from(selectedPermissions), // Chuyển Set thành Array
                };

                if (isUpdate) {
                    // Cập nhật
                    await axios.put(`${apiUrl}/${item.id}`, payload, { 
                        headers: { Authorization: `Bearer ${token}` } 
                    });
                    showNotifyModal('Cập nhật thành công!');
                } else {
                    // Thêm mới
                    await axios.post(apiUrl, payload, { 
                        headers: { Authorization: `Bearer ${token}` } 
                    });
                    showNotifyModal('Thêm mới thành công!');
                }
                onClose(); // Đóng popup
                onSuccess(); // Tải lại danh sách
            } catch (err) {
                showNotifyModal('Thao tác thất bại: ' + (err.response?.data?.message || err.message), false);
            } finally {
                setIsSubmitting(false);
            }
        });
    };

    return (
        // Sử dụng class của cshtml để đồng bộ giao diện
        <div className="popup-overlay">
            <div className="popup-content p-0" style={{ width: '900px' }}>
                {/* Sử dụng cấu trúc của modal-content
                    Nếu bạn dùng Bootstrap modal, bạn có thể thay thế
                */}
                <div className="modal-content">
                    {/* Header */}
                    <div className="modal-header">
                        <h2 className="fw-bold">
                            {isUpdate ? 'Cập nhật vai trò' : 'Thêm mới vai trò'}
                        </h2>
                        <button className="btn btn-icon btn-sm" onClick={onClose} disabled={isSubmitting}>
                            {/* Icon 'X' (Bootstrap) */}
                            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-x-lg" viewBox="0 0 16 16">
                                <path d="M2.146 2.854a.5.5 0 1 1 .708-.708L8 7.293l5.146-5.147a.5.5 0 0 1 .708.708L8.707 8l5.147 5.146a.5.5 0 0 1-.708.708L8 8.707l-5.146 5.147a.5.5 0 0 1-.708-.708L7.293 8 2.146 2.854Z"/>
                            </svg>
                        </button>
                    </div>

                    {/* Body */}
                    <div className="modal-body p-4">
                        {/* Thêm 1 div có thể cuộn (giống data-kt-scroll)
                            Chiều cao tối đa 60vh (60% viewport height)
                        */}
                        <div className="scroll-y" style={{ maxHeight: '60vh', overflowY: 'auto', padding: '0 1.5rem' }}>
                            <form id="role_form">
                                {/* Tên vai trò */}
                                <div className="form-group mb-3">
                                    <label className="form-label fw-bold">Tên vai trò</label>
                                    <input
                                        type="text"
                                        className={`form-control ${errors.name ? 'is-invalid' : ''}`}
                                        name="name"
                                        value={values.name}
                                        onChange={handleChange}
                                    />
                                    {errors.name && <div className="invalid-feedback">{errors.name}</div>}
                                </div>

                                {/* Mô tả */}
                                <div className="form-group mb-3">
                                    <label className="form-label fw-bold">Mô tả</label>
                                    <textarea
                                        className={`form-control ${errors.description ? 'is-invalid' : ''}`}
                                        name="description"
                                        value={values.description}
                                        onChange={handleChange}
                                        rows={3}
                                    />
                                    {errors.description && <div className="invalid-feedback">{errors.description}</div>}
                                </div>
                                
                                {/* Phân Quyền */}
                                <div className="mt-4">
                                    <h4 className="fw-bold">Phân quyền vai trò</h4>
                                    
                                    {loadingPermissions ? (
                                        <div className="text-center p-5">Đang tải danh sách quyền...</div>
                                    ) : (
                                        <div className="table-responsive">
                                            {/* Chọn tất cả */}
                                            <div className="form-check form-check-custom form-check-solid mb-2">
                                                <input 
                                                    className="form-check-input" 
                                                    type="checkbox" 
                                                    id="select_all_permissions"
                                                    ref={selectAllCheckboxRef}
                                                    onChange={handleSelectAll}
                                                />
                                                <label className="form-check-label fw-bold" htmlFor="select_all_permissions">
                                                    Chọn tất cả
                                                </label>
                                            </div>
                                            <hr className="my-2"/>

                                            {/* Bảng Quyền */}
                                            <table className="table table-flush align-middle">
                                                <tbody className="fs-7">
                                                    {allPermissions.map((group) => (
                                                        <React.Fragment key={group.groupName}>
                                                            {/* Tên Nhóm Quyền */}
                                                            <tr>
                                                                <td className="text-dark fw-bold pt-4" colSpan={4}>
                                                                    {group.groupName}
                                                                </td>
                                                            </tr>
                                                            {/* Danh sách quyền */}
                                                            <tr>
                                                                {group.permissions.map((perm, index) => (
                                                                    <td key={perm.code} className="p-2">
                                                                        <div className="form-check form-check-custom form-check-solid">
                                                                            <input 
                                                                                className="form-check-input" 
                                                                                type="checkbox" 
                                                                                value={perm.code} 
                                                                                id={`perm_${perm.id}`}
                                                                                checked={selectedPermissions.has(perm.code)}
                                                                                onChange={() => handlePermissionChange(perm.code)}
                                                                            />
                                                                            <label className="form-check-label" htmlFor={`perm_${perm.id}`}>
                                                                                {perm.name}
                                                                            </label>
                                                                        </div>
                                                                    </td>
                                                                ))}
                                                                {/* Thêm các ô trống nếu hàng không đủ 4 */}
                                                                {Array(Math.max(0, 3 - group.permissions.length)).fill(0).map((_, i) => <td key={i}></td>)}
                                                            </tr>
                                                        </React.Fragment>
                                                    ))}
                                                </tbody>
                                            </table>
                                        </div>
                                    )}
                                </div>
                            </form>
                        </div>
                    </div>

                    {/* Footer */}
                    <div className="modal-footer flex-center p-4">
                        <button className="btn btn-secondary me-3" onClick={onClose} disabled={isSubmitting}>
                            Hủy bỏ
                        </button>
                        <button
                            className="btn btn-primary"
                            onClick={handleSubmit}
                            disabled={isSubmitDisabled || loadingPermissions || isSubmitting}
                        >
                            {isSubmitting ? (
                                <>
                                    <span>Đang lưu...</span>
                                    <span className="spinner-border spinner-border-sm align-middle ms-2"></span>
                                </>
                            ) : (
                                <span>Lưu</span>
                            )}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default RoleFormPopup;
