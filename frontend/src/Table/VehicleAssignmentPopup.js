import { useEffect, useState } from 'react';
import axios from 'axios';
import { API_URL } from '~/api/api';

export default function VehicleAssignmentPopup({ show, handleClose, handleSave, editItem, apiUrl, token }) {
    const isEdit = !!editItem;
    const [formData, setFormData] = useState({
        vehicleId: '',
        driverId: '',
        assignmentDate: '',
        endDate: '',
        status: 0,
        notes: ''
    });
    const [error, setError] = useState('');

    useEffect(() => {
        if (editItem) {
            setFormData({
                vehicleId: editItem.vehicleId || '',
                driverId: editItem.driverId || '',
                assignmentDate: editItem.assignmentDate
                    ? editItem.assignmentDate.slice(0, 16)
                    : '',
                endDate: editItem.endDate ? editItem.endDate.slice(0, 16) : '',
                status: editItem.status ?? 0,
                notes: editItem.notes || ''
            });
        } else {
            setFormData({
                vehicleId: '',
                driverId: '',
                assignmentDate: '',
                endDate: '',
                status: 0,
                notes: ''
            });
        }
    }, [editItem]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData((prev) => ({ ...prev, [name]: value }));
    };

    const validate = () => {
        if (!formData.vehicleId || !formData.driverId || !formData.assignmentDate)
            return 'Vui lòng nhập đầy đủ thông tin bắt buộc';
        return '';
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        const errMsg = validate();
        if (errMsg) {
            setError(errMsg);
            return;
        }

        try {
            const usedToken =
                token || JSON.parse(localStorage.getItem('userData'))?.resources?.accessToken;
            const headers = usedToken ? { Authorization: `Bearer ${usedToken}` } : {};
            const url = apiUrl || `${API_URL}/vehicle-assignment`;

            if (isEdit) {
                await axios.put(`${url}/${editItem.id}`, formData, { headers });
            } else {
                await axios.post(url, formData, { headers });
            }

            handleSave();
        } catch (err) {
            setError(`Lỗi khi lưu: ${err.response?.data?.message || err.message}`);
        }
    };

    if (!show) return null;

    return (
        <div className="modal fade show d-block" style={{ background: 'rgba(0,0,0,0.5)' }}>
            <div className="modal-dialog modal-lg modal-dialog-centered">
                <div className="modal-content">
                    <div className="modal-header">
                        <h5 className="modal-title">{isEdit ? 'Cập nhật phân công' : 'Thêm phân công'}</h5>
                        <button className="btn-close" onClick={handleClose}></button>
                    </div>
                    <form onSubmit={handleSubmit}>
                        <div className="modal-body">
                            {error && <div className="alert alert-danger">{error}</div>}

                            <div className="mb-3">
                                <label className="form-label">ID Xe</label>
                                <input
                                    type="number"
                                    name="vehicleId"
                                    value={formData.vehicleId}
                                    onChange={handleChange}
                                    className="form-control"
                                    required
                                />
                            </div>

                            <div className="mb-3">
                                <label className="form-label">ID Tài xế</label>
                                <input
                                    type="number"
                                    name="driverId"
                                    value={formData.driverId}
                                    onChange={handleChange}
                                    className="form-control"
                                    required
                                />
                            </div>

                            <div className="mb-3">
                                <label className="form-label">Ngày bắt đầu</label>
                                <input
                                    type="datetime-local"
                                    name="assignmentDate"
                                    value={formData.assignmentDate}
                                    onChange={handleChange}
                                    className="form-control"
                                    required
                                />
                            </div>

                            <div className="mb-3">
                                <label className="form-label">Ngày kết thúc</label>
                                <input
                                    type="datetime-local"
                                    name="endDate"
                                    value={formData.endDate}
                                    onChange={handleChange}
                                    className="form-control"
                                />
                            </div>

                            <div className="mb-3">
                                <label className="form-label">Trạng thái</label>
                                <select
                                    name="status"
                                    value={formData.status}
                                    onChange={handleChange}
                                    className="form-select"
                                >
                                    <option value={0}>Kết thúc</option>
                                    <option value={1}>Đang hoạt động</option>
                                </select>
                            </div>

                            <div className="mb-3">
                                <label className="form-label">Ghi chú</label>
                                <textarea
                                    name="notes"
                                    value={formData.notes}
                                    onChange={handleChange}
                                    className="form-control"
                                    rows="3"
                                />
                            </div>
                        </div>
                        <div className="modal-footer">
                            <button className="btn btn-secondary" type="button" onClick={handleClose}>
                                Hủy
                            </button>
                            <button className="btn btn-primary" type="submit">
                                {isEdit ? 'Cập nhật' : 'Thêm mới'}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
}
