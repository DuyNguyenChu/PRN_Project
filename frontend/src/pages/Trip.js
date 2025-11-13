// Trip.js
import { useState, useRef, useEffect } from 'react';
import axios from 'axios';
import TripTable from '../Table/TripTable'; // Sửa
import TripFormPopup from '../Table/TripFormPopup'; // Sửa
import ConfirmModal from '../Table/ConfirmModal';
import '../styles/css/Trip.css';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import { API_URL } from '~/api/api';
import moment from 'moment';
import { canView } from '~/utils/permissionUtils';
import { useNavigate } from 'react-router-dom';
import { PERMISSION_IDS } from '~/utils/menuIdForPermission'; // Sửa: Dùng file constants

export default function Trip() {
    const [showFilter, setShowFilter] = useState(false);
    const toggleFilter = () => setShowFilter(!showFilter);

    const apiUrl = `${API_URL}/Trip`; // Sửa: URL API
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
    const [deleteId, setDeleteId] = useState(null); // State cho ID xóa

    const [notifyMessage, setNotifyMessage] = useState('');
    const [showNotify, setShowNotify] = useState(false);
    const [notifySuccess, setNotifySuccess] = useState(true);

    // --- State cho Bộ lọc ---
    const [dateRangeCreated, setDateRangeCreated] = useState([null, null]);
    const [appliedFilters, setAppliedFilters] = useState({});
    const [filterInputs, setFilterInputs] = useState({
        requesterIds: [],
        vehicleIds: [],
        driverIds: [],
        tripStatusIds: [],
    });

    // State cho dữ liệu dropdown của bộ lọc
    const [loadingFilters, setLoadingFilters] = useState(true);
    const [userOptions, setUserOptions] = useState([]);
    const [vehicleOptions, setVehicleOptions] = useState([]);
    const [driverOptions, setDriverOptions] = useState([]);
    const [statusOptions, setStatusOptions] = useState([]);
    // --- Kết thúc State Bộ lọc ---

    const tableRef = useRef();
    const navigate = useNavigate();
    // (Bỏ qua logic phân quyền canView ban đầu)
    // const [isAccessChecked, setIsAccessChecked] = useState(false);
    // const [isAllowedToView, setIsAllowedToView] = useState(false);

    // useEffect(() => {
    //     if (!canView(PERMISSION_IDS.TRIP)) { // Giả sử có ID này
    //         navigate('/error');
    //     } else {
    //         setIsAllowedToView(true);
    //     }
    //     setIsAccessChecked(true);
    // }, [navigate]);

    // SỬA: Fetch dữ liệu cho 4 bộ lọc
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
    }, [token]); // Chỉ chạy 1 lần khi có token

    // --- Các hàm Handler (Giống Driver.js) ---
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

    // --- Logic Xóa (Giống Driver.js) ---
    const confirmDelete = async () => {
        try {
            await axios.delete(`${apiUrl}/${deleteId}`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            showNotifyModal('Xóa thành công!', true);
            reloadTable();
        } catch (err) {
            showNotifyModal('Xóa thất bại: ' + (err.response?.data?.message || err.message), false);
        } finally {
            setDeleteId(null);
        }
    };

    const handleDeleteClick = (id) => {
        setDeleteId(id);
        showConfirmModal('Bạn có chắc chắn muốn xóa chuyến đi này?', () => confirmDelete());
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

    // (Bỏ qua block check quyền)
    // if (!isAccessChecked) { ... }

    return (
        <div className="container-fluid pt-4 px-4">
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
                                            <option key={driver.Id} value={driver.Id}>{driver.FullName}</option>
                                        ))}
                                    </select>
                                </div>
                                <div className="col-md-3 mb-3">
                                    <label htmlFor="filter_vehicleIds" className="form-label">Xe</label>
                                    <select multiple className="form-select" id="filter_vehicleIds" name="vehicleIds"
                                        value={filterInputs.vehicleIds} onChange={handleMultiSelectChange} style={{ height: '150px' }}>
                                        {vehicleOptions.map((vehicle) => (
                                            <option key={vehicle.id} value={vehicle.id}>{vehicle.licensePlate}</option>
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
                        <h6 className="mb-4">Danh sách chuyến đi</h6> {/* Sửa */}
                        <button type="button" className="btn btn-primary" onClick={handleAdd}>
                            <i className="fa fa-plus me-2"></i>Thêm mới
                        </button>
                    </div>
                    <TripTable // Sửa
                        apiUrl={apiUrl}
                        token={token}
                        onEdit={handleEdit}
                        onDelete={handleDeleteClick} // Sửa: Dùng logic xóa
                        refreshFlag={refreshFlag}
                        filters={appliedFilters}
                    />
                </div>
            </div>
            
            {/* Giữ nguyên các modal */}
            {showPopup && (
                <TripFormPopup // Sửa
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