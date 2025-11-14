// TripTable.js
import { useEffect, useState, useCallback, useRef } from 'react';
import moment from 'moment';
import axios from 'axios';

// Giữ nguyên hàm getContrastColor
function getContrastColor(hexColor) {
    // ... (code giữ nguyên)
}

// SỬA: Thay 'onDelete' bằng các props phân quyền và hành động
export default function TripTable({ 
    apiUrl, token, onEdit, refreshFlag, filters,
    isDispatcherRole, isDriverRole, isUserRole,
    onAccept, onReject, onArriving, onPickedUp, onEnRoute, onArrived, onComplete, onCancel,
    tripStatusConstants // Nhận các hằng số ID
}) {
    const [data, setData] = useState([]);
    const [totalRecords, setTotalRecords] = useState(0);
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(10);
    const [search, setSearch] = useState('');
    const [sortField, setSortField] = useState('CreatedDate');
    const [sortDir, setSortDir] = useState('desc');
    const [loading, setLoading] = useState(false);

    // XÓA: State modal Xóa (showConfirm, showResult, ...)
    
    const isInitialMount = useRef(true);

    useEffect(() => {
        if (isInitialMount.current) {
            isInitialMount.current = false;
        } else {
            setPage(1);
        }
    }, [filters]);

    // Fetch dữ liệu (Giữ nguyên logic fetch)
    const fetchData = useCallback(async () => {
        setLoading(true);
        try {
            // Sửa: Cập nhật mảng columns
            const requestBody = {
                draw: page,
                columns: [
                    { data: 'Id', name: '', searchable: true, orderable: true, search: { value: '', regex: false } },
                    { data: 'RequesterName', name: '', searchable: true, orderable: true, search: { value: '', regex: false }, },
                    { data: 'DriverName', name: '', searchable: true, orderable: true, search: { value: '', regex: false }, },
                    { data: 'VehicleModelName', name: '', searchable: true, orderable: true, search: { value: '', regex: false }, },
                    { data: 'FromLocation', name: '', searchable: true, orderable: true, search: { value: '', regex: false }, }, 
                    { data: 'TripStatusName', name: '', searchable: true, orderable: true, search: { value: '', regex: false }, },
                    { data: 'ActualStartTime', name: '', searchable: true, orderable: true, search: { value: '', regex: false }, },
                    { data: 'ActualEndTime', name: '', searchable: true, orderable: true, search: { value: '', regex: false }, },
                    { data: 'CreatedDate', name: '', searchable: true, orderable: true, search: { value: '', regex: false }, },
                    { data: 'Id', name: '', searchable: false, orderable: false, search: { value: '', regex: false } }, // Thao tác
                ],
                order: [
                    {
                        column:
                            sortField === 'RequesterName' ? 1 : 
                            sortField === 'DriverName' ? 2 : 
                            sortField === 'VehicleModelName' ? 3 : 
                            sortField === 'FromLocation' ? 4 : 
                            sortField === 'TripStatusName' ? 5 : 
                            sortField === 'ActualStartTime' ? 6 : 
                            sortField === 'ActualEndTime' ? 7 : 
                            sortField === 'CreatedDate' ? 8 : 0, 
                        dir: sortDir,
                        name: '',
                    },
                ],
                start: (page - 1) * pageSize,
                length: pageSize,
                search: { value: search, regex: false },
                additionalValues: [],
                requesterIds: filters?.requesterIds || [],
                vehicleIds: filters?.vehicleIds || [],
                driverIds: filters?.driverIds || [],
                tripStatusIds: filters?.tripStatusIds || [],
            };
            
            if (filters) {
                if (filters.createdDate) {
                    requestBody.columns[8].search.value = filters.createdDate; 
                }
            }

            const res = await axios.post(`${apiUrl}/paged-advanced`, requestBody, {
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`,
                },
            });

            const json = res.data;
            if (json?.resources) {
                setData(json.resources.data);
                setTotalRecords(json.resources.recordsFiltered);
            } else {
                setData([]);
                setTotalRecords(0);
            }
        } catch (err) {
            console.error('❌ Lỗi tải dữ liệu:', err);
        } finally {
            setLoading(false);
        }
    }, [apiUrl, token, page, pageSize, search, sortField, sortDir, filters]);


    useEffect(() => {
        fetchData();
    }, [fetchData, refreshFlag]);

    // Giữ nguyên handleSort
    const handleSort = (field) => {
        // ... (code giữ nguyên)
    };

    // XÓA: Toàn bộ logic Xóa (handleDeleteClick, confirmDelete, closeConfirm, closeResult)

    // SỬA: Hàm render nút hành động cho Lái xe
    const renderDriverActions = (row) => {
        // Phải dùng 'tripStatusId' (chữ 't' thường) vì nó đến từ API paged-advanced
        const status = row.tripStatusId; 
        
        switch (status) {
            case tripStatusConstants.ASSIGNED: // 2: Đã khởi hành
                return (
                    <>
                        <button className="btn btn-sm btn-success me-2" onClick={() => onAccept(row)}>Nhận</button>
                        <button className="btn btn-sm btn-warning" onClick={() => onReject(row)}>Từ chối</button>
                    </>
                );
            case tripStatusConstants.ACCEPTED: // 3: Lái xe đã nhận
                return (
                    <>
                        <button className="btn btn-sm btn-info me-2" onClick={() => onArriving(row)}>Đang đến đón</button>
                        <button className="btn btn-sm btn-secondary" onClick={() => onCancel(row)}>Hủy</button>
                    </>
                );
            case tripStatusConstants.ARRIVING: // 4: Lái xe đang đi đến đón
                return (
                    <>
                        <button className="btn btn-sm btn-primary me-2" onClick={() => onPickedUp(row)}>Đã đến đón</button>
                        <button className="btn btn-sm btn-secondary" onClick={() => onCancel(row)}>Hủy</button>
                    </>
                );
            case tripStatusConstants.PICKED_UP: // 5: Đã đến đón
                return (
                    <>
                        <button className="btn btn-sm btn-info me-2" onClick={() => onEnRoute(row)}>Đang đến đích</button>
                        <button className="btn btn-sm btn-secondary" onClick={() => onCancel(row)}>Hủy</button>
                    </>
                );
            case tripStatusConstants.EN_ROUTE: // 6: Đang đi đến đích
                return (
                    <>
                        <button className="btn btn-sm btn-primary me-2" onClick={() => onArrived(row)}>Đã đến đích</button>
                        <button className="btn btn-sm btn-secondary" onClick={() => onCancel(row)}>Hủy</button>
                    </>
                );
            case tripStatusConstants.ARRIVED: // 7: Đã đến đích
                return (
                    <>
                        <button className="btn btn-sm btn-success me-2" onClick={() => onComplete(row)}>Hoàn thành</button>
                        <button className="btn btn-sm btn-secondary" onClick={() => onCancel(row)}>Hủy</button>
                    </>
                );
            case tripStatusConstants.COMPLETED: // 8: Hoàn thành
            case tripStatusConstants.CANCELLED: // 9: Đã hủy
            case tripStatusConstants.REJECTED: // 10: Đã từ chối
                return null; // Không có hành động
            default:
                return null;
        }
    };


    return (
        <div>
            {/* Thanh tìm kiếm (Giữ nguyên) */}
            <div className="d-flex justify-content-between align-items-center mb-3">
                {/* ... (code giữ nguyên) ... */}
            </div>

            <div className="table-responsive">
                <table className="table table-hover">
                    <thead>
                        {/* Tiêu đề bảng (Giữ nguyên) */}
                        <tr className="text-uppercase text-gray-500 fs-7">
                            <th style={{ width: '5%' }}>STT</th>
                            <th onClick={() => handleSort('RequesterName')}>Người yêu cầu</th>
                            <th onClick={() => handleSort('DriverName')}>Tài xế</th>
                            <th onClick={() => handleSort('VehicleModelName')}>Xe</th>
                            <th onClick={() => handleSort('FromLocation')}>Lộ trình</th>
                            <th onClick={() => handleSort('TripStatusName')}>Trạng thái</th>
                            <th onClick={() => handleSort('ActualStartTime')}>Bắt đầu</th>
                            <th onClick={() => handleSort('CreatedDate')}>Ngày tạo</th>
                            <th className="text-end">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody>
                        {loading ? (
                            <tr><td colSpan={9} className="text-center">Đang tải...</td></tr>
                        ) : data.length === 0 ? (
                            <tr><td colSpan={9} className="text-center">Không có dữ liệu</td></tr>
                        ) : (
                            data.map((row, index) => (
                                <tr key={row.Id || row.id}>
                                    <td>{(page - 1) * pageSize + index + 1}</td>
                                    <td>{row.requesterName}</td>
                                    <td>{row.driverName}</td>
                                    <td>{`${row.vehicleBrandName} ${row.vehicleModelName}`}</td>
                                    <td>
                                        Từ: {row.fromLocation} <br /> <span className="text-muted">Đến: {row.toLocation}</span>
                                    </td>
                                    <td>
                                        <span className="badge"
                                            style={{
                                                backgroundColor: row.tripStatusColor,
                                                color: getContrastColor(row.tripStatusColor),
                                            }}>
                                            {row.tripStatusName}
                                        </span>
                                    </td>
                                    <td>
                                        {row.actualStartTime
                                            ? moment(row.ActualStartTime).format('DD/MM/YYYY HH:mm')
                                            : 'Chưa bắt đầu'}
                                    </td>
                                    <td>{moment(row.CreatedDate).format('DD/MM/YYYY HH:mm')}</td>
                                    
                                    {/* SỬA: Cột Thao tác dựa trên Role */}
                                    <td className="text-end">
                                        {isDispatcherRole && onEdit && 
                                            (row.tripStatusId === tripStatusConstants.ACCEPTED || 
                                             row.tripStatusId === tripStatusConstants.ARRIVING ||
                                            row.tripStatusId === tripStatusConstants.ASSIGNED) && (
                                            <button className="btn btn-sm btn-primary me-2" onClick={() => onEdit(row)}>
                                                Sửa
                                            </button>
                                        )}
                                        {isDriverRole && renderDriverActions(row)}
                                        {/* Người dùng (isUserRole) không thấy nút nào */}
                                    </td>
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>

            {/* XÓA: Modal xác nhận và Modal kết quả (đã bị xóa) */}
            
            {/* Phân trang (Giữ nguyên) */}
            <div className="d-flex justify-content-between align-items-center mt-3">
                {/* chọn số bản ghi mỗi trang */}
                <select
                    className="form-select w-auto"
                    value={pageSize}
                    onChange={(e) => {
                        setPageSize(Number(e.target.value));
                        setPage(1); // reset về trang đầu
                    }}
                >
                    {[5, 10, 20, 50].map((n) => (
                        <option key={n} value={n}>
                            {n} / trang
                        </option>
                    ))}
                </select>

                {/* nhóm nút phân trang */}
                <div className="btn-group" role="group" aria-label="Pagination buttons">
                    {/* Trang đầu */}
                    <button className="btn btn-outline-primary" disabled={page === 1} onClick={() => setPage(1)}>
                        «
                    </button>

                    {/* Trang trước */}
                    <button
                        className="btn btn-outline-primary"
                        disabled={page === 1}
                        onClick={() => setPage((p) => p - 1)}
                    >
                        ‹
                    </button>

                    {/* Số trang hiển thị giới hạn */}
                    {(() => {
                        const totalPages = Math.ceil(totalRecords / pageSize);
                        const maxVisible = 5; // số nút tối đa hiển thị
                        let start = Math.max(1, page - Math.floor(maxVisible / 2));
                        let end = Math.min(totalPages, start + maxVisible - 1);

                        // đảm bảo hiển thị 5 nút khi gần cuối danh sách
                        if (end - start < maxVisible - 1) {
                            start = Math.max(1, end - maxVisible + 1);
                        }

                        const buttons = [];

                        // Nút đầu tiên "1 ..."
                        if (start > 1) {
                            buttons.push(
                                <button key={1} className="btn btn-outline-primary" onClick={() => setPage(1)}>
                                    1
                                </button>,
                            );
                            if (start > 2)
                                buttons.push(
                                    <button key="start-ellipsis" className="btn btn-light" disabled>
                                        ...
                                    </button>,
                                );
                        }

                        // Các nút giữa
                        for (let i = start; i <= end; i++) {
                            buttons.push(
                                <button
                                    key={i}
                                    className={`btn ${i === page ? 'btn-primary' : 'btn-outline-primary'}`}
                                    onClick={() => setPage(i)}
                                >
                                    {i}
                                </button>,
                            );
                        }

                        // Nút cuối "... N"
                        if (end < totalPages) {
                            if (end < totalPages - 1)
                                buttons.push(
                                    <button key="end-ellipsis" className="btn btn-light" disabled>
                                        ...
                                    </button>,
                                );
                            buttons.push(
                                <button
                                    key={totalPages}
                                    className="btn btn-outline-primary"
                                    onClick={() => setPage(totalPages)}
                                >
                                    {totalPages}
                                </button>,
                            );
                        }

                        return buttons;
                    })()}

                    {/* Trang sau */}
                    <button
                        className="btn btn-outline-primary"
                        disabled={page >= Math.ceil(totalRecords / pageSize)}
                        onClick={() => setPage((p) => p + 1)}
                    >
                        ›
                    </button>

                    {/* Trang cuối */}
                    <button
                        className="btn btn-outline-primary"
                        disabled={page >= Math.ceil(totalRecords / pageSize)}
                        onClick={() => setPage(Math.ceil(totalRecords / pageSize))}
                    >
                        »
                    </button>
                </div>
            </div>
        </div>
    );
}