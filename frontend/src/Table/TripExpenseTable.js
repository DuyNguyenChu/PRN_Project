// TripExpenseTable.js
import { useEffect, useState, useCallback, useRef } from 'react';
import moment from 'moment';
import axios from 'axios';

// Giữ nguyên hàm getContrastColor
function getContrastColor(hexColor) {
    if (!hexColor) return '#000';
    try {
        const hex = hexColor.replace('#', '');
        const r = parseInt(hex.substring(0, 2), 16);
        const g = parseInt(hex.substring(2, 4), 16);
        const b = parseInt(hex.substring(4, 6), 16);
        const yiq = ((r * 299) + (g * 587) + (b * 114)) / 1000;
        return (yiq >= 128) ? '#000' : '#FFF';
    } catch (e) {
        console.error("Lỗi chuyển đổi màu:", e);
        return '#000';
    }
}


export default function TripExpenseTable({ apiUrl, token, onEdit, refreshFlag, filters }) {
    const [data, setData] = useState([]);
    const [totalRecords, setTotalRecords] = useState(0);
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(10);
    const [search, setSearch] = useState('');
    const [sortField, setSortField] = useState('CreatedDate'); // Sửa
    const [sortDir, setSortDir] = useState('desc');
    const [loading, setLoading] = useState(false);

    // Giữ nguyên state Popup
    const [showConfirm, setShowConfirm] = useState(false);
    const [showResult, setShowResult] = useState(false);
    const [resultMessage, setResultMessage] = useState('');
    const [resultSuccess, setResultSuccess] = useState(false);
    const [deleteId, setDeleteId] = useState(null);

    const isInitialMount = useRef(true);

    useEffect(() => {
        if (isInitialMount.current) {
            isInitialMount.current = false;
        } else {
            setPage(1);
        }
    }, [filters]);

    // Fetch dữ liệu
    const fetchData = useCallback(async () => {
        setLoading(true);
        try {
            // Sửa: Cập nhật mảng columns
            const requestBody = {
                draw: page,
                columns: [
                    { data: 'Id', name: '', searchable: true, orderable: true, search: { value: '', regex: false } },
                    { data: 'DriverName', name: '', searchable: true, orderable: true, search: { value: '', regex: false } },
                    { data: 'VehicleModelName', name: '', searchable: true, orderable: true, search: { value: '', regex: false } }, // Kết hợp
                    { data: 'ExpenseTypeName', name: '', searchable: true, orderable: true, search: { value: '', regex: false } },
                    { data: 'Amount', name: '', searchable: true, orderable: true, search: { value: '', regex: false } },
                    { data: 'StatusName', name: '', searchable: true, orderable: true, search: { value: '', regex: false } }, // API trả về StatusName
                    { data: 'ExpenseDate', name: '', searchable: true, orderable: true, search: { value: '', regex: false } },
                    { data: 'CreatedDate', name: '', searchable: true, orderable: true, search: { value: '', regex: false } },
                    { data: 'ApprovalDate', name: '', searchable: true, orderable: true, search: { value: '', regex: false } },
                    { data: 'Id', name: '', searchable: false, orderable: false, search: { value: '', regex: false } }, // Thao tác
                ],
                order: [
                    {
                        // Sửa: Cập nhật index
                        column:
                            sortField === 'DriverName' ? 1 :
                            sortField === 'VehicleModelName' ? 2 :
                            sortField === 'ExpenseTypeName' ? 3 :
                            sortField === 'Amount' ? 4 :
                            sortField === 'StatusName' ? 5 :
                            sortField === 'ExpenseDate' ? 6 :
                            sortField === 'CreatedDate' ? 7 :
                            sortField === 'ApprovalDate' ? 8 :
                            0, // Mặc định
                        dir: sortDir,
                        name: '',
                    },
                ],
                start: (page - 1) * pageSize,
                length: pageSize,
                search: { value: search, regex: false },

                // Sửa: Thêm các trường filter đặc thù
                additionalValues: [],
                vehicleIds: filters?.vehicleIds || [],
                statusIds: filters?.statusIds || [],
                expenseTypeIds: filters?.expenseTypeIds || [],
                driverIds: filters?.driverIds || [],
                tripIds: filters?.tripIds || [],
            };

            // Sửa: Cập nhật logic filter
            if (filters) {
                // Lọc theo 3 cột ngày (theo C#)
                if (filters.expenseDate) {
                    requestBody.columns[6].search.value = filters.expenseDate;
                }
                if (filters.createdDate) {
                    requestBody.columns[7].search.value = filters.createdDate;
                }
                 if (filters.approvalDate) {
                    requestBody.columns[8].search.value = filters.approvalDate;
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
    }, [apiUrl, token, page, pageSize, search, sortField, sortDir, filters]); // Sửa dependencies

    useEffect(() => {
        fetchData();
    }, [fetchData, refreshFlag]);

    // Giữ nguyên handleSort
    const handleSort = (field) => {
        if (sortField === field) {
            setSortDir(sortDir === 'asc' ? 'desc' : 'asc');
        } else {
            setSortField(field);
            setSortDir('asc');
        }
    };

    // --- Giữ nguyên logic Xóa (Delete Handlers) ---
    const handleDeleteClick = (id) => {
        setDeleteId(id);
        setShowConfirm(true);
    };

    const confirmDelete = async () => {
        try {
            await axios.delete(`${apiUrl}/${deleteId}`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            setResultSuccess(true);
            setResultMessage('Xóa thành công!');
        } catch (err) {
            console.error('❌ Lỗi xóa:', err);
            setResultSuccess(false);
            setResultMessage(`Xóa thất bại: ${err.response?.data?.message || err.message}`);
        } finally {
            setShowConfirm(false);
            setShowResult(true);
            fetchData();
        }
    };

    const closeConfirm = () => setShowConfirm(false);
    const closeResult = () => setShowResult(false);
    // --- Kết thúc logic Xóa ---

    return (
        <div>
            <div className="d-flex justify-content-between align-items-center mb-3">
                <input
                    type="text"
                    className="form-control w-auto"
                    placeholder="Tìm kiếm"
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                />
                <span className="text-muted">Tổng: {totalRecords} bản ghi</span>
            </div>

            <div className="table-responsive">
                <table className="table table-hover">
                    <thead>
                        {/* Sửa: Cập nhật các cột tiêu đề */}
                        <tr className="text-uppercase text-gray-500 fs-7">
                            <th style={{width: '5%'}}>STT</th>
                            <th onClick={() => handleSort('DriverName')} style={{ cursor: 'pointer' }}>
                                Tài xế {sortField === 'DriverName' && (sortDir === 'asc' ? '▲' : '▼')}
                            </th>
                            <th onClick={() => handleSort('VehicleModelName')} style={{ cursor: 'pointer' }}>
                                Tên xe {sortField === 'VehicleModelName' && (sortDir === 'asc' ? '▲' : '▼')}
                            </th>
                            <th onClick={() => handleSort('ExpenseTypeName')} style={{ cursor: 'pointer' }}>
                                Loại chi phí {sortField === 'ExpenseTypeName' && (sortDir === 'asc' ? '▲' : '▼')}
                            </th>
                            <th onClick={() => handleSort('Amount')} style={{ cursor: 'pointer' }}>
                                Số tiền {sortField === 'Amount' && (sortDir === 'asc' ? '▲' : '▼')}
                            </th>
                            <th onClick={() => handleSort('StatusName')} style={{ cursor: 'pointer' }}>
                                Trạng thái {sortField === 'StatusName' && (sortDir === 'asc' ? '▲' : '▼')}
                            </th>
                             <th onClick={() => handleSort('ExpenseDate')} style={{ cursor: 'pointer' }}>
                                Ngày chi phí {sortField === 'ExpenseDate' && (sortDir === 'asc' ? '▲' : '▼')}
                            </th>
                            <th onClick={() => handleSort('CreatedDate')} style={{ cursor: 'pointer' }}>
                                Ngày tạo {sortField === 'CreatedDate' && (sortDir === 'asc' ? '▲' : '▼')}
                            </th>
                            <th className="text-end">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody>
                        {loading ? (
                            <tr>
                                <td colSpan={9} className="text-center"> {/* Sửa: colSpan */}
                                    Đang tải...
                                </td>
                            </tr>
                        ) : data.length === 0 ? (
                            <tr>
                                <td colSpan={9} className="text-center"> {/* Sửa: colSpan */}
                                    Không có dữ liệu
                                </td>
                            </tr>
                        ) : (
                            data.map((row, index) => (
                                <tr key={row.Id}>
                                    <td>{(page - 1) * pageSize + index + 1}</td>
                                    <td>{row.driverName}</td>
                                    <td>{`${row.vehicleBrandName} ${row.vehicleModelName}`}</td>
                                    <td>{row.expenseTypeName}</td>
                                    <td>{row.amount} đ</td>
                                    <td>
                                        {/* Sửa: Pill trạng thái (API đã trả về) */ }
                                        <span 
                                            className="badge" 
                                            style={{ 
                                                backgroundColor: row.statusColor,
                                                color: getContrastColor(row.statusColor)
                                            }}
                                        >
                                            {row.statusName}
                                        </span>
                                    </td>
                                    <td>{moment(row.expenseDate).format('DD/MM/YYYY')}</td>
                                    <td>{moment(row.CreatedDate).format('DD/MM/YYYY HH:mm')}</td>
                                    <td className="text-end">
                                        <button className="btn btn-sm btn-primary me-2" onClick={() => onEdit(row)}>
                                            Sửa
                                        </button>
                                        <button className="btn btn-sm btn-danger" onClick={() => handleDeleteClick(row.Id)}>
                                            Xóa
                                        </button>
                                    </td>
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>
            
            {/* Giữ nguyên Modal xác nhận xóa */}
            {showConfirm && (
                <div className="modal fade show d-block" style={{ background: 'rgba(0,0,0,0.4)' }}>
                    <div className="modal-dialog modal-dialog-centered">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h5 className="modal-title">Xác nhận xóa</h5>
                            </div>
                            <div className="modal-body">
                                <p>Bạn có chắc muốn xóa hành động này không?</p>
                            </div>
                            <div className="modal-footer">
                                <button className="btn btn-secondary" onClick={closeConfirm}>
                                    Hủy
                                </button>
                                <button className="btn btn-danger" onClick={confirmDelete}>
                                    Có, xóa
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}

            {/* Giữ nguyên Modal kết quả */}
            {showResult && (
                <div className="modal fade show d-block" style={{ background: 'rgba(0,0,0,0.4)' }}>
                    <div className="modal-dialog modal-dialog-centered">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h5 className={`modal-title ${resultSuccess ? 'text-success' : 'text-danger'}`}>
                                    {resultSuccess ? 'Thành công' : 'Thất bại'}
                                </h5>
                            </div>
                            <div className="modal-body">
                                <p>{resultMessage}</p>
                            </div>
                            <div className="modal-footer">
                                <button className="btn btn-primary" onClick={closeResult}>
                                    Đóng
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}

            {/* Giữ nguyên Phân trang */}
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