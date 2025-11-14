import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { API_URL } from '~/api/api';
import { format, parseISO } from 'date-fns';

export default function VehicleInsurancePopup(props) {
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

    const visible = typeof show !== 'undefined' ? show : true;
    const closeFn = handleClose ?? onClose ?? (() => { });
    const successFn = handleSave ?? onSuccess ?? (() => { });
    const edit = editItem ?? item ?? null;
    const apiUrl = propApiUrl ?? `${API_URL}/vehicle-insurances`;
    const token = propToken ?? (JSON.parse(localStorage.getItem('userData'))?.resources?.accessToken);
    const [vehicles, setVehicles] = useState([]);
    useEffect(() => {
        if (!visible) {
            console.log("Popup is hidden -> Skip API");
            return;
        }
        const fetchData = async () => {
            try {
                const usedToken =
                    token || JSON.parse(localStorage.getItem('userData'))?.resources?.accessToken;
                const headers = usedToken ? { Authorization: `Bearer ${usedToken}` } : {};

                const vehicleRes = await axios.get(`${API_URL}/Vehicle`, { headers });
                setVehicles(vehicleRes.data?.resources || []); // <--- SAFE FIX

            } catch (err) {
                console.error("Lỗi tải danh sách:", err);
                setVehicles([]);
            }
        };

        fetchData();
    }, [show]);
    const [formData, setFormData] = useState({
        vehicleId: '',
        insuranceProvider: '',
        policyNumber: '',
        startDate: format(new Date(), 'yyyy-MM-dd'),
        endDate: format(new Date(), 'yyyy-MM-dd'),
        premium: 0,
        status: 1
    });
    const [saving, setSaving] = useState(false);
    const [errors, setErrors] = useState({});

    useEffect(() => {
        if (edit) {
            setFormData({
                vehicleId: edit.vehicleId ?? '',
                insuranceProvider: edit.insuranceProvider ?? '',
                policyNumber: edit.policyNumber ?? '',
                startDate: edit.startDate
                    ? format(parseISO(edit.startDate), 'yyyy-MM-dd')
                    : format(new Date(), 'yyyy-MM-dd'),
                endDate: edit.endDate
                    ? format(parseISO(edit.endDate), 'yyyy-MM-dd')
                    : format(new Date(), 'yyyy-MM-dd'),
                premium: edit.premium ?? 0,
                status: edit.status ?? 1
            });
        } else {
            setFormData({
                vehicleId: '',
                insuranceProvider: '',
                policyNumber: '',
                startDate: format(new Date(), 'yyyy-MM-dd'),
                endDate: format(new Date(), 'yyyy-MM-dd'),
                premium: 0,
                status: 1
            });
        }
        setErrors({});
    }, [edit]);


    const validate = () => {
        const e = {};
        if (!formData.vehicleId) e.vehicleId = 'Vui lòng chọn xe';
        if (!formData.insuranceProvider) e.insuranceProvider = 'Vui lòng nhập nhà cung cấp bảo hiểm';
        if (!formData.policyNumber) e.policyNumber = 'Vui lòng nhập số hợp đồng';
        if (!formData.startDate) e.startDate = 'Vui lòng chọn ngày bắt đầu';
        if (!formData.endDate) e.endDate = 'Vui lòng chọn ngày kết thúc';
        if (new Date(formData.endDate) < new Date(formData.startDate)) e.endDate = 'Ngày kết thúc không hợp lệ';
        if (formData.premium <= 0) e.premium = "Phí bảo hiểm không thể mang giá trị âm được";
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
                insuranceProvider: formData.insuranceProvider,
                policyNumber: formData.policyNumber,
                startDate: formData.startDate,
                endDate: formData.endDate,
                premium: Number(formData.premium),
                status: Number(formData.status)
            }, {
                headers: token ? { Authorization: `Bearer ${token}` } : {}
            });

            successFn();
        } catch (err) {
            console.error('Lỗi khi lưu bảo hiểm:', err);
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
                            <h5 className="modal-title">{edit ? 'Sửa bảo hiểm xe' : 'Thêm bảo hiểm xe'}</h5>
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
                                <label>Nhà cung cấp bảo hiểm</label>
                                <input
                                    type="text"
                                    className={`form-control ${errors.insuranceProvider ? 'is-invalid' : ''}`}
                                    name="insuranceProvider"
                                    value={formData.insuranceProvider}
                                    onChange={onChange}
                                />
                                {errors.insuranceProvider && <div className="invalid-feedback">{errors.insuranceProvider}</div>}
                            </div>

                            <div className="form-group mb-2">
                                <label>Số hợp đồng</label>
                                <input
                                    type="text"
                                    className={`form-control ${errors.policyNumber ? 'is-invalid' : ''}`}
                                    name="policyNumber"
                                    value={formData.policyNumber}
                                    onChange={onChange}
                                />
                                {errors.policyNumber && <div className="invalid-feedback">{errors.policyNumber}</div>}
                            </div>

                            <div className="form-group mb-2">
                                <label>Ngày bắt đầu</label>
                                <input
                                    type="date"
                                    className={`form-control ${errors.startDate ? 'is-invalid' : ''}`}
                                    name="startDate"
                                    value={formData.startDate}
                                    onChange={onChange}
                                />
                                {errors.startDate && <div className="invalid-feedback">{errors.startDate}</div>}
                            </div>

                            <div className="form-group mb-2">
                                <label>Ngày kết thúc</label>
                                <input
                                    type="date"
                                    className={`form-control ${errors.endDate ? 'is-invalid' : ''}`}
                                    name="endDate"
                                    value={formData.endDate}
                                    onChange={onChange}
                                />
                                {errors.endDate && <div className="invalid-feedback">{errors.endDate}</div>}
                            </div>

                            <div className="form-group mb-2">
                                <label>Phí bảo hiểm</label>
                                <input
                                    type="number"
                                    className={`form-control ${errors.premium ? 'is-invalid' : ''}`}
                                    name="premium"
                                    value={formData.premium}
                                    onChange={onChange}
                                />
                                {errors.premium && <div className="invalid-feedback">{errors.premium}</div>}
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
