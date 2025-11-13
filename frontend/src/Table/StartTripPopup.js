// StartTripPopup.js
import React, { useState } from 'react';

// SỬA: Nhận thêm prop 'isEnRoute'
function StartTripPopup({ onClose, onSubmit, isEnRoute = false }) {
    const [odometer, setOdometer] = useState('');
    const [error, setError] = useState('');

    const handleSubmit = () => {
        const odoValue = Number(odometer);
        if (isNaN(odoValue) || odoValue <= 0) {
            setError('Vui lòng nhập số Odometer (km) bắt đầu hợp lệ.');
            return;
        }
        onSubmit(odoValue);
    };

    // SỬA: Dùng text động
    const title = isEnRoute ? 'Bắt đầu đi đến đích' : 'Bắt đầu đi đến đón';
    const description = isEnRoute 
        ? 'Vui lòng nhập số Odometer (km) hiện tại của xe để bắt đầu đi đến đích.'
        : 'Vui lòng nhập số Odometer (km) hiện tại của xe để bắt đầu đi đón khách.';

    return (
        <div className="popup-overlay">
            <div className="popup-content p-4 rounded shadow bg-white" style={{ maxWidth: '500px', width: '100%' }}>
                <h5>{title}</h5>
                <p>{description}</p>
                
                <div className="form-group mt-3">
                    <label>Số Odometer bắt đầu (km)</label>
                    <input
                        type="number"
                        className={`form-control ${error ? 'is-invalid' : ''}`}
                        value={odometer}
                        onChange={(e) => {
                            setOdometer(e.target.value);
                            if (e.target.value) setError('');
                        }}
                    />
                    {error && <div className="text-danger mt-1">{error}</div>}
                </div>

                <div className="text-end mt-4">
                    <button className="btn btn-secondary me-2" onClick={onClose}>
                        Hủy
                    </button>
                    <button className="btn btn-success" onClick={handleSubmit}>
                        Xác nhận Bắt đầu
                    </button>
                </div>
            </div>
        </div>
    );
}

export default StartTripPopup;