import { useState, useRef, useEffect } from 'react';
import MaintenanceRecordTable from '../Table/MaintenanceRecordTable'; 
import MaintenanceRecordFormPopup from '../Table/MaintenanceRecordFormPopup'; 
import RejectFormPopup from '../Table/RejectFormPopup'; 
import ConfirmModal from '../Table/ConfirmModal';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import Select from 'react-select'; 
import { API_URL } from '~/api/api';
import moment from 'moment';
import axios from 'axios'; 
import { canView } from '~/utils/permissionUtils'; 
import { useNavigate } from 'react-router-dom';
import { PERMISSION_IDS } from '~/utils/menuIdForPermission';

export default function MaintenanceRecord({ permissionFlags }) {
    const [showFilter, setShowFilter] = useState(false);
    const toggleFilter = () => setShowFilter(!showFilter);
    const apiUrl = `${API_URL}/MaintenanceRecord`; // URL API của MaintenanceRecord
    const userDataString = localStorage.getItem('userData');
    const userData = JSON.parse(userDataString);
    if (!userData || !userData.resources || !userData.resources.accessToken) {
        throw new Error('Không tìm thấy token người dùng. Vui lòng đăng nhập lại.');
    }
    const token = userData.resources.accessToken;

    // State cho Popups
    const [showPopup, setShowPopup] = useState(false);
    const [showRejectPopup, setShowRejectPopup] = useState(false); 
    const [editingItem, setEditingItem] = useState(null);
    const [rejectingItem, setRejectingItem] = useState(null); 
    const [refreshFlag, setRefreshFlag] = useState(false);
    const [showConfirm, setShowConfirm] = useState(false);
    const [confirmMessage, setConfirmMessage] = useState('');
    const [confirmAction, setConfirmAction] = useState(() => {});
    const [notifyMessage, setNotifyMessage] = useState('');
    const [showNotify, setShowNotify] = useState(false);
    const [notifySuccess, setNotifySuccess] = useState(true);
    const [onCancelConfirm, setOnCancelConfirm] = useState(null);

    // State cho dữ liệu bộ lọc
    const [vehicleList, setVehicleList] = useState([]);
    const [driverList, setDriverList] = useState([]);
    const [tripList, setTripList] = useState([]);
    const [serviceTypeList, setServiceTypeList] = useState([]); // Mới
    const [statusList, setStatusList] = useState([]);

    // State cho giá trị input bộ lọc
    const [dateRange, setDateRange] = useState([null, null]);
    const [startDate, endDate] = dateRange;
    const [filterInputs, setFilterInputs] = useState({ serviceProvider: '', tripCode: '' }); // Sửa
    const [selectedVehicles, setSelectedVehicles] = useState([]);
    const [selectedDrivers, setSelectedDrivers] = useState([]);
    const [selectedTrips, setSelectedTrips] = useState([]);
    const [selectedStatuses, setSelectedStatuses] = useState([]);

    // State cho bộ lọc đã "Áp dụng"
    const [appliedFilters, setAppliedFilters] = useState({});

    const navigate = useNavigate();
    const [isAccessChecked, setIsAccessChecked] = useState(false);
    const [isAllowedToView, setIsAllowedToView] = useState(false);

    useEffect(() => {
        if (!canView(PERMISSION_IDS.MAINTENANCE_RECORD)) {
            console.warn(`Người dùng không có quyền xem trang (ID: ${PERMISSION_IDS.MAINTENANCE_RECORD}). Đang chuyển hướng...`);
            setIsAllowedToView(false);
            navigate('/error');
        } else {
            setIsAllowedToView(true);
        }
        setIsAccessChecked(true);
    }, [navigate]);

    // Fetch dữ liệu cho các dropdown bộ lọc
    useEffect(() => {
        const headers = { Authorization: `Bearer ${token}` };
        // Giả sử bạn có các API endpoint này để lấy danh sách (không có /all)
        axios.get(`${API_URL}/Vehicle`, { headers }).then(res => setVehicleList(res.data.resources.map(v => ({ value: v.id, label: `[${v.registrationNumber}] ${v.vehicleModelName}` })))).catch(err => console.error("Lỗi tải Vehicle:", err));
        axios.get(`${API_URL}/Driver`, { headers }).then(res => setDriverList(res.data.resources.map(d => ({ value: d.id, label: d.name })))).catch(err => console.error("Lỗi tải Driver:", err));
        axios.get(`${API_URL}/Trip`, { headers }).then(res => setTripList(res.data.resources.map(t => ({ value: t.id, label: t.description })))).catch(err => console.error("Lỗi tải Trip:", err));
        
        // API mới từ MaintenanceRecordController
        axios.get(`${apiUrl}/service-types`, { headers }).then(res => setServiceTypeList(res.data.resources.map(f => ({ value: f.id, label: f.name })))).catch(err => console.error("Lỗi tải ServiceTypes:", err));
        
        // Dùng chung API status từ FuelLog (vì backend dùng chung CommonConstants)
        axios.get(`${API_URL}/FuelLog/status`, { headers }).then(res => setStatusList(res.data.resources.map(s => ({ value: s.id, label: s.name })))).catch(err => console.error("Lỗi tải Status:", err));
    }, [token, apiUrl]);

    // Handlers cho các hành động
    const handleAdd = () => {
        setEditingItem(null);
        setShowPopup(true);
    };

const handleEdit = (item) => {
        // Gọi GetByIdAsync để lấy chi tiết (details)
        axios.get(`${apiUrl}/${item.id}`, { headers: { Authorization: `Bearer ${token}` } })
            .then(res => {
                const itemDetails = res.data.resources;
                
                // Chuyển đổi định dạng cho form
                const formattedItem = {
                    // Lấy tất cả dữ liệu chi tiết
                    ...itemDetails, 

                    // === SỬA LỖI Ở ĐÂY ===
                    // Ghi đè 'status' (là object) bằng 'status.id' (là number)
                    // Logic isReadOnly trong FormPopup sẽ dựa vào con số này
                    status: itemDetails.status.id, 
                    // ======================

                    // Map lại các ID cho <Select>
                    vehicleId: itemDetails.vehicle?.id || null,
                    tripId: itemDetails.trip?.id || null,
                    serviceType: itemDetails.serviceType?.id || null,
                    
                    // Map lại details (giữ nguyên)
                    details: itemDetails.detail.map(d => ({
                        description: d.description,
                        quantity: d.quantity,
                        unitPrice: d.unitPrice
                    }))
                };

                setEditingItem(formattedItem);
                setShowPopup(true);
            })
            .catch(err => showNotifyModal("Lỗi khi tải chi tiết: " + err.message, false));
    };
    const handleDelete = (id) => {
        showConfirmModal('Bạn có chắc chắn muốn xóa bản ghi này? (Không thể hoàn tác)', async () => {
            try {
                // Gọi API DELETE mới
                await axios.delete(`${apiUrl}/${id}`, { headers: { Authorization: `Bearer ${token}` } });
                showNotifyModal('Xóa thành công!');
                reloadTable();
            } catch (err) {
                showNotifyModal('Xóa thất bại: ' + (err.response?.data?.message || err.message), false);
            }
        });
    };

    const reloadTable = () => setRefreshFlag((prev) => !prev);

    // Handlers cho modal
    const showConfirmModal = (message, action, onCancel) => {
        setConfirmMessage(message);
        setConfirmAction(() => action);
        setShowConfirm(true);
        setOnCancelConfirm(() => onCancel);
    };

    const handleCancelConfirm = () => {
        setShowConfirm(false);
        if (onCancelConfirm) onCancelConfirm();
    };

    const showNotifyModal = (message, success = true) => {
        setNotifyMessage(message);
        setNotifySuccess(success);
        setShowNotify(true);
    };


    
    const handleApprove = (id) => {
        showConfirmModal('Bạn có chắc chắn muốn duyệt bản ghi này?', async () => {
            try {
                await axios.put(`${apiUrl}/${id}/approve`, {}, { headers: { Authorization: `Bearer ${token}` } });
                showNotifyModal('Duyệt thành công!');
                reloadTable();
            } catch (err) {
                showNotifyModal('Duyệt thất bại: ' + (err.response?.data?.message || err.message), false);
            }
        });
    };

    const handleReject = (item) => {
        setRejectingItem(item);
        setShowRejectPopup(true);
    };

    // Handlers cho bộ lọc
    const handleApplyFilter = () => {
        const [startDate, endDate] = dateRange;
        let startTimeFilter = null;
        if (startDate && endDate) {
            startTimeFilter = `${moment(startDate).format('DD/MM/YYYY')} - ${moment(endDate).format('DD/MM/YYYY')}`;
        } else if (startDate) {
            startTimeFilter = moment(startDate).format('DD/MM/YYYY');
        }

        setAppliedFilters({
            serviceProvider: filterInputs.serviceProvider.trim(),
            tripCode: filterInputs.tripCode.trim(),
            startTime: startTimeFilter, // Sửa tên
            vehicleIds: selectedVehicles.map(v => v.value),
            driverIds: selectedDrivers.map(d => d.value),
            tripIds: selectedTrips.map(t => t.value),
            statusIds: selectedStatuses.map(s => s.value),
            // Không có serviceTypes trong DTParameters
        });
        setShowFilter(false);
    };

    const handleResetFilter = () => {
        setFilterInputs({ serviceProvider: '', tripCode: '' });
        setDateRange([null, null]);
        setSelectedVehicles([]);
        setSelectedDrivers([]);
        setSelectedTrips([]);
        setSelectedStatuses([]);
        setAppliedFilters({});
    };

    return (
        <div className="container-fluid pt-4 px-4">
            <button type="button" className="btn btn-outline-primary m-2" onClick={toggleFilter}>
                <i className="fa fa-filter me-2"></i>Bộ lọc
            </button>
            <div className={`col-sm-12 col-xl-12 filter-box ${showFilter ? 'show' : 'hide'}`}>
                <div className="bg-light rounded h-100 p-4">
                    <h6 className="mb-4">Tuỳ chọn bộ lọc</h6>
                    <div className="row">
                        {/* Hàng 1: Inputs */}
                        <div className="col-xl-4 mb-3">
                            <label className="form-label">Nơi bảo dưỡng:</label>
                            <input
                                type="text"
                                className="form-control"
                                placeholder="Nơi bảo dưỡng"
                                value={filterInputs.serviceProvider}
                                onChange={(e) => setFilterInputs({ ...filterInputs, serviceProvider: e.target.value })}
                            />
                        </div>
                        <div className="col-xl-4 mb-3">
                            <label className="form-label">Mã chuyến đi:</label>
                            <input
                                type="text"
                                className="form-control"
                                placeholder="Mã chuyến đi"
                                value={filterInputs.tripCode}
                                onChange={(e) => setFilterInputs({ ...filterInputs, tripCode: e.target.value })}
                            />
                        </div>
                        <div className="col-xl-4 mb-3">
                            <label className="form-label">Ngày bắt đầu</label>
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
                        {/* Hàng 2: Selects */}
                        <div className="col-xl-4 mb-3">
                            <label className="form-label">Xe:</label>
                            <Select isMulti options={vehicleList} value={selectedVehicles} onChange={setSelectedVehicles} placeholder="Chọn xe" />
                        </div>
                        <div className="col-xl-4 mb-3">
                            <label className="form-label">Lái xe:</label>
                            <Select isMulti options={driverList} value={selectedDrivers} onChange={setSelectedDrivers} placeholder="Chọn lái xe" />
                        </div>
                        <div className="col-xl-4 mb-3">
                            <label className="form-label">Chuyến đi:</label>
                            <Select isMulti options={tripList} value={selectedTrips} onChange={setSelectedTrips} placeholder="Chọn chuyến đi" />
                        </div>
                        {/* Hàng 3: Selects */}
                        <div className="col-xl-4 mb-3">
                            <label className="form-label">Trạng thái:</label>
                            <Select isMulti options={statusList} value={selectedStatuses} onChange={setSelectedStatuses} placeholder="Chọn trạng thái" />
                        </div>
                    </div>
                    <div className="d-flex justify-content-end">
                        <button type="submit" className="btn btn-primary me-3" onClick={handleApplyFilter}>
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
                        <h6 className="mb-0">Hồ sơ Bảo dưỡng</h6>
                        <button type="button" className="btn btn-primary" onClick={handleAdd}>
                            <i className="fa fa-plus me-2"></i>Thêm mới
                        </button>
                    </div>
                    <MaintenanceRecordTable
                        apiUrl={apiUrl}
                        token={token}
                        onEdit={handleEdit}
                        onApprove={handleApprove}
                        onReject={handleReject}
                        refreshFlag={refreshFlag}
                        filters={appliedFilters}
                        onDelete={handleDelete}                    
                        />
                </div>
            </div>

            {/* Popup nhập liệu */}
            {showPopup && (
                <MaintenanceRecordFormPopup
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
                    // Truyền danh sách đã fetch cho form
                    vehicleList={vehicleList}
                    tripList={tripList}
                    serviceTypeList={serviceTypeList}
                />
            )}

            {/* Popup từ chối */}
            {showRejectPopup && (
                <RejectFormPopup
                    item={rejectingItem}
                    onClose={() => setShowRejectPopup(false)}
                    apiUrl={apiUrl}
                    token={token}
                    onSuccess={() => {
                        reloadTable();
                        setShowRejectPopup(false);
                    }}
                    showConfirmModal={(message, action) => showConfirmModal(message, action, () => setShowRejectPopup(true))}
                    showNotifyModal={showNotifyModal}
                />
            )}

            {/* Modal xác nhận chung */}
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

            {/* Modal thông báo */}
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