import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useFormValidation } from '../validator/useFormValidation';
import { required } from '../validator/validators'; 
import Select from 'react-select'; 

const PENDING_STATUS = 0;

function FuelLogFormPopup({ 
    item, 
    onClose, 
    apiUrl, 
    token, 
    onSuccess, 
    showConfirmModal, 
    showNotifyModal,
    vehicleList,
    tripList,
    fuelTypeList 
}) {
    
    const findOption = (list, value) => list.find(opt => opt.value === value) || null;

    const initialState = {
        vehicleId: findOption(vehicleList, item?.vehicleId),
        tripId: findOption(tripList, item?.tripId),
        odometer: item?.odometer || '',
        fuelType: findOption(fuelTypeList, item?.fuelType),
        quantity: item?.quantity || '',
        unitPrice: item?.unitPrice || '',
        totalCost: item?.totalCost || '',
        gasStation: item?.gasStation || '',
        notes: item?.notes || '',
    };

    const validationRules = {
        odometer: [required],
        fuelType: [required],
        quantity: [required],
        unitPrice: [], 
        totalCost: [required],
        gasStation: [required],
        notes: [], 
    };

    // === THAY ĐỔI 1: Xóa setValues và setErrors ===
    // Hook của bạn không trả về 2 hàm này
    const { values, errors, handleChange, validateForm, isSubmitDisabled } = useFormValidation(
        initialState,
        validationRules,
    );

    // === THAY ĐỔI 2: Tạo state local cho lỗi logic ===
    const [vehicleError, setVehicleError] = useState(null);

    const [id, setId] = useState(item?.id || null);
    const isReadOnly = item && item.status !== PENDING_STATUS;

    // === THAY ĐỔI 3: Dùng state local 'setVehicleError' ===
    useEffect(() => {
        if (!values.tripId && !values.vehicleId) {
            setVehicleError('Phải chọn Xe nếu không thuộc Chuyến đi');
        } else {
            setVehicleError(null); // Xóa lỗi nếu đã hợp lệ
        }
    }, [values.tripId, values.vehicleId]);


    const handleSubmit = () => {
        const isFormValid = validateForm();
        
        // === THAY ĐỔI 4: Kiểm tra lỗi local ===
        if (vehicleError) {
             return; // Dừng lại nếu có lỗi logic
        }

        if (!isFormValid) return;

        onClose();
        showConfirmModal(item ? 'Bạn có chắc chắn muốn cập nhật?' : 'Bạn có chắc chắn muốn thêm mới?', async () => {
            try {
                const payload = {
                    ...values,
                    vehicleId: values.vehicleId?.value || 0, 
                    tripId: values.tripId?.value || null, 
                    fuelType: values.fuelType?.value || '', 
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

    // === THAY ĐỔI 5: Sửa hàm autoCalculateTotal ===
    // Vì không có setValues, chúng ta phải gọi handleChange 2 lần
    const autoCalculateTotal = (key, value) => {
        // 1. Cập nhật giá trị của trường đang thay đổi (quantity hoặc unitPrice)
        // (Chúng ta giả định handleChange có thể nhận 1 object event giả)
        handleChange({ target: { name: key, value: value } });

        // 2. Tính toán và cập nhật totalCost
        const qty = parseFloat(key === 'quantity' ? value : values.quantity);
        const price = parseFloat(key === 'unitPrice' ? value : values.unitPrice);

        if (qty > 0 && price > 0) {
            const total = qty * price;
            // 3. Gọi handleChange lần 2 cho totalCost
            handleChange({ target: { name: 'totalCost', value: total.toFixed(2) } });
        }
    };


    return (
        <div className="popup-overlay">
            <div className="popup-content p-4 rounded shadow bg-white" style={{maxWidth: '800px', width: '100%'}}>
                <h5>{isReadOnly ? 'Chi tiết nhật ký' : (item ? 'Cập nhật nhật ký' : 'Thêm mới nhật ký')}</h5>
                
                <div className="row mt-3">
                    {/* Cột 1 */}
                    <div className="col-md-6">
                        <div className="form-group mb-3">
                            <label>Chuyến đi (Nếu có)</label>
                            <Select 
                                options={tripList} 
                                value={values.tripId}
                                // === THAY ĐỔI 6: Sửa lại logic onChange ===
                                // Phải dùng handleChange để hook cập nhật giá trị
                                onChange={option => {
                                    handleChange({ target: { name: 'tripId', value: option } });
                                    // Nếu chọn chuyến đi, reset xe
                                    if(option) {
                                        handleChange({ target: { name: 'vehicleId', value: null } });
                                    }
                                }} 
                                isClearable 
                                isDisabled={isReadOnly}
                                placeholder="Chọn chuyến đi..."
                            />
                        </div>

                        <div className="form-group mb-3">
                            <label>Xe (Bắt buộc nếu không chọn chuyến đi)</label>
                            <Select 
                                options={vehicleList}
                                value={values.vehicleId}
                                onChange={option => handleChange({ target: { name: 'vehicleId', value: option } })}
                                isDisabled={isReadOnly || !!values.tripId} 
                                // === THAY ĐỔI 7: Hiển thị lỗi local ===
                                className={vehicleError ? 'is-invalid' : ''}
                                placeholder="Chọn xe..."
                            />
                            {/* Hiển thị lỗi hook (nếu có) hoặc lỗi local */}
                            {(errors.vehicleId || vehicleError) && <div className="text-danger mt-1">{vehicleError || errors.vehicleId}</div>}
                        </div>

                        <div className="form-group mb-3">
                            <label>Loại nhiên liệu</label>
                            <Select 
                                options={fuelTypeList} 
                                value={values.fuelType}
                                onChange={option => handleChange({ target: { name: 'fuelType', value: option } })}
                                isDisabled={isReadOnly}
                                className={errors.fuelType ? 'is-invalid' : ''}
                                placeholder="Chọn loại nhiên liệu..."
                            />
                            {errors.fuelType && <div className="text-danger mt-1">{errors.fuelType}</div>}
                        </div>
                        
                        <div className="form-group mb-3">
                            <label>Trạm xăng</label>
                            <input
                                type="text"
                                className={`form-control ${errors.gasStation ? 'is-invalid' : ''}`}
                                name="gasStation"
                                value={values.gasStation}
                                onChange={handleChange}
                                readOnly={isReadOnly}
                            />
                            {errors.gasStation && <div className="text-danger mt-1">{errors.gasStation}</div>}
                        </div>

                    </div>

                    {/* Cột 2 */}
                    <div className="col-md-6">
                        <div className="form-group mb-3">
                            <label>Số Odometer</label>
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

                        <div className="form-group mb-3">
                            <label>Số lượng</label>
                            <input
                                type="number"
                                className={`form-control ${errors.quantity ? 'is-invalid' : ''}`}
                                name="quantity"
                                value={values.quantity}
                                // === THAY ĐỔI 8: Dùng hàm autoCalculateTotal đã sửa ===
                                onChange={e => autoCalculateTotal('quantity', e.target.value)}
                                readOnly={isReadOnly}
                            />
                            {errors.quantity && <div className="text-danger mt-1">{errors.quantity}</div>}
                        </div>
                        
                        <div className="form-group mb-3">
                            <label>Đơn giá</label>
                            <input
                                type="number"
                                className={`form-control ${errors.unitPrice ? 'is-invalid' : ''}`}
                                name="unitPrice"
                                value={values.unitPrice}
                                // === THAY ĐỔI 9: Dùng hàm autoCalculateTotal đã sửa ===
                                onChange={e => autoCalculateTotal('unitPrice', e.target.value)}
                                readOnly={isReadOnly}
                            />
                            {errors.unitPrice && <div className="text-danger mt-1">{errors.unitPrice}</div>}
                        </div>
                        
                        <div className="form-group mb-3">
                            <label>Tổng tiền</label>
                            <input
                                type="number"
                                className={`form-control ${errors.totalCost ? 'is-invalid' : ''}`}
                                name="totalCost"
                                value={values.totalCost}
                                onChange={handleChange}
                                readOnly={isReadOnly} 
                            />
                            {errors.totalCost && <div className="text-danger mt-1">{errors.totalCost}</div>}
                        </div>

                    </div>

                    {/* Hàng Ghi chú */}
                    <div className="col-12">
                         <div className="form-group mb-3">
                            <label>Ghi chú</label>
                            <textarea
                                className={`form-control ${errors.notes ? 'is-invalid' : ''}`}
                                name="notes"
                                value={values.notes}
                                onChange={handleChange}
                                rows="3"
                                readOnly={isReadOnly}
                            ></textarea>
                            {errors.notes && <div className="text-danger mt-1">{errors.notes}</div>}
                        </div>
                    </div>
                </div>


                <div className="text-end mt-4">
                    <button className="btn btn-secondary me-2" onClick={onClose}>
                        {isReadOnly ? 'Đóng' : 'Hủy'}
                    </button>
                    {!isReadOnly && (
                        <button
                            className="btn btn-primary"
                            onClick={handleSubmit}
                            // === THAY ĐỔI 10: Kiểm tra cả lỗi hook và lỗi local ===
                            disabled={isSubmitDisabled || vehicleError} 
                        >
                            {item ? 'Lưu thay đổi' : 'Thêm mới'}
                        </button>
                    )}
                </div>
            </div>
        </div>
    );
}

export default FuelLogFormPopup;