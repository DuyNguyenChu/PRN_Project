import React, { useState, useEffect } from 'react';
import axios from 'axios';
import moment from 'moment';
import { API_URL } from '~/api/api';

export default function VehicleAccidentPopup(props) {
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
        showNotifyModal
    } = props;

    // ✅ fallback prop tương tự VehicleTypePopup
    const visible = typeof show !== 'undefined' ? show : true;
    const closeFn = handleClose ?? onClose ?? (() => {});
    const successFn = handleSave ?? onSuccess ?? (() => {});
    const edit = editItem ?? item ?? null;
    const apiUrl = propApiUrl ?? `${API_URL}/vehicle-accident`;
    const token =
        propToken ??
        JSON.parse(localStorage.getItem('userData'))?.resources?.accessToken;

    const [formData, setFormData] = useState({
        vehicleId: '',
        driverId: '',
        accidentDate: '',
        location: '',
        description: '',
        damageCost: '',
        status: 0
    });
    const [saving, setSaving] = useState(false);
    const [errors, setErrors] = useState({});

    // ✅ reset form khi edit thay đổi
    useEffect(() => {
        if (edit) {
            setFormData({
                vehicleId: edit.vehicleId ?? '',
                driverId: edit.driverId ?? '',
                accidentDate: edit.accidentDate
                    ? moment(edit.accidentDate).format('YYYY-MM-DD')
                    : '',
                location: edit.location ?? '',
                description: edit.description ?? '',
                damageCost: edit.damageCost ?? '',
                status: edit.status ?? 0
            });
        } else {
            setFormData({
                vehicleId: '',
                driverId: '',
                accidentDate: '',
                location: '',
                description: '',
                damageCost: '',
                status: 0
            });
        }
        setErrors({});
    }, [edit]);

    // ✅ validate đồng nhất với VehicleTypePopup (đã fix .trim)
    const validate = () => {
        const e = {};
        if (!String(formData.vehicleId || '').trim())
            e.vehicleId = 'Vui lòng nhập mã xe';
        if (!String(formData.driverId || '').trim())
            e.driverId = 'Vui lòng nhập mã tài xế';
        if (!formData.accidentDate)
            e.accidentDate = 'Vui lòng chọn ngày tai nạn';
        if (!String(formData.location || '').trim())
            e.location = 'Vui lòng nhập địa điểm';
        setErrors(e);
        return Object.keys(e).length === 0;
    };

    const onChange = (e) => {
        const { name, value } = e.target;
        setFormData((prev) => ({ ...prev, [name]: value ?? '' }));
    };

    const onSubmit = async (ev) => {
        ev.preventDefault();
        if (!validate()) return;

        setSaving(true);
        try {
            const url = `${apiUrl}` + (edit ? `/${edit.id}` : '');
            const method = edit ? axios.put : axios.post;

            await method(url, formData, {
                headers: token ? { Authorization: `Bearer ${token}` } : {}
            });

            successFn(); // ✅ reload list cha
            closeFn(); // ✅ đóng popup
        } catch (err) {
            console.error('❌ Lỗi khi lưu vụ tai nạn:', err);
            const msg =
                err.response?.data?.message ||
                err.message ||
                'Lưu thất bại, vui lòng thử lại';
            setErrors((prev) => ({ ...prev, submit: msg }));
            if (typeof showNotifyModal === 'function') {
                showNotifyModal(msg, false);
            }
        } finally {
            setSaving(false);
        }
    };

    if (!visible) return null;

    return (
        <div
            className="modal fade show d-block"
            style={{ background: 'rgba(0,0,0,0.4)' }}
        >
            <div className="modal-dialog modal-dialog-centered">
                <div className="modal-content">
                    <form onSubmit={onSubmit} noValidate>
                        <div className="modal-header">
                            <h5 className="modal-title">
                                {edit
                                    ? 'Cập nhật vụ tai nạn'
                                    : 'Thêm vụ tai nạn mới'}
                            </h5>
                            <button
                                type="button"
                                className="close"
                                onClick={closeFn}
                                disabled={saving}
                            >
                                <span>&times;</span>
                            </button>
                        </div>

                        <div className="modal-body">
                            {errors.submit && (
                                <div className="alert alert-danger">
                                    {errors.submit}
                                </div>
                            )}

                            <div className="form-group mb-2">
                                <label>Mã xe</label>
                                <input
                                    type="text"
                                    className={`form-control ${
                                        errors.vehicleId ? 'is-invalid' : ''
                                    }`}
                                    name="vehicleId"
                                    value={formData.vehicleId}
                                    onChange={onChange}
                                />
                                {errors.vehicleId && (
                                    <div className="invalid-feedback">
                                        {errors.vehicleId}
                                    </div>
                                )}
                            </div>

                            <div className="form-group mb-2">
                                <label>Mã tài xế</label>
                                <input
                                    type="text"
                                    className={`form-control ${
                                        errors.driverId ? 'is-invalid' : ''
                                    }`}
                                    name="driverId"
                                    value={formData.driverId}
                                    onChange={onChange}
                                />
                                {errors.driverId && (
                                    <div className="invalid-feedback">
                                        {errors.driverId}
                                    </div>
                                )}
                            </div>

                            <div className="form-group mb-2">
                                <label>Ngày tai nạn</label>
                                <input
                                    type="date"
                                    className={`form-control ${
                                        errors.accidentDate ? 'is-invalid' : ''
                                    }`}
                                    name="accidentDate"
                                    value={formData.accidentDate}
                                    onChange={onChange}
                                />
                                {errors.accidentDate && (
                                    <div className="invalid-feedback">
                                        {errors.accidentDate}
                                    </div>
                                )}
                            </div>

                            <div className="form-group mb-2">
                                <label>Địa điểm</label>
                                <input
                                    type="text"
                                    className={`form-control ${
                                        errors.location ? 'is-invalid' : ''
                                    }`}
                                    name="location"
                                    value={formData.location}
                                    onChange={onChange}
                                />
                                {errors.location && (
                                    <div className="invalid-feedback">
                                        {errors.location}
                                    </div>
                                )}
                            </div>

                            <div className="form-group mb-2">
                                <label>Mô tả</label>
                                <textarea
                                    className="form-control"
                                    name="description"
                                    value={formData.description}
                                    onChange={onChange}
                                />
                            </div>

                            <div className="form-group mb-2">
                                <label>Chi phí thiệt hại</label>
                                <input
                                    type="number"
                                    className="form-control"
                                    name="damageCost"
                                    value={formData.damageCost}
                                    onChange={onChange}
                                />
                            </div>

                            <div className="form-group mb-2">
                                <label>Trạng thái</label>
                                <select
                                    className="form-select"
                                    name="status"
                                    value={formData.status}
                                    onChange={onChange}
                                >
                                    <option value={0}>Chờ xử lý</option>
                                    <option value={1}>Đã duyệt</option>
                                </select>
                            </div>
                        </div>

                        <div className="modal-footer">
                            <button
                                type="button"
                                className="btn btn-secondary"
                                onClick={closeFn}
                                disabled={saving}
                            >
                                Hủy
                            </button>
                            <button
                                type="submit"
                                className="btn btn-primary"
                                disabled={saving}
                            >
                                {saving
                                    ? 'Đang lưu...'
                                    : edit
                                    ? 'Cập nhật'
                                    : 'Thêm'}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
}
