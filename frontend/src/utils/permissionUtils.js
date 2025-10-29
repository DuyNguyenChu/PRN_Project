/**
 * permissionUtils.js
 * * Các hàm tiện ích để kiểm tra quyền của người dùng
 * dựa trên dữ liệu 'Permissions' được lưu trong localStorage.
 * Dữ liệu mong đợi: [{menuId: number, actionIds: Array<number>}, ...]
 */

// --- (QUAN TRỌNG) ĐỊNH NGHĨA CÁC ID HÀNH ĐỘNG ---
// Các ID này PHẢI KHỚP với ID trong CSDL của bạn (từ API Actions)
const ACTION_ID = {
    VIEW: 2,    // Giả định 'Xem' là ID 2
    CREATE: 3,  // Giả định 'Thêm' là ID 3
    UPDATE: 4,  // Giả định 'Sửa' là ID 4
    DELETE: 9,  // Giả định 'Xoá' là ID 9
    // Thêm các ID khác nếu cần (vd: APPROVE: 5)
};

/**
 * Lấy và phân tích (parse) mảng permissions từ localStorage.
 * Đảm bảo cấu trúc [{menuId: number, actionIds: Array<number>}, ...].
 * Trả về một mảng rỗng [] nếu không tìm thấy, lỗi, hoặc cấu trúc sai.
 * @returns {Array<{menuId: number, actionIds: Array<number>}>}
 */
function getStoredPermissions() {
    try {
        // Đọc key 'Permissions'
        const permissionsString = localStorage.getItem('Permissions');
        if (!permissionsString) {
            console.warn("Không tìm thấy key 'Permissions' trong localStorage.");
            return []; // Không có key
        }

        const permissionsData = JSON.parse(permissionsString);

        // --- (SỬA) Kiểm tra cấu trúc dữ liệu ---
        if (!Array.isArray(permissionsData)) {
            console.warn("'Permissions' trong localStorage không phải là một mảng.");
            return [];
        }

        // Lọc và chuẩn hóa dữ liệu: đảm bảo mỗi item là object hợp lệ
        const validPermissions = permissionsData
            .map(item => {
                // Đảm bảo item là object, có menuId và actionIds là mảng
                if (typeof item !== 'object' || item === null ||
                    typeof item.menuId === 'undefined' || !Array.isArray(item.actionIds)) {
                    return null; // Bỏ qua item không hợp lệ
                }
                // Chuẩn hóa kiểu dữ liệu
                const menuId = Number(item.menuId);
                const actionIds = item.actionIds
                                    .map(id => Number(id))
                                    .filter(id => !isNaN(id)); // Chỉ giữ lại các số hợp lệ

                // Chỉ trả về nếu menuId hợp lệ
                return !isNaN(menuId) ? { menuId, actionIds } : null;
            })
            .filter(item => item !== null); // Loại bỏ các item null (không hợp lệ)

        return validPermissions;
        // --- Kết thúc kiểm tra cấu trúc ---

    } catch (e) {
        console.error("Lỗi parse 'Permissions' từ localStorage:", e);
        return []; // Lỗi parse JSON
    }
}

/**
 * Hàm trợ giúp (helper) chung để kiểm tra một quyền cụ thể cho một menu cụ thể.
 * @param {number | string} menuId - ID của menu cần kiểm tra.
 * @param {number} actionId - ID của hành động (VD: 2, 3, 4, 9).
 * @returns {boolean} - True nếu có quyền, ngược lại False.
 */
function hasPermission(menuId, actionId) {
    const permissions = getStoredPermissions(); // Lấy mảng [{menuId: 1, actionIds: [2]}, ...]

    if (!permissions || permissions.length === 0) {
        return false; // Không có quyền nào
    }

    // --- (SỬA) Logic kiểm tra mới ---
    // 1. Tìm object khớp với menuId (dùng so sánh lỏng ==)
    const menuPermission = permissions.find(p => p.menuId == menuId);

    // 2. Nếu không tìm thấy object cho menuId này -> không có quyền
    if (!menuPermission) {
        return false;
    }

    // 3. Nếu tìm thấy, kiểm tra xem mảng actionIds của nó có chứa actionId cần tìm không
    return menuPermission.actionIds.includes(actionId);
    // --- Kết thúc sửa logic ---
}

// --- Các hàm export chính (KHÔNG THAY ĐỔI) ---
// Các hàm này giờ sẽ gọi `hasPermission` đã được sửa đổi và hoạt động đúng

/**
 * Kiểm tra quyền XEM (VIEW) cho một menu.
 * @param {number | string} menuId - ID của menu
 * @returns {boolean}
 */
export function canView(menuId) {
    return hasPermission(menuId, ACTION_ID.VIEW);
}

/**
 * Kiểm tra quyền THÊM MỚI (CREATE) cho một menu.
 * @param {number | string} menuId - ID của menu
 * @returns {boolean}
 */
export function canCreate(menuId) {
    return hasPermission(menuId, ACTION_ID.CREATE);
}

/**
 * Kiểm tra quyền SỬA (UPDATE) cho một menu.
 * @param {number | string} menuId - ID của menu
 * @returns {boolean}
 */
export function canUpdate(menuId) {
    return hasPermission(menuId, ACTION_ID.UPDATE);
}

/**
 * Kiểm tra quyền XOÁ (DELETE) cho một menu.
 * @param {number | string} menuId - ID của menu
 * @returns {boolean}
 */
export function canDelete(menuId) {
    return hasPermission(menuId, ACTION_ID.DELETE);
}

/**
 * (Tùy chọn) Export các hằng số nếu bạn muốn dùng chúng ở nơi khác.
 */
// export const PERMISSION_ACTIONS = ACTION_ID;

