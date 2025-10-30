import { useState, useCallback } from 'react';

/**
 * Custom hook để quản lý trạng thái và validation của form.
 * @param {object} initialState - Giá trị khởi tạo cho các trường trong form.
 * @param {object} validationRules - Một object chứa các quy tắc validate cho từng trường.
 * @returns {object} - Gồm state, errors, và các hàm xử lý.
 */
export const useFormValidation = (initialState, validationRules) => {
    const [values, setValues] = useState(initialState);
    const [errors, setErrors] = useState({});

    // Hàm validate một trường duy nhất, dùng để phản hồi ngay lập tức khi người dùng nhập
    const validateField = useCallback(
        (name, value) => {
            const rules = validationRules[name];
            if (!rules) return null; // Không có quy tắc thì bỏ qua

            for (const rule of rules) {
                const errorMessage = rule(value);
                if (errorMessage) {
                    return errorMessage; // Trả về lỗi đầu tiên tìm thấy
                }
            }
            return null; // Không có lỗi
        },
        [validationRules],
    );

    // Xử lý khi người dùng thay đổi giá trị input
    const handleChange = useCallback(
        (event) => {
            const { name, value } = event.target;
            // Cập nhật giá trị
            setValues((prevValues) => ({
                ...prevValues,
                [name]: value,
            }));
            // Cập nhật lỗi cho trường đó ngay lập tức
            const errorMessage = validateField(name, value);
            setErrors((prevErrors) => ({
                ...prevErrors,
                [name]: errorMessage,
            }));
        },
        [validateField],
    );

    // Hàm kiểm tra toàn bộ form, dùng trước khi submit
    const validateForm = useCallback(() => {
        const newErrors = {};
        let isFormValid = true;

        for (const fieldName in validationRules) {
            const errorMessage = validateField(fieldName, values[fieldName]);
            if (errorMessage) {
                newErrors[fieldName] = errorMessage;
                isFormValid = false;
            }
        }

        setErrors(newErrors);
        return isFormValid;
    }, [values, validationRules, validateField]);

    // Kiểm tra xem có lỗi nào đang tồn tại không
    const isSubmitDisabled = Object.values(errors).some((error) => error !== null);

    return {
        values,
        errors,
        handleChange,
        validateForm,
        isSubmitDisabled,
        setValues,
    };
};
