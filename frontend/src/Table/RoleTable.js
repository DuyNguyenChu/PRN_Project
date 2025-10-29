import { useEffect, useState, useCallback, useRef } from 'react';
import moment from 'moment';
import axios from 'axios';

/**
 * Component quản lý danh sách Vai trò (Roles)
 * HIỂN THỊ: Dạng Thẻ (Card)
 * LOGIC: Dựa trên API POST mới (pageIndex, pageSize, keyword...)
 */
export default function RoleList({ apiUrl, onEdit, refreshFlag, filters }) {
    const [data, setData] = useState([]);
    const [totalRecords, setTotalRecords] = useState(0);
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(10); // Có thể đổi giá trị mặc định, vd: 9 (để chia hết cho 3)
    const [search, setSearch] = useState('');
    const [sortField, setSortField] = useState('createdDate');
    const [sortDir, setSortDir] = useState('desc');
    const [loading, setLoading] = useState(false);

    // Popup trạng thái
    const [showConfirm, setShowConfirm] = useState(false);
    const [showResult, setShowResult] = useState(false);
    const [resultMessage, setResultMessage] = useState('');
    const [resultSuccess, setResultSuccess] = useState(false);
    const [deleteId, setDeleteId] = useState(null);

    // Ref để theo dõi lần render đầu tiên
    const isInitialMount = useRef(true);
    // Lấy token từ localStorage
    const userDataString = localStorage.getItem('userData'); 

    // Tự động reset về trang 1 khi người dùng áp dụng bộ lọc mới
    useEffect(() => {
        if (isInitialMount.current) {
            isInitialMount.current = false;
        } else {
            setPage(1);
        }
    }, [filters]);

    // Fetch dữ liệu (Đã CẬP NHẬT hoàn toàn)
    const fetchData = useCallback(async () => {
        setLoading(true);
        try {
            // Logic lấy token từ localStorage
            const userData = JSON.parse(userDataString);
            if (!userData || !userData.resources || !userData.resources.accessToken) {
                throw new Error('Không tìm thấy token người dùng. Vui lòng đăng nhập lại.');
            }
            const token = userData.resources.accessToken;

            // Xây dựng request body mới theo API
            const requestBody = {
                pageIndex: page,
                pageSize: pageSize,
                keyword: search,
                sortType: sortDir.toUpperCase(), // 'asc' -> 'ASC'
                orderBy: sortField
            };

            // Dùng API POST (không có /paged)
            const res = await axios.post(`${apiUrl}/paged`, requestBody, {
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`, // Dùng token từ localStorage
                },
            });

            const json = res.data;
            
            // Xử lý response mới
            if (json?.resources && Array.isArray(json.resources)) {
                setData(json.resources);
                // LƯU Ý: API response mẫu không có tổng số bản ghi.
                // Phân trang cần 'totalRecords'. 
                // Giả định API trả về 'totalRecords' hoặc 'totalCount' ở cấp cao nhất.
                // Nếu không, phân trang sẽ không chính xác.
                setTotalRecords(json.totalRecords || json.totalCount || 0);
            } else {
                setData([]);
                setTotalRecords(0);
            }
        } catch (err) {
            console.error('❌ Lỗi tải dữ liệu Vai trò:', err);
            setData([]); // Đảm bảo clear data khi lỗi
            setTotalRecords(0);
        } finally {
            setLoading(false);
        }
    }, [apiUrl, page, pageSize, search, sortField, sortDir, userDataString]); // Bỏ 'filters' vì API mới không dùng

    useEffect(() => {
        fetchData();
    }, [fetchData, refreshFlag]);

    // Logic Xóa (Cập nhật để lấy token từ localStorage)
    const handleDeleteClick = (id) => {
        setDeleteId(id);
        setShowConfirm(true);
    };

    const confirmDelete = async () => {
        try {
            // Logic lấy token từ localStorage
            const userData = JSON.parse(userDataString);
            if (!userData || !userData.resources || !userData.resources.accessToken) {
                throw new Error('Không tìm thấy token người dùng. Vui lòng đăng nhập lại.');
            }
            const token = userData.resources.accessToken;

            await axios.delete(`${apiUrl}/${deleteId}`, {
                headers: { Authorization: `Bearer ${token}` }, // Dùng token từ localStorage
            });
            setResultSuccess(true);
            setResultMessage('Xóa thành công!');
        } catch (err) {
            console.error('❌ Lỗi xóa Vai trò:', err);
            setResultSuccess(false);
            setResultMessage(`Xóa thất bại: ${err.response?.data?.message || err.message}`);
        } finally {
            setShowConfirm(false);
            setShowResult(true);
            fetchData();
        }
    };

    const closeConfirm = () => setShowConfirm(false);
    const closeResult = () => setShowResult(false);

    return (
        <div>

            {/* Khu vực hiển thị Thẻ (Card) */}
            <div className="row g-5 g-xl-9">
                {/* Thẻ "Thêm mới" */}
                <div className="col-md-6 col-xl-4">
                    <div 
                        className="card h-100 border-2 border-dashed" 
                        style={{ cursor: 'pointer' }} 
                        onClick={() => onEdit(null)} // Gọi onEdit(null) để mở popup Thêm mới
                    >
                        <div className="card-body d-flex flex-column justify-content-center align-items-center p-5">
                            <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="currentColor" className="bi bi-plus-lg text-primary" viewBox="0 0 16 16">
                                <path fillRule="evenodd" d="M8 2a.5.5 0 0 1 .5.5v5h5a.5.5 0 0 1 0 1h-5v5a.5.5 0 0 1-1 0v-5h-5a.5.5 0 0 1 0-1h5v-5A.5.5 0 0 1 8 2z"/>
                            </svg>
                            <div className="fw-bold fs-5 text-primary mt-3">Thêm vai trò mới</div>
                        </div>
                    </div>
                </div>

                {/* Danh sách các thẻ vai trò (data.map) */}
                {loading ? (
                    <div className="col-12 text-center p-5">
                        <p>Đang tải dữ liệu...</p>
                    </div>
                ) : data.length === 0 ? (
                     <div className="col-12 text-center p-5">
                        <p>Không có dữ liệu</p>
                    </div>
                ) : (
                    data.map((row) => (
                        <div key={row.id} className="col-md-6 col-xl-4">
                            <div className="card h-100">
                                <div className="card-body p-5">
                                    {/* Tên Vai trò */}
                                    <h5 className="card-title text-primary mb-2">{row.name}</h5>
                                    
                                    {/* Mô tả (HIỂN THỊ TEXT) */}
                                    <p className="text-muted" style={{ minHeight: '40px' }}>
                                        {row.description || '(Không có mô tả)'}
                                    </p>

                                    {/* Hiển thị totalUser và permissions (từ API mới) */}
                                    <div className="mb-4">
                                        <div className="text-dark">
                                            <span className="fw-bold">{row.totalUser || 0}</span> người dùng
                                        </div>
                                        <div className="text-dark">
                                            {/* API response có 'permissons' (thiếu 'i') */}
                                            <span className="fw-bold">{row.permissons?.length || 0}</span> quyền
                                        </div>
                                    </div>
                                </div>
                                
                                {/* Footer: Ngày tạo và Nút Thao tác */}
                                <div className="card-footer d-flex justify-content-between align-items-center p-3">
                                    <span className="text-muted fs-7">
                                        Ngày tạo: {moment(row.createdDate).format('DD/MM/YYYY')}
                                    </span>
                                    <div>
                                        <button 
                                            className="btn btn-sm btn-light-primary btn-icon me-1" 
                                            title="Sửa"
                                            onClick={() => onEdit(row)}
                                        >
                                             {/* Icon Sửa (SVG) */}
                                            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-pencil-fill" viewBox="0 0 16 16">
                                                <path d="M12.854.146a.5.5 0 0 0-.707 0L10.5 1.793 14.207 5.5l1.647-1.646a.5.5 0 0 0 0-.708l-3-3zm.646 6.061L9.793 2.5 3.293 9H3.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.207l6.5-6.5zm-7.468 7.468A.5.5 0 0 1 6 13.5V13h-.5a.5.5 0 0 1-.5-.5V12h-.5a.5.5 0 0 1-.5-.5V11h-.5a.5.5 0 0 1-.5-.5V10h-.5a.499.499 0 0 1-.175-.032l-.179.178a.5.5 0 0 0-.11.168l-2 5a.5.5 0 0 0 .65.65l5-2a.5.5 0 0 0 .168-.11l.178-.178z"/>
                                            </svg>
                                        </button>
                                        <button 
                                            className="btn btn-sm btn-light-danger btn-icon" 
                                            title="Xóa"
                                            onClick={() => handleDeleteClick(row.id)}
                                        >
                                            {/* Icon Xóa (SVG) */}
                                            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-trash-fill" viewBox="0 0 16 16">
                                                <path d="M2.5 1a1 1 0 0 0-1 1v1a1 1 0 0 0 1 1H3v9a2 2 0 0 0 2 2h6a2 2 0 0 0 2-2V4h.5a1 1 0 0 0 1-1V2a1 1 0 0 0-1-1H10a1 1 0 0 0-1-1H7a1 1 0 0 0-1 1H2.5zm3 4a.5.5 0 0 1 .5.5v7a.5.5 0 0 1-1 0v-7a.5.5 0 0 1 .5-.5zM8 5a.5.5 0 0 1 .5.5v7a.5.5 0 0 1-1 0v-7A.5.5 0 0 1 8 5zm3 .5v7a.5.5 0 0 1-1 0v-7a.5.5 0 0 1 1 0z"/>
                                            </svg>
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    ))
                )}
            </div>
            
            {/* Modal xác nhận xóa */}
            {showConfirm && (
                <div className="modal fade show d-block" style={{ background: 'rgba(0,0,0,0.4)' }}>
                    <div className="modal-dialog modal-dialog-centered">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h5 className="modal-title">Xác nhận xóa</h5>
                            </div>
                            <div className="modal-body">
                                <p>Bạn có chắc muốn xóa vai trò này không?</p>
                            </div>
                            <div className="modal-footer">
                                <button className="btn btn-secondary" onClick={closeConfirm}>
                                    Hủy
                                </button>
                                <button className="btn btn-danger" onClick={confirmDelete}>
                                    Có, xóa
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}

            {/* Modal kết quả */}
            {showResult && (
                <div className="modal fade show d-block" style={{ background: 'rgba(0,0,0,0.4)' }}>
                    <div className="modal-dialog modal-dialog-centered">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h5 className={`modal-title ${resultSuccess ? 'text-success' : 'text-danger'}`}>
                                    {resultSuccess ? 'Thành công' : 'Thất bại'}
                                </h5>
                            </div>
                            <div className="modal-body">
                                <p>{resultMessage}</p>
                            </div>
                            <div className="modal-footer">
                                <button className="btn btn-primary" onClick={closeResult}>
                                    Đóng
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}

