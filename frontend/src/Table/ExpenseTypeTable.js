import { useEffect, useState, useCallback, useRef } from 'react';
import moment from 'moment';
import axios from 'axios';

export default function ExpenseTypeTable({ apiUrl, token, onEdit, refreshFlag, filters }) {
    // THAY ĐỔI
    const [data, setData] = useState([]);
    const [totalRecords, setTotalRecords] = useState(0);
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(10);
    const [search, setSearch] = useState('');
    const [sortField, setSortField] = useState('createdDate');
    const [sortDir, setSortDir] = useState('desc');
    const [loading, setLoading] = useState(false);

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
    }, [filters, search]);

    const fetchData = useCallback(async () => {
        setLoading(true);
        try {
            const requestBody = {
                draw: page,
                columns: [
                    {
                        data: 'id',
                        name: '',
                        searchable: true,
                        orderable: true,
                        search: { value: '', regex: false, fixed: [] },
                    },
                    {
                        data: 'name',
                        name: '',
                        searchable: true,
                        orderable: true,
                        search: { value: '', regex: false, fixed: [] },
                    },
                    // CỘT DESCRIPTION
                    {
                        data: 'description',
                        name: '',
                        searchable: true,
                        orderable: true,
                        search: { value: '', regex: false, fixed: [] },
                    },
                    // BỎ CỘT COLOR
                    {
                        data: 'createdDate',
                        name: '',
                        searchable: true,
                        orderable: true,
                        search: { value: '', regex: false, fixed: [] },
                    },
                    {
                        data: 'id',
                        name: '',
                        searchable: true,
                        orderable: true,
                        search: { value: '', regex: false, fixed: [] },
                    },
                ],
                order: [
                    {
                        column: // CẬP NHẬT INDEX
                            sortField === 'id'
                                ? 0
                                : sortField === 'name'
                                ? 1
                                : sortField === 'description'
                                ? 2
                                : sortField === 'createdDate'
                                ? 3 // CẬP NHẬT
                                : 0,
                        dir: sortDir,
                        name: '',
                    },
                ],
                start: (page - 1) * pageSize,
                length: pageSize,
                search: { value: search, regex: false, fixed: [] },
            };

            if (filters) {
                if (filters.name) {
                    requestBody.columns[1].search.value = filters.name;
                }
                if (filters.createdDate) {
                    // CẬP NHẬT INDEX
                    requestBody.columns[3].search.value = filters.createdDate;
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
        } catch (err)
        {
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
                    placeholder="Tìm kiếm..."
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                />
                <span className="text-muted">Tổng: {totalRecords} bản ghi</span>
            </div>

            <table className="table table-hover">
                <thead>
                    <tr className="text-uppercase text-gray-500 fs-7">
                        <th>STT</th>
                        <th onClick={() => handleSort('name')} style={{ cursor: 'pointer' }}>
                            Tên {sortField === 'name' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        <th onClick={() => handleSort('description')} style={{ cursor: 'pointer' }}>
                            Mô tả {sortField === 'description' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        {/* BỎ CỘT MÀU SẮC */}
                        <th onClick={() => handleSort('createdDate')} style={{ cursor: 'pointer' }}>
                            Ngày tạo {sortField === 'createdDate' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        <th className="text-end">Thao tác</th>
                    </tr>
                </thead>
                <tbody>
                    {loading ? (
                        <tr>
                            <td colSpan={5} className="text-center">
                                {/* CẬP NHẬT COLSPAN */}
                                Đang tải...
                            </td>
                        </tr>
                    ) : data.length === 0 ? (
                        <tr>
                            <td colSpan={5} className="text-center">
                                {/* CẬP NHẬT COLSPAN */}
                                Không có dữ liệu
                            </td>
                        </tr>
                    ) : (
                        data.map((row, index) => (
                            <tr key={row.id}>
                                <td>{(page - 1) * pageSize + index + 1}</td>
                                <td>{row.name}</td>
                                <td>{row.description}</td>
                                {/* BỎ CỘT MÀU SẮC */}
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
                                <p>Bạn có chắc muốn xóa loại chi phí này không?</p>
                                {/* THAY ĐỔI TEXT */}
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
                        <option key={n} value={n}>
                            {n} / trang
                        </option>
                    ))}
                </select>

                <div className="btn-group" role="group" aria-label="Pagination buttons">
                    {(() => {
                        const totalPages = Math.ceil(totalRecords / pageSize);
                        if (totalPages <= 1) return null;

                        const maxVisible = 5;
                        let start = Math.max(1, page - Math.floor(maxVisible / 2));
                        let end = Math.min(totalPages, start + maxVisible - 1);
                        if (end - start < maxVisible - 1) {
                            start = Math.max(1, end - maxVisible + 1);
                        }
                        const buttons = [];

                        buttons.push(
                            <button
                                key="first"
                                className="btn btn-outline-primary"
                                disabled={page === 1}
                                onClick={() => setPage(1)}
                            >
                                «
                            </button>,
                        );
                        buttons.push(
                            <button
                                key="prev"
                                className="btn btn-outline-primary"
                                disabled={page === 1}
                                onClick={() => setPage((p) => p - 1)}
                            >
                                ‹
                            </button>,
                        );

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

                        buttons.push(
                            <button
                                key="next"
                                className="btn btn-outline-primary"
                                disabled={page >= totalPages}
                                onClick={() => setPage((p) => p + 1)}
                            >
                                ›
                            </button>,
                        );
                        buttons.push(
                            <button
                                key="last"
                                className="btn btn-outline-primary"
                                disabled={page >= totalPages}
                                onClick={() => setPage(totalPages)}
                            >
                                »
                            </button>,
                        );

                        return buttons;
                    })()}
                </div>
            </div>
        </div>
    );
}