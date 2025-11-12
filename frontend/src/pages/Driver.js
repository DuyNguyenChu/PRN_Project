// Driver.js
import { useState, useRef, useEffect } from 'react';
import axios from 'axios';
import '../styles/css/Driver.css';
import DriverTable from '../Table/DriverTable'; // Sửa
import DriverFormPopup from '../Table/DriverFormPopup'; // Sửa
import ConfirmModal from '../Table/ConfirmModal';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import { API_URL } from '~/api/api';
import moment from 'moment';
import { canView } from '~/utils/permissionUtils';
import { useNavigate } from 'react-router-dom';
import { PERMISSION_IDS } from '~/utils/menuIdForPermission'; // Sửa: Dùng file constants

// THÊM: Hardcode Hạng bằng lái xe (theo C# CommonConstants)
const LICENSE_CLASSES = [
    { id: 'A1', name: 'A1' },
    { id: 'B1', name: 'B1' },
    { id: 'B2', name: 'B2' },
    { id: 'C', name: 'C' },
    { id: 'D', name: 'D' },
    { id: 'E', name: 'E' },
    { id: 'F', name: 'F' },
];

export default function Driver() {
    const [showFilter, setShowFilter] = useState(false);
    const toggleFilter = () => setShowFilter(!showFilter);

    const apiUrl = `${API_URL}/Driver`; // Sửa: URL API
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
    const [confirmAction, setConfirmAction] = useState(() => {}); // Đổi tên: confirmUser -> confirmAction

    const [notifyMessage, setNotifyMessage] = useState('');
    const [showNotify, setShowNotify] = useState(false);
    const [notifySuccess, setNotifySuccess] = useState(true);

    // --- State cho Bộ lọc ---
    const [dateRange, setDateRange] = useState([null, null]);
    const [startDate, endDate] = dateRange;
    const [appliedFilters, setAppliedFilters] = useState({});
    const [filterInputs, setFilterInputs] = useState({
        experienceYears: '', // Sẽ là chuỗi "1,2,3"
        licenseClasses: [], // Mảng string
        driverStatusIds: [], // Mảng number
    });
    const [statusOptions, setStatusOptions] = useState([]);
    const [loadingFilters, setLoadingFilters] = useState(true);
    // --- Kết thúc State Bộ lọc ---

    const tableRef = useRef();
    const navigate = useNavigate();
    const [isAccessChecked, setIsAccessChecked] = useState(false);
    const [isAllowedToView, setIsAllowedToView] = useState(false);

    useEffect(() => {
        // SỬA: Dùng ID của trang Driver
        if (!canView(PERMISSION_IDS.DRIVER_LIST)) { // Giả sử bạn có ID này
            console.warn(`Người dùng không có quyền xem trang (ID: ${PERMISSION_IDS.DRIVER_LIST}). Đang chuyển hướng...`);
            setIsAllowedToView(false);
            navigate('/error');
        } else {
            setIsAllowedToView(true);
        }
        setIsAccessChecked(true);
    }, [navigate]);

    // Fetch dữ liệu cho bộ lọc (DriverStatus)
    useEffect(() => {
        const fetchFilterData = async () => {
            if (!token) return;
            setLoadingFilters(true);
            try {
                const statusRes = await axios.get(`${API_URL}/DriverStatus`, { headers: { Authorization: `Bearer ${token}` } });
                setStatusOptions(statusRes.data.resources || []);
            } catch (err) {
                console.error('Lỗi tải dữ liệu filter:', err);
            } finally {
                setLoadingFilters(false);
            }
        };
        fetchFilterData();
    }, [token]);

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
        setConfirmAction(() => action); // Sửa
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
    const handleFilterInputChange = (e) => {
        const { name, value } = e.target;
        setFilterInputs((prev) => ({ ...prev, [name]: value }));
    };

    const handleMultiSelectChange = (e) => {
        const { name } = e.target;
        // Xử lý riêng cho licenseClasses (mảng string)
        if (name === 'licenseClasses') {
            const selectedStrings = Array.from(e.target.selectedOptions, (option) => option.value);
            setFilterInputs((prev) => ({ ...prev, [name]: selectedStrings }));
        } else {
            // Xử lý cho driverStatusIds (mảng number)
            const selectedIds = Array.from(e.target.selectedOptions, (option) => Number(option.value));
            setFilterInputs((prev) => ({ ...prev, [name]: selectedIds }));
        }
    };

    const handleApplyFilter = () => {
        const [startDate, endDate] = dateRange;
        const newFilters = {
            // Chuyển đổi chuỗi experienceYears thành mảng số
            experienceYears: filterInputs.experienceYears
                .split(',')
                .map(Number)
                .filter(n => n > 0 && Number.isInteger(n)), // Lọc số nguyên dương
            
            licenseClasses: filterInputs.licenseClasses,
            driverStatusIds: filterInputs.driverStatusIds,
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
        setFilterInputs({
            experienceYears: '',
            licenseClasses: [],
            driverStatusIds: [],
        });
        setDateRange([null, null]);
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
                                    <label htmlFor="filter_experienceYears" className="form-label">
                                        Thâm niên (năm)
                                    </label>
                                    <input
                                        type="text"
                                        className="form-control"
                                        id="filter_experienceYears"
                                        name="experienceYears"
                                        value={filterInputs.experienceYears}
                                        onChange={handleFilterInputChange}
                                        placeholder="Nhập số, cách nhau bởi dấu phẩy (vd: 1,3,5)"
                                    />
                                </div>
                                <div className="col-md-4 mb-3">
                                    <label htmlFor="filter_licenseClasses" className="form-label">
                                        Hạng bằng lái
                                    </label>
                                    <select
                                        multiple
                                        className="form-select"
                                        id="filter_licenseClasses"
                                        name="licenseClasses"
                                        value={filterInputs.licenseClasses}
                                        onChange={handleMultiSelectChange}
                                        style={{ height: '150px' }}
                                    >
                                        {LICENSE_CLASSES.map((cls) => (
                                            <option key={cls.id} value={cls.id}>
                                                {cls.name}
                                            </option>
                                        ))}
                                    </select>
                                </div>
                                <div className="col-md-4 mb-3">
                                    <label htmlFor="filter_driverStatusIds" className="form-label">
                                        Trạng thái
                                    </label>
                                    <select
                                        multiple
                                        className="form-select"
                                        id="filter_driverStatusIds"
                                        name="driverStatusIds"
                                        value={filterInputs.driverStatusIds}
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
                            </div>
                            <div className="row">
                                <div className="col-xl-12 mb-3">
                                    <label htmlFor="filter_created_date" className="form-label">
                                        Ngày tạo
                                    </label>
                                    <DatePicker
                                        selectsRange
                                        startDate={startDate}
                                        endDate={endDate}
                                        onChange={(update) => setDateRange(update)}
                                        isClearable={true}
                                        className="form-control"
                                        placeholderText="Chọn khoảng ngày"
                                        dateFormat="dd/MM/yyyy"
                                    />
                                </div>
                            </div>
                        </>
                    )}
                    <div className="d-flex justify-content-end">
                        <button
                            type="submit"
                            className="btn btn-primary me-3"
                            onClick={handleApplyFilter}
                            disabled={loadingFilters}
                        >
                            Áp dụng
                        </button>
                        <button
                            type="reset"
                            className="btn btn-outline-primary"
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
                        <h6 className="mb-4">Danh sách tài xế</h6> {/* Sửa */}
                        <button type="button" className="btn btn-primary" onClick={handleAdd}>
                            <i className="fa fa-plus me-2"></i>Thêm mới
                        </button>
                    </div>
                    <DriverTable // Sửa
                        apiUrl={apiUrl}
                        token={token}
                        onEdit={handleEdit}
                        refreshFlag={refreshFlag}
                        filters={appliedFilters}
                    />
                </div>
            </div>
            
            {showPopup && (
                <DriverFormPopup // Sửa
                    item={editingItem}
                    onClose={() => setShowPopup(false)}
                    apiUrl={apiUrl}
                    token={token}
                    onSuccess={() => {
                        reloadTable();
                        setShowPopup(false);
                    }}
                    showConfirmModal={showConfirmModal} // Sửa
                    showNotifyModal={showNotifyModal}
                />
            )}
            
            {showConfirm && (
                <ConfirmModal
                    message={confirmMessage}
                    onClose={handleCancelConfirm}
                    onConfirm={() => {
                        confirmAction(); // Sửa
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