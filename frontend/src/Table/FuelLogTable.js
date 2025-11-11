import { useEffect, useState, useCallback, useRef } from 'react';
import moment from 'moment';
import axios from 'axios';

export default function FuelLogTable({ apiUrl, token, onEdit, onDelete, onApprove, onReject, refreshFlag, filters }) {
    const [data, setData] = useState([]);
    const [totalRecords, setTotalRecords] = useState(0);
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(10);
    const [search, setSearch] = useState('');
    const [sortField, setSortField] = useState('createdDate');
    const [sortDir, setSortDir] = useState('desc');
    const [loading, setLoading] = useState(false);

    const isInitialMount = useRef(true);

    // Reset về trang 1 khi filter hoặc search thay đổi
    useEffect(() => {
        if (isInitialMount.current) {
            isInitialMount.current = false;
        } else {
            setPage(1);
        }
    }, [filters, search]); // <--- SỬA LỖI Ở ĐÂY

    const formatCurrency = (value) => {
        if (value === null || value === undefined) return '';
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
    };

    // Fetch dữ liệu
    const fetchData = useCallback(async () => {
        setLoading(true);
        try {
            // Định nghĩa các cột theo FuelLogAggregate
            const columns = [
                { data: 'id', name: '', searchable: true, orderable: true, search: { value: '', regex: false } }, // 0
                { data: 'vehicleRegistrationNumber', name: '', searchable: true, orderable: true, search: { value: '', regex: false } }, // 1
                { data: 'driverName', name: '', searchable: true, orderable: true, search: { value: '', regex: false } }, // 2
                { data: 'tripCode', name: '', searchable: true, orderable: true, search: { value: '', regex: false } }, // 3
                { data: 'gasStation', name: '', searchable: true, orderable: true, search: { value: '', regex: false } }, // 4
                { data: 'createdDate', name: '', searchable: true, orderable: true, search: { value: '', regex: false } }, // 5
                { data: 'fuelTypeName', name: '', searchable: true, search: { value: '', regex: false } }, // 6
                { data: 'totalCost', name: '', searchable: true, orderable: true, search: { value: '', regex: false } }, // 7
                { data: 'statusName', name: '', searchable: true, search: { value: '', regex: false } }, // 8
                { data: 'id', name: '', searchable: false, orderable: false, search: { value: '', regex: false } }, // 9 (Actions)
            ];

            // Map sortField to column index
            const sortColumnIndex = {
                id: 0,
                vehicleRegistrationNumber: 1,
                driverName: 2,
                tripCode: 3,
                gasStation: 4,
                createdDate: 5,
                fuelTypeName: 6,
                totalCost: 7,
                statusName: 8,
            }[sortField] || 5; // Mặc định là createdDate

            // Áp dụng bộ lọc cột từ `filters` (cho thanh lọc)
            if (filters.tripCode) columns[3].search.value = filters.tripCode;
            if (filters.gasStation) columns[4].search.value = filters.gasStation;
            if (filters.createdDate) columns[5].search.value = filters.createdDate;
            
            const requestBody = {
                draw: page,
                columns: columns,
                order: [{ column: sortColumnIndex, dir: sortDir }],
                start: (page - 1) * pageSize,
                length: pageSize,
                search: { value: search, regex: false }, // Gửi "Tìm kiếm chung"

                // Bộ lọc nâng cao (từ FuelLogDTParameters)
                vehicleIds: filters.vehicleIds || [],
                driverIds: filters.driverIds || [],
                tripIds: filters.tripIds || [],
                fuelTypes: filters.fuelTypes || [],
                statusIds: filters.statusIds || [],
            };

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
            console.error(' ❌  Lỗi tải dữ liệu:', err);
        } finally {
            setLoading(false);
        }
    }, [apiUrl, token, page, pageSize, search, sortDir, sortField, filters]);

    useEffect(() => {
        fetchData();
    }, [fetchData, refreshFlag]);

    const handleSort = (field) => {
        if (sortField === field) {
            setSortDir(sortDir === 'asc' ? 'desc' : 'asc');
        } else {
            setSortField(field);
            setSortDir('asc');
        }
    };

    const PENDING_STATUS = 0; 
        const renderActions = (row) => {
        // Logic if/else này đã đúng, chỉ cần hằng số PENDING_STATUS đúng
        if (row.status === PENDING_STATUS) {
            return (
                <>
                    <button className="btn btn-sm btn-success me-1" onClick={() => onApprove(row.id)} title="Duyệt">
                        <i className="fa fa-check"></i>
                    </button>
                    <button className="btn btn-sm btn-warning me-1" onClick={() => onReject(row)} title="Từ chối">
                        <i className="fa fa-times"></i>
                    </button>
                    <button className="btn btn-sm btn-primary me-1" onClick={() => onEdit(row)} title="Sửa">
                        <i className="fa fa-pen"></i>
                    </button>
                    <button className="btn btn-sm btn-danger" onClick={() => onDelete(row.id)} title="Xóa">
                        <i className="fa fa-trash"></i>
                    </button>
                </>
            );
        }
        // Với các status khác (1: Đã duyệt, 2: Từ chối), chỉ hiện nút "Xem"
        return (
            <button className="btn btn-sm btn-secondary me-1" onClick={() => onEdit(row)} title="Xem chi tiết">
                <i className="fa fa-eye"></i>
            </button>
        );
    };

    return (
        <div>
            <div className="d-flex justify-content-between align-items-center mb-3">
                <input
                    type="text"
                    className="form-control w-auto"
                    placeholder="Tìm kiếm chung..."
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                />
                <span className="text-muted">Tổng: {totalRecords} bản ghi</span>
            </div>
            <div className="table-responsive">
                <table className="table table-hover">
                    <thead>
                        <tr className="text-uppercase text-gray-500 fs-7">
                            <th>STT</th>
                            <th onClick={() => handleSort('vehicleRegistrationNumber')} style={{ cursor: 'pointer' }}>
                                Xe {sortField === 'vehicleRegistrationNumber' && (sortDir === 'asc' ? '▲' : '▼')}
                            </th>
                            <th onClick={() => handleSort('driverName')} style={{ cursor: 'pointer' }}>
                                Lái xe {sortField === 'driverName' && (sortDir === 'asc' ? '▲' : '▼')}
                            </th>
                            <th onClick={() => handleSort('tripCode')} style={{ cursor: 'pointer' }}>
                                Chuyến đi {sortField === 'tripCode' && (sortDir === 'asc' ? '▲' : '▼')}
                            </th>
                            <th onClick={() => handleSort('gasStation')} style={{ cursor: 'pointer' }}>
                                Trạm xăng {sortField === 'gasStation' && (sortDir === 'asc' ? '▲' : '▼')}
                            </th>
                            <th onClick={() => handleSort('createdDate')} style={{ cursor: 'pointer' }}>
                                Ngày tạo {sortField === 'createdDate' && (sortDir === 'asc' ? '▲' : '▼')}
                            </th>
                            <th onClick={() => handleSort('fuelTypeName')} style={{ cursor: 'pointer' }}>
                                Nhiên liệu 
                            </th>
                            <th onClick={() => handleSort('totalCost')} style={{ cursor: 'pointer' }}>
                                Tổng tiền {sortField === 'totalCost' && (sortDir ==='asc' ? '▲' : '▼')}
                            </th>
                            <th onClick={() => handleSort('statusName')} style={{ cursor: 'pointer' }}>
                                Trạng thái 
                            </th>
                            <th className="text-end">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody>
                        {loading ? (
                            <tr><td colSpan={10} className="text-center">Đang tải...</td></tr>
                        ) : data.length === 0 ? (
                            <tr><td colSpan={10} className="text-center">Không có dữ liệu</td></tr>
                        ) : (
                            data.map((row, index) => (
                                <tr key={row.id}>
                                    <td>{(page - 1) * pageSize + index + 1}</td>
                                    <td>{`[${row.vehicleRegistrationNumber}] ${row.vehicleModelName}`}</td>
                                    <td>{row.driverName}</td>
                                    <td>{row.tripCode || 'N/A'}</td>
                                    <td>{row.gasStation}</td>
                                    <td>{moment(row.createdDate).format('DD/MM/YYYY HH:mm')}</td>
                                    <td>{row.fuelTypeName}</td>
                                    <td>{formatCurrency(row.totalCost)}</td>
                                    <td>
                                        <span className={`badge`} style={{ backgroundColor: row.statusColor, color: '#fff' }}>
                                            {row.statusName}
                                        </span>
                                    </td>
                                    <td className="text-end">{renderActions(row)}</td>
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>

            {/* Pagination Controls */}
            <div className="d-flex justify-content-between align-items-center mt-3">
                <select
                    className="form-select w-auto"
                    value={pageSize}
                    onChange={(e) => {
                        setPageSize(Number(e.target.value));
                        setPage(1);
                    }}
                >
                    {[5, 10, 20, 50].map((n) => (
                        <option key={n} value={n}>{n} / trang</option>
                    ))}
                </select>
                <div className="btn-group" role="group">
                    {(() => {
                        const totalPages = Math.ceil(totalRecords / pageSize);
                        if (totalPages === 0) return null; // Không hiển thị gì nếu không có trang
                        const maxVisible = 5;
                        let start = Math.max(1, page - Math.floor(maxVisible / 2));
                        let end = Math.min(totalPages, start + maxVisible - 1);
                        if (end - start < maxVisible - 1) {
                            start = Math.max(1, end - maxVisible + 1);
                        }
                        const buttons = [];
                        if (start > 1) {
                            buttons.push(<button key={1} className="btn btn-outline-primary" onClick={() => setPage(1)}>1</button>);
                            if (start > 2) buttons.push(<button key="start-ellipsis" className="btn btn-light" disabled>...</button>);
                        }
                        for (let i = start; i <= end; i++) {
                            buttons.push(<button key={i} className={`btn ${i === page ? 'btn-primary' : 'btn-outline-primary'}`} onClick={() => setPage(i)}>{i}</button>);
                        }
                        if (end < totalPages) {
                            if (end < totalPages - 1) buttons.push(<button key="end-ellipsis" className="btn btn-light" disabled>...</button>);
                            buttons.push(<button key={totalPages} className="btn btn-outline-primary" onClick={() => setPage(totalPages)}>{totalPages}</button>);
                        }
                        return (
                            <>
                                <button className="btn btn-outline-primary" disabled={page === 1} onClick={() => setPage(1)}>«</button>
                                <button className="btn btn-outline-primary" disabled={page === 1} onClick={() => setPage((p) => p - 1)}>‹</button>
                                {buttons}
                                <button className="btn btn-outline-primary" disabled={page >= totalPages} onClick={() => setPage((p) => p + 1)}>›</button>
                                <button className="btn btn-outline-primary" disabled={page >= totalPages} onClick={() => setPage(totalPages)}>»</button>
                            </>
                        );
                    })()}
                </div>
            </div>
        </div>
    );
}