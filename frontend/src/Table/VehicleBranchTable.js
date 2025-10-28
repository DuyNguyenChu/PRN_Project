import { useEffect, useState, useCallback, useRef } from 'react';
import moment from 'moment';
import axios from 'axios';

export default function VehicleBranchTable({ apiUrl, token, onEdit, refreshFlag, filters }) {
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
                    {
                        data: 'color',
                        name: '',
                        searchable: true,
                        orderable: true,
                        search: { value: '', regex: false, fixed: [] },
                    },
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
                        column:
                            sortField === 'id'
                                ? 0
                                : sortField === 'name'
                                ? 1
                                : sortField === 'color'
                                ? 2
                                : sortField === 'createdDate'
                                ? 3
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
                // Lọc theo tên
                if (filters.name) {
                    requestBody.columns[1].search.value = filters.name;
                }

                // Lọc theo ngày tạo (đã được format thành chuỗi)
                if (filters.createdDate) {
                    // Cột `createdDate` có index là 3 trong mảng `columns`
                    requestBody.columns[3].search.value = filters.createdDate;
                }
            }

            // const res = await axios.post(`${apiUrl}/paged-advanced`, requestBody, {
            //     headers: {
            //         'Content-Type': 'application/json',
            //         Authorization: `Bearer ${token}`,
            //     },
            // });

            // const json = res.data;
            // if (json?.resources) {
            //     setData(json.resources.data);
            //     setTotalRecords(json.resources.recordsFiltered);
            // } else {
            //     setData([]);
            //     setTotalRecords(0);
            // }

            const response = await fetch(apiUrl, { headers: { Authorization: `Bearer ${token}` } });

            const result = await response.json();

            if (result.isSucceeded && Array.isArray(result.resources)) {
                const filteredData = result.resources.filter((item) => !item.isDeleted);

                let sortedData = [...filteredData];
                sortedData.sort((a, b) => {
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
                    return 0;
                });
                // setData(sortedData);
                const pagedData = sortedData.slice((page - 1) * pageSize, page * pageSize);

                setData(pagedData);
                setTotalRecords(sortedData.length);
            } else {
                console.error('API returned unexpected structure:', result);
            }
        } catch (err) {
            console.error('❌ Lỗi tải dữ liệu:', err);
        } finally {
            setLoading(false);
        }
    }, [apiUrl, token, page, pageSize, search, sortDir, filters]);

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
                        <th onClick={() => handleSort('createdDate')} style={{ cursor: 'pointer' }}>
                            Ngày tạo {sortField === 'createdDate' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        <th onClick={() => handleSort('lastModifiedDate')} style={{ cursor: 'pointer' }}>
                            Ngày sửa đổi cuối cùng {sortField === 'lastModifiedDate' && (sortDir === 'asc' ? '▲' : '▼')}
                        </th>
                        <th className="text-end">Thao tác</th>
                    </tr>
                </thead>
                <tbody>
                    {loading ? (
                        <tr>
                            <td colSpan={5} className="text-center">
                                Đang tải...
                            </td>
                        </tr>
                    ) : data.length === 0 ? (
                        <tr>
                            <td colSpan={5} className="text-center">
                                Không có dữ liệu
                            </td>
                        </tr>
                    ) : (
                        data.map((row, index) => (
                            <tr key={row.id}>
                                <td>{(page - 1) * pageSize + index + 1}</td>
                                <td>{row.name}</td>
                                <td>{row.description}</td>
                                <td>{moment(row.createdDate).format('DD/MM/YYYY HH:mm:ss')}</td>
                                <td>{moment(row.lastModifiedDate).format('DD/MM/YYYY HH:mm:ss')}</td>

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
                                <p>Bạn có chắc muốn xóa trạng thái này không?</p>
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
