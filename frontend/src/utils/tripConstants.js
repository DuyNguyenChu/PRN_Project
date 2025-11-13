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