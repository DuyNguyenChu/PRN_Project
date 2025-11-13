import { useState, useRef, useEffect } from 'react';
import FuelLogTable from '../Table/FuelLogTable'; // Thay đổi
import FuelLogFormPopup from '../Table/FuelLogFormPopup'; // Thay đổi
import RejectFormPopup from '../Table/RejectFormPopup'; // Component mới
import ConfirmModal from '../Table/ConfirmModal';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import Select from 'react-select'; // Thêm react-select cho bộ lọc
import { API_URL } from '~/api/api';
import moment from 'moment';
import axios from 'axios'; // Thêm axios
import { canView } from '~/utils/permissionUtils'; // Bỏ canCreate, canUpdate, canDelete nếu không dùng
import { useNavigate } from 'react-router-dom';
import { PERMISSION_IDS } from '~/utils/menuIdForPermission';

export default function FuelLog({ permissionFlags }) {
    const [showFilter, setShowFilter] = useState(false);
    const toggleFilter = () => setShowFilter(!showFilter);
    const apiUrl = `${API_URL}/FuelLog`; // URL API của FuelLog
    const userDataString = localStorage.getItem('userData');
    const userData = JSON.parse(userDataString);
    if (!userData || !userData.resources || !userData.resources.accessToken) {
        throw new Error('Không tìm thấy token người dùng. Vui lòng đăng nhập lại.');
    }
    const token = userData.resources.accessToken;

    // State cho Popups
    const [showPopup, setShowPopup] = useState(false);
    const [showRejectPopup, setShowRejectPopup] = useState(false); // Popup từ chối
    const [editingItem, setEditingItem] = useState(null);
    const [rejectingItem, setRejectingItem] = useState(null); // Item bị từ chối
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
    // [THAY ĐỔI] Đã xóa driverList
    // [THAY ĐỔI] Đã xóa tripList
    const [fuelTypeList, setFuelTypeList] = useState([]);
    const [statusList, setStatusList] = useState([]);

    // State cho giá trị input bộ lọc
    const [dateRange, setDateRange] = useState([null, null]);
    const [startDate, endDate] = dateRange;
    // [THAY ĐỔI] Thêm driverName vào filterInputs
    const [filterInputs, setFilterInputs] = useState({ gasStation: '', tripCode: '', driverName: '' });
    const [selectedVehicles, setSelectedVehicles] = useState([]);
    // [THAY ĐỔI] Đã xóa selectedDrivers
    // [THAY ĐỔI] Đã xóa selectedTrips
    const [selectedFuelTypes, setSelectedFuelTypes] = useState([]);
    const [selectedStatuses, setSelectedStatuses] = useState([]);

    const navigate = useNavigate();
    const [isAccessChecked, setIsAccessChecked] = useState(false);
    const [isAllowedToView, setIsAllowedToView] = useState(false);

    useEffect(() => {
        if (!canView(PERMISSION_IDS.FUEL_LOG)) {
            console.warn(`Người dùng không có quyền xem trang (ID: ${PERMISSION_IDS.FUEL_LOG}). Đang chuyển hướng...`);
            setIsAllowedToView(false);
            navigate('/error');
        } else {
            setIsAllowedToView(true);
        }
        setIsAccessChecked(true);
    }, [navigate]);

    // State cho bộ lọc đã "Áp dụng"
    const [appliedFilters, setAppliedFilters] = useState({});

    // Fetch dữ liệu cho các dropdown bộ lọc
    useEffect(() => {
        const headers = { Authorization: `Bearer ${token}` };

        // GIẢ SỬ BẠN CÓ CÁC API ENDPOINT NÀY ĐỂ LẤY DANH SÁCH
        // THAY ĐỔI: Đã xóa /all
        axios
            .get(`${API_URL}/Vehicle`, { headers })
            .then((res) =>
                // Vui lòng kiểm tra lại cấu trúc trả về.
                // Tôi đang giả định nó trả về { resources: [...] } giống như các API khác.
                setVehicleList(
                    res.data.resources.map((v) => ({
                        value: v.id,
                        label: `[${v.registrationNumber}] ${v.vehicleModelName}`,
                    })),
                ),
            )
            .catch((err) => console.error('Lỗi tải Vehicle:', err));

        // [THAY ĐỔI] Đã xóa fetch cho Driver

        // [THAY ĐỔI] Đã xóa fetch cho Trip

        // Các API này đúng theo FuelLogController.cs
        axios
            .get(`${apiUrl}/fuel-types`, { headers })
            .then((res) => setFuelTypeList(res.data.resources.map((f) => ({ value: f.id, label: f.name }))))
            .catch((err) => console.error('Lỗi tải FuelTypes:', err));

        axios
            .get(`${apiUrl}/status`, { headers })
            .then((res) => setStatusList(res.data.resources.map((s) => ({ value: s.id, label: s.name }))))
            .catch((err) => console.error('Lỗi tải Status:', err));
    }, [token, apiUrl]);

    // Handlers cho các hành động
    const handleAdd = () => {
        setEditingItem(null);
        setShowPopup(true);
    };

    const handleEdit = (item) => {
        setEditingItem(item);
        setShowPopup(true);
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

    // Handlers cho các hành động CRUD từ Table
    const handleDelete = (id) => {
        showConfirmModal('Bạn có chắc chắn muốn xóa nhật ký này?', async () => {
            try {
                await axios.delete(`${apiUrl}/${id}`, { headers: { Authorization: `Bearer ${token}` } });
                showNotifyModal('Xóa thành công!');
                reloadTable();
            } catch (err) {
                showNotifyModal('Xóa thất bại: ' + (err.response?.data?.message || err.message), false);
            }
        });
    };

    const handleApprove = (id) => {
        showConfirmModal('Bạn có chắc chắn muốn duyệt nhật ký này?', async () => {
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
        let createdDateFilter = null;
        if (startDate && endDate) {
            createdDateFilter = `${moment(startDate).format('DD/MM/YYYY')} - ${moment(endDate).format('DD/MM/YYYY')}`;
        } else if (startDate) {
            createdDateFilter = moment(startDate).format('DD/MM/YYYY');
        }

        // [THAY ĐỔI] Cập nhật setAppliedFilters
        setAppliedFilters({
            gasStation: filterInputs.gasStation.trim(),
            tripCode: filterInputs.tripCode.trim(),
            driverName: filterInputs.driverName.trim(), // Thêm driverName
            createdDate: createdDateFilter,
            vehicleIds: selectedVehicles.map((v) => v.value),
            // driverIds: Đã xóa
            // tripIds: Đã xóa
            fuelTypes: selectedFuelTypes.map((f) => f.value),
            statusIds: selectedStatuses.map((s) => s.value),
        });
        setShowFilter(false);
    };

    const handleResetFilter = () => {
        // [THAY ĐỔI] Cập nhật reset filterInputs
        setFilterInputs({ gasStation: '', tripCode: '', driverName: '' });
        setDateRange([null, null]);
        setSelectedVehicles([]);
        // [THAY ĐỔI] Đã xóa setSelectedDrivers
        // [THAY ĐỔI] Đã xóa setSelectedTrips
        setSelectedFuelTypes([]);
        setSelectedStatuses([]);
        setAppliedFilters({});
    };

    return (
        <div className="container-fluid pt-4 px-4">
            <button type="button" className="btn btn-outline-primary m-2" id="btn_fuellog_filter" onClick={toggleFilter}>
                <i className="fa fa-filter me-2"></i>Bộ lọc
            </button>
            <div className={`col-sm-12 col-xl-12 filter-box ${showFilter ? 'show' : 'hide'}`}>
                <div className="bg-light rounded h-100 p-4">
                    <h6 className="mb-4">Tuỳ chọn bộ lọc</h6>
                    <div className="row">
                        {/* Hàng 1: Inputs */}
                        <div className="col-xl-4 mb-3">
                            <label htmlFor="filter_gasStation" className="form-label">
                                Trạm xăng:
                            </label>
                            <input
                                type="text"
                                className="form-control"
                                id="filter_gasStation"
                                placeholder="Trạm xăng"
                                value={filterInputs.gasStation}
                                onChange={(e) => setFilterInputs({ ...filterInputs, gasStation: e.target.value })}
                            />
                        </div>
                        <div className="col-xl-4 mb-3">
                            <label htmlFor="filter_tripCode" className="form-label">
                                Mã chuyến đi:
                            </label>
                            <input
                                type="text"
                                className="form-control"
                                id="filter_tripCode"
                                placeholder="Mã chuyến đi"
                                value={filterInputs.tripCode}
                                onChange={(e) => setFilterInputs({ ...filterInputs, tripCode: e.target.value })}
                            />
                        </div>
                        <div className="col-xl-4 mb-3">
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
                        {/* Hàng 2: Selects & Inputs */}
                        <div className="col-xl-4 mb-3">
                            <label className="form-label">Xe:</label>
                            <Select
                                isMulti
                                options={vehicleList}
                                value={selectedVehicles}
                                onChange={setSelectedVehicles}
                                placeholder="Chọn xe"
                            />
                        </div>

                        {/* [THAY ĐỔI] Thay thế Select Lái xe bằng Input */}
                        <div className="col-xl-4 mb-3">
                            <label htmlFor="filter_driverName" className="form-label">
                                Lái xe:
                            </label>
                            <input
                                type="text"
                                className="form-control"
                                id="filter_driverName"
                                placeholder="Tên lái xe"
                                value={filterInputs.driverName}
                                onChange={(e) => setFilterInputs({ ...filterInputs, driverName: e.target.value })}
                            />
                        </div>

                        {/* [THAY ĐỔI] Đã xóa Select Chuyến đi */}

                        {/* Hàng 3: Selects */}
                        <div className="col-xl-4 mb-3">
                            <label className="form-label">Loại nhiên liệu:</label>
                            <Select
                                isMulti
                                options={fuelTypeList}
                                value={selectedFuelTypes}
                                onChange={setSelectedFuelTypes}
                                placeholder="Chọn loại nhiên liệu"
                            />
                        </div>
                        <div className="col-xl-4 mb-3">
                            <label className="form-label">Trạng thái:</label>
                            <Select
                                isMulti
                                options={statusList}
                                value={selectedStatuses}
                                onChange={setSelectedStatuses}
                                placeholder="Chọn trạng thái"
                            />
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
                        <h6 className="mb-0">Nhật ký nhiên liệu</h6> {/* Sửa tiêu đề */}
                        <button type="button" className="btn btn-primary" id="btn_add_fuellog" onClick={handleAdd}>
                            <i className="fa fa-plus me-2"></i>Thêm mới
                        </button>
                    </div>
                    <FuelLogTable
                        apiUrl={apiUrl}
                        token={token}
                        onEdit={handleEdit}
                        onDelete={handleDelete}
                        onApprove={handleApprove}
                        onReject={handleReject}
                        refreshFlag={refreshFlag}
                        filters={appliedFilters}
                    />
                </div>
            </div>

            {/* Popup nhập liệu */}
            {showPopup && (
                <FuelLogFormPopup
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
                    // [THAY ĐỔI] Đã xóa tripList
                    fuelTypeList={fuelTypeList}
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