// Trip.js
import { useState, useRef, useEffect } from 'react';
import axios from 'axios';
import TripTable from '../Table/TripTable';
import TripFormPopup from '../Table/TripFormPopup';
import ConfirmModal from '../Table/ConfirmModal';
import '../styles/css/Trip.css';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import { API_URL } from '~/api/api';
import moment from 'moment';
// SỬA: Import các hàm phân quyền
import { canView, isDispatcher, isDriver, isUser } from '~/utils/permissionUtils';
import { useNavigate } from 'react-router-dom';
import { PERMISSION_IDS } from '~/utils/menuIdForPermission';

// THÊM: Import các popup hành động
import StartTripPopup from '../Table/StartTripPopup';
import CompleteTripPopup from '../Table/CompleteTripPopup';
import CancelTripPopup from '../Table/CancelTripPopup';

// !! QUAN TRỌNG: ĐỊNH NGHĨA CÁC ID TRẠNG THÁI CHUYẾN ĐI
// !! Vui lòng sửa các giá trị số (2, 3, 4...) cho khớp với CSDL của bạn
const TRIP_STATUS = {
    ASSIGNED: 4, // Đã khởi hành / Giao cho tài xế
    ACCEPTED: 5, // Lái xe đã nhận
    ARRIVING: 8, // Lái xe đang đi đến đón
    PICKED_UP: 9, // Đã đến đón
    EN_ROUTE: 10, // Đang đến đích
    ARRIVED: 11, // Đã đến đích
    COMPLETED: 12, // Hoàn thành
    CANCELLED: 15, // Đã hủy
    REJECTED: 6, // Lái xe từ chối
};

export default function Trip() {
    const [showFilter, setShowFilter] = useState(false);
    const toggleFilter = () => setShowFilter(!showFilter);

    const apiUrl = `${API_URL}/Trip`;
    const userDataString = localStorage.getItem('userData');

    const userData = JSON.parse(userDataString);
    if (!userData || !userData.resources || !userData.resources.accessToken) {
        throw new Error('Không tìm thấy token người dùng. Vui lòng đăng nhập lại.');
    }
    const token = userData.resources.accessToken;
    const loggedInUserId = userData.resources.id || 0;

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

    // XÓA: State cho ID Xóa (deleteId)

    // --- SỬA: State cho các popup hành động ---
    const [showStartPopup, setShowStartPopup] = useState(false);
    const [showCompletePopup, setShowCompletePopup] = useState(false);
    const [showCancelPopup, setShowCancelPopup] = useState(false);
    const [currentItemForAction, setCurrentItemForAction] = useState(null);
    const [isStartForEnRoute, setIsStartForEnRoute] = useState(false); // Cờ phân biệt popup Bắt đầu (đến đón) hay (đến đích)

    // --- State cho Bộ lọc (Giữ nguyên) ---
    const [dateRangeCreated, setDateRangeCreated] = useState([null, null]);
    const [appliedFilters, setAppliedFilters] = useState({});
    const [filterInputs, setFilterInputs] = useState({
        requesterIds: [],
        vehicleIds: [],
        driverIds: [],
        tripStatusIds: [],
    });
    const [loadingFilters, setLoadingFilters] = useState(true);
    const [userOptions, setUserOptions] = useState([]);
    const [vehicleOptions, setVehicleOptions] = useState([]);
    const [driverOptions, setDriverOptions] = useState([]);
    const [statusOptions, setStatusOptions] = useState([]);
    // --- Kết thúc State Bộ lọc ---

    const tableRef = useRef();
    const navigate = useNavigate();

    // --- SỬA: State Phân quyền ---
    const [isAccessChecked, setIsAccessChecked] = useState(false);
    const [isAllowedToView, setIsAllowedToView] = useState(false);
    const [isDispatcherRole, setIsDispatcherRole] = useState(false);
    const [isDriverRole, setIsDriverRole] = useState(false);
    const [isUserRole, setIsUserRole] = useState(false);

    useEffect(() => {
        // 1. Kiểm tra quyền xem trang
        if (!canView(PERMISSION_IDS.TRIP_LIST)) { // Giả sử có ID này
            navigate('/error');
            return;
        }
        setIsAllowedToView(true);

        // 2. Tải vai trò
        // (permissionUtils đã cache vai trò nên gọi trực tiếp)
        setIsDispatcherRole(isDispatcher());
        setIsDriverRole(isDriver());
        setIsUserRole(isUser());
        
        setIsAccessChecked(true);
    }, [navigate]);

    // SỬA: Fetch dữ liệu cho 4 bộ lọc (Giữ nguyên)
    useEffect(() => {
        const fetchFilterData = async () => {
            if (!token) return;
            setLoadingFilters(true);
            try {
                const [userRes, vehicleRes, driverRes, statusRes] = await Promise.all([
                    axios.get(`${API_URL}/User`, { headers: { Authorization: `Bearer ${token}` } }),
                    axios.get(`${API_URL}/Vehicle`, { headers: { Authorization: `Bearer ${token}` } }),
                    axios.get(`${API_URL}/Driver`, { headers: { Authorization: `Bearer ${token}` } }),
                    axios.get(`${API_URL}/TripStatus`, { headers: { Authorization: `Bearer ${token}` } }),
                ]);

                setUserOptions(userRes.data.resources || []);
                setVehicleOptions(vehicleRes.data.resources || []);
                setDriverOptions(driverRes.data.resources || []);
                setStatusOptions(statusRes.data.resources || []);
            } catch (err) {
                console.error('Lỗi tải dữ liệu filter:', err);
                showNotifyModal('Lỗi tải dữ liệu cho bộ lọc: ' + err.message, false);
            } finally {
                setLoadingFilters(false);
            }
        };
        fetchFilterData();
    }, [token]);

    // --- Các hàm Handler (Giữ nguyên) ---
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

    // --- XÓA: Logic Xóa (confirmDelete, handleDeleteClick) ---

    // --- THÊM: Các hàm xử lý hành động của Lái xe ---

    // Hàm gọi API chung cho các hành động đơn giản (không cần popup)
    const submitSimpleAction = async (endpoint, payload, successMsg, errorMsg) => {
        try {
            await axios.put(`${apiUrl}/${endpoint}`, payload, { headers: { Authorization: `Bearer ${token}` } });
            showNotifyModal(successMsg, true);
            reloadTable();
        } catch (err) {
            showNotifyModal(`${errorMsg}: ${err.response?.data?.message || err.message}`, false);
        }
    };

    // 1. Nhận chuyến
    const handleAcceptClick = (item) => {
        showConfirmModal('Bạn có chắc chắn muốn NHẬN chuyến đi này?', () => 
            submitSimpleAction(
                'driver/accept', 
                { TripId: item.Id || item.id }, 
                'Nhận chuyến thành công!', 
                'Nhận chuyến thất bại'
            )
        );
    };

    // 2. Từ chối chuyến
    const handleRejectClick = (item) => {
        showConfirmModal('Bạn có chắc chắn muốn TỪ CHỐI chuyến đi này?', () => 
            submitSimpleAction(
                'driver/reject', 
                { TripId: item.Id || item.id, driverId: item.DriverId }, // Giả sử API cần driverId để reject
                'Từ chối thành công!', 
                'Từ chối thất bại'
            )
        );
    };
    
    // 3. Hủy chuyến (Mở popup lý do)
    const handleCancelClick = (item) => {
        setCurrentItemForAction(item);
        setShowCancelPopup(true);
    };
    const onCancelSubmit = async (reason) => {
        const payload = { 
            TripId: currentItemForAction.Id || currentItemForAction.id, 
            cancelReason: reason, 
            cancelStatusId: 15,
        };
        await submitSimpleAction('cancel', payload, 'Hủy chuyến thành công!', 'Hủy chuyến thất bại');
        setShowCancelPopup(false);
    };

    // 4. Đang đến đón (Mở popup Odometer)
    const handleArrivingClick = (item) => {
        setCurrentItemForAction(item);
        setIsStartForEnRoute(false); // Đây là "đến đón"
        setShowStartPopup(true);
    };

    // 5. Đã đến đón (Confirm đơn giản)
    const handlePickedUpClick = (item) => {
        showConfirmModal('Xác nhận đã ĐẾN ĐÓN khách?', () => 
            submitSimpleAction(
                'driver/arrived-at-pick-up', 
                { TripId: item.Id || item.id, updatedBy: loggedInUserId }, 
                'Xác nhận đến đón thành công!', 
                'Xác nhận thất bại'
            )
        );
    };

    // 6. Đang đến đích (Mở popup Odometer)
    const handleEnRouteClick = (item) => {
        setCurrentItemForAction(item);
        setIsStartForEnRoute(true); // Đây là "đến đích"
        setShowStartPopup(true);
    };

    // 7. Đã đến đích (Confirm đơn giản)
    const handleArrivedClick = (item) => {
        showConfirmModal('Xác nhận đã ĐẾN ĐÍCH?', () => 
            submitSimpleAction(
                'driver/arrived-at-destination', 
                { TripId: item.Id || item.id, updatedBy: loggedInUserId }, 
                'Xác nhận đến đích thành công!', 
                'Xác nhận thất bại'
            )
        );
    };

    // 8. Hoàn thành (Mở popup Odometer kết thúc)
    const handleCompleteClick = (item) => {
        setCurrentItemForAction(item);
        setShowCompletePopup(true);
    };

    // --- THÊM: Các hàm Submit từ Popup ---
    const onStartSubmit = async (odometer) => {
        const endpoint = isStartForEnRoute ? 'driver/moving-to-destination' : 'driver/moving-to-pick-up';
        const successMsg = isStartForEnRoute ? 'Bắt đầu đến đích thành công!' : 'Bắt đầu đến đón thành công!';
        const errorMsg = isStartForEnRoute ? 'Bắt đầu đến đích thất bại' : 'Bắt đầu đến đón thất bại';
        
        const payload = { 
            TripId: currentItemForAction.Id || currentItemForAction.id, 
            StartOdometer: odometer, 
            updatedBy: loggedInUserId 
        };
        
        await submitSimpleAction(endpoint, payload, successMsg, errorMsg);
        setShowStartPopup(false);
    };

    const onCompleteSubmit = async (odometer) => {
        const payload = { 
            TripId: currentItemForAction.Id || currentItemForAction.id, 
            EndOdometer: odometer, 
            updatedBy: loggedInUserId 
        };
        await submitSimpleAction('driver/complete', payload, 'Hoàn thành chuyến đi thành công!', 'Hoàn thành chuyến đi thất bại');
        setShowCompletePopup(false);
    };


    // --- Logic xử lý Filter (Giữ nguyên) ---
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
        };
        setAppliedFilters(newFilters);
        setShowFilter(false);
    };

    const handleResetFilter = () => {
        setFilterInputs({
            requesterIds: [],
            vehicleIds: [],
            driverIds: [],
            tripStatusIds: [],
        });
        setDateRangeCreated([null, null]);
        setAppliedFilters({});
    };

    const reloadTable = () => setRefreshFlag((prev) => !prev);
    // --- Kết thúc Logic Filter ---


    if (!isAccessChecked) {
        return <div className="text-center p-5">Đang kiểm tra quyền truy cập...</div>;
    }
    if (!isAllowedToView) {
        return null; // Đã bị chuyển hướng
    }

    return (
        <div className="container-fluid pt-4 px-4">
            {/* ... (Giữ nguyên JSX của Bộ lọc) ... */}
            <button type="button" className="btn btn-outline-primary m-2" onClick={toggleFilter}>
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
                                <div className="col-md-3 mb-3">
                                    <label htmlFor="filter_tripStatusIds" className="form-label">Trạng thái</label>
                                    <select multiple className="form-select" id="filter_tripStatusIds" name="tripStatusIds"
                                        value={filterInputs.tripStatusIds} onChange={handleMultiSelectChange} style={{ height: '150px' }}>
                                        {statusOptions.map((status) => (
                                            <option key={status.id} value={status.id}>{status.name}</option>
                                        ))}
                                    </select>
                                </div>
                                <div className="col-md-3 mb-3">
                                    <label htmlFor="filter_requesterIds" className="form-label">Người yêu cầu</label>
                                    <select multiple className="form-select" id="filter_requesterIds" name="requesterIds"
                                        value={filterInputs.requesterIds} onChange={handleMultiSelectChange} style={{ height: '150px' }}>
                                        {userOptions.map((user) => (
                                            <option key={user.id} value={user.id}>{user.firstName} {user.lastName}</option>
                                        ))}
                                    </select>
                                </div>
                                <div className="col-md-3 mb-3">
                                    <label htmlFor="filter_driverIds" className="form-label">Tài xế</label>
                                    <select multiple className="form-select" id="filter_driverIds" name="driverIds"
                                        value={filterInputs.driverIds} onChange={handleMultiSelectChange} style={{ height: '150px' }}>
                                        {driverOptions.map((driver) => (
                                            <option key={driver.id} value={driver.id}>{driver.fullName}</option>
                                        ))}
                                    </select>
                                </div>
                                <div className="col-md-3 mb-3">
                                    <label htmlFor="filter_vehicleIds" className="form-label">Xe</label>
                                    <select multiple className="form-select" id="filter_vehicleIds" name="vehicleIds"
                                        value={filterInputs.vehicleIds} onChange={handleMultiSelectChange} style={{ height: '150px' }}>
                                        {vehicleOptions.map((vehicle) => (
                                            <option key={vehicle.id} value={vehicle.id}>{vehicle.name}</option>
                                        ))}
                                    </select>
                                </div>
                            </div>
                            <div className="row">
                                <div className="col-xl-12 mb-3">
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
                        <h6 className="mb-4">Danh sách chuyến đi</h6>
                        {/* SỬA: Chỉ Điều phối viên thấy nút Thêm mới */}
                        {isDispatcherRole && (
                            <button type="button" className="btn btn-primary" onClick={handleAdd}>
                                <i className="fa fa-plus me-2"></i>Thêm mới
                            </button>
                        )}
                    </div>

                    {/* SỬA: Truyền props phân quyền và hành động xuống Table */}
                    <TripTable
                        apiUrl={apiUrl}
                        token={token}
                        // Chỉ ĐPV mới có quyền Sửa
                        onEdit={isDispatcherRole ? handleEdit : null} 
                        // XÓA: onDelete
                        refreshFlag={refreshFlag}
                        filters={appliedFilters}
                        // Truyền vai trò
                        isDispatcherRole={isDispatcherRole}
                        isDriverRole={isDriverRole}
                        isUserRole={isUserRole}
                        // Truyền các hành động
                        onAccept={handleAcceptClick}
                        onReject={handleRejectClick}
                        onArriving={handleArrivingClick}
                        onPickedUp={handlePickedUpClick}
                        onEnRoute={handleEnRouteClick}
                        onArrived={handleArrivedClick}
                        onComplete={handleCompleteClick}
                        onCancel={handleCancelClick}
                        // Truyền hằng số trạng thái
                        tripStatusConstants={TRIP_STATUS}
                    />
                </div>
            </div>
            
            {/* SỬA: Popup Form (Chỉ mở bởi ĐPV nên không cần ReadOnly) */}
            {showPopup && (
                <TripFormPopup
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

            {/* Modal Confirm và Notify (Giữ nguyên) */}
            {showConfirm && (
                <ConfirmModal message={confirmMessage} onClose={handleCancelConfirm}
                    onConfirm={() => { confirmAction(); setShowConfirm(false); }} />
            )}
            {showNotify && (
                <ConfirmModal message={notifyMessage} onClose={() => setShowNotify(false)} onlyClose={true} success={notifySuccess} />
            )}

            {/* THÊM: Render các popup hành động mới */}
            {showStartPopup && (
                <StartTripPopup
                    onClose={() => setShowStartPopup(false)}
                    onSubmit={onStartSubmit}
                    isEnRoute={isStartForEnRoute} // Cờ để thay đổi text
                />
            )}
            {showCompletePopup && (
                <CompleteTripPopup
                    onClose={() => setShowCompletePopup(false)}
                    onSubmit={onCompleteSubmit}
                />
            )}
            {showCancelPopup && (
                <CancelTripPopup
                    onClose={() => setShowCancelPopup(false)}
                    onSubmit={onCancelSubmit}
                />
            )}
        </div>
    );
}