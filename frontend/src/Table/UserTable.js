import { useEffect, useState, useCallback, useRef } from 'react';
import moment from 'moment';
import axios from 'axios';

/**
 * Tính toán màu chữ (đen/trắng) tương phản tốt nhất với màu nền
 * @param {string} hexColor - Màu nền dạng hex (ví dụ: "#00b315")
 * @returns {string} - Trả về "#FFF" (trắng) hoặc "#000" (đen)
 */
function getContrastColor(hexColor) {
    if (!hexColor) return '#000';
    try {
        // Xóa dấu #
        const hex = hexColor.replace('#', '');
        
        // Chuyển đổi hex sang RGB
        const r = parseInt(hex.substring(0, 2), 16);
        const g = parseInt(hex.substring(2, 4), 16);
        const b = parseInt(hex.substring(4, 6), 16);

        // Công thức tính độ sáng YIQ
        const yiq = ((r * 299) + (g * 587) + (b * 114)) / 1000;
        
        // Trả về màu tương phản
        return (yiq >= 128) ? '#000' : '#FFF';
    } catch (e) {
        console.error("Lỗi chuyển đổi màu:", e);
        return '#000';
    }
}


export default function UserTable({ apiUrl, token, onEdit, refreshFlag, filters }) {
    const [data, setData] = useState([]);
    const [totalRecords, setTotalRecords] = useState(0);
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(10);
    const [search, setSearch] = useState('');
    const [sortField, setSortField] = useState('createdDate');
    const [sortDir, setSortDir] = useState('desc');
    const [loading, setLoading] = useState(false);

    // Popup trạng thái
    const [showConfirm, setShowConfirm] = useState(false);
    const [showResult, setShowResult] = useState(false);
    const [resultMessage, setResultMessage] = useState('');
    const [resultSuccess, setResultSuccess] = useState(false);
    const [deleteId, setDeleteId] = useState(null);

    // Ref để theo dõi lần render đầu tiên, tránh reset page khi mới vào
    const isInitialMount = useRef(true);

    // Tự động reset về trang 1 khi người dùng áp dụng bộ lọc mới
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
            // Cập nhật mảng columns để khớp với API mới
            const requestBody = {
                draw: page,
                columns: [
                    { data: 'id', name: '', searchable: true, orderable: true, search: { value: '', regex: false, fixed: [] } },
                    { data: 'firstName', name: '', searchable: true, orderable: true, search: { value: '', regex: false, fixed: [] } }, // Sửa: Cột 1 là firstName
                    { data: 'email', name: '', searchable: true, orderable: true, search: { value: '', regex: false, fixed: [] } }, // Sửa: Cột 2 là email
                    { data: 'phoneNumber', name: '', searchable: true, orderable: true, search: { value: '', regex: false, fixed: [] } }, // Sửa: Cột 3 là phoneNumber
                    { data: 'userStatusName', name: '', searchable: true, orderable: true, search: { value: '', regex: false, fixed: [] } }, // Sửa: Cột 4 là userStatusName
                    { data: 'createdDate', name: '', searchable: true, orderable: true, search: { value: '', regex: false, fixed: [] } }, // Sửa: Cột 5 là createdDate
                    { data: 'id', name: '', searchable: false, orderable: false, search: { value: '', regex: false, fixed: [] } }, // Cột 6 (Roles) không tìm/sắp xếp
                    { data: 'id', name: '', searchable: false, orderable: false, search: { value: '', regex: false, fixed: [] } }, // Cột 7 (Thao tác)
                ],
                order: [
                    {
                        // Cập nhật index cho sortField
                        column:
                            sortField === 'firstName' ? 1 :
                            sortField === 'email' ? 2 :
                            sortField === 'phoneNumber' ? 3 :
                            sortField === 'userStatusName' ? 4 :
                            sortField === 'createdDate' ? 5 :
                            0, // Mặc định sort theo id (cột 0) nếu không khớp
                        dir: sortDir,
                        name: '',
                    },
                ],
                start: (page - 1) * pageSize,
                length: pageSize,
                search: { value: search, regex: false, fixed: [] },
            };

            if (filters) {
                // TODO: Cập nhật logic filter dựa trên các trường mới (nếu cần)
                // Ví dụ lọc theo email (cột 2)
                // if (filters.email) {
                //     requestBody.columns[2].search.value = filters.email;
                // }

                // Lọc theo ngày tạo (đã được format thành chuỗi)
                if (filters.createdDate) {
                    // Cột `createdDate` có index là 5
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
    }, [apiUrl, token, page, pageSize, search, sortDir, filters]); // Giữ nguyên dependencies

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

    // Khi bấm nút "Xóa"
    const handleDeleteClick = (id) => {
        setDeleteId(id);
        setShowConfirm(true);
    };

    // Xác nhận xóa
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

            <table className="table table-hover">
                <thead>
                    {/* Cập nhật các cột tiêu đề */}
                    <tr className="text-uppercase text-gray-500 fs-7">
                        <th style={{width: '5%'}}>STT</th>
                        <th onClick={() => handleSort('firstName')} style={{ cursor: 'pointer' }}>
                            Tên {sortField === 'firstName' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        <th onClick={() => handleSort('email')} style={{ cursor: 'pointer' }}>
                            Email {sortField === 'email' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        <th onClick={() => handleSort('phoneNumber')} style={{ cursor: 'pointer' }}>
                            SĐT {sortField === 'phoneNumber' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        <th onClick={() => handleSort('userStatusName')} style={{ cursor: 'pointer' }}>
                            Trạng thái {sortField === 'userStatusName' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        <th>Vai trò</th>
                        <th onClick={() => handleSort('createdDate')} style={{ cursor: 'pointer' }}>
                            Ngày tạo {sortField === 'createdDate' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        <th className="text-end">Thao tác</th>
                    </tr>
                </thead>
                <tbody>
                    {loading ? (
                        <tr>
                            <td colSpan={8} className="text-center"> {/* Cập nhật colSpan */}
                                Đang tải...
                            </td>
                        </tr>
                    ) : data.length === 0 ? (
                        <tr>
                            <td colSpan={8} className="text-center"> {/* Cập nhật colSpan */}
                                Không có dữ liệu
                            </td>
                        </tr>
                    ) : (
                        data.map((row, index) => (
                            <tr key={row.id}>
                                <td>{(page - 1) * pageSize + index + 1}</td>
                                <td>{`${row.firstName} ${row.lastName}`}</td>
                                <td>{row.email}</td>
                                <td>{row.phoneNumber}</td>
                                <td>
                                    {/* Pill trạng thái với màu động */}
                                    <span 
                                        className="badge" 
                                        style={{ 
                                            backgroundColor: row.userStatusColor,
                                            color: getContrastColor(row.userStatusColor)
                                        }}
                                    >
                                        {row.userStatusName}
                                    </span>
                                </td>
                                <td>
                                    {/* Danh sách pill roles */}
                                    {row.roles.map(role => (
                                        <span key={role.id} className="badge bg-secondary me-1">
                                            {role.name}
                                        </span>
                                    ))}
                                </td>
                                <td>{moment(row.createdDate).format('DD/MM/YYYY HH:mm:ss')}</td>
                                <td className="text-end">
                                    <button className="btn btn-sm btn-primary me-2" onClick={() => onEdit(row)}>
                                        Sửa
                                    </button>
                                    <button className="btn btn-sm btn-danger" onClick={() => handleDeleteClick(row.id)}>
                                        Xóa
                                    </button>
                                </td>
                            </tr>
                        ))
                    )}
                </tbody>
            </table>
            
            {/* Modal xác nhận xóa */}
            {showConfirm && (
                <div className="modal fade show d-block" style={{ background: 'rgba(0,0,0,0.4)' }}>
                    <div className="modal-dialog modal-dialog-centered">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h5 className="modal-title">Xác nhận xóa</h5>
                            </div>
                            <div className="modal-body">
                                <p>Bạn có chắc muốn xóa người dùng này không?</p>
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

            {/* Modal kết quả */}
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

            {/* Phân trang (Giữ nguyên) */}
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

