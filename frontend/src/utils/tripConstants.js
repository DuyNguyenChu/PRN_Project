// ~/utils/tripConstants.js

// QUAN TRỌNG: Hãy đảm bảo các chuỗi (string) này khớp 100%
// với tên role trong localStorage (userData.resources.roleName)
export const ROLES = {
    USER: 'Người dùng',
    COORDINATOR: 'Điều phối viên',
};

// QUAN TRỌNG: Đảm bảo các giá trị (số) này khớp 100%
// với giá trị 'status' trả về từ API
export const TRIP_REQUEST_STATUS = {
    PENDING: 2,
    APPROVED: 3,
    REJECTED: 4,
    CANCELLED: 5,
};

export const TRIP_STATUS = {
    ASSIGNED: 4, // Đã khởi hành / Giao cho tài xế
    ACCEPTED: 5, // Lái xe đã nhận
    ARRIVING: 8, // Lái xe đang đi đến đón
    PICKED_UP: 9, // Đã đến đón
    EN_ROUTE: 10, // Đang đến đích
    ARRIVED: 11, // Đã đến đích
    COMPLETED: 12, // Hoàn thành
    CANCELLED: 15, // Đã hủy
    REJECTED: 6, // Lái xe từ chối
};