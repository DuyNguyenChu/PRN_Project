import { useEffect, useState, useCallback, useRef } from 'react';
import moment from 'moment';
import axios from 'axios';
import { API_URL } from '~/api/api';
import VehicleInsurancePopup from './VehicleInsurancePopup';

export default function VehicleInsuranceTable({ apiUrl, token, onEdit, refreshFlag, filters }) {
    const [data, setData] = useState([]);
    const [totalRecords, setTotalRecords] = useState(0);
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(10);
    const [search, setSearch] = useState('');
    const [sortField, setSortField] = useState('startDate');
    const [sortDir, setSortDir] = useState('desc');
    const [loading, setLoading] = useState(false);

    // Popup & modal state
    const [showConfirm, setShowConfirm] = useState(false);
    const [showResult, setShowResult] = useState(false);
    const [resultMessage, setResultMessage] = useState('');
    const [resultSuccess, setResultSuccess] = useState(false);
    const [deleteId, setDeleteId] = useState(null);
    const [showPopup, setShowPopup] = useState(false);
    const [editItem, setEditItem] = useState(null);

    const handleOpenEdit = (row) => {
        if (typeof onEdit === 'function') return onEdit(row);
        setEditItem(row || null);
        setShowPopup(true);
    };

    const handleAdd = () => {
        if (typeof onEdit === 'function') return onEdit(null);
        setEditItem(null);
        setShowPopup(true);
    };

    // Reset trang khi thay đổi bộ lọc
    const isInitialMount = useRef(true);
    useEffect(() => {
        if (isInitialMount.current) {
            isInitialMount.current = false;
        } else {
            setPage(1);
        }
    }, [filters]);

    // ====== FETCH DATA ======
    const fetchData = useCallback(async () => {
        setLoading(true);
        try {
            const usedToken = token || JSON.parse(localStorage.getItem('userData'))?.resources?.accessToken;
            const url = apiUrl || `${API_URL}/vehicle-insurance`;

            const res = await axios.get(url, {
                headers: usedToken ? { Authorization: `Bearer ${usedToken}` } : {}
            });

            const payload = res.data;
            let list = [];

            if (Array.isArray(payload)) list = payload;
            else if (Array.isArray(payload.resources)) list = payload.resources;
            else if (Array.isArray(payload.data)) list = payload.data;
            else if (payload && payload.isSucceeded && Array.isArray(payload.resources)) list = payload.resources;

            // Lọc tìm kiếm
            if (search && search.trim()) {
                const q = search.trim().toLowerCase();
                list = list.filter(it =>
                    (it.insuranceProvider || '').toLowerCase().includes(q) ||
                    String(it.vehicleId || '').toLowerCase().includes(q) ||
                    (it.policyNumber || '').toLowerCase().includes(q)
                );
            }

            // Sắp xếp
            const sortedData = [...list].sort((a, b) => {
                const valA = a[sortField];
                const valB = b[sortField];
                if (valA == null) return 1;
                if (valB == null) return -1;

                if (moment(valA).isValid() && moment(valB).isValid()) {
                    return sortDir === 'asc'
                        ? moment(valA).diff(moment(valB))
                        : moment(valB).diff(moment(valA));
                }
                if (typeof valA === 'string') {
                    return sortDir === 'asc' ? valA.localeCompare(valB) : valB.localeCompare(valA);
                }
                if (typeof valA === 'number') {
                    return sortDir === 'asc' ? valA - valB : valB - valA;
                }
                return 0;
            });

            const total = sortedData.length;
            const paged = sortedData.slice((page - 1) * pageSize, page * pageSize);

            setData(paged);
            setTotalRecords(total);
        } catch (err) {
            console.error('❌ Lỗi tải dữ liệu:', err);
            setData([]);
            setTotalRecords(0);
        } finally {
            setLoading(false);
        }
    }, [apiUrl, token, page, pageSize, search, sortDir, sortField, refreshFlag]);

    useEffect(() => {
        fetchData();
    }, [fetchData]);

    // ====== SORT HANDLER ======
    const handleSort = (field) => {
        if (sortField === field) {
            setSortDir(sortDir === 'asc' ? 'desc' : 'asc');
        } else {
            setSortField(field);
            setSortDir('asc');
        }
    };

    // ====== DELETE ======
    const handleDeleteClick = (id) => {
        setDeleteId(id);
        setShowConfirm(true);
    };

    const confirmDelete = async () => {
        try {
            const usedToken = token || JSON.parse(localStorage.getItem('userData'))?.resources?.accessToken;
            await axios.delete(`${apiUrl || `${API_URL}/vehicle-insurance`}/${deleteId}`, {
                headers: usedToken ? { Authorization: `Bearer ${usedToken}` } : {}
            });
            setResultSuccess(true);
            setResultMessage('Xóa thành công!');
        } catch (err) {
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

    // ====== RENDER ======
    return (
        <div>
            <div className="d-flex justify-content-between align-items-center mb-3">
                <input
                    type="text"
                    className="form-control w-auto"
                    placeholder="Tìm kiếm (Xe ID, Số HĐ, Nhà bảo hiểm)"
                    value={search}
                    onChange={(e) => { setSearch(e.target.value); setPage(1); }}
                />
                <span className="text-muted">Tổng: {totalRecords} bản ghi</span>
            </div>

            <table className="table table-hover">
                <thead>
                    <tr className="text-uppercase text-gray-500 fs-7">
                        <th>STT</th>
                        <th onClick={() => handleSort('vehicleId')} style={{ cursor: 'pointer' }}>
                            Xe ID {sortField === 'vehicleId' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        <th onClick={() => handleSort('policyNumber')} style={{ cursor: 'pointer' }}>
                            Số hợp đồng {sortField === 'policyNumber' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        <th onClick={() => handleSort('insuranceProvider')} style={{ cursor: 'pointer' }}>
                            Nhà bảo hiểm {sortField === 'insuranceProvider' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        <th onClick={() => handleSort('startDate')} style={{ cursor: 'pointer' }}>
                            Ngày bắt đầu {sortField === 'startDate' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        <th onClick={() => handleSort('endDate')} style={{ cursor: 'pointer' }}>
                            Ngày hết hạn {sortField === 'endDate' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        <th onClick={() => handleSort('premium')} style={{ cursor: 'pointer' }}>
                            Phí bảo hiểm {sortField === 'premium' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        <th onClick={() => handleSort('status')} style={{ cursor: 'pointer' }}>
                            Trạng thái {sortField === 'status' && (sortDir === 'asc' ? '▲' : '▼')}
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
                        data.map((row, index) => {
                            const vehicleLabel = row.vehicle?.name || row.vehicle?.plateNumber || `${row.vehicleId}`;
                            const start = moment(row.startDate).isValid() ? moment(row.startDate).format('DD/MM/YYYY') : '-';
                            const end = moment(row.endDate).isValid() ? moment(row.endDate).format('DD/MM/YYYY') : '-';
                            const company = row.insuranceProvider || '-';
                            const amount = row.premium != null
                                ? row.premium.toLocaleString() + ' ₫'
                                : '-';
                            const statusLabel = (row.status === 1 || row.status === 'active') ? 'Hiệu lực' : 'Hết hạn';

                            return (
                                <tr key={row.id ?? index}>
                                    <td>{(page - 1) * pageSize + index + 1}</td>
                                    <td>{vehicleLabel}</td>
                                    <td>{row.policyNumber || '-'}</td>
                                    <td>{company}</td>
                                    <td>{start}</td>
                                    <td>{end}</td>
                                    <td>{amount}</td>
                                    <td>{statusLabel}</td>
                                    <td className="text-end">
                                        <button className="btn btn-sm btn-primary me-2" onClick={() => handleOpenEdit(row)}>Sửa</button>
                                        <button className="btn btn-sm btn-danger" onClick={() => handleDeleteClick(row.id)}>Xóa</button>
                                    </td>
                                </tr>
                            );
                        })
                    )}
                </tbody>
            </table>

            {/* Modal xác nhận xóa */}
            {showConfirm && (
                <div className="modal fade show d-block" style={{ background: 'rgba(0,0,0,0.4)' }}>
                    <div className="modal-dialog modal-dialog-centered">
                        <div className="modal-content">
                            <div className="modal-header"><h5 className="modal-title">Xác nhận xóa</h5></div>
                            <div className="modal-body"><p>Bạn có chắc muốn xóa bản ghi này không?</p></div>
                            <div className="modal-footer">
                                <button className="btn btn-secondary" onClick={closeConfirm}>Hủy</button>
                                <button className="btn btn-danger" onClick={confirmDelete}>Có, xóa</button>
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
                            <div className="modal-body"><p>{resultMessage}</p></div>
                            <div className="modal-footer">
                                <button className="btn btn-primary" onClick={closeResult}>Đóng</button>
                            </div>
                        </div>
                    </div>
                </div>
            )}
            {/* Popup thêm/sửa */}
            {showPopup && (
                <VehicleInsurancePopup
                    show={showPopup}
                    handleClose={() => setShowPopup(false)}
                    handleSave={() => {
                        setShowPopup(false);
                        fetchData();
                    }}
                    editItem={editItem}
                    apiUrl={apiUrl}
                    token={token}
                />
            )}
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
