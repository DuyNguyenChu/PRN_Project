// CancelTripPopup.js
import React, { useState } from 'react';

function CancelTripPopup({ onClose, onSubmit }) {
    const [reason, setReason] = useState('');
    const [error, setError] = useState('');

    const handleSubmit = () => {
        if (!reason.trim()) {
            setError('Vui lòng nhập lý do hủy chuyến.');
            return;
        }
        onSubmit(reason);
    };

    return (
        <div className="popup-overlay">
            <div className="popup-content p-4 rounded shadow bg-white" style={{ maxWidth: '500px', width: '100%' }}>
                <h5>Hủy chuyến đi</h5>
                <p>Bạn có chắc chắn muốn hủy chuyến đi này? Vui lòng cung cấp lý do.</p>
                
                <div className="form-group mt-3">
                    <label>Lý do hủy</label>
                    <textarea
                        className={`form-control ${error ? 'is-invalid' : ''}`}
                        rows="3"
                        value={reason}
                        onChange={(e) => {
                            setReason(e.target.value);
                            if (e.target.value.trim()) setError('');
                        }}
                    ></textarea>
                    {error && <div className="text-danger mt-1">{error}</div>}
                </div>

                <div className="text-end mt-4">
                    <button className="btn btn-secondary me-2" onClick={onClose}>
                        Đóng
                    </button>
                    <button className="btn btn-danger" onClick={handleSubmit}>
                        Xác nhận Hủy
                    </button>
                </div>
            </div>
        </div>
    );
}

export default CancelTripPopup;