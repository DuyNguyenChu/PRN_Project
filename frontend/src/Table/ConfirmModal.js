import React from 'react';

function ConfirmModal({ message, onClose, onConfirm, onlyClose = false, success = true }) {
    return (
        <div className="modal fade show d-block" style={{ background: 'rgba(0,0,0,0.4)' }}>
            <div className="modal-dialog modal-dialog-centered">
                <div className="modal-content">
                    <div className="modal-header">
                        <h5 className={`modal-title ${onlyClose ? (success ? 'text-success' : 'text-danger') : ''}`}>
                            {onlyClose ? (success ? 'Thành công' : 'Thất bại') : 'Xác nhận'}
                        </h5>
                    </div>
                    <div className="modal-body">
                        <p className={onlyClose ? (success ? 'text-success' : 'text-danger') : ''}>{message}</p>
                    </div>
                    <div className="modal-footer">
                        {onlyClose ? (
                            <button className="btn btn-primary" onClick={onClose}>
                                OK
                            </button>
                        ) : (
                            <>
                                <button className="btn btn-secondary" onClick={onClose}>
                                    Hủy
                                </button>
                                <button className="btn btn-primary" onClick={onConfirm}>
                                    Có
                                </button>
                            </>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
}

export default ConfirmModal;
