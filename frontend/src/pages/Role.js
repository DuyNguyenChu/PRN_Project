import { useState, useRef, useEffect } from 'react';
import RoleTable from '../Table/RoleTable';
import '../styles/css/Role.css';
import RoleFormPopup from '../Table/RoleFormPopup';
import ConfirmModal from '../Table/ConfirmModal';
import DatePicker from 'react-datepicker'; // Thêm thư viện react-datepicker
import 'react-datepicker/dist/react-datepicker.css'; // Thêm CSS cho datepicker
import { API_URL } from '~/api/api';
import moment from 'moment';
import { canView, canCreate, canUpdate, canDelete } from '~/utils/permissionUtils';
import { useNavigate } from 'react-router-dom';
import { PERMISSION_IDS } from '~/utils/menuIdForPermission';

export default function Role() {
    const [showFilter, setShowFilter] = useState(false);
    const toggleFilter = () => setShowFilter(!showFilter);

    const apiUrl = `${API_URL}/Role`; // URL API của bạn
    const menuApiUrl = `${API_URL}/Menu`; // (MỚI) URL API cho Menu
    const actionApiUrl = `${API_URL}/Action`; // (MỚI) URL API cho Action
    const menuPermissionsApiUrl = `${API_URL}/Menu/permissons`; // (MỚI) URL API cho Menu Permissions
    const token = localStorage.getItem('accessToken'); // Lấy token từ login

    const [showForm, setShowForm] = useState(false);
    const [showPopup, setShowPopup] = useState(false);
    const [editingItem, setEditingItem] = useState(null);
    const [refreshFlag, setRefreshFlag] = useState(false);

    const [showConfirm, setShowConfirm] = useState(false);
    const [confirmMessage, setConfirmMessage] = useState('');
    const [confirmRole, setConfirmRole] = useState(() => {});

    const [notifyMessage, setNotifyMessage] = useState('');
    const [showNotify, setShowNotify] = useState(false);
    const [notifySuccess, setNotifySuccess] = useState(true);

    // State cho khoảng ngày được chọn
    const [dateRange, setDateRange] = useState([null, null]);
    const [startDate, endDate] = dateRange;

    // State cho các giá trị lọc đã được "Áp dụng" và sẽ được truyền xuống bảng
    const [appliedFilters, setAppliedFilters] = useState({});

    // State cho các giá trị đang được nhập trong form lọc
    const [filterInputs, setFilterInputs] = useState({ name: '', description: '' });

    const tableRef = useRef();

    const navigate = useNavigate(); // THÊM VÀO
    const [isAccessChecked, setIsAccessChecked] = useState(false); // THÊM VÀO
    const [isAllowedToView, setIsAllowedToView] = useState(false); // THÊM VÀO
    
        // THÊM VÀO: useEffect kiểm tra quyền xem trang
    useEffect(() => {
        if (!canView(PERMISSION_IDS.ROLE)) {
            console.warn(`Người dùng không có quyền xem trang Action (ID: ${PERMISSION_IDS.ROLE}). Đang chuyển hướng...`);
            setIsAllowedToView(false);
            navigate('/error'); // Chuyển hướng đến trang lỗi
        } else {
            setIsAllowedToView(true);
        }
        setIsAccessChecked(true);
    }, [navigate]);

    const handleAdd = () => {
        setEditingItem(null);
        setShowPopup(true);
    };

    const handleEdit = (item) => {
        setEditingItem(item);
        setShowPopup(true);
    };

    // gọi confirm modal từ popup
    const showConfirmModal = (message, Role, onCancel) => {
        setConfirmMessage(message);
        setConfirmRole(() => Role);
        setShowConfirm(true);

        // lưu lại callback khi user bấm Hủy confirm
        setOnCancelConfirm(() => onCancel);
    };

    // Hàm xóa khoảng ngày đã chọn
    const clearDateRange = () => {
        setDateRange([null, null]);
    };

    const [onCancelConfirm, setOnCancelConfirm] = useState(null);

    // xử lý khi confirm modal Hủy
    const handleCancelConfirm = () => {
        setShowConfirm(false);
        if (onCancelConfirm) onCancelConfirm();
    };

    // gọi thông báo modal
    const showNotifyModal = (message, success = true) => {
        setNotifyMessage(message);
        setNotifySuccess(success);
        setShowNotify(true);
    };

    const handleClose = () => setShowPopup(false);

    const handleApplyFilter = () => {
        const [startDate, endDate] = dateRange;
        const newFilters = {
            name: filterInputs.name.trim(),
            description: filterInputs.description.trim(),
            createdDate: null, // Khởi tạo là null
        };

        // Tạo chuỗi ngày tháng theo định dạng "DD/MM/YYYY - DD/MM/YYYY"
        if (startDate && endDate) {
            newFilters.createdDate = `${moment(startDate).format('DD/MM/YYYY')} - ${moment(endDate).format(
                'DD/MM/YYYY',
            )}`;
        } else if (startDate) {
            // Xử lý trường hợp chỉ chọn 1 ngày
            newFilters.createdDate = moment(startDate).format('DD/MM/YYYY');
        }

        setAppliedFilters(newFilters);
        setShowFilter(false);
    };

    const handleResetFilter = () => {
        // Reset các giá trị trong ô input về rỗng
        setFilterInputs({ name: '', description: '' });
        setDateRange([null, null]);

        // Reset bộ lọc đã áp dụng để bảng fetch lại dữ liệu gốc
        setAppliedFilters({});
    };

    // Hàm gọi lại fetchData ở bảng
    const reloadTable = () => setRefreshFlag((prev) => !prev);

    return (
        <div className="container-fluid px-4">
            <div className="col-sm-12 col-xl-12 py-4">
                <div className="bg-light rounded h-100 p-4">
                    <div className="d-flex justify-content-between align-items-center mb-4">
                        <h6>Vai trò người dùng</h6>
                    </div>
                    <RoleTable
                        apiUrl={apiUrl}
                        token={token}
                        onEdit={handleEdit}
                        refreshFlag={refreshFlag}
                        filters={appliedFilters}
                    />
                </div>
            </div>
            {/* Popup nhập liệu */}
            {showPopup && (
                <RoleFormPopup
                    item={editingItem}
                    onClose={() => setShowPopup(false)} // dùng showPopup
                    apiUrl={apiUrl}
                    menuApiUrl={menuApiUrl} // (MỚI) API cho Menu (Vd: /api/v1/menu)
                    actionApiUrl={actionApiUrl}
                    menuPermissionsApiUrl={menuPermissionsApiUrl}
                    token={token}
                    onSuccess={() => {
                        reloadTable(); // reload bảng
                        setShowPopup(false);
                    }}
                    showConfirmModal={(message, Role) => showConfirmModal(message, Role, () => setShowPopup(true))}
                    showNotifyModal={showNotifyModal}
                />
            )}
            {showConfirm && (
                <ConfirmModal
                    message={confirmMessage}
                    onClose={handleCancelConfirm} // khi bấm Hủy
                    onConfirm={() => {
                        confirmRole();
                        setShowConfirm(false);
                    }}
                />
            )}

            {showNotify && (
                <ConfirmModal
                    message={notifyMessage}
                    onClose={() => setShowNotify(false)}
                    onlyClose={true} // chỉ hiện nút OK
                    success={notifySuccess}
                />
            )}
        </div>
    );
}
