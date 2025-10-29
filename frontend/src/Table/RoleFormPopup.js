import React, { useState, useEffect, useRef, useCallback } from 'react';
import axios from 'axios';
import { useFormValidation } from '../validator/useFormValidation'; // Sửa lại đường dẫn nếu cần
import { required, maxLength } from '../validator/validators'; // Sửa lại đường dẫn nếu cần

// --- Helper: Lấy từ Menu.js ---
const flattenAndMapMenus = (menus) => {
    let flatList = [];
    const traverse = (menuItems) => {
        if (!menuItems || menuItems.length === 0) return;
        for (const item of menuItems) {
            const { child, sortOrder, ...rest } = item;
            const mappedItem = { ...rest, order: sortOrder };
            flatList.push(mappedItem);
            if (child && child.length > 0) traverse(child);
        }
    };
    traverse(menus);
    return flatList;
};

// --- Helper: Component đệ quy cho Bảng Quyền ---
function MenuPermissionRow({ menu, allMenus, allActions, selectedMap, onToggle, onToggleRow, depth, menuActionPermissions }) {
    // Logic 1: Menu cha cao nhất?
    // (Chúng ta dùng so sánh lỏng == vì parentId có thể là 0, null, hoặc undefined)
    const isRoot = !menu.parentId; 
    
    // Logic 2: Tìm các con của menu này
    const children = allMenus
        .filter(m => m.parentId == menu.id)
        .sort((a, b) => a.order - b.order);

    // Logic 3: Kiểm tra xem hàng này (Tất cả quyền của menu) đã được check hay chưa

    // Map Action Name sang Property Name (giữ nguyên)
    const actionNameToPropertyMap = {
        "Xem": "hasRead", "Thêm": "hasCreate", "Sửa": "hasUpdate", "Xoá": "hasDelete"
    };
    // Lấy thông tin quyền chi tiết cho menu này từ Map
    const specificPermissions = menuActionPermissions.get(menu.id) || {};

    // --- (SỬA) Cập nhật logic isRowChecked ---
    const isRowChecked = () => {
        const selectedActions = selectedMap.get(menu.id);
        if (!selectedActions || selectedActions.size === 0) return false;

        // Đếm số action được phép chọn VÀ THỰC SỰ ĐƯỢC PHÉP (enabled)
        let enabledAllowedActionCount = 0;
        allActions.forEach(action => {
            const propName = actionNameToPropertyMap[action.name];
            const isAllowedByMenuPerms = specificPermissions[propName] === true; // Check API /Menu/permissons
            const isViewAction = allActions.length > 0 && action.id === allActions[0].id;
            const isEffectivelyEnabled = !((isRoot && !isViewAction) || !isAllowedByMenuPerms); // Check xem ô có bị disable không

            if (isEffectivelyEnabled) {
                enabledAllowedActionCount++;
            }
        });

        // Chỉ được tính là check all nếu đã chọn TẤT CẢ các ô được phép chọn
        return selectedActions.size === enabledAllowedActionCount && enabledAllowedActionCount > 0;
    };


    return (
        <React.Fragment>
            <tr className="menu-permission-row">
                <td className="p-2">
                    <span style={{ paddingLeft: `${depth * 20}px` }}>{menu.name}</span>
                </td>
                {allActions.map((action) => {
                    // --- (SỬA) Logic Disable Mới ---
                    const propName = actionNameToPropertyMap[action.name];
                    // Check xem API /Menu/permissons có cho phép action này trên menu này không
                    const isAllowedByMenuPerms = specificPermissions[propName] === true;

                    // Disable nếu:
                    // 1. Là root VÀ action không phải là "Xem" (action đầu tiên)
                    // HOẶC 2. API /Menu/permissons trả về false cho action này
                    const isViewAction = allActions.length > 0 && action.id === allActions[0].id;
                    const isDisabled = (isRoot && !isViewAction) || !isAllowedByMenuPerms;
                    // --- Kết thúc sửa Logic Disable ---

                    const isChecked = selectedMap.get(menu.id)?.has(action.id) || false;

                    return (
                        <td key={action.id} className="text-center p-2">
                            <div className="form-check form-check-custom form-check-solid d-inline-block">
                                <input
                                    className="form-check-input"
                                    type="checkbox"
                                    // Nếu đã check thì hiển thị check, KỂ CẢ KHI BỊ DISABLE
                                    // Điều này quan trọng để user biết quyền này đang được cấp (dù có thể không sửa được ở đây)
                                    checked={isChecked}
                                    // Disable theo logic mới
                                    disabled={isDisabled}
                                    onChange={() => onToggle(menu.id, action.id, isDisabled)}
                                />
                            </div>
                        </td>
                    );
                })}
                <td className="text-center p-2">
                     <div className="form-check form-check-custom form-check-solid d-inline-block">
                        <input
                            className="form-check-input"
                            type="checkbox"
                            checked={isRowChecked()}
                            // (SỬA) Disable nút check all của hàng nếu menu này không có quyền nào được phép (enabled)
                            disabled={!allActions.some(action => {
                                const propName = actionNameToPropertyMap[action.name];
                                const isAllowedByMenuPerms = specificPermissions[propName] === true;
                                const isViewAction = allActions.length > 0 && action.id === allActions[0].id;
                                const isEffectivelyEnabled = !((isRoot && !isViewAction) || !isAllowedByMenuPerms);
                                return isEffectivelyEnabled;
                            })}
                            onChange={(e) => onToggleRow(menu.id, e.target.checked, isRoot)}
                        />
                    </div>
                </td>
            </tr>
            {children.map(child => (
                <MenuPermissionRow
                    key={child.id}
                    menu={child}
                    allMenus={allMenus}
                    allActions={allActions}
                    menuActionPermissions={menuActionPermissions} // Truyền xuống
                    selectedMap={selectedMap}
                    onToggle={onToggle}
                    onToggleRow={onToggleRow}
                    depth={depth + 1}
                />
            ))}
        </React.Fragment>
    );
}


// --- Component Chính: RoleFormPopup ---
function RoleFormPopup({ 
    item, 
    onClose, 
    apiUrl,           // API cho Role (Vd: /api/v1/roles)
    menuApiUrl,       // (MỚI) API cho Menu (Vd: /api/v1/menu)
    actionApiUrl,     // (MỚI) API cho Actions (Vd: /api/v1/actions)
    menuPermissionsApiUrl,
    onSuccess, 
    showConfirmModal, 
    showNotifyModal 
}) {
    const isUpdate = !!item;
    const userDataString = localStorage.getItem('userData'); 
    
    const { values, errors, handleChange, validateForm, isSubmitDisabled } = useFormValidation(
        {
            name: item?.name || '',
            description: item?.description || '',
        },
        {
            name: [required, maxLength(255)],
            description: [maxLength(500)],
        }
    );

    // State cho Dữ liệu
    const [allMenus, setAllMenus] = useState([]);
    const [allActions, setAllActions] = useState([]);
    
    // State Cốt lõi: Dùng Map<menuId, Set<actionId>>
    const [selectedPermissions, setSelectedPermissions] = useState(new Map());
    
    // State Tải dữ liệu
    const [menuActionPermissions, setMenuActionPermissions] = useState(new Map());
    const [loadingData, setLoadingData] = useState(true);
    const [isSubmitting, setIsSubmitting] = useState(false);

    // Ref cho "Chọn tất cả"
    const selectAllCheckboxRef = useRef(null);

    // 1. Fetch đồng thời Menus, Actions (và Role chi tiết nếu là Update)
    useEffect(() => {
        const fetchData = async () => {
            setLoadingData(true);
            try {
                const userData = JSON.parse(userDataString);
                const token = userData?.resources?.accessToken;
                if (!token) throw new Error("Không tìm thấy token.");

                const headers = { Authorization: `Bearer ${token}` };

                

                // (SỬA) Gọi 4 API
                const [menuRes, actionRes, menuPermRes, roleRes] = await Promise.all([
                    axios.get(menuApiUrl, { headers }),
                    axios.get(actionApiUrl, { headers }),
                    // (MỚI) Gọi API /Menu/permissons (Giả định là GET)
                    axios.get(menuPermissionsApiUrl, { headers }),
                    isUpdate ? axios.get(`${apiUrl}/${item.id}/permissons`, { headers }) : Promise.resolve(null)
                ]);

                // Xử lý Menus
                const nestedMenus = menuRes.data.resources || [];
                const flatMenus = flattenAndMapMenus(nestedMenus);
                setAllMenus(flatMenus);

                // Xử lý Actions
                const actions = actionRes.data.resources || [];
                setAllActions(actions);

                // (MỚI) Xử lý Menu Permissions (lấy has... để disable)
                const menuPermsData = menuPermRes.data.resources || [];
                const menuPermMap = new Map();
                menuPermsData.forEach(perm => menuPermMap.set(perm.id, perm)); // Dùng Map<menuId, object>
                setMenuActionPermissions(menuPermMap);
                
                // Xử lý Role (Permissions đã chọn)
                if (isUpdate && roleRes) {
                    const permissions = roleRes.data.resources?.permissions || [];
                    // Chuyển đổi mảng permissions -> Map
                    const initialMap = new Map();
                    for (const perm of permissions) {
                        if (!initialMap.has(perm.menuId)) {
                            initialMap.set(perm.menuId, new Set());
                        }
                        initialMap.get(perm.menuId).add(perm.actionId);
                    }
                    setSelectedPermissions(initialMap);
                }

            } catch (err) {
                showNotifyModal('Lỗi tải dữ liệu phân quyền: ' + (err.response?.data?.message || err.message), false);
            } finally {
                setLoadingData(false);
            }
        };

        fetchData();
    }, [apiUrl, menuApiUrl, actionApiUrl, item, isUpdate, userDataString, showNotifyModal]);


    // (SỬA) Cập nhật trạng thái checkbox "Chọn tất cả" (tính cả disable)
    useEffect(() => {
        if (loadingData || !selectAllCheckboxRef.current || allMenus.length === 0 || allActions.length === 0) return;

        let totalEnabledPossible = 0; // Tổng số ô KHÔNG bị disable
        let totalSelectedAndEnabled = 0; // Tổng số ô đã chọn VÀ KHÔNG bị disable

        const actionMap = { "Xem": "hasRead", "Thêm": "hasCreate", "Sửa": "hasUpdate", "Xoá": "hasDelete", "Export": "hasExport", "Approve": "hasApprove" };

        allMenus.forEach(menu => {
            const isRoot = !menu.parentId;
            const selectedSet = selectedPermissions.get(menu.id);
            const specificPermissions = menuActionPermissions.get(menu.id) || {};

            allActions.forEach(action => {
                const propName = actionMap[action.name];
                const isAllowedByMenuPerms = specificPermissions[propName] === true;
                const isViewAction = allActions.length > 0 && action.id === allActions[0].id;
                const isDisabled = (isRoot && !isViewAction) || !isAllowedByMenuPerms;

                if (!isDisabled) { // Chỉ tính những ô không bị disable
                    totalEnabledPossible++;
                    if (selectedSet?.has(action.id)) {
                        totalSelectedAndEnabled++;
                    }
                }
            });
        });

        if (totalEnabledPossible === 0) {
             selectAllCheckboxRef.current.checked = false;
             selectAllCheckboxRef.current.indeterminate = false;
             selectAllCheckboxRef.current.disabled = true;
        } else if (totalSelectedAndEnabled === 0) {
            selectAllCheckboxRef.current.disabled = false;
            selectAllCheckboxRef.current.checked = false;
            selectAllCheckboxRef.current.indeterminate = false;
        } else if (totalSelectedAndEnabled === totalEnabledPossible) {
            selectAllCheckboxRef.current.disabled = false;
            selectAllCheckboxRef.current.checked = true;
            selectAllCheckboxRef.current.indeterminate = false;
        } else {
            selectAllCheckboxRef.current.disabled = false;
            selectAllCheckboxRef.current.checked = false;
            selectAllCheckboxRef.current.indeterminate = true;
        }
    }, [selectedPermissions, allMenus, allActions, menuActionPermissions, loadingData]);


    // Handlers cho Bảng Quyền (cập nhật để chỉ chọn/bỏ chọn ô enabled)
     const actionNameToPropertyMap = { "Xem": "hasRead", "Thêm": "hasCreate", "Sửa": "hasUpdate", "Xoá": "hasDelete"};

    const handleToggle = (menuId, actionId, isDisabled) => { /* Giữ nguyên */
        if (isDisabled) return;
        const newMap = new Map(selectedPermissions);
        const actionSet = newMap.get(menuId) || new Set();
        if (actionSet.has(actionId)) actionSet.delete(actionId); else actionSet.add(actionId);
        if (actionSet.size === 0) newMap.delete(menuId); else newMap.set(menuId, actionSet);
        setSelectedPermissions(newMap);
    };

    const handleToggleRow = (menuId, isChecked, isRoot) => { /* ... */
        const newMap = new Map(selectedPermissions);
        const menu = allMenus.find(m => m.id === menuId);
        if (!menu) return;
        const specificPermissions = menuActionPermissions.get(menuId) || {};

        if (isChecked) {
            const newActionSet = new Set();
             allActions.forEach(action => {
                 const propName = actionNameToPropertyMap[action.name];
                 const isAllowedByMenuPerms = specificPermissions[propName] === true;
                 const isViewAction = allActions.length > 0 && action.id === allActions[0].id;
                 const isDisabled = (isRoot && !isViewAction) || !isAllowedByMenuPerms;
                 // Chỉ thêm action nếu nó KHÔNG bị disable
                 if (!isDisabled) {
                     newActionSet.add(action.id);
                 }
             });
             if (newActionSet.size > 0) newMap.set(menuId, newActionSet);
             else newMap.delete(menuId);

        } else newMap.delete(menuId);
        setSelectedPermissions(newMap);
     };
    const handleSelectAll = (e) => { /* ... */
        if (e.target.checked) {
            const newMap = new Map();
            allMenus.forEach(menu => {
                const isRoot = !menu.parentId;
                const specificPermissions = menuActionPermissions.get(menu.id) || {};
                const newActionSet = new Set();
                 allActions.forEach(action => {
                     const propName = actionNameToPropertyMap[action.name];
                     const isAllowedByMenuPerms = specificPermissions[propName] === true;
                     const isViewAction = allActions.length > 0 && action.id === allActions[0].id;
                     const isDisabled = (isRoot && !isViewAction) || !isAllowedByMenuPerms;
                     if (!isDisabled) { // Chỉ thêm action không bị disable
                         newActionSet.add(action.id);
                     }
                 });
                 if (newActionSet.size > 0) newMap.set(menu.id, newActionSet);
            });
            setSelectedPermissions(newMap);
        } else setSelectedPermissions(new Map());
    };

    // 6. Xử lý Submit
    const handleSubmit = () => {
        if (!validateForm()) return;

        const title = isUpdate ? 'Bạn có chắc chắn muốn cập nhật?' : 'Bạn có chắc chắn muốn thêm mới?';
        
        showConfirmModal(title, async () => {
            setIsSubmitting(true);
            try {
                const userData = JSON.parse(userDataString);
                const token = userData?.resources?.accessToken;
                if (!token) throw new Error('Không tìm thấy token.');

                // Chuyển đổi Map<menuId, Set<actionId>> -> mảng payload
                const permissionsPayload = [];
                for (const [menuId, actionSet] of selectedPermissions.entries()) {
                    for (const actionId of actionSet) {
                        permissionsPayload.push({ menuId, actionId });
                    }
                }

                const payload = {
                    id: item?.id || null, // Gửi ID nếu là update
                    name: values.name,
                    description: values.description,
                    permissions: permissionsPayload, // Gửi mảng permissions mới
                };

                if (isUpdate) {
                    await axios.put(`${apiUrl}`, payload, { headers: { Authorization: `Bearer ${token}` } });
                    showNotifyModal('Cập nhật thành công!');
                } else {
                    await axios.post(apiUrl, payload, { headers: { Authorization: `Bearer ${token}` } });
                    showNotifyModal('Thêm mới thành công!');
                }
                onClose();
                onSuccess();
            } catch (err) {
                showNotifyModal('Thao tác thất bại: ' + (err.response?.data?.message || err.message), false);
            } finally {
                setIsSubmitting(false);
            }
        });
    };

    // Lọc ra các menu gốc (cấp 1) để bắt đầu đệ quy
    const rootMenus = allMenus
        .filter(m => !m.parentId)
        .sort((a, b) => a.order - b.order);

    
    // --- (SỬA) Tính isSubmitDisabled dựa trên errors ---
    const isSubmitDisabledCalculated = Object.values(errors).some(error => error !== null);
    // --- Kết thúc sửa ---

    return (
        <div className="popup-overlay">
            <div className="popup-content p-0" style={{ maxWidth: '1000px', width: '90vw' }}>
                <div className="modal-content">
                    {/* Header */}
                    <div className="modal-header">
                        <h2 className="fw-bold">
                            {isUpdate ? 'Cập nhật vai trò' : 'Thêm mới vai trò'}
                        </h2>
                        <button className="btn btn-icon btn-sm" onClick={onClose} disabled={isSubmitting}>
                            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-x-lg" viewBox="0 0 16 16">
                                <path d="M2.146 2.854a.5.5 0 1 1 .708-.708L8 7.293l5.146-5.147a.5.5 0 0 1 .708.708L8.707 8l5.147 5.146a.5.5 0 0 1-.708.708L8 8.707l-5.146 5.147a.5.5 0 0 1-.708-.708L7.293 8 2.146 2.854Z"/>
                            </svg>
                        </button>
                    </div>

                    {/* Body */}
                    <div className="modal-body p-4">
                        <div className="scroll-y" style={{ maxHeight: '60vh', overflowY: 'auto', padding: '0 1.5rem' }}>
                            <form id="role_form">
                                {/* Thông tin vai trò */}
                                <div className="row">
                                    <div className="col-md-6">
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
                                    </div>
                                    <div className="col-md-6">
                                        <div className="form-group mb-3">
                                            <label className="form-label fw-bold">Mô tả</label>
                                            <input
                                                type="text"
                                                className={`form-control ${errors.description ? 'is-invalid' : ''}`}
                                                name="description"
                                                value={values.description}
                                                onChange={handleChange}
                                            />
                                            {errors.description && <div className="invalid-feedback">{errors.description}</div>}
                                        </div>
                                    </div>
                                </div>
                                
                                {/* Phân Quyền */}
                                <div className="mt-4">
                                    <h4 className="fw-bold">Phân quyền vai trò</h4>
                                    {loadingData ? ( <div className="text-center p-5">Đang tải danh sách quyền...</div> ) : (
                                        <div className="table-responsive">
                                            <table className="table table-bordered table-flush align-middle fs-7">
                                                <thead className="thead-light">
                                                    <tr className="text-uppercase">
                                                        <th className="p-2">Tên Menu</th>
                                                        {allActions.map(action => ( <th key={action.id} className="text-center p-2">{action.name}</th> ))}
                                                        <th className="text-center p-2">
                                                            <div className="form-check form-check-custom form-check-solid d-inline-block">
                                                                <input className="form-check-input" type="checkbox" id="select_all_permissions" ref={selectAllCheckboxRef} onChange={handleSelectAll} />
                                                                <label className="form-check-label" htmlFor="select_all_permissions">Tất cả</label>
                                                            </div>
                                                        </th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    {rootMenus.length > 0 ? (
                                                        rootMenus.map(menu => (
                                                            <MenuPermissionRow
                                                                key={menu.id}
                                                                menu={menu}
                                                                allMenus={allMenus}
                                                                allActions={allActions}
                                                                menuActionPermissions={menuActionPermissions} // Truyền xuống
                                                                selectedMap={selectedPermissions}
                                                                onToggle={handleToggle}
                                                                onToggleRow={handleToggleRow}
                                                                depth={0}
                                                            />
                                                        ))
                                                    ) : (
                                                        <tr><td colSpan={allActions.length + 2} className="text-center p-5">Không tìm thấy dữ liệu menu.</td></tr>
                                                    )}
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
                            disabled={isSubmitDisabled || loadingData || isSubmitting}
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

