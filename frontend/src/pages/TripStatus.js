import { useState, useRef } from 'react';
import TripStatusTable from '../Table/TripStatusTable';
import '../styles/css/TripStatus.css';
import TripStatusFormPopup from '../Table/TripStatusFormPopup';
import ConfirmModal from '../Table/ConfirmModal';
import DatePicker from 'react-datepicker'; // Thêm thư viện react-datepicker
import 'react-datepicker/dist/react-datepicker.css'; // Thêm CSS cho datepicker
import { API_URL } from '~/api/api';
import moment from 'moment';

export default function TripStatus({ permissionFlags }) {
    const [showFilter, setShowFilter] = useState(false);
    const toggleFilter = () => setShowFilter(!showFilter);

    const apiUrl = `${API_URL}/TripStatus`; // URL API của bạn
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
    const [confirmAction, setConfirmAction] = useState(() => {});

    const [notifyMessage, setNotifyMessage] = useState('');
    const [showNotify, setShowNotify] = useState(false);
    const [notifySuccess, setNotifySuccess] = useState(true);

    // State cho khoảng ngày được chọn
    const [dateRange, setDateRange] = useState([null, null]);
    const [startDate, endDate] = dateRange;

    // State cho các giá trị lọc đã được "Áp dụng" và sẽ được truyền xuống bảng
    const [appliedFilters, setAppliedFilters] = useState({});

    // State cho các giá trị đang được nhập trong form lọc
    const [filterInputs, setFilterInputs] = useState({ name: '' });

    const tableRef = useRef();

    const handleAdd = () => {
        setEditingItem(null);
        setShowPopup(true);
    };

    const handleEdit = (item) => {
        setEditingItem(item);
        setShowPopup(true);
    };

    // gọi confirm modal từ popup
    const showConfirmModal = (message, action, onCancel) => {
        setConfirmMessage(message);
        setConfirmAction(() => action);
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
        setFilterInputs({ name: '' });
        setDateRange([null, null]);

        // Reset bộ lọc đã áp dụng để bảng fetch lại dữ liệu gốc
        setAppliedFilters({});
    };

    // Hàm gọi lại fetchData ở bảng
    const reloadTable = () => setRefreshFlag((prev) => !prev);

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
                        <h6 className="mb-4">Trạng thái chuyến</h6>
                        <button type="button" className="btn btn-primary" id="btn_add_user_status" onClick={handleAdd}>
                            <i className="fa fa-plus me-2"></i>Thêm mới
                        </button>
                    </div>
                    <TripStatusTable
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
                <TripStatusFormPopup
                    item={editingItem}
                    onClose={() => setShowPopup(false)} // dùng showPopup
                    apiUrl={apiUrl}
                    token={token}
                    onSuccess={() => {
                        reloadTable(); // reload bảng
                        setShowPopup(false);
                    }}
                    showConfirmModal={(message, action) => showConfirmModal(message, action, () => setShowPopup(true))}
                    showNotifyModal={showNotifyModal}
                />
            )}
            {showConfirm && (
                <ConfirmModal
                    message={confirmMessage}
                    onClose={handleCancelConfirm} // khi bấm Hủy
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
                    onlyClose={true} // chỉ hiện nút OK
                    success={notifySuccess}
                />
            )}
        </div>
    );
}
