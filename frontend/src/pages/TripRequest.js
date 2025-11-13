import { useState, useRef, useEffect } from 'react';
import axios from 'axios'; // THÊM VÀO
import TripRequestTable from '../Table/TripRequestTable';
import '../styles/css/TripRequest.css';
import TripRequestFormPopup from '../Table/TripRequestFormPopup';
import ConfirmModal from '../Table/ConfirmModal';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import { API_URL } from '~/api/api';
import moment from 'moment';
import { canView, canCreate, canUpdate, canDelete } from '~/utils/permissionUtils';
import { useNavigate } from 'react-router-dom';
import { PERMISSION_IDS } from '~/utils/menuIdForPermission'; // SỬA: Dùng file constants

export default function TripRequest() {
    const [showFilter, setShowFilter] = useState(false);
    const toggleFilter = () => setShowFilter(!showFilter);

    const apiUrl = `${API_URL}/TripRequest`; // URL API của bạn
    const userDataString = localStorage.getItem('userData');

    const userData = JSON.parse(userDataString);
    if (!userData || !userData.resources || !userData.resources.accessToken) {
        throw new Error('Không tìm thấy token người dùng. Vui lòng đăng nhập lại.');
    }
    const token = userData.resources.accessToken;

    const [showForm, setShowForm] = useState(false);
    const [showPopup, setShowPopup] = useState(false);
    const [editingItem, setEditingItem] = useState(null);
    const [refreshFlag, setRefreshFlag] = useState(false);

    const [showConfirm, setShowConfirm] = useState(false);
    const [confirmMessage, setConfirmMessage] = useState('');
    const [confirmUser, setConfirmUser] = useState(() => {});

    const [notifyMessage, setNotifyMessage] = useState('');
    const [showNotify, setShowNotify] = useState(false);
    const [notifySuccess, setNotifySuccess] = useState(true);

    // --- (SỬA) State cho Bộ lọc ---
    const [dateRange, setDateRange] = useState([null, null]);
    const [startDate, endDate] = dateRange;

    // State cho các giá trị lọc đã được "Áp dụng"
    const [appliedFilters, setAppliedFilters] = useState({});

    // State cho các giá trị đang được nhập trong form lọc
    const [filterInputs, setFilterInputs] = useState({
        fromLocation: '',
        toLocation: '',
        tripRequestStatusIds: [],
        requesterIds: [],
        cancellerIds: [],
    });

    // State cho dữ liệu dropdown của bộ lọc
    const [statusOptions, setStatusOptions] = useState([]);
    const [userOptions, setUserOptions] = useState([]);
    const [loadingFilters, setLoadingFilters] = useState(true);
    // --- (Kết thúc) State cho Bộ lọc ---

    const tableRef = useRef();
    const navigate = useNavigate();
    const [isAccessChecked, setIsAccessChecked] = useState(false);
    const [isAllowedToView, setIsAllowedToView] = useState(false);

    // useEffect kiểm tra quyền xem trang
    useEffect(() => {
        // SỬA: Đổi ID thành ID của trang TripRequest
        // (Giả sử ID là PERMISSION_IDS.TRIP_REQUEST, bạn hãy thay bằng ID đúng)
        if (!canView(PERMISSION_IDS.TRIP_REQUEST_LIST)) {
            console.warn(`Người dùng không có quyền xem trang (ID: ${PERMISSION_IDS.TRIP_REQUEST_LIST}). Đang chuyển hướng...`);
            setIsAllowedToView(false);
            navigate('/error');
        } else {
            setIsAllowedToView(true);
        }
        setIsAccessChecked(true);
    }, [navigate]);

    // THÊM VÀO: useEffect fetch dữ liệu cho bộ lọc
    useEffect(() => {
        const fetchFilterData = async () => {
            if (!token) return; // Đảm bảo có token
            setLoadingFilters(true);
            try {
                // Giả sử API trả về mảng trong `resources`
                const [statusRes, userRes] = await Promise.all([
                    axios.get(`${API_URL}/TripRequestStatus`, { headers: { Authorization: `Bearer ${token}` } }),
                    axios.get(`${API_URL}/User`, { headers: { Authorization: `Bearer ${token}` } }), // API lấy tất cả user
                ]);

                setStatusOptions(statusRes.data.resources || []);
                setUserOptions(userRes.data.resources || []);
            } catch (err) {
                console.error('Lỗi tải dữ liệu filter:', err);
                // Dùng showNotifyModal nội bộ (nếu có) hoặc console.error
            } finally {
                setLoadingFilters(false);
            }
        };

        fetchFilterData();
    }, [token]); // Chỉ chạy 1 lần khi có token

    const handleAdd = () => {
        setEditingItem(null);
        setShowPopup(true);
    };

    const handleEdit = (item) => {
        setEditingItem(item);
        setShowPopup(true);
    };

    const showConfirmModal = (message, user, onCancel) => {
        setConfirmMessage(message);
        setConfirmUser(() => user);
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

    const handleClose = () => setShowPopup(false);

    // --- (SỬA) Logic xử lý Filter ---

    // Xử lý input text
    const handleFilterInputChange = (e) => {
        const { name, value } = e.target;
        setFilterInputs((prev) => ({ ...prev, [name]: value }));
    };

    // Xử lý select multiple
    const handleMultiSelectChange = (e) => {
        const { name } = e.target;
        const selectedIds = Array.from(e.target.selectedOptions, (option) => Number(option.value));
        setFilterInputs((prev) => ({ ...prev, [name]: selectedIds }));
    };

    const handleApplyFilter = () => {
        const [startDate, endDate] = dateRange;
        const newFilters = {
            // Lấy từ state filterInputs
            fromLocation: filterInputs.fromLocation.trim(),
            toLocation: filterInputs.toLocation.trim(),
            tripRequestStatusIds: filterInputs.tripRequestStatusIds,
            requesterIds: filterInputs.requesterIds,
            cancellerIds: filterInputs.cancellerIds,
            createdDate: null,
        };

        // Tạo chuỗi ngày tháng
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
        // Reset các giá trị trong ô input
        setFilterInputs({
            fromLocation: '',
            toLocation: '',
            tripRequestStatusIds: [],
            requesterIds: [],
            cancellerIds: [],
        });
        setDateRange([null, null]);

        // Reset bộ lọc đã áp dụng
        setAppliedFilters({});
    };

    // --- (Kết thúc) Logic xử lý Filter ---

    const reloadTable = () => setRefreshFlag((prev) => !prev);

    // THÊM: Kiểm tra quyền truy cập
    if (!isAccessChecked) {
        return <div className="text-center p-5">Đang kiểm tra quyền truy cập...</div>;
    }
    if (!isAllowedToView) {
        return null; // Đã bị chuyển hướng
    }

    return (
        <div className="container-fluid pt-4 px-4">
            <button
                type="button"
                className="btn btn-outline-primary m-2"
                id="btn_user_status_filter"
                onClick={toggleFilter}
            >
                <i className="fa fa-filter me-2"></i>Bộ lọc
            </button>
            <div className={`col-sm-12 col-xl-12 filter-box ${showFilter ? 'show' : 'hide'}`}>
                <div className="bg-light rounded h-100 p-4">
                    <h6 className="mb-4">Tuỳ chọn bộ lọc</h6>
                    {loadingFilters ? (
                        <p>Đang tải dữ liệu lọc...</p>
                    ) : (
                        // SỬA: Thêm các ô lọc
                        <>
                            <div className="row">
                                <div className="col-md-6 mb-3">
                                    <label htmlFor="filter_fromLocation" className="form-label">
                                        Từ địa điểm
                                    </label>
                                    <input
                                        type="text"
                                        className="form-control"
                                        id="filter_fromLocation"
                                        name="fromLocation"
                                        value={filterInputs.fromLocation}
                                        onChange={handleFilterInputChange}
                                    />
                                </div>
                                <div className="col-md-6 mb-3">
                                    <label htmlFor="filter_toLocation" className="form-label">
                                        Đến địa điểm
                                    </label>
                                    <input
                                        type="text"
                                        className="form-control"
                                        id="filter_toLocation"
                                        name="toLocation"
                                        value={filterInputs.toLocation}
                                        onChange={handleFilterInputChange}
                                    />
                                </div>
                            </div>
                            <div className="row">
                                <div className="col-md-4 mb-3">
                                    <label htmlFor="filter_tripRequestStatusIds" className="form-label">
                                        Trạng thái
                                    </label>
                                    <select
                                        multiple
                                        className="form-select"
                                        id="filter_tripRequestStatusIds"
                                        name="tripRequestStatusIds"
                                        value={filterInputs.tripRequestStatusIds}
                                        onChange={handleMultiSelectChange}
                                        style={{ height: '150px' }}
                                    >
                                        {statusOptions.map((status) => (
                                            <option key={status.id} value={status.id}>
                                                {status.name}
                                            </option>
                                        ))}
                                    </select>
                                </div>
                                <div className="col-md-4 mb-3">
                                    <label htmlFor="filter_requesterIds" className="form-label">
                                        Người yêu cầu
                                    </label>
                                    <select
                                        multiple
                                        className="form-select"
                                        id="filter_requesterIds"
                                        name="requesterIds"
                                        value={filterInputs.requesterIds}
                                        onChange={handleMultiSelectChange}
                                        style={{ height: '150px' }}
                                    >
                                        {userOptions.map((user) => (
                                            <option key={user.id} value={user.id}>
                                                {user.firstName} {user.lastName} ({user.email})
                                            </option>
                                        ))}
                                    </select>
                                </div>
                                <div className="col-md-4 mb-3">
                                    <label htmlFor="filter_cancellerIds" className="form-label">
                                        Người hủy
                                    </label>
                                    <select
                                        multiple
                                        className="form-select"
                                        id="filter_cancellerIds"
                                        name="cancellerIds"
                                        value={filterInputs.cancellerIds}
                                        onChange={handleMultiSelectChange}
                                        style={{ height: '150px' }}
                                    >
                                        {userOptions.map((user) => (
                                            <option key={user.id} value={user.id}>
                                                {user.firstName} {user.lastName} ({user.email})
                                            </option>
                                        ))}
                                    </select>
                                </div>
                            </div>
                            <div className="row">
                                <div className="col-xl-12 mb-3">
                                    <label htmlFor="filter_created_date" className="form-label">
                                        Ngày tạo
                                    </label>
                                    <div className="input-group">
                                        <DatePicker
                                            selectsRange
                                            startDate={startDate}
                                            endDate={endDate}
                                            onChange={(update) => setDateRange(update)}
                                            isClearable={true}
                                            className="form-control datepicker-custom"
                                            placeholderText="Chọn khoảng ngày"
                                            dateFormat="dd/MM/yyyy"
                                            popperPlacement="bottom"
                                        />
                                    </div>
                                </div>
                            </div>
                        </>
                    )}
                    <div className="d-flex justify-content-end">
                        <button
                            type="submit"
                            className="btn btn-primary me-3"
                            id="btn_apply_filter"
                            onClick={handleApplyFilter}
                            disabled={loadingFilters} // Vô hiệu hóa khi đang tải
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
                        {/* SỬA: Tiêu đề */}
                        <h6 className="mb-4">Danh sách yêu cầu chuyến đi</h6>
                        <button type="button" className="btn btn-primary" id="btn_add_user_status" onClick={handleAdd}>
                            <i className="fa fa-plus me-2"></i>Thêm mới
                        </button>
                    </div>
                    <TripRequestTable
                        apiUrl={apiUrl}
                        token={token}
                        onEdit={handleEdit}
                        refreshFlag={refreshFlag}
                        filters={appliedFilters} // Truyền filter đã áp dụng xuống
                    />
                </div>
            </div>
            {/* Popup nhập liệu */}
            {showPopup && (
                <TripRequestFormPopup
                    item={editingItem}
                    onClose={() => setShowPopup(false)}
                    apiUrl={apiUrl}
                    token={token}
                    onSuccess={() => {
                        reloadTable();
                        setShowPopup(false);
                    }}
                    showConfirmModal={(message, user) => showConfirmModal(message, user, () => setShowPopup(true))}
                    showNotifyModal={showNotifyModal}
                />
            )}
            {/* Giữ nguyên các Modal Confirm và Notify */}
            {showConfirm && (
                <ConfirmModal
                    message={confirmMessage}
                    onClose={handleCancelConfirm}
                    onConfirm={() => {
                        confirmUser();
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