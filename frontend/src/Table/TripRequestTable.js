import { useEffect, useState, useCallback, useRef } from 'react';
import moment from 'moment';
import axios from 'axios';

/**
 * Tính toán màu chữ (đen/trắng) tương phản tốt nhất với màu nền
 * (Giữ nguyên từ UserTable.js)
 */
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


export default function TripRequestTable({ apiUrl, token, onEdit, refreshFlag, filters }) {
    const [data, setData] = useState([]);
    const [totalRecords, setTotalRecords] = useState(0);
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(10);
    const [search, setSearch] = useState('');
    const [sortField, setSortField] = useState('CreatedDate'); // Sửa: Sắp xếp theo CreatedDate
    const [sortDir, setSortDir] = useState('desc');
    const [loading, setLoading] = useState(false);

    // Popup trạng thái (Giữ nguyên)
    const [showConfirm, setShowConfirm] = useState(false);
    const [showResult, setShowResult] = useState(false);
    const [resultMessage, setResultMessage] = useState('');
    const [resultSuccess, setResultSuccess] = useState(false);
    const [deleteId, setDeleteId] = useState(null);

    // Ref (Giữ nguyên)
    const isInitialMount = useRef(true);

    // Tự động reset về trang 1 khi lọc (Giữ nguyên)
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
            // Sửa: Cập nhật mảng columns để khớp với API mới
            const requestBody = {
                draw: page,
                columns: [
                    { data: 'Id', name: '', searchable: true, orderable: true, search: { value: '', regex: false, fixed: [] } },
                    { data: 'RequesterName', name: '', searchable: true, orderable: true, search: { value: '', regex: false, fixed: [] } },
                    { data: 'FromLocation', name: '', searchable: true, orderable: true, search: { value: '', regex: false, fixed: [] } },
                    { data: 'ToLocation', name: '', searchable: true, orderable: true, search: { value: '', regex: false, fixed: [] } },
                    { data: 'TripRequestStatusName', name: '', searchable: true, orderable: true, search: { value: '', regex: false, fixed: [] } },
                    { data: 'CreatedDate', name: '', searchable: true, orderable: true, search: { value: '', regex: false, fixed: [] } },
                    { data: 'Id', name: '', searchable: false, orderable: false, search: { value: '', regex: false, fixed: [] } }, // Cột Thao tác
                ],
                order: [
                    {
                        // Sửa: Cập nhật index cho sortField
                        column:
                            sortField === 'RequesterName' ? 1 :
                            sortField === 'FromLocation' ? 2 :
                            sortField === 'ToLocation' ? 3 :
                            sortField === 'TripRequestStatusName' ? 4 :
                            sortField === 'CreatedDate' ? 5 :
                            0, // Mặc định sort theo id (cột 0)
                        dir: sortDir,
                        name: '', // C# không dùng 'name'
                    },
                ],
                start: (page - 1) * pageSize,
                length: pageSize,
                search: { value: search, regex: false, fixed: [] },

                // Sửa: Thêm các trường filter đặc thù cho TripRequest từ C#
                additionalValues: [], // Mặc định
                tripRequestStatusIds: filters?.tripRequestStatusIds || [],
                requesterIds: filters?.requesterIds || [],
                cancellerIds: filters?.cancellerIds || [],
            };

            // Sửa: Cập nhật logic filter dựa trên các trường mới
            if (filters) {
                // Lọc theo cột (dựa trên C#)
                if (filters.fromLocation) {
                    requestBody.columns[2].search.value = filters.fromLocation;
                }
                if (filters.toLocation) {
                    requestBody.columns[3].search.value = filters.toLocation;
                }
                // Dựa theo logic C#: "tripRequestStatusName" dùng để tìm theo ID
                if (filters.tripRequestStatusName) {
                     // Giả sử filters.tripRequestStatusName là mảng ID, join thành chuỗi
                    requestBody.columns[4].search.value = Array.isArray(filters.tripRequestStatusName)
                        ? filters.tripRequestStatusName.join(',')
                        : filters.tripRequestStatusName;
                }
                // Lọc theo ngày tạo (giống UserTable)
                if (filters.createdDate) {
                    requestBody.columns[5].search.value = filters.createdDate;
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
        // Sửa: dependencies bao gồm cả `filters`
    }, [apiUrl, token, page, pageSize, search, sortField, sortDir, filters]);

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

    // --- Giữ nguyên toàn bộ logic Xóa (Delete Handlers) ---
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

    const closeConfirm = () => {
        setShowConfirm(false);
        setDeleteId(null);
    };

    const closeResult = () => {
        setShowResult(false);
    };
    // --- Kết thúc logic Xóa ---


    return (
        <div>
            {/* Giữ nguyên thanh tìm kiếm */}
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

            <table className="table table-hover">
                <thead>
                    {/* Sửa: Cập nhật các cột tiêu đề */}
                    <tr className="text-uppercase text-gray-500 fs-7">
                        <th style={{width: '5%'}}>STT</th>
                        <th onClick={() => handleSort('RequesterName')} style={{ cursor: 'pointer' }}>
                            Người yêu cầu {sortField === 'RequesterName' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        <th onClick={() => handleSort('FromLocation')} style={{ cursor: 'pointer' }}>
                            Điểm đi {sortField === 'FromLocation' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        <th onClick={() => handleSort('ToLocation')} style={{ cursor: 'pointer' }}>
                            Điểm đến {sortField === 'ToLocation' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        <th onClick={() => handleSort('TripRequestStatusName')} style={{ cursor: 'pointer' }}>
                            Trạng thái {sortField === 'TripRequestStatusName' && (sortDir === 'asc' ? '▲' : '▼')}
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
                            <td colSpan={7} className="text-center"> {/* Sửa: Cập nhật colSpan */}
                                Đang tải...
                            </td>
                        </tr>
                    ) : data.length === 0 ? (
                        <tr>
                            <td colSpan={7} className="text-center"> {/* Sửa: Cập nhật colSpan */}
                                Không có dữ liệu
                            </td>
                        </tr>
                    ) : (
                        data.map((row, index) => (
                            <tr key={row.Id}> {/* Sửa: Id (viết hoa) */}
                                <td>{(page - 1) * pageSize + index + 1}</td>
                                <td>{row.requesterName}</td>
                                <td>{row.fromLocation}</td>
                                <td>{row.toLocation}</td>
                                <td>
                                    {/* Sửa: Dùng pill trạng thái từ C# */}
                                    <span 
                                        className="badge" 
                                        style={{ 
                                            backgroundColor: row.tripRequestStatusColor,
                                            color: getContrastColor(row.tripRequestStatusColor)
                                        }}
                                    >
                                        {row.tripRequestStatusName}
                                    </span>
                                </td>
                                <td>{moment(row.CreatedDate).format('DD/MM/YYYY HH:mm:ss')}</td>
                                <td className="text-end">
                                    <button className="btn btn-sm btn-primary me-2" onClick={() => onEdit(row)}>
                                        Sửa
                                    </button>
                                    <button className="btn btn-sm btn-danger" onClick={() => handleDeleteClick(row.Id)}> {/* Sửa: Id (viết hoa) */}
                                        Xóa
                                    </button>
                                </td>
                            </tr>
                        ))
                    )}
                </tbody>
            </table>
            
            {/* Giữ nguyên Modal xác nhận xóa */}
            {showConfirm && (
                <div className="modal fade show d-block" style={{ background: 'rgba(0,0,0,0.4)' }}>
                    <div className="modal-dialog modal-dialog-centered">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h5 className="modal-title">Xác nhận xóa</h5>
                            </div>
                            <div className="modal-body">
                                <p>Bạn có chắc muốn xóa yêu cầu này không?</p>
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

            {/* GiFữ nguyên Modal kết quả */}
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
                <select
                    className="form-select w-auto"
                    value={pageSize}
                    onChange={(e) => {
                        setPageSize(Number(e.target.value));
                        setPage(1);
                    }}
                >
                    {[5, 10, 20, 50].map((n) => (
                        <option key={n} value={n}>
                            {n} / trang
                        </option>
                    ))}
                </select>
                <div className="btn-group" role="group" aria-label="Pagination buttons">
                    {/* ... (Toàn bộ logic nút phân trang được giữ nguyên) ... */}
                    <button className="btn btn-outline-primary" disabled={page === 1} onClick={() => setPage(1)}>
                        «
                    </button>
                    <button
                        className="btn btn-outline-primary"
                        disabled={page === 1}
                        onClick={() => setPage((p) => p - 1)}
                    >
                        ‹
                    </button>
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
                    <button
                        className="btn btn-outline-primary"
                        disabled={page >= Math.ceil(totalRecords / pageSize)}
                        onClick={() => setPage((p) => p + 1)}
                    >
                        ›
                    </button>
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