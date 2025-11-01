import { useEffect, useState, useCallback, useRef } from 'react';
import moment from 'moment';
import axios from 'axios';
import { API_URL } from '~/api/api';
import VehicleInspectionPopup from './VehicleInspectionPopup';

export default function VehicleInspectionTable({ apiUrl, token, onEdit, refreshFlag, filters }) {
    const [data, setData] = useState([]);
    const [totalRecords, setTotalRecords] = useState(0);
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(10);
    const [search, setSearch] = useState('');
    const [sortField, setSortField] = useState('createdDate');
    const [sortDir, setSortDir] = useState('desc');
    const [loading, setLoading] = useState(false);

    // Popup states
    const [showConfirm, setShowConfirm] = useState(false);
    const [showResult, setShowResult] = useState(false);
    const [resultMessage, setResultMessage] = useState('');
    const [resultSuccess, setResultSuccess] = useState(false);
    const [deleteId, setDeleteId] = useState(null);
    const [showPopup, setShowPopup] = useState(false);
    const [editItem, setEditItem] = useState(null);

    const handleOpenEdit = (row) => {
        if (typeof onEdit === 'function') {
            // nếu parent muốn xử lý, gọi lên parent
            return onEdit(row);
        }
        setEditItem(row || null);
        setShowPopup(true);
    };

    const handleAdd = () => {
        if (typeof onEdit === 'function') {
            // parent có thể mở form riêng, gọi với null
            return onEdit(null);
        }
        setEditItem(null);
        setShowPopup(true);
    };
    const isInitialMount = useRef(true);

    useEffect(() => {
        if (isInitialMount.current) {
            isInitialMount.current = false;
        } else {
            setPage(1);
        }
    }, [filters]);

    const fetchData = useCallback(async () => {
        setLoading(true);
        try {
            const usedToken = token || JSON.parse(localStorage.getItem('userData'))?.resources?.accessToken;
            const url = apiUrl || `${API_URL}/vehicle-inspections`;

            const res = await axios.get(url, {
                headers: usedToken ? { Authorization: `Bearer ${usedToken}` } : {}
            });
            console.log('vehicle-inspections payload:', res.data);

            const payload = res.data;
            let list = [];

            // Hỗ trợ nhiều dạng response: plain array, { resources: [...] }, { data: [...] }, or wrapped { isSucceeded, resources }
            if (Array.isArray(payload)) {
                list = payload;
            } else if (Array.isArray(payload.resources)) {
                list = payload.resources;
            } else if (Array.isArray(payload.data)) {
                list = payload.data;
            } else if (payload && payload.isSucceeded && Array.isArray(payload.resources)) {
                list = payload.resources;
            } else {
                list = [];
            }

            // loại bỏ bản ghi bị đánh dấu xóa (nếu có)
            list = list.filter(item => !item.isDeleted);

            // debug example
            console.log('vehicle-inspections list[0]:', list[0]);

            // áp dụng search client-side (nếu cần)
            if (search && search.trim()) {
                const q = search.trim().toLowerCase();
                list = list.filter(it =>
                    (it.result || '').toString().toLowerCase().includes(q) ||
                    (it.notes || '').toString().toLowerCase().includes(q) ||
                    (it.vehicleId || it.name || it.vehicleName || '').toString().toLowerCase().includes(q)
                );
            }

            // sort
            const sortedData = [...list].sort((a, b) => {
                const valA = a[sortField];
                const valB = b[sortField];

                if (valA === null || valA === undefined) return 1;
                if (valB === null || valB === undefined) return -1;

                if (typeof valA === 'string') {
                    return sortDir === 'asc' ? valA.localeCompare(valB) : valB.localeCompare(valA);
                }
                if (typeof valA === 'number') {
                    return sortDir === 'asc' ? valA - valB : valB - valA;
                }
                // date strings
                if (moment(valA).isValid() && moment(valB).isValid()) {
                    return sortDir === 'asc'
                        ? moment(valA).diff(moment(valB))
                        : moment(valB).diff(moment(valA));
                }
                return 0;
            });

            // pagination client-side
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

    const handleSort = (field) => {
        if (sortField === field) {
            setSortDir(sortDir === 'asc' ? 'desc' : 'asc');
        } else {
            setSortField(field);
            setSortDir('asc');
        }
    };

    const handleDeleteClick = (id) => {
        setDeleteId(id);
        setShowConfirm(true);
    };

    const confirmDelete = async () => {
        try {
            const usedToken = token || JSON.parse(localStorage.getItem('userData'))?.resources?.accessToken;
            await axios.delete(`${apiUrl || `${API_URL}/vehicle-inspections`}/${deleteId}`, {
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

    return (
        <div>
            <div className="d-flex justify-content-between align-items-center mb-3">
                <input
                    type="text"
                    className="form-control w-auto"
                    placeholder="Tìm kiếm..."
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
                            Xe {sortField === 'vehicleId' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        <th onClick={() => handleSort('inspectionDate')} style={{ cursor: 'pointer' }}>
                            Ngày kiểm định {sortField === 'inspectionDate' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        <th onClick={() => handleSort('inspectorId')} style={{ cursor: 'pointer' }}>
                            Người kiểm định {sortField === 'inspectorId' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        <th onClick={() => handleSort('result')} style={{ cursor: 'pointer' }}>
                            Kết quả {sortField === 'result' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        <th onClick={() => handleSort('status')} style={{ cursor: 'pointer' }}>
                            Trạng thái {sortField === 'status' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        <th className="text-end">Thao tác</th>
                    </tr>
                </thead>
                <tbody>
                    {loading ? (
                        <tr>
                            <td colSpan={7} className="text-center">Đang tải...</td>
                        </tr>
                    ) : data.length === 0 ? (
                        <tr>
                            <td colSpan={7} className="text-center">Không có dữ liệu</td>
                        </tr>
                    ) : (
                        data.map((row, index) => {
                            const vehicleLabel =
                                row.name ||
                                row.vehicleName ||
                                (row.vehicle && (row.vehicle.name || row.vehicle.plateNumber)) ||
                                row.vehicleId ||
                                '-';

                            const dateValue =
                                (row.inspectionDate && moment(row.inspectionDate).isValid())
                                    ? moment(row.inspectionDate).format('DD/MM/YYYY')
                                    : (row.createdDate && moment(row.createdDate).isValid()
                                        ? moment(row.createdDate).format('DD/MM/YYYY')
                                        : (row.inspectionDate || row.createdDate || '-'));

                            const inspectorLabel =
                                (row.inspector && (row.inspector.fullName || row.inspector.name)) ||
                                row.inspectorName ||
                                row.inspectorFullName ||
                                row.inspectorId ||
                                (row.createdByUser && (row.createdByUser.fullName || row.createdByUser.name)) ||
                                row.createdByName ||
                                row.createdBy ||
                                '-';

                            const resultLabel =
                                row.result ||
                                row.inspectionResult ||
                                row.outcome ||
                                row.description ||
                                row.notes ||
                                '-';

                            const statusVal = (row.status ?? row.Status ?? row.vehicleStatusId ?? row.vehicleStatus);

                            return (
                                <tr key={row.id ?? index}>
                                    <td>{(page - 1) * pageSize + index + 1}</td>
                                    <td>{vehicleLabel}</td>
                                    <td>{dateValue}</td>
                                    <td>{inspectorLabel}</td>
                                    <td>{resultLabel}</td>
                                    <td>{statusVal === 1 ? 'Hoạt động' : 'Không hoạt động'}</td>
                                    <td className="text-end">
                                        <button className="btn btn-sm btn-primary me-2" onClick={() => handleOpenEdit(row)}>
                                            Sửa
                                        </button>
                                        <button className="btn btn-sm btn-danger" onClick={() => handleDeleteClick(row.id)}>
                                            Xóa
                                        </button>
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
                            <div className="modal-header">
                                <h5 className="modal-title">Xác nhận xóa</h5>
                            </div>
                            <div className="modal-body">
                                <p>Bạn có chắc muốn xóa bản ghi này không?</p>
                            </div>
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
                            <div className="modal-body">
                                <p>{resultMessage}</p>
                            </div>
                            <div className="modal-footer">
                                <button className="btn btn-primary" onClick={closeResult}>Đóng</button>
                            </div>
                        </div>
                    </div>
                </div>
            )}

            {/* Phân trang */}
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

                <div className="btn-group">
                    <button className="btn btn-outline-primary" disabled={page === 1} onClick={() => setPage(1)}>
                        «
                    </button>
                    <button className="btn btn-outline-primary" disabled={page === 1} onClick={() => setPage(p => p - 1)}>
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
                                <button key={1} className="btn btn-outline-primary" onClick={() => setPage(1)}>1</button>
                            );
                            if (start > 2) {
                                buttons.push(
                                    <button key="start-ellipsis" className="btn btn-light" disabled>...</button>
                                );
                            }
                        }

                        for (let i = start; i <= end; i++) {
                            buttons.push(
                                <button
                                    key={i}
                                    className={`btn ${i === page ? 'btn-primary' : 'btn-outline-primary'}`}
                                    onClick={() => setPage(i)}
                                >
                                    {i}
                                </button>
                            );
                        }

                        if (end < totalPages) {
                            if (end < totalPages - 1) {
                                buttons.push(
                                    <button key="end-ellipsis" className="btn btn-light" disabled>...</button>
                                );
                            }
                            buttons.push(
                                <button
                                    key={totalPages}
                                    className="btn btn-outline-primary"
                                    onClick={() => setPage(totalPages)}
                                >
                                    {totalPages}
                                </button>
                            );
                        }

                        return buttons;
                    })()}
                    <button
                        className="btn btn-outline-primary"
                        disabled={page >= Math.ceil(totalRecords / pageSize)}
                        onClick={() => setPage(p => p + 1)}
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