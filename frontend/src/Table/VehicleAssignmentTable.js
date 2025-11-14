import { useEffect, useState, useCallback } from 'react';
import axios from 'axios';
import moment from 'moment';
import { API_URL } from '~/api/api';
import VehicleAssignmentPopup from './VehicleAssignmentPopup';

export default function VehicleAssignmentTable({ apiUrl, token, refreshFlag }) {
    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(false);
    const [showPopup, setShowPopup] = useState(false);
    const [editItem, setEditItem] = useState(null);
    const [showConfirm, setShowConfirm] = useState(false);
    const [deleteId, setDeleteId] = useState(null);
    const [message, setMessage] = useState('');
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(10);
    const [totalRecords, setTotalRecords] = useState(0);
    // 🔹 Fetch dữ liệu
    const fetchData = useCallback(async () => {
        setLoading(true);
        try {
            const usedToken =
                token || JSON.parse(localStorage.getItem('userData'))?.resources?.accessToken;
            const url = apiUrl || `${API_URL}/vehicle-assignment`;

            const res = await axios.get(url, {
                headers: usedToken ? { Authorization: `Bearer ${usedToken}` } : {}
            });

            let list = [];
            const payload = res.data;

            if (Array.isArray(payload)) list = payload;
            else if (Array.isArray(payload.resources)) list = payload.resources;
            else if (Array.isArray(payload.data)) list = payload.data;
            else if (payload && payload.isSucceeded && Array.isArray(payload.resources))
                list = payload.resources;

            setData(list);
        } catch (err) {
            console.error('❌ Lỗi tải danh sách phân công xe:', err);
            setData([]);
        } finally {
            setLoading(false);
        }
    }, [apiUrl, token, refreshFlag]);

    useEffect(() => {
        fetchData();
    }, [fetchData]);

    // 🔹 Thêm
    const handleAdd = () => {
        setEditItem(null);
        setShowPopup(true);
    };

    // 🔹 Sửa
    const handleEdit = (item) => {
        setEditItem(item);
        setShowPopup(true);
    };

    // 🔹 Xóa
    const handleDelete = (id) => {
        setDeleteId(id);
        setShowConfirm(true);
    };

    // 🔹 Xác nhận xóa
    const confirmDelete = async () => {
        try {
            const usedToken =
                token || JSON.parse(localStorage.getItem('userData'))?.resources?.accessToken;
            await axios.delete(`${apiUrl || `${API_URL}/vehicle-assignment`}/${deleteId}`, {
                headers: usedToken ? { Authorization: `Bearer ${usedToken}` } : {}
            });
            setMessage('✅ Xóa thành công');
            fetchData();
        } catch (err) {
            setMessage(`❌ Xóa thất bại: ${err.response?.data?.message || err.message}`);
        } finally {
            setShowConfirm(false);
        }
    };

    return (
        <div>
            {/* Header */}
            <div className="d-flex justify-content-between align-items-center mb-3">
                <h5>Danh sách phân công xe</h5>
                <button className="btn btn-primary" onClick={handleAdd}>
                    + Thêm phân công
                </button>
            </div>

            {/* Thông báo */}
            {message && <div className="alert alert-info py-2">{message}</div>}

            {/* Bảng */}
            <table className="table table-bordered table-hover">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>ID Xe</th>
                        <th>ID Tài xế</th>
                        <th>Ngày bắt đầu</th>
                        <th>Ngày kết thúc</th>
                        <th>Trạng thái</th>
                        <th>Ghi chú</th>
                        <th className="text-end">Thao tác</th>
                    </tr>
                </thead>
                <tbody>
                    {loading ? (
                        <tr>
                            <td colSpan="8" className="text-center">
                                Đang tải...
                            </td>
                        </tr>
                    ) : data.length === 0 ? (
                        <tr>
                            <td colSpan="8" className="text-center">
                                Không có dữ liệu
                            </td>
                        </tr>
                    ) : (
                        data.map((item) => (
                            <tr key={item.id}>
                                <td>{item.id}</td>
                                <td>{item.vehicleId}</td>
                                <td>{item.driverId}</td>
                                <td>
                                    {item.assignmentDate
                                        ? moment(item.assignmentDate).format('DD/MM/YYYY HH:mm')
                                        : '-'}
                                </td>
                                <td>
                                    {item.endDate
                                        ? moment(item.endDate).format('DD/MM/YYYY HH:mm')
                                        : '-'}
                                </td>
                                <td>{item.status === 1 ? 'Đang hoạt động' : 'Kết thúc'}</td>
                                <td>{item.notes || '-'}</td>
                                <td className="text-end">
                                    <button
                                        className="btn btn-sm btn-primary me-2"
                                        onClick={() => handleEdit(item)}
                                    >
                                        Sửa
                                    </button>
                                    <button
                                        className="btn btn-sm btn-danger"
                                        onClick={() => handleDelete(item.id)}
                                    >
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
                <div
                    className="modal fade show d-block"
                    style={{ background: 'rgba(0,0,0,0.4)' }}
                >
                    <div className="modal-dialog modal-dialog-centered">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h5>Xác nhận xóa</h5>
                            </div>
                            <div className="modal-body">
                                <p>Bạn có chắc muốn xóa phân công này không?</p>
                            </div>
                            <div className="modal-footer">
                                <button
                                    className="btn btn-secondary"
                                    onClick={() => setShowConfirm(false)}
                                >
                                    Hủy
                                </button>
                                <button className="btn btn-danger" onClick={confirmDelete}>
                                    Xóa
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}

            {/* Popup thêm/sửa */}
            {showPopup && (
                <VehicleAssignmentPopup
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
