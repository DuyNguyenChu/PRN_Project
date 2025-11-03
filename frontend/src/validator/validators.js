/**
 * Trả về một message lỗi nếu không hợp lệ, ngược lại trả về null.
 */

// Quy tắc: Bắt buộc nhập
export const required = (value) => {
    // Nếu giá trị là null, undefined, hoặc chuỗi rỗng (sau khi cắt khoảng trắng) thì báo lỗi
    if (value === null || value === undefined || String(value).trim() === '') {
        return 'Trường này là bắt buộc.';
    }
    return null; // Hợp lệ
};

// Quy tắc: Giới hạn độ dài tối đa
// Đây là một "higher-order function", nó nhận max và trả về một hàm validator
export const maxLength = (max) => (value) => {
    // Bỏ qua nếu giá trị rỗng (để kết hợp với `required` nếu cần)
    if (!value) {
        return null;
    }
    if (String(value).length > max) {
        return `Tối đa ${max} ký tự.`;
    }
    return null; // Hợp lệ
};

export const minLength = (min) => (value) => {
    // Bỏ qua nếu giá trị rỗng (để kết hợp với `required` nếu cần)
    if (!value) {
        return null;
    }
    if (String(value).length < min) {
        return `Tối thiểu ${min} ký tự.`;
    }
    return null; // Hợp lệ
};

// Quy tắc: Phải là một số
export const isNumber = (value) => {
    if (!value) {
        return null;
    }
    if (isNaN(Number(value))) {
        return 'Vui lòng nhập một số hợp lệ.';
    }
    return null; // Hợp lệ
};

// Bạn có thể thêm nhiều quy tắc khác ở đây...
// ví dụ: isEmail, minLength, v.v.

export const emailValidator = (value) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (value && !emailRegex.test(value)) {
        return 'Email không hợp lệ.';
    }
    return null;
};

export const phoneValidator = (value) => {
    if (!value) {
        return null; // Bỏ qua nếu rỗng
    }
    const phoneRegex = /^(0[3|5|7|8|9])+([0-9]{8})\b/;
    if (!phoneRegex.test(value)) {
        return 'Số điện thoại không hợp lệ (gồm 10 số, bắt đầu bằng 0).';
    }
    return null;
};
