// CompleteTripPopup.js
import React, { useState } from 'react';

function CompleteTripPopup({ onClose, onSubmit }) {
    const [odometer, setOdometer] = useState('');
    const [error, setError] = useState('');

    const handleSubmit = () => {
        const odoValue = Number(odometer);
        if (isNaN(odoValue) || odoValue <= 0) {
            setError('Vui lòng nhập số Odometer (km) kết thúc hợp lệ.');
            return;
        }
        onSubmit(odoValue);
    };

    return (
        <div className="popup-overlay">
            <div className="popup-content p-4 rounded shadow bg-white" style={{ maxWidth: '500px', width: '100%' }}>
                <h5>Hoàn thành chuyến đi</h5>
                <p>Vui lòng nhập số Odometer (km) lúc kết thúc chuyến đi.</p>
                
                <div className="form-group mt-3">
                    <label>Số Odometer kết thúc (km)</label>
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
                        Đóng
                    </button>
                    <button className="btn btn-success" onClick={handleSubmit}>
                        Hoàn thành
                    </button>
                </div>
            </div>
        </div>
    );
}

export default CompleteTripPopup;