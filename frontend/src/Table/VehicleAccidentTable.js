import { useEffect, useState, useCallback } from 'react';
import axios from 'axios';
import moment from 'moment';
import { API_URL } from '~/api/api';
import VehicleAccidentPopup from './VehicleAccidentPopup';

export default function VehicleAccidentTable({ apiUrl, token, refreshFlag }) {
    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(false);
    const [showPopup, setShowPopup] = useState(false);
    const [editItem, setEditItem] = useState(null);
    const [showConfirm, setShowConfirm] = useState(false);
    const [deleteId, setDeleteId] = useState(null);
    const [message, setMessage] = useState('');

    // 🔹 Fetch dữ liệu
    const fetchData = useCallback(async () => {
        setLoading(true);
        try {
            const usedToken = token || JSON.parse(localStorage.getItem('userData'))?.resources?.accessToken;
            const url = apiUrl || `${API_URL}/vehicle-accident`;

            const res = await axios.get(url, {
                headers: usedToken ? { Authorization: `Bearer ${usedToken}` } : {}
            });

            let list = [];
            const payload = res.data;

            if (Array.isArray(payload)) list = payload;
            else if (Array.isArray(payload.resources)) list = payload.resources;
            else if (Array.isArray(payload.data)) list = payload.data;
            else if (payload && payload.isSucceeded && Array.isArray(payload.resources)) list = payload.resources;

            setData(list);
        } catch (err) {
            console.error('❌ Lỗi tải vụ tai nạn:', err);
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
            const usedToken = token || JSON.parse(localStorage.getItem('userData'))?.resources?.accessToken;
            await axios.delete(`${apiUrl || `${API_URL}/vehicle-accident`}/${deleteId}`, {
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
                <h5>Danh sách vụ tai nạn</h5>
                <button className="btn btn-primary" onClick={handleAdd}>+ Thêm vụ tai nạn</button>
            </div>

            {/* Thông báo */}
            {message && <div className="alert alert-info py-2">{message}</div>}

            {/* Bảng */}
            <table className="table table-bordered table-hover">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>ID Xe</th>
                        <th>Ngày tai nạn</th>
                        <th>Địa điểm</th>
                        <th>Mô tả</th>
                        <th>Chi phí thiệt hại</th>
                        <th>Trạng thái</th>
                        <th className="text-end">Thao tác</th>
                    </tr>
                </thead>
                <tbody>
                    {loading ? (
                        <tr><td colSpan="8" className="text-center">Đang tải...</td></tr>
                    ) : data.length === 0 ? (
                        <tr><td colSpan="8" className="text-center">Không có dữ liệu</td></tr>
                    ) : (
                        data.map((item) => (
                            <tr key={item.id}>
                                <td>{item.id}</td>
                                <td>{item.vehicleId || '-'}</td>
                                <td>{item.accidentDate ? moment(item.accidentDate).format('DD/MM/YYYY') : '-'}</td>
                                <td>{item.location || '-'}</td>
                                <td>{item.description || '-'}</td>
                                <td>{item.damageCost ? item.damageCost.toLocaleString() + ' ₫' : '-'}</td>
                                <td>{item.status === 1 ? 'Đã duyệt' : 'Chờ xử lý'}</td>
                                <td className="text-end">
                                    <button className="btn btn-sm btn-primary me-2" onClick={() => handleEdit(item)}>Sửa</button>
                                    <button className="btn btn-sm btn-danger" onClick={() => handleDelete(item.id)}>Xóa</button>
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
                            <div className="modal-header"><h5>Xác nhận xóa</h5></div>
                            <div className="modal-body"><p>Bạn có chắc muốn xóa vụ tai nạn này không?</p></div>
                            <div className="modal-footer">
                                <button className="btn btn-secondary" onClick={() => setShowConfirm(false)}>Hủy</button>
                                <button className="btn btn-danger" onClick={confirmDelete}>Xóa</button>
                            </div>
                        </div>
                    </div>
                </div>
            )}

            {/* Popup thêm/sửa */}
            {showPopup && (
                <VehicleAccidentPopup
                    show={showPopup}
                    handleClose={() => setShowPopup(false)}
                    handleSave={() => { setShowPopup(false); fetchData(); }}
                    editItem={editItem}
                    apiUrl={apiUrl}
                    token={token}
                />
            )}
        </div>
    );
}
