import React, { useState, useEffect, useCallback, useRef } from 'react';
import axios from 'axios';
import MenuTree from '../Table/MenuTree';
import MenuFormPopup from '../Table/MenuFormPopup';
import { API_URL } from '~/api/api';
import { useNavigate } from 'react-router-dom';
import { canView, canCreate, canUpdate, canDelete } from '~/utils/permissionUtils';

const MENU_MANAGEMENT_ID = 8;

const flattenAndMapMenus = (menus) => {
    let flatList = [];

    const traverse = (menuItems) => {
        if (!menuItems || menuItems.length === 0) {
            return;
        }

        for (const item of menuItems) {
            // Lấy mảng 'child' và 'sortOrder' ra
            const { child, sortOrder, ...rest } = item;

            flatList.push(item); // Thêm item (cha) đã map vào danh sách

            // Đệ quy vào mảng 'child'
            if (child && child.length > 0) {
                traverse(child);
            }
        }
    };

    traverse(menus); // Bắt đầu đệ quy
    return flatList;
};

/// --- Component Trang Chính (Page) ---
export default function Menu({
    apiUrl, // Vd: /api/v1/menu
    apiMenuTypeUrl, // Vd: /api/v1/menu-type
    // !! XÓA PROPS showConfirmModal VÀ showNotifyModal
}) {
    const navigate = useNavigate();

    const [allMenus, setAllMenus] = useState([]);
    const [menuTypes, setMenuTypes] = useState([]);
    const [selectedMenuType, setSelectedMenuType] = useState('');

    const [loadingMenus, setLoadingMenus] = useState(false);
    const [loadingTypes, setLoadingTypes] = useState(true);
    const [token, setToken] = useState('');

    // State cho Modal Add/Edit
    const [showModal, setShowModal] = useState(false);
    const [currentItem, setCurrentItem] = useState(null);
    const [refreshFlag, setRefreshFlag] = useState(false);

    const [isAccessChecked, setIsAccessChecked] = useState(false);
    const [isAllowedToView, setIsAllowedToView] = useState(false);

    const [isPopupReadOnly, setIsPopupReadOnly] = useState(false);

    // --- (SỬA) THÊM STATE CHO MODAL NỘI BỘ ---
    // (Giống RoleList.js)
    const [confirmState, setConfirmState] = useState({
        show: false,
        message: '',
        onConfirm: null, // Callback để thực thi khi nhấn "Xác nhận"
    });
    const [notifyState, setNotifyState] = useState({
        show: false,
        message: '',
        isSuccess: false,
    });

    apiUrl = `${API_URL}/Menu`;
    apiMenuTypeUrl = `${API_URL}/Menu/types`;
    const actionApiUrl = `${API_URL}/Action`; // (MỚI) URL API cho Action
    // --- KẾT THÚC THÊM STATE ---

    // 1. Lấy token khi component mount
    useEffect(() => {
        const userDataString = localStorage.getItem('userData');
        const userData = JSON.parse(userDataString);
        if (!userData || !userData.resources || !userData.resources.accessToken) {
            // Nếu chưa có hàm notify, dùng tạm alert (hoặc đợi hàm notify nội bộ)
            console.error('Không tìm thấy token. Vui lòng đăng nhập lại.');
        } else {
            setToken(userData.resources.accessToken);
        }

        if (!canView(MENU_MANAGEMENT_ID)) {
            console.warn(`Người dùng không có quyền xem trang Menu (ID: ${MENU_MANAGEMENT_ID}). Đang chuyển hướng...`);
            setIsAllowedToView(false);
            navigate('/error'); // Chuyển hướng đến trang lỗi
        } else {
            setIsAllowedToView(true);
        }
        setIsAccessChecked(true);
    }, [navigate]);

    // --- (SỬA) TẠO CÁC HÀM MODAL NỘI BỘ ---
    const internalShowNotifyModal = (message, isSuccess = false) => {
        setNotifyState({ show: true, message, isSuccess });
    };

    const internalShowConfirmModal = (message, onConfirmCallback) => {
        setConfirmState({
            show: true,
            message: message,
            onConfirm: onConfirmCallback, // Lưu callback lại
        });
    };

    // Hàm xử lý khi nhấn "Xác nhận" trên modal confirm
    const handleDoConfirm = () => {
        if (confirmState.onConfirm) {
            confirmState.onConfirm(); // Chạy callback đã lưu
        }
        setConfirmState({ show: false, message: '', onConfirm: null }); // Đóng modal
    };

    // Hàm đóng modal
    const handleCloseConfirm = () => setConfirmState({ show: false, message: '', onConfirm: null });
    const handleCloseNotify = () => setNotifyState({ show: false, message: '', isSuccess: false });
    // --- KẾT THÚC HÀM MODAL NỘI BỘ ---

    // 2. Fetch danh sách Menu Types
    useEffect(() => {
        if (!token || !apiMenuTypeUrl) return;

        const fetchMenuTypes = async () => {
            setLoadingTypes(true);
            try {
                const res = await axios.get(apiMenuTypeUrl, {
                    headers: { Authorization: `Bearer ${token}` },
                });
                const types = res.data.resources || [];
                setMenuTypes(types);
                if (types.length > 0) {
                    setSelectedMenuType(types[0].id);
                }
            } catch (err) {
                internalShowNotifyModal('Lỗi tải loại menu: ' + (err.response?.data?.message || err.message), false);
            } finally {
                setLoadingTypes(false);
            }
        };
        fetchMenuTypes();
        // Thêm dependency
    }, [token, apiMenuTypeUrl]);

    // 3. Fetch danh sách Menus (Sử dụng POST)
    const fetchMenus = useCallback(async () => {
        if (!token || !selectedMenuType || !apiUrl) return;

        setLoadingMenus(true);
        try {
            const requestBody = {
                keyword: selectedMenuType,
                pageIndex: 1,
                pageSize: 200, // Lấy tất cả
                sortType: 'ASC',
                orderBy: 'order',
            };

            const res = await axios.post(`${apiUrl}/paged`, requestBody, {
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`,
                },
            });

            const nestedMenus = res.data.resources || [];
            const flatMenus = flattenAndMapMenus(nestedMenus);
            setAllMenus(flatMenus);
        } catch (err) {
            internalShowNotifyModal('Lỗi tải danh sách menu: ' + (err.response?.data?.message || err.message), false);
            setAllMenus([]);
        } finally {
            setLoadingMenus(false);
        }
    }, [token, selectedMenuType, apiUrl]); // Bỏ showNotifyModal (giờ là hàm nội bộ)

    useEffect(() => {
        fetchMenus();
    }, [fetchMenus, refreshFlag]);

    // --- Handlers cho Modal ---
    const handleAdd = () => {
        setCurrentItem(null);
        setShowModal(true);
        setIsPopupReadOnly(false);
    };

    const handleEdit = (item, readOnly = false) => {
        setCurrentItem(item);
        setShowModal(true);
        setIsPopupReadOnly(readOnly);
    };

    const handleCloseModal = () => {
        setShowModal(false);
        setCurrentItem(null);
        setIsPopupReadOnly(false);
    };

    const handleSuccess = () => {
        handleCloseModal();
        setRefreshFlag((prev) => !prev);
    };

    // --- (SỬA) Handlers cho Xóa ---
    const handleDeleteClick = (id) => {
        // Dùng hàm nội bộ
        internalShowConfirmModal('Bạn có chắc chắn muốn xóa menu này?', () => confirmDelete(id));
    };

    // Callback xác nhận xóa (nhận `id` làm tham số)
    const confirmDelete = async (id) => {
        try {
            await axios.delete(`${apiUrl}/${id}`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            internalShowNotifyModal('Xóa thành công!', true); // Dùng hàm nội bộ
            setRefreshFlag((prev) => !prev);
        } catch (err) {
            internalShowNotifyModal('Xóa thất bại: ' + (err.response?.data?.message || err.message), false); // Dùng hàm nội bộ
        }
    };

    const showAddButton = canCreate(MENU_MANAGEMENT_ID);
    const allowUpdate = canUpdate(MENU_MANAGEMENT_ID);
    const allowDelete = canDelete(MENU_MANAGEMENT_ID);

    if (!isAccessChecked) {
        return <div className="text-center p-5">Đang kiểm tra quyền truy cập...</div>; // Hoặc một spinner
    }

    // Nếu không được phép xem, component sẽ không render gì cả (vì đã bị redirect)
    // Nhưng để chắc chắn, thêm điều kiện render null
    if (!isAllowedToView) {
        return null;
    }

    // --- Render ---
    return (
        <div className="container-fluid pt-4 px-4">
            <div className="card">
                <div className="card-header border-0 pt-6">
                    <div className="card-title">
                        {loadingTypes ? (
                            <span className="text-muted">Đang tải loại menu...</span>
                        ) : (
                            <select
                                className="form-select form-control min-w-200px"
                                value={selectedMenuType}
                                onChange={(e) => setSelectedMenuType(e.target.value)}
                                disabled={menuTypes.length === 0}
                            >
                                {menuTypes.length === 0 ? (
                                    <option value="">Không có loại menu</option>
                                ) : (
                                    menuTypes.map((type) => (
                                        <option key={type.id} value={type.id}>
                                            {type.name}
                                        </option>
                                    ))
                                )}
                            </select>
                        )}
                    </div>
                    {showAddButton && (
                        <div className="card-toolbar">
                            <button
                                type="button"
                                className="btn btn-primary"
                                onClick={handleAdd}
                                disabled={loadingTypes || !selectedMenuType}
                            >
                                <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    width="16"
                                    height="16"
                                    fill="currentColor"
                                    className="bi bi-plus-lg me-1"
                                    viewBox="0 0 16 16"
                                >
                                    <path
                                        fillRule="evenodd"
                                        d="M8 2a.5.5 0 0 1 .5.5v5h5a.5.5 0 0 1 0 1h-5v5a.5.5 0 0 1-1 0v-5h-5a.5.5 0 0 1 0-1h5v-5A.5.5 0 0 1 8 2z"
                                    />
                                </svg>
                                Thêm mới
                            </button>
                        </div>
                    )}
                </div>

                <div className="card-body py-4">
                    {loadingMenus ? (
                        <div className="text-center p-5">Đang tải danh sách menu...</div>
                    ) : allMenus.length === 0 ? (
                        <div className="text-center p-5 text-muted">Không có menu nào cho loại này.</div>
                    ) : (
                        <div className="menu-tree-container">
                            <MenuTree
                                menus={allMenus}
                                parentId={null}
                                menuId={MENU_MANAGEMENT_ID}
                                onEdit={handleEdit}
                                onDelete={handleDeleteClick}
                                allowUpdate={allowUpdate}
                                allowDelete={allowDelete}
                            />
                        </div>
                    )}
                </div>

                {/* (SỬA) Render Modal Add/Edit VÀ TRUYỀN HÀM NỘI BỘ */}
                {showModal && (
                    <MenuFormPopup
                        item={currentItem}
                        allMenus={allMenus}
                        menuType={selectedMenuType}
                        apiUrl={apiUrl}
                        actionApiUrl={actionApiUrl}
                        token={token}
                        menuId={MENU_MANAGEMENT_ID}
                        onClose={handleCloseModal}
                        onSuccess={handleSuccess}
                        showConfirmModal={internalShowConfirmModal} // Truyền hàm nội bộ
                        showNotifyModal={internalShowNotifyModal} // Truyền hàm nội bộ
                        isReadOnly={isPopupReadOnly}
                    />
                )}

                {/* (SỬA) THÊM JSX CỦA 2 MODAL NỘI BỘ */}
                {/* (Giống RoleList.js) */}
                {confirmState.show && (
                    <div className="modal fade show d-block" style={{ background: 'rgba(0,0,0,0.4)' }}>
                        <div className="modal-dialog modal-dialog-centered">
                            <div className="modal-content">
                                <div className="modal-header">
                                    <h5 className="modal-title">Xác nhận</h5>
                                </div>
                                <div className="modal-body">
                                    <p>{confirmState.message}</p>
                                </div>
                                <div className="modal-footer">
                                    <button className="btn btn-secondary" onClick={handleCloseConfirm}>
                                        Hủy
                                    </button>
                                    <button className="btn btn-danger" onClick={handleDoConfirm}>
                                        Xác nhận
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                )}

                {notifyState.show && (
                    <div className="modal fade show d-block" style={{ background: 'rgba(0,0,0,0.4)' }}>
                        <div className="modal-dialog modal-dialog-centered">
                            <div className="modal-content">
                                <div className="modal-header">
                                    <h5
                                        className={`modal-title ${
                                            notifyState.isSuccess ? 'text-success' : 'text-danger'
                                        }`}
                                    >
                                        {notifyState.isSuccess ? 'Thành công' : 'Thất bại'}
                                    </h5>
                                </div>
                                <div className="modal-body">
                                    <p>{notifyState.message}</p>
                                </div>
                                <div className="modal-footer">
                                    <button className="btn btn-primary" onClick={handleCloseNotify}>
                                        Đóng
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
}
