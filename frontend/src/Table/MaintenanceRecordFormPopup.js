import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useFormValidation } from '../validator/useFormValidation'; 
import { required } from '../validator/validators'; 
import Select from 'react-select'; 
import DatePicker from 'react-datepicker';

const PENDING_STATUS = 0;

// Định dạng tiền tệ
const formatCurrency = (value) => {
    if (value == null) return "0 VND";
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
};

function MaintenanceRecordFormPopup({ 
    item, 
    onClose, 
    apiUrl, 
    token, 
    onSuccess, 
    showConfirmModal, 
    showNotifyModal,
    vehicleList,
    // [THAY ĐỔI] Đã xóa tripList
    serviceTypeList 
}) {
    
    const findOption = (list, value) => list.find(opt => opt.value === value) || null;

    // --- State cho Form chính ---
    const initialState = {
        vehicleId: findOption(vehicleList, item?.vehicleId),
        // [THAY ĐỔI] Đã xóa tripId
        // tripId: findOption(tripList, item?.tripId),
        odometer: item?.odometer || '',
        serviceType: findOption(serviceTypeList, item?.serviceType),
        serviceProvider: item?.serviceProvider || '',
        startTime: item?.startTime ? new Date(item.startTime) : new Date(), // DatePicker cần object Date
        endTime: item?.endTime ? new Date(item.endTime) : null,
        notes: item?.notes || '',
    };

    const validationRules = {
        vehicleId: [required],
        odometer: [required],
        serviceType: [required],
        serviceProvider: [required],
        startTime: [required],
    };

    const { values, errors, handleChange, validateForm, isSubmitDisabled } = useFormValidation(
        initialState,
        validationRules,
    );
    
    // --- State cho Form chi tiết (nested) ---
    const [details, setDetails] = useState(item?.details || []); 
    const [detailError, setDetailError] = useState(null); 
    const [totalCost, setTotalCost] = useState(0);

    // State cho 1 dòng chi tiết mới
    const [newDetail, setNewDetail] = useState({
        description: '',
        quantity: 1,
        unitPrice: 0
    });
    const [newDetailErrors, setNewDetailErrors] = useState({});

    const [id, setId] = useState(item?.id || null);
    const isReadOnly = item && item.status !== PENDING_STATUS;

    // Tính tổng tiền mỗi khi 'details' thay đổi
    useEffect(() => {
        const total = details.reduce((acc, curr) => acc + (curr.quantity * curr.unitPrice), 0);
        setTotalCost(total);
    }, [details]);
    
    // === Quản lý Form Chi Tiết ===
    const handleNewDetailChange = (e) => {
        const { name, value } = e.target;
        setNewDetail(prev => ({ ...prev, [name]: value }));
    };

    const validateNewDetail = () => {
        const err = {};
        if (!newDetail.description) err.description = "Mô tả là bắt buộc";
        if (newDetail.quantity <= 0) err.quantity = "SL phải > 0";
        if (newDetail.unitPrice <= 0) err.unitPrice = "Đơn giá phải > 0";
        setNewDetailErrors(err);
        return Object.keys(err).length === 0;
    };

    const handleAddDetail = () => {
        if (!validateNewDetail()) return;

        setDetails(prev => [...prev, newDetail]);
        setNewDetail({ description: '', quantity: 1, unitPrice: 0 });
        setNewDetailErrors({});
        setDetailError(null); 
    };

    const handleRemoveDetail = (indexToRemove) => {
        setDetails(prev => prev.filter((_, index) => index !== indexToRemove));
    };

    // === Gửi Form ===
    const handleSubmit = () => {
        const isFormValid = validateForm();
        
        if (details.length === 0) {
            setDetailError("Phải có ít nhất một chi tiết bảo dưỡng.");
            return;
        }

        if (!isFormValid || detailError) return;

        onClose();
        showConfirmModal(item ? 'Bạn có chắc chắn muốn cập nhật?' : 'Bạn có chắc chắn muốn thêm mới?', async () => {
            try {
                // Chuẩn bị payload
                const payload = {
                    ...values,
                    vehicleId: values.vehicleId?.value || 0, 
                    // [THAY ĐỔI] Đã xóa tripId
                    // tripId: values.tripId?.value || null, 
                    serviceType: values.serviceType?.value || '', 
                    details: details 
                };

                if (item) {
                    await axios.put(`${apiUrl}`, { ...payload, id }, { headers: { Authorization: `Bearer ${token}` } });
                    showNotifyModal('Cập nhật thành công!');
                } else {
                    await axios.post(apiUrl, payload, { headers: { Authorization: `Bearer ${token}` } });
                    showNotifyModal('Thêm mới thành công!');
                }
                onSuccess();
            } catch (err) {
                showNotifyModal('Thao tác thất bại: ' + (err.response?.data?.message || err.message), false);
            }
        });
    };

    return (
        <div className="popup-overlay">
            <div className="popup-content p-4 rounded shadow bg-white" style={{maxWidth: '900px', width: '100%'}}>
                <h5>{isReadOnly ? 'Chi tiết bảo dưỡng' : (item ? 'Cập nhật bảo dưỡng' : 'Thêm mới bảo dưỡng')}</h5>
                
                {/* Form chính */}
                <div className="row mt-3">
                    {/* [THAY ĐỔI] Sửa layout cột */}
                    <div className="col-md-8"> 
                        <div className="form-group mb-3">
                            <label>Xe (*)</label>
                            <Select 
                                options={vehicleList}
                                value={values.vehicleId}
                                onChange={option => handleChange({ target: { name: 'vehicleId', value: option } })}
                                isDisabled={isReadOnly}
                                className={errors.vehicleId ? 'is-invalid' : ''}
                                placeholder="Chọn xe..."
                            />
                            {errors.vehicleId && <div className="text-danger mt-1">{errors.vehicleId}</div>}
                        </div>
                    </div>
                    
                    {/* [THAY ĐỔI] Đã xóa cột Chuyến đi */}

                    <div className="col-md-4">
                        <div className="form-group mb-3">
                            <label>Số Odometer (*)</label>
                            <input
                                type="number"
                                className={`form-control ${errors.odometer ? 'is-invalid' : ''}`}
                                name="odometer"
                                value={values.odometer}
                                onChange={handleChange}
                                readOnly={isReadOnly}
                            />
                            {errors.odometer && <div className="text-danger mt-1">{errors.odometer}</div>}
                        </div>
                    </div>
                    <div className="col-md-4">
                        <div className="form-group mb-3">
                            <label>Loại dịch vụ (*)</label>
                            <Select 
                                options={serviceTypeList} 
                                value={values.serviceType}
                                onChange={option => handleChange({ target: { name: 'serviceType', value: option } })}
                                isDisabled={isReadOnly}
                                className={errors.serviceType ? 'is-invalid' : ''}
                                placeholder="Chọn loại dịch vụ..."
                            />
                            {errors.serviceType && <div className="text-danger mt-1">{errors.serviceType}</div>}
                        </div>
                    </div>
                    <div className="col-md-8">
                        <div className="form-group mb-3">
                            <label>Nơi bảo dưỡng (*)</label>
                            <input
                                type="text"
                                className={`form-control ${errors.serviceProvider ? 'is-invalid' : ''}`}
                                name="serviceProvider"
                                value={values.serviceProvider}
                                onChange={handleChange}
                                readOnly={isReadOnly}
                            />
                            {errors.serviceProvider && <div className="text-danger mt-1">{errors.serviceProvider}</div>}
                        </div>
                    </div>
                    <div className="col-md-4">
                        <div className="form-group mb-3">
                            <label>Ngày bắt đầu (*)</label>
                            <DatePicker
                                selected={values.startTime}
                                onChange={(date) => handleChange({ target: { name: 'startTime', value: date } })}
                                className={`form-control ${errors.startTime ? 'is-invalid' : ''}`}
                                dateFormat="dd/MM/yyyy HH:mm"
                                showTimeSelect
                                timeFormat="HH:mm"
                                disabled={isReadOnly}
                            />
                            {errors.startTime && <div className="text-danger mt-1">{errors.startTime}</div>}
                        </div>
                    </div>
                     <div className="col-md-4">
                        <div className="form-group mb-3">
                            <label>Ngày kết thúc</label>
                            <DatePicker
                                selected={values.endTime}
                                onChange={(date) => handleChange({ target: { name: 'endTime', value: date } })}
                                className={`form-control ${errors.endTime ? 'is-invalid' : ''}`}
                                dateFormat="dd/MM/yyyy HH:mm"
                                showTimeSelect
                                timeFormat="HH:mm"
                                isClearable
                                disabled={isReadOnly}
                            />
                             {errors.endTime && <div className="text-danger mt-1">{errors.endTime}</div>}
                        </div>
                    </div>
                     <div className="col-md-4">
                        <div className="form-group mb-3">
                            <label>Ghi chú</label>
                            <input
                                type="text"
                                className="form-control"
                                name="notes"
                                value={values.notes}
                                onChange={handleChange}
                                readOnly={isReadOnly}
                            />
                        </div>
                    </div>
                </div>

                {/* Form Chi Tiết (Không thay đổi) */}
                <hr />
                <h6 className="mb-3">Chi tiết bảo dưỡng</h6>
                <div className="table-responsive" style={{maxHeight: '250px', overflowY: 'auto'}}>
                    <table className="table table-sm">
                        <thead className="table-light" style={{position: 'sticky', top: 0}}>
                            <tr>
                                <th>Nội dung</th>
                                <th style={{width: '120px'}}>Số lượng</th>
                                <th style={{width: '150px'}}>Đơn giá</th>
                                <th style={{width: '150px'}}>Thành tiền</th>
                                {!isReadOnly && <th style={{width: '50px'}}></th>}
                            </tr>
                        </thead>
                        <tbody>
                            {details.map((detail, index) => (
                                <tr key={index}>
                                    <td>{detail.description}</td>
                                    <td>{detail.quantity}</td>
                                    <td>{formatCurrency(detail.unitPrice)}</td>
                                    <td>{formatCurrency(detail.quantity * detail.unitPrice)}</td>
                                    {!isReadOnly && (
                                        <td>
                                            <button 
                                                type="button" 
                                                className="btn btn-sm btn-outline-danger"
                                                onClick={() => handleRemoveDetail(index)}
                                            >
                                                &times;
                                            </button>
                                        </td>
                                    )}
                                </tr>
                            ))}
                        </tbody>
                        {!isReadOnly && (
                            <tfoot>
                                <tr>
                                    <td>
                                        <input
                                            type="text"
                                            name="description"
                                            placeholder="Nội dung sửa chữa"
                                            className={`form-control form-control-sm ${newDetailErrors.description ? 'is-invalid' : ''}`}
                                            value={newDetail.description}
                                            onChange={handleNewDetailChange}
                                        />
                                        {newDetailErrors.description && <div className="invalid-feedback">{newDetailErrors.description}</div>}
                                    </td>
                                    <td>
                                        <input
                                            type="number"
                                            name="quantity"
                                            className={`form-control form-control-sm ${newDetailErrors.quantity ? 'is-invalid' : ''}`}
                                            value={newDetail.quantity}
                                            onChange={handleNewDetailChange}
                                        />
                                        {newDetailErrors.quantity && <div className="invalid-feedback">{newDetailErrors.quantity}</div>}
                                    </td>
                                    <td>
                                        <input
                                            type="number"
                                            name="unitPrice"
                                            className={`form-control form-control-sm ${newDetailErrors.unitPrice ? 'is-invalid' : ''}`}
                                            value={newDetail.unitPrice}
                                            onChange={handleNewDetailChange}
                                        />
                                        {newDetailErrors.unitPrice && <div className="invalid-feedback">{newDetailErrors.unitPrice}</div>}
                                    </td>
                                    <td>
                                        {formatCurrency(newDetail.quantity * newDetail.unitPrice)}
                                    </td>
                                    <td>
                                        <button type="button" className="btn btn-sm btn-primary" onClick={handleAddDetail}>
                                            Thêm
                                        </button>
                                    </td>
                                </tr>
                            </tfoot>
                        )}
                    </table>
                </div>
                {detailError && <div className="text-danger mt-2">{detailError}</div>}
                
                <h5 className="text-end mt-3">Tổng chi phí: {formatCurrency(totalCost)}</h5>

                {/* Nút bấm (Không thay đổi) */}
                <div className="text-end mt-4">
                    <button className="btn btn-secondary me-2" onClick={onClose}>
                        {isReadOnly ? 'Đóng' : 'Hủy'}
                    </button>
                    {!isReadOnly && (
                        <button
                            className="btn btn-primary"
                            onClick={handleSubmit}
                            disabled={isSubmitDisabled || !!detailError} 
                        >
                            {item ? 'Lưu thay đổi' : 'Thêm mới'}
                        </button>
                    )}
                </div>
            </div>
        </div>
    );
}

export default MaintenanceRecordFormPopup;