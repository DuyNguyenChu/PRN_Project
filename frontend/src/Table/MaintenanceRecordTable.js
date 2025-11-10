import { useEffect, useState, useCallback, useRef } from 'react';
import moment from 'moment';
import axios from 'axios';

// Giả định Status = 0 là "Chờ duyệt"
const PENDING_STATUS = 0;

export default function MaintenanceRecordTable({ 
    apiUrl, 
    token, 
    onEdit, 
    onApprove, 
    onReject, 
    refreshFlag, 
    filters,
    onDelete
}) {
    const [data, setData] = useState([]);
    const [totalRecords, setTotalRecords] = useState(0);
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(10);
    const [search, setSearch] = useState('');
    const [sortField, setSortField] = useState('startTime'); // Sửa sort mặc định
    const [sortDir, setSortDir] = useState('desc');
    const [loading, setLoading] = useState(false);
    const isInitialMount = useRef(true);

    useEffect(() => {
        if (isInitialMount.current) {
            isInitialMount.current = false;
        } else {
            setPage(1);
        }
    }, [filters, search]);

    const formatCurrency = (value) => {
        if (value == null) return "N/A";
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
    };

    const fetchData = useCallback(async () => {
        setLoading(true);
        try {
            // Các cột này ánh xạ 1:1 với MaintenanceRecordAggregate
            const columns = [
                { data: 'id', name: '', searchable: true, orderable: true, search: { value: '', regex: false } }, // 0
                { data: 'vehicleRegistrationNumber', name: '', searchable: true, orderable: true, search: { value: '', regex: false } }, // 1
                { data: 'driverName', name: '', searchable: true, orderable: true, search: { value: '', regex: false } }, // 2
                { data: 'tripCode', name: '', searchable: true, orderable: true, search: { value: '', regex: false } }, // 3
                { data: 'serviceProvider', name: '', searchable: true, orderable: true, search: { value: '', regex: false } }, // 4
                { data: 'startTime', name: '', searchable: true, orderable: true, search: { value: '', regex: false } }, // 5
                { data: 'serviceTypeName', name: '', searchable: true, orderable: true, search: { value: '', regex: false } }, // 6
                { data: 'serviceCost', name: '', searchable: true, orderable: true, search: { value: '', regex: false } }, // 7
                { data: 'statusName', name: '', searchable: true, orderable: true, search: { value: '', regex: false } }, // 8
                { data: 'id', name: '', searchable: false, orderable: false, search: { value: '', regex: false } }, // 9 (Actions)
            ];

            const sortColumnIndex = {
                id: 0,
                vehicleRegistrationNumber: 1,
                driverName: 2,
                tripCode: 3,
                serviceProvider: 4,
                startTime: 5,
                serviceTypeName: 6,
                serviceCost: 7,
                statusName: 8,
            }[sortField] || 5; 

            // Áp dụng bộ lọc cột từ `filters`
            if (filters.tripCode) columns[3].search.value = filters.tripCode;
            if (filters.serviceProvider) columns[4].search.value = filters.serviceProvider;
            if (filters.startTime) columns[5].search.value = filters.startTime;
            
            const requestBody = {
                draw: page,
                columns: columns,
                order: [{ column: sortColumnIndex, dir: sortDir }],
                start: (page - 1) * pageSize,
                length: pageSize,
                search: { value: search, regex: false },

                // Bộ lọc nâng cao (từ MaintenanceRecordDTParameters)
                vehicleIds: filters.vehicleIds || [],
                driverIds: filters.driverIds || [],
                tripIds: filters.tripIds || [],
                statusIds: filters.statusIds || [],
            };

            const res = await axios.post(`${apiUrl}/paged`, requestBody, { // Dùng /paged
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

    const renderActions = (row) => {
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
                    <button className="btn btn-sm btn-danger" onClick={() => onDelete(row.id)} title="Xóa" >
                        <i className="fa fa-trash"></i>
                    </button>
                </>
            );
        }
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
                            <th onClick={() => handleSort('serviceProvider')} style={{ cursor: 'pointer' }}>
                                Nơi BD {sortField === 'serviceProvider' && (sortDir === 'asc' ? '▲' : '▼')}
                            </th>
                            <th onClick={() => handleSort('startTime')} style={{ cursor: 'pointer' }}>
                                Ngày BD {sortField === 'startTime' && (sortDir === 'asc' ? '▲' : '▼')}
                            </th>
                            <th onClick={() => handleSort('serviceTypeName')} style={{ cursor: 'pointer' }}>
                                Loại DV {sortField === 'serviceTypeName' && (sortDir === 'asc' ? '▲' : '▼')}
                            </th>
                            <th onClick={() => handleSort('serviceCost')} style={{ cursor: 'pointer' }}>
                                Chi phí {sortField === 'serviceCost' && (sortDir === 'asc' ? '▲' : '▼')}
                            </th>
                            <th onClick={() => handleSort('statusName')} style={{ cursor: 'pointer' }}>
                                Trạng thái {sortField === 'statusName' && (sortDir === 'asc' ? '▲' : '▼')}
                            </th>
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
                                <tr key={row.id}>
                                    <td>{(page - 1) * pageSize + index + 1}</td>
                                    <td>{`[${row.vehicleRegistrationNumber}] ${row.vehicleModelName}`}</td>
                                    <td>{row.driverName}</td>
                                    <td>{row.serviceProvider}</td>
                                    <td>{moment(row.startTime).format('DD/MM/YYYY HH:mm')}</td>
                                    <td>{row.serviceTypeName}</td>
                                    <td>{formatCurrency(row.serviceCost)}</td>
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
            {/* (Copy code phân trang từ FuelLogTable.js vào đây) */}
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