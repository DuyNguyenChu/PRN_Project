import { useState, useRef, useEffect } from 'react';
import DriverStatusTable from '../Table/DriverStatusTable'; // THAY ĐỔI
import '../styles/css/DriverStatus.css'; // THAY ĐỔI
import DriverStatusFormPopup from '../Table/DriverStatusFormPopup'; // THAY ĐỔI
import ConfirmModal from '../Table/ConfirmModal';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import { API_URL } from '~/api/api';
import moment from 'moment';
import { canView } from '~/utils/permissionUtils'; // Bỏ canCreate, canUpdate, canDelete nếu không dùng
import { useNavigate } from 'react-router-dom';
import { PERMISSION_IDS } from '~/utils/menuIdForPermission';

export default function DriverStatus() {
    // THAY ĐỔI
    const [showFilter, setShowFilter] = useState(false);
    const toggleFilter = () => setShowFilter(!showFilter);

    const apiUrl = `${API_URL}/driver-status`; // THAY ĐỔI
    const userDataString = localStorage.getItem('userData');

    const userData = JSON.parse(userDataString);
    if (!userData || !userData.resources || !userData.resources.accessToken) {
        throw new Error('Không tìm thấy token người dùng. Vui lòng đăng nhập lại.');
    }
    const token = userData.resources.accessToken;

    const [showPopup, setShowPopup] = useState(false);
    const [editingItem, setEditingItem] = useState(null);
    const [refreshFlag, setRefreshFlag] = useState(false);

    const [showConfirm, setShowConfirm] = useState(false);
    const [confirmMessage, setConfirmMessage] = useState('');
    const [confirmAction, setConfirmAction] = useState(() => {});

    const [notifyMessage, setNotifyMessage] = useState('');
    const [showNotify, setShowNotify] = useState(false);
    const [notifySuccess, setNotifySuccess] = useState(true);

    const [dateRange, setDateRange] = useState([null, null]);
    const [startDate, endDate] = dateRange;

    const [appliedFilters, setAppliedFilters] = useState({});
    const [filterInputs, setFilterInputs] = useState({ name: '' });

    const navigate = useNavigate();
    const [isAccessChecked, setIsAccessChecked] = useState(false);
    const [isAllowedToView, setIsAllowedToView] = useState(false);

    // useEffect kiểm tra quyền xem trang
    useEffect(() => {
        // THAY ĐỔI PERMISSION_ID
        if (!canView(PERMISSION_IDS.DRIVER_STATUS)) {
            console.warn(
                `Người dùng không có quyền xem trang Driver Status (ID: ${PERMISSION_IDS.DRIVER_STATUS}). Đang chuyển hướng...`,
            );
            setIsAllowedToView(false);
            navigate('/error');
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

    const showConfirmModal = (message, action, onCancel) => {
        setConfirmMessage(message);
        setConfirmAction(() => action);
        setShowConfirm(true);
        setOnCancelConfirm(() => onCancel);
    };

    const [onCancelConfirm, setOnCancelConfirm] = useState(null);

    const handleCancelConfirm = () => {
        setShowConfirm(false);
        if (onCancelConfirm) onCancelConfirm();
    };

    const showNotifyModal = (message, success = true) => {
        setNotifyMessage(message);
        setNotifySuccess(success);
        setShowNotify(true);
    };

    const handleApplyFilter = () => {
        const [startDate, endDate] = dateRange;
        const newFilters = {
            name: filterInputs.name.trim(),
            createdDate: null,
        };

        if (startDate && endDate) {
            newFilters.createdDate = `${moment(startDate).format('DD/MM/YYYY')} - ${moment(endDate).format(
                'DD/MM/YYYY',
            )}`;
        } else if (startDate) {
            newFilters.createdDate = moment(startDate).format('DD/MM/YYYY');
        }

        setAppliedFilters(newFilters);
        setShowFilter(false);
    };

    const handleResetFilter = () => {
        setFilterInputs({ name: '' });
        setDateRange([null, null]);
        setAppliedFilters({});
    };

    const reloadTable = () => setRefreshFlag((prev) => !prev);

    // Nếu chưa kiểm tra quyền xong hoặc không có quyền, không render gì cả
    if (!isAccessChecked || !isAllowedToView) {
        return null;
    }

    return (
        <div className="container-fluid pt-4 px-4">
            <button
                type="button"
                className="btn btn-outline-primary m-2"
                id="btn_driver_status_filter" // THAY ĐỔI
                onClick={toggleFilter}
            >
                <i className="fa fa-filter me-2"></i>Bộ lọc
            </button>
            <div className={`col-sm-12 col-xl-12 filter-box ${showFilter ? 'show' : 'hide'}`}>
                <div className="bg-light rounded h-100 p-4">
                    <h6 className="mb-4">Tuỳ chọn bộ lọc</h6>
                    <div className="row">
                        <div className="col-xl-6 mb-3">
                            <label htmlFor="filter_name" className="form-label">
                                Tên:
                            </label>
                            <input
                                type="text"
                                className="form-control"
                                id="filter_name"
                                placeholder="Tên"
                                value={filterInputs.name} // Thêm value để reset
                                onChange={(e) => setFilterInputs({ ...filterInputs, name: e.target.value })}
                            />
                        </div>
                        <div className="col-xl-6 mb-3">
                            <label htmlFor="filter_created_date" className="form-label">
                                Ngày tạo
                            </label>
                            <div className="input-group">
                                <DatePicker
                                    selectsRange
                                    startDate={startDate}
                                    endDate={endDate}
                                    onChange={(update) => {
                                        setDateRange(update);
                                    }}
                                    isClearable={true}
                                    className="form-control datepicker-custom"
                                    placeholderText="Chọn khoảng ngày"
                                    dateFormat="dd/MM/yyyy"
                                    popperPlacement="bottom"
                                />
                            </div>
                        </div>
                    </div>
                    <div className="d-flex justify-content-end">
                        <button
                            type="submit"
                            className="btn btn-primary me-3"
                            id="btn_apply_filter"
                            onClick={handleApplyFilter}
                        >
                            Áp dụng
                        </button>
                        <button
                            type="reset"
                            className="btn btn-outline-primary"
                            id="btn_reset_filter"
                            onClick={handleResetFilter}
                        >
                            Đặt lại
                        </button>
                    </div>
                </div>
            </div>

            <div className="col-sm-12 col-xl-12 py-4">
                <div className="bg-light rounded h-100 p-4">
                    <div className="d-flex justify-content-between align-items-center mb-4">
                        <h6 className="mb-4">Trạng thái tài xế</h6>
                        {/* THAY ĐỔI */}
                        <button
                            type="button"
                            className="btn btn-primary"
                            id="btn_add_driver_status" // THAY ĐỔI
                            onClick={handleAdd}
                        >
                            <i className="fa fa-plus me-2"></i>Thêm mới
                        </button>
                    </div>
                    <DriverStatusTable // THAY ĐỔI
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
                <DriverStatusFormPopup // THAY ĐỔI
                    item={editingItem}
                    onClose={() => setShowPopup(false)}
                    apiUrl={apiUrl}
                    token={token}
                    onSuccess={() => {
                        reloadTable();
                        setShowPopup(false);
                    }}
                    showConfirmModal={(message, action) => showConfirmModal(message, action, () => setShowPopup(true))}
                    showNotifyModal={showNotifyModal}
                />
            )}
            {showConfirm && (
                <ConfirmModal
                    message={confirmMessage}
                    onClose={handleCancelConfirm}
                    onConfirm={() => {
                        confirmAction();
                        setShowConfirm(false);
                    }}
                />
            )}

            {showNotify && (
                <ConfirmModal
                    message={notifyMessage}
                    onClose={() => setShowNotify(false)}
                    onlyClose={true}
                    success={notifySuccess}
                />
            )}
        </div>
    );
}