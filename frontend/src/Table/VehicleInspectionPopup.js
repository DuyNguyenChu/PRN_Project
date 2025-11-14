import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { API_URL } from '~/api/api';
import { format, parseISO } from 'date-fns';

export default function VehicleInspectionPopup(props) {
    // hỗ trợ 2 kiểu prop names:
    // popup có thể nhận (show, handleClose, handleSave, editItem)
    // hoặc (item, onClose, onSuccess, apiUrl, token, showConfirmModal, showNotifyModal)
    const {
        show,
        handleClose,
        handleSave,
        editItem,
        item,
        onClose,
        onSuccess,
        apiUrl: propApiUrl,
        token: propToken,
        showConfirmModal,
        showNotifyModal
    } = props;

    const visible = typeof show !== 'undefined' ? show : true; // nếu component được mount bởi parent có điều kiện, visible = true
    const closeFn = handleClose ?? onClose ?? (() => { });
    const successFn = handleSave ?? onSuccess ?? (() => { });
    const edit = editItem ?? item ?? null;
    const apiUrl = propApiUrl ?? `${API_URL}/vehicle-inspections`;
    const token = propToken ?? (JSON.parse(localStorage.getItem('userData'))?.resources?.accessToken);

    const [formData, setFormData] = useState({
        vehicleId: '',
        inspectionDate: format(new Date(), 'yyyy-MM-dd'),
        inspectorId: '',
        result: '',
        notes: '',
        status: 1
    });
    const [saving, setSaving] = useState(false);
    const [errors, setErrors] = useState({});
    const [vehicles, setVehicles] = useState([]);
    const [drivers, setDrivers] = useState([]);
    useEffect(() => {
        if (!visible) return;

        const fetchData = async () => {
            try {
                const usedToken =
                    token || JSON.parse(localStorage.getItem('userData'))?.resources?.accessToken;
                const headers = usedToken ? { Authorization: `Bearer ${usedToken}` } : {};

                const vehicleRes = await axios.get(`${API_URL}/Vehicle`, { headers });
                setVehicles(vehicleRes.data?.resources || []); // <--- SAFE FIX

                const driverRes = await axios.get(`${API_URL}/Driver`, { headers });
                setDrivers(driverRes.data?.resources || []); // <--- SAFE FIX

            } catch (err) {
                console.error("Lỗi tải danh sách:", err);
                setVehicles([]);
                setDrivers([]);
            }
        };

        fetchData();
    }, [show]);
    useEffect(() => {
        if (edit) {
            setFormData({
                vehicleId: edit.vehicleId ?? edit.vehicleId ?? '',
                inspectionDate: edit.inspectionDate
                    ? format(parseISO(edit.inspectionDate), 'yyyy-MM-dd')
                    : edit.createdDate
                        ? format(parseISO(edit.createdDate), 'yyyy-MM-dd')
                        : format(new Date(), 'yyyy-MM-dd'),
                inspectorId: edit.inspectorId ?? edit.createdBy ?? '',
                result: edit.result ?? edit.description ?? '',
                notes: edit.notes ?? '',
                status: edit.status ?? edit.vehicleStatusId ?? 1
            });
        } else {
            setFormData({
                vehicleId: '',
                inspectionDate: format(new Date(), 'yyyy-MM-dd'),
                inspectorId: '',
                result: '',
                notes: '',
                status: 1
            });
        }
        setErrors({});
    }, [edit]);

    const validate = () => {
        const e = {};
        if (!formData.vehicleId) e.vehicleId = 'Vui lòng chọn xe';
        if (!formData.inspectionDate) e.inspectionDate = 'Vui lòng chọn ngày';
        if (!formData.inspectorId) e.inspectorId = 'Vui lòng nhập người kiểm định';
        if (!formData.result) e.result = 'Vui lòng nhập kết quả';
        setErrors(e);
        return Object.keys(e).length === 0;
    };

    const onChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const onSubmit = async (ev) => {
        ev.preventDefault();
        if (!validate()) return;
        setSaving(true);
        try {
            const url = `${apiUrl}` + (edit ? `/${edit.id}` : '');
            const method = edit ? axios.put : axios.post;
            await method(url, {
                vehicleId: Number(formData.vehicleId),
                inspectionDate: formData.inspectionDate,
                inspectorId: formData.inspectorId ? Number(formData.inspectorId) : null,
                result: formData.result,
                notes: formData.notes,
                status: Number(formData.status)
            }, {
                headers: token ? { Authorization: `Bearer ${token}` } : {}
            });

            successFn();
        } catch (err) {
            console.error('Lỗi khi lưu kiểm định:', err);
            setErrors(prev => ({ ...prev, submit: err.response?.data?.message || err.message || 'Lưu thất bại' }));
            if (typeof showNotifyModal === 'function') {
                showNotifyModal(err.response?.data?.message || err.message || 'Lưu thất bại', false);
            }
        } finally {
            setSaving(false);
        }
    };

    if (!visible) return null;

    return (
        <div className={`modal fade show d-block`} style={{ background: 'rgba(0,0,0,0.4)' }}>
            <div className="modal-dialog">
                <div className="modal-content">
                    <form onSubmit={onSubmit} noValidate>
                        <div className="modal-header">
                            <h5 className="modal-title">{edit ? 'Sửa kiểm định' : 'Thêm kiểm định'}</h5>
                            <button type="button" className="close" onClick={closeFn} disabled={saving}>
                                <span>&times;</span>
                            </button>
                        </div>
                        <div className="modal-body">
                            {errors.submit && <div className="alert alert-danger">{errors.submit}</div>}

                            <div className="mb-3">
                                <label className="form-label">Xe</label>
                                <select
                                    name="vehicleId"
                                    value={formData.vehicleId}
                                    onChange={onChange}
                                    className="form-select"
                                    required
                                >
                                    <option value="">-- Chọn xe --</option>
                                    {vehicles && vehicles.length > 0 &&
                                        vehicles.map(v => (
                                            <option key={v.id} value={v.id}>
                                                {v.name}
                                            </option>
                                        ))
                                    }

                                </select>
                            </div>

                            <div className="form-group mb-2">
                                <label>Ngày kiểm định</label>
                                <input
                                    type="date"
                                    className={`form-control ${errors.inspectionDate ? 'is-invalid' : ''}`}
                                    name="inspectionDate"
                                    value={formData.inspectionDate}
                                    onChange={onChange}
                                />
                                {errors.inspectionDate && <div className="invalid-feedback">{errors.inspectionDate}</div>}
                            </div>
{/* 
                            <div className="form-group mb-2">
                                <label>Người kiểm định</label>
                                <input
                                    type="text"
                                    className={`form-control ${errors.inspectorId ? 'is-invalid' : ''}`}
                                    name="inspectorId"
                                    value={formData.inspectorId}
                                    onChange={onChange}
                                />
                                {errors.inspectorId && <div className="invalid-feedback">{errors.inspectorId}</div>}
                            </div> */}

                            <div className="form-group mb-2">
                                <label className="form-label">Người kiểm định</label>
                                <select
                                    name="inspectorId"
                                    value={formData.inspectorId}
                                    onChange={onChange}
                                    className={`form-select ${errors.inspectorId ? 'is-invalid' : ''}`}
                                    required
                                >
                                    <option value="">-- Chọn người kiểm định --</option>
                                    {drivers.map(i => (
                                        <option key={i.userId} value={i.userId}>
                                            {i.fullName}
                                        </option>
                                    ))}
                                </select>
                                {errors.inspectorId && <div className="invalid-feedback">{errors.inspectorId}</div>}
                            </div>

                            <div className="form-group mb-2">
                                <label>Kết quả</label>
                                <textarea
                                    className={`form-control ${errors.result ? 'is-invalid' : ''}`}
                                    name="result"
                                    value={formData.result}
                                    onChange={onChange}
                                    rows="3"
                                />
                                {errors.result && <div className="invalid-feedback">{errors.result}</div>}
                            </div>

                            <div className="form-group mb-2">
                                <label>Ghi chú</label>
                                <textarea
                                    className="form-control"
                                    name="notes"
                                    value={formData.notes}
                                    onChange={onChange}
                                    rows="2"
                                />
                            </div>

                            <div className="form-group mb-2">
                                <label>Trạng thái</label>
                                <select className="form-control" name="status" value={formData.status} onChange={onChange}>
                                    <option value={1}>Hoạt động</option>
                                    <option value={0}>Không hoạt động</option>
                                </select>
                            </div>
                        </div>

                        <div className="modal-footer">
                            <button type="button" className="btn btn-secondary" onClick={closeFn} disabled={saving}>
                                Hủy
                            </button>
                            <button type="submit" className="btn btn-primary" disabled={saving}>
                                {saving ? 'Đang lưu...' : (edit ? 'Cập nhật' : 'Thêm')}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
}