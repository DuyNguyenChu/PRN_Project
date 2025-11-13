import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { API_URL } from '~/api/api';
import { format, parseISO } from 'date-fns';

export default function VehicleTypePopup(props) {
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

    const visible = typeof show !== 'undefined' ? show : true;
    const closeFn = handleClose ?? onClose ?? (() => {});
    const successFn = handleSave ?? onSuccess ?? (() => {});
    const edit = editItem ?? item ?? null;
    const apiUrl = propApiUrl ?? `${API_URL}/vehicle-types`;
    const token = propToken ?? (JSON.parse(localStorage.getItem('userData'))?.resources?.accessToken);

    const [formData, setFormData] = useState({
        name: '',
        color: '#000000',
        description: ''
    });
    const [saving, setSaving] = useState(false);
    const [errors, setErrors] = useState({});

    useEffect(() => {
        if (edit) {
            setFormData({
                name: edit.name ?? '',
                color: edit.color ?? '#000000',
                description: edit.description ?? ''
            });
        } else {
            setFormData({ name: '', color: '#000000', description: '' });
        }
        setErrors({});
    }, [edit]);

    const validate = () => {
        const e = {};
        if (!formData.name.trim()) e.name = 'Vui lòng nhập tên loại xe';
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
                name: formData.name,
                color: formData.color,
                description: formData.description
            }, {
                headers: token ? { Authorization: `Bearer ${token}` } : {}
            });

            successFn();
        } catch (err) {
            console.error('Lỗi khi lưu loại xe:', err);
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
                            <h5 className="modal-title">{edit ? 'Sửa loại xe' : 'Thêm loại xe'}</h5>
                            <button type="button" className="close" onClick={closeFn} disabled={saving}>
                                <span>&times;</span>
                            </button>
                        </div>
                        <div className="modal-body">
                            {errors.submit && <div className="alert alert-danger">{errors.submit}</div>}

                            <div className="form-group mb-2">
                                <label>Tên loại xe</label>
                                <input
                                    type="text"
                                    className={`form-control ${errors.name ? 'is-invalid' : ''}`}
                                    name="name"
                                    value={formData.name}
                                    onChange={onChange}
                                />
                                {errors.name && <div className="invalid-feedback">{errors.name}</div>}
                            </div>

                            <div className="form-group mb-2">
                                <label>Màu hiển thị</label>
                                <input
                                    type="color"
                                    className="form-control form-control-color"
                                    name="color"
                                    value={formData.color}
                                    onChange={onChange}
                                />
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
