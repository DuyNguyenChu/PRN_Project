// TripExpense.js
import { useState, useRef, useEffect } from 'react';
import axios from 'axios';
import '../styles/css/TripExpense.css';
import TripExpenseTable from '../Table/TripExpenseTable'; // Sửa
import TripExpenseFormPopup from '../Table/TripExpenseFormPopup'; // Sửa
import ConfirmModal from '../Table/ConfirmModal';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import { API_URL } from '~/api/api';
import moment from 'moment';
import { canView } from '~/utils/permissionUtils';
import { useNavigate } from 'react-router-dom';
import { PERMISSION_IDS } from '~/utils/menuIdForPermission'; // Sửa: Dùng file constants

// THÊM: Hardcode Trạng thái (dựa trên C# CommonConstants.ApprovalStatuses)
const APPROVAL_STATUSES = [
    { id: 1, name: 'Chờ duyệt', color: '#ffc107' }, // Giả sử ID=1
    { id: 2, name: 'Đã duyệt', color: '#28a745' }, // Giả sử ID=2
    { id: 3, name: 'Từ chối', color: '#dc3545' }, // Giả sử ID=3
];

export default function TripExpense() {
    const [showFilter, setShowFilter] = useState(false);
    const toggleFilter = () => setShowFilter(!showFilter);

    const apiUrl = `${API_URL}/TripExpense`; // Sửa: URL API
    const userDataString = localStorage.getItem('userData');

    const userData = JSON.parse(userDataString);
    if (!userData || !userData.resources || !userData.resources.accessToken) {
        throw new Error('Không tìm thấy token người dùng. Vui lòng đăng nhập lại.');
    }
    const token = userData.resources.accessToken;

    // State cho popup
    const [showPopup, setShowPopup] = useState(false);
    const [editingItem, setEditingItem] = useState(null);
    const [refreshFlag, setRefreshFlag] = useState(false);

    // State cho modal
    const [showConfirm, setShowConfirm] = useState(false);
    const [confirmMessage, setConfirmMessage] = useState('');
    const [confirmAction, setConfirmAction] = useState(() => {});

    const [notifyMessage, setNotifyMessage] = useState('');
    const [showNotify, setShowNotify] = useState(false);
    const [notifySuccess, setNotifySuccess] = useState(true);

    // --- State cho Bộ lọc ---
    const [dateRangeCreated, setDateRangeCreated] = useState([null, null]);
    const [dateRangeExpense, setDateRangeExpense] = useState([null, null]);
    const [dateRangeApproval, setDateRangeApproval] = useState([null, null]);
    
    const [appliedFilters, setAppliedFilters] = useState({});
    const [filterInputs, setFilterInputs] = useState({
        vehicleIds: [],
        statusIds: [],
        expenseTypeIds: [],
        driverIds: [],
        tripIds: [],
    });

    // State cho dữ liệu dropdown của bộ lọc
    const [loadingFilters, setLoadingFilters] = useState(true);
    const [vehicleOptions, setVehicleOptions] = useState([]);
    const [expenseTypeOptions, setExpenseTypeOptions] = useState([]);
    const [driverOptions, setDriverOptions] = useState([]);
    const [tripOptions, setTripOptions] = useState([]);
    // --- Kết thúc State Bộ lọc ---

    const tableRef = useRef();
    const navigate = useNavigate();
    const [isAccessChecked, setIsAccessChecked] = useState(false);
    const [isAllowedToView, setIsAllowedToView] = useState(false);

    // Kiểm tra quyền
    useEffect(() => {
        // SỬA: Dùng ID của trang TripExpense
        if (!canView(PERMISSION_IDS.TRIP_EXPENSE)) { // Giả sử bạn có ID này
            console.warn(`Người dùng không có quyền xem trang (ID: ${PERMISSION_IDS.TRIP_EXPENSE}). Đang chuyển hướng...`);
            setIsAllowedToView(false);
            navigate('/error');
        } else {
            setIsAllowedToView(true);
        }
        setIsAccessChecked(true);
    }, [navigate]);

    // Fetch dữ liệu cho bộ lọc
    useEffect(() => {
        const fetchFilterData = async () => {
            if (!token) return;
            setLoadingFilters(true);
            try {
                // Gọi 4 API để lấy data cho filter
                const [vehicleRes, typeRes, driverRes, tripRes] = await Promise.all([
                    axios.get(`${API_URL}/Vehicle`, { headers: { Authorization: `Bearer ${token}` } }),
                    axios.get(`${API_URL}/ExpenseType`, { headers: { Authorization: `Bearer ${token}` } }),
                    axios.get(`${API_URL}/Driver`, { headers: { Authorization: `Bearer ${token}` } }),
                    axios.get(`${API_URL}/Trip`, { headers: { Authorization: `Bearer ${token}` } }),
                ]);

                setVehicleOptions(vehicleRes.data.resources || []);
                setExpenseTypeOptions(typeRes.data.resources || []);
                setDriverOptions(driverRes.data.resources || []);
                setTripOptions(tripRes.data.resources || []);

            } catch (err) {
                console.error('Lỗi tải dữ liệu filter:', err);
                showNotifyModal('Lỗi tải dữ liệu cho bộ lọc: ' + err.message, false);
            } finally {
                setLoadingFilters(false);
            }
        };
        fetchFilterData();
    }, [token]); // Chỉ chạy 1 lần khi có token

    // --- Các hàm Handler (giữ nguyên logic) ---
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

    // --- Logic xử lý Filter ---
    const handleMultiSelectChange = (e) => {
        const { name } = e.target;
        const selectedIds = Array.from(e.target.selectedOptions, (option) => Number(option.value));
        setFilterInputs((prev) => ({ ...prev, [name]: selectedIds }));
    };

    const formatDateRange = (dateRange) => {
        const [startDate, endDate] = dateRange;
        if (startDate && endDate) {
            return `${moment(startDate).format('DD/MM/YYYY')} - ${moment(endDate).format('DD/MM/YYYY')}`;
        } else if (startDate) {
            return moment(startDate).format('DD/MM/YYYY');
        }
        return null;
    };

    const handleApplyFilter = () => {
        const newFilters = {
            ...filterInputs,
            createdDate: formatDateRange(dateRangeCreated),
            expenseDate: formatDateRange(dateRangeExpense),
            approvalDate: formatDateRange(dateRangeApproval),
        };

        setAppliedFilters(newFilters);
        setShowFilter(false);
    };

    const handleResetFilter = () => {
        setFilterInputs({
            vehicleIds: [],
            statusIds: [],
            expenseTypeIds: [],
            driverIds: [],
            tripIds: [],
        });
        setDateRangeCreated([null, null]);
        setDateRangeExpense([null, null]);
        setDateRangeApproval([null, null]);
        setAppliedFilters({});
    };
    // --- Kết thúc Logic Filter ---

    const reloadTable = () => setRefreshFlag((prev) => !prev);

    if (!isAccessChecked) {
        return <div className="text-center p-5">Đang kiểm tra quyền truy cập...</div>;
    }
    if (!isAllowedToView) {
        return null;
    }

    return (
        <div className="container-fluid pt-4 px-4">
            <button
                type="button"
                className="btn btn-outline-primary m-2"
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
                        <>
                            <div className="row">
                                <div className="col-md-4 mb-3">
                                    <label className="form-label">Ngày tạo</label>
                                    <DatePicker
                                        selectsRange
                                        startDate={dateRangeCreated[0]}
                                        endDate={dateRangeCreated[1]}
                                        onChange={(update) => setDateRangeCreated(update)}
                                        isClearable={true}
                                        className="form-control"
                                        placeholderText="Chọn khoảng ngày tạo"
                                        dateFormat="dd/MM/yyyy"
                                    />
                                </div>
                                <div className="col-md-4 mb-3">
                                    <label className="form-label">Ngày chi phí</label>
                                    <DatePicker
                                        selectsRange
                                        startDate={dateRangeExpense[0]}
                                        endDate={dateRangeExpense[1]}
                                        onChange={(update) => setDateRangeExpense(update)}
                                        isClearable={true}
                                        className="form-control"
                                        placeholderText="Chọn khoảng ngày chi phí"
                                        dateFormat="dd/MM/yyyy"
                                    />
                                </div>
                                <div className="col-md-4 mb-3">
                                    <label className="form-label">Ngày duyệt</label>
                                    <DatePicker
                                        selectsRange
                                        startDate={dateRangeApproval[0]}
                                        endDate={dateRangeApproval[1]}
                                        onChange={(update) => setDateRangeApproval(update)}
                                        isClearable={true}
                                        className="form-control"
                                        placeholderText="Chọn khoảng ngày duyệt"
                                        dateFormat="dd/MM/yyyy"
                                    />
                                </div>
                            </div>
                            <div className="row">
                                <div className="col-md-4 mb-3">
                                    <label htmlFor="filter_statusIds" className="form-label">Trạng thái</label>
                                    <select multiple className="form-select" id="filter_statusIds" name="statusIds"
                                        value={filterInputs.statusIds} onChange={handleMultiSelectChange} style={{ height: '150px' }}>
                                        {APPROVAL_STATUSES.map((status) => (
                                            <option key={status.id} value={status.id}>{status.name}</option>
                                        ))}
                                    </select>
                                </div>
                                 <div className="col-md-4 mb-3">
                                    <label htmlFor="filter_expenseTypeIds" className="form-label">Loại chi phí</label>
                                    <select multiple className="form-select" id="filter_expenseTypeIds" name="expenseTypeIds"
                                        value={filterInputs.expenseTypeIds} onChange={handleMultiSelectChange} style={{ height: '150px' }}>
                                        {expenseTypeOptions.map((type) => (
                                            <option key={type.id} value={type.id}>{type.name}</option>
                                        ))}
                                    </select>
                                </div>
                                <div className="col-md-4 mb-3">
                                    <label htmlFor="filter_driverIds" className="form-label">Tài xế</label>
                                    <select multiple className="form-select" id="filter_driverIds" name="driverIds"
                                        value={filterInputs.driverIds} onChange={handleMultiSelectChange} style={{ height: '150px' }}>
                                        {driverOptions.map((driver) => (
                                            <option key={driver.Id} value={driver.Id}>{driver.FullName}</option> // Giả sử API /Driver trả về Id và FullName
                                        ))}
                                    </select>
                                </div>
                            </div>
                             <div className="row">
                                <div className="col-md-6 mb-3">
                                    <label htmlFor="filter_vehicleIds" className="form-label">Xe</label>
                                    <select multiple className="form-select" id="filter_vehicleIds" name="vehicleIds"
                                        value={filterInputs.vehicleIds} onChange={handleMultiSelectChange} style={{ height: '150px' }}>
                                        {vehicleOptions.map((vehicle) => (
                                            <option key={vehicle.id} value={vehicle.id}>{vehicle.licensePlate}</option> // Giả sử API /Vehicle trả về id và licensePlate
                                        ))}
                                    </select>
                                </div>
                                <div className="col-md-6 mb-3">
                                    <label htmlFor="filter_tripIds" className="form-label">Chuyến đi</label>
                                    <select multiple className="form-select" id="filter_tripIds" name="tripIds"
                                        value={filterInputs.tripIds} onChange={handleMultiSelectChange} style={{ height: '150px' }}>
                                        {tripOptions.map((trip) => (
                                            <option key={trip.id} value={trip.id}>{trip.id} (Từ {trip.fromLocation} đến {trip.toLocation})</option> // Giả sử API /Trip trả về
                                        ))}
                                    </select>
                                </div>
                            </div>
                        </>
                    )}
                    <div className="d-flex justify-content-end">
                        <button type="submit" className="btn btn-primary me-3" onClick={handleApplyFilter} disabled={loadingFilters}>
                            Áp dụng
                        </button>
                        <button type="reset" className="btn btn-outline-primary" onClick={handleResetFilter}>
                            Đặt lại
                        </button>
                    </div>
                </div>
            </div>

            <div className="col-sm-12 col-xl-12 py-4">
                <div className="bg-light rounded h-100 p-4">
                    <div className="d-flex justify-content-between align-items-center mb-4">
                        <h6 className="mb-4">Danh sách chi phí</h6> {/* Sửa */}
                        <button type="button" className="btn btn-primary" onClick={handleAdd}>
                            <i className="fa fa-plus me-2"></i>Thêm mới
                        </button>
                    </div>
                    <TripExpenseTable // Sửa
                        apiUrl={apiUrl}
                        token={token}
                        onEdit={handleEdit}
                        refreshFlag={refreshFlag}
                        filters={appliedFilters}
                    />
                </div>
            </div>
            
            {/* Giữ nguyên các modal */}
            {showPopup && (
                <TripExpenseFormPopup // Sửa
                    item={editingItem}
                    onClose={() => setShowPopup(false)}
                    apiUrl={apiUrl}
                    token={token}
                    onSuccess={() => {
                        reloadTable();
                        setShowPopup(false);
                    }}
                    showConfirmModal={showConfirmModal}
                    showNotifyModal={showNotifyModal}
                />
            )}
            {showConfirm && (
                <ConfirmModal message={confirmMessage} onClose={handleCancelConfirm}
                    onConfirm={() => { confirmAction(); setShowConfirm(false); }} />
            )}
            {showNotify && (
                <ConfirmModal message={notifyMessage} onClose={() => setShowNotify(false)} onlyClose={true} success={notifySuccess} />
            )}
        </div>
    );
}