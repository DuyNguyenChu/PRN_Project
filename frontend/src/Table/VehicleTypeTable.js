import { useEffect, useState, useCallback } from 'react';
import axios from 'axios';
import { API_URL } from '~/api/api';
import VehicleTypePopup from './VehicleTypePopup';

export default function VehicleTypeTable({ apiUrl, token, refreshFlag }) {
    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(false);
    const [showPopup, setShowPopup] = useState(false);
    const [editItem, setEditItem] = useState(null);
    const [showConfirm, setShowConfirm] = useState(false);
    const [deleteId, setDeleteId] = useState(null);
    const [message, setMessage] = useState('');

    const fetchData = useCallback(async () => {
        setLoading(true);
        try {
            const usedToken = token || JSON.parse(localStorage.getItem('userData'))?.resources?.accessToken;
            const url = apiUrl || `${API_URL}/vehicle-types`;

            const res = await axios.get(url, {
                headers: usedToken ? { Authorization: `Bearer ${usedToken}` } : {}
            });

            const payload = res.data;
            let list = [];

            if (Array.isArray(payload)) list = payload;
            else if (Array.isArray(payload.resources)) list = payload.resources;
            else if (Array.isArray(payload.data)) list = payload.data;
            else if (payload && payload.isSucceeded && Array.isArray(payload.resources)) list = payload.resources;

            setData(list);
        } catch (err) {
            console.error('Lỗi tải loại xe:', err);
            setData([]);
        } finally {
            setLoading(false);
        }
    }, [apiUrl, token, refreshFlag]);

    useEffect(() => {
        fetchData();
    }, [fetchData]);

    const handleAdd = () => {
        setEditItem(null);
        setShowPopup(true);
    };

    const handleEdit = (item) => {
        setEditItem(item);
        setShowPopup(true);
    };

    const handleDelete = (id) => {
        setDeleteId(id);
        setShowConfirm(true);
    };

    const confirmDelete = async () => {
        try {
            const usedToken = token || JSON.parse(localStorage.getItem('userData'))?.resources?.accessToken;
            await axios.delete(`${apiUrl || `${API_URL}/vehicle-types`}/${deleteId}`, {
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
            <div className="d-flex justify-content-between align-items-center mb-3">
                <h5>Danh sách loại xe</h5>
                <button className="btn btn-primary" onClick={handleAdd}>+ Thêm loại xe</button>
            </div>

            {message && <div className="alert alert-info py-2">{message}</div>}

            <table className="table table-bordered table-hover">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Tên loại xe</th>
                        <th>Màu</th>
                        <th>Mô tả</th>
                        <th>Ngày tạo</th>
                        <th className="text-end">Thao tác</th>
                    </tr>
                </thead>
                <tbody>
                    {loading ? (
                        <tr><td colSpan="6" className="text-center">Đang tải...</td></tr>
                    ) : data.length === 0 ? (
                        <tr><td colSpan="6" className="text-center">Không có dữ liệu</td></tr>
                    ) : (
                        data.map((item) => (
                            <tr key={item.id}>
                                <td>{item.id}</td>
                                <td>{item.name}</td>
                                <td>
                                    <div style={{
                                        width: 24,
                                        height: 24,
                                        backgroundColor: item.color,
                                        border: '1px solid #ccc',
                                        borderRadius: 4
                                    }} />
                                </td>
                                <td>{item.description || '-'}</td>
                                <td>{item.createdDate ? new Date(item.createdDate).toLocaleDateString('vi-VN') : '-'}</td>
                                <td className="text-end">
                                    <button className="btn btn-sm btn-primary me-2" onClick={() => handleEdit(item)}>Sửa</button>
                                    <button className="btn btn-sm btn-danger" onClick={() => handleDelete(item.id)}>Xóa</button>
                                </td>
                            </tr>
                        ))
                    )}
                </tbody>
            </table>

            {showConfirm && (
                <div className="modal fade show d-block" style={{ background: 'rgba(0,0,0,0.4)' }}>
                    <div className="modal-dialog modal-dialog-centered">
                        <div className="modal-content">
                            <div className="modal-header"><h5>Xác nhận xóa</h5></div>
                            <div className="modal-body"><p>Bạn có chắc muốn xóa loại xe này không?</p></div>
                            <div className="modal-footer">
                                <button className="btn btn-secondary" onClick={() => setShowConfirm(false)}>Hủy</button>
                                <button className="btn btn-danger" onClick={confirmDelete}>Xóa</button>
                            </div>
                        </div>
                    </div>
                </div>
            )}

            {showPopup && (
                <VehicleTypePopup
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
