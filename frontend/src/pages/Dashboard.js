import React, { useEffect, useRef, useState } from 'react';
// import DatePicker from "react-datepicker";
import axios from 'axios';
import { API_URL } from '~/api/api';
import { isDispatcher, isUser, isDriver } from '~/utils/permissionUtils';
import { TRIP_REQUEST_STATUS, TRIP_STATUS } from '~/utils/tripConstants';
import 'react-datepicker/dist/react-datepicker.css';

const getUserData = () => {
    try {
        const userDataString = localStorage.getItem('userData');
        const userData = JSON.parse(userDataString);
        return {
            token: userData?.resources?.accessToken || '',
            userId: userData?.resources?.id || 0,
            // Giả sử driverId được lưu sau khi đăng nhập (nếu là Lái xe)
            // Nếu không, backend sẽ phải tự tìm driverId từ userId
        };
    } catch (e) {
        console.error('Lỗi đọc localStorage:', e);
        return { token: '', userId: 0, driverId: 0 };
    }
};

const getMonthlyCounts = (data, dateField, year) => {
    const monthlyCounts = Array(12).fill(0);
    // XÓA: Dòng 'currentYear' đã bị di chuyển ra ngoài
    // const currentYear = new Date().getFullYear();

    data.forEach((item) => {
        if (item[dateField]) {
            const itemDate = new Date(item[dateField]);
            // SỬA: So sánh với 'year' được truyền vào
            if (itemDate.getFullYear() === year) {
                const monthIndex = itemDate.getMonth();
                monthlyCounts[monthIndex]++;
            }
        }
    });
    return monthlyCounts;
};

function StatCard({ title, value, iconClass }) {
    return (
        <div className="col-sm-6 col-xl-3">
            <div className="bg-light rounded d-flex align-items-center justify-content-between p-4">
                <i className={`fa ${iconClass} fa-3x text-primary`}></i>
                <div className="ms-3 text-end">
                    <p className="mb-2">{title}</p>
                    <h6 className="mb-0">{value}</h6>
                </div>
            </div>
        </div>
    );
}

export default function Dashboard() {
    const chart1Ref = useRef(null);
    const chart2Ref = useRef(null);

    // XÓA: State 'userRole' không còn cần thiết
    // const [userRole, setUserRole] = useState(null);
    const [stats, setStats] = useState({});
    const [loading, setLoading] = useState(true);
    // State để lưu tiêu đề biểu đồ, tách biệt khỏi logic vẽ
    const [chartTitles, setChartTitles] = useState({ chart1: 'Đang tải...', chart2: 'Đang tải...' });

    const monthLabels = [
        'Thg 1',
        'Thg 2',
        'Thg 3',
        'Thg 4',
        'Thg 5',
        'Thg 6',
        'Thg 7',
        'Thg 8',
        'Thg 9',
        'Thg 10',
        'Thg 11',
        'Thg 12',
    ];

    // SỬA LỖI: Gộp logic fetch và draw vào một useEffect duy nhất
    useEffect(() => {
        // 1. Xác định vai trò
        // (Không cần gán vào state)

        // 2. Định nghĩa hàm fetch và draw
        // SỬA: Bỏ tham số currentRole
        const fetchAndDrawData = async () => {
            setLoading(true);
            const { token } = getUserData();
            if (!token) {
                setLoading(false);
                console.error('Không tìm thấy token');
                return;
            }
            const headers = { Authorization: `Bearer ${token}` };

            let newStats = {};
            let newChartData = {
                chart1: { title: 'Lỗi', labels: monthLabels, data: Array(12).fill(0) },
                chart2: { title: 'Lỗi', labels: monthLabels, data: Array(12).fill(0) },
            };

            try {
                // Gọi API
                // SỬA: Dùng API /getall (vì /current không tồn tại trong file service)
                const [tripRes, reqRes] = await Promise.all([
                    axios.get(`${API_URL}/Trip/current`, { headers }),
                    axios.get(`${API_URL}/TripRequest/current`, { headers }),
                ]);

                const trips = tripRes.data.resources || [];
                const requests = reqRes.data.resources || [];

                let displayYear = new Date().getFullYear(); // Mặc định là năm hiện tại
                if (trips.length > 0) {
                    // Lấy năm từ chuyến đi đầu tiên
                    displayYear = new Date(trips[0].createdDate).getFullYear();
                } else if (requests.length > 0) {
                    // Hoặc lấy năm từ yêu cầu đầu tiên
                    displayYear = new Date(requests[0].createdDate).getFullYear();
                }

                // 4. Xử lý dữ liệu theo vai trò
                // SỬA: Dùng trực tiếp hàm isDispatcher()
                if (isDispatcher()) {
                    const [vehicleRes, driverRes] = await Promise.all([
                        axios.get(`${API_URL}/Vehicle`, { headers }),
                        axios.get(`${API_URL}/Driver`, { headers }),
                    ]);

                    newStats = {
                        'Tổng số chuyến': trips.length,
                        'Tổng số yêu cầu': requests.length,
                        'Tổng số xe': vehicleRes.data.resources?.length || 0,
                        'Tổng số lái xe': driverRes.data.resources?.length || 0,
                    };
                    newChartData.chart1 = {
                        // SỬA: Dùng displayYear
                        title: 'Số chuyến theo tháng (Năm ' + displayYear + ')',
                        labels: monthLabels,
                        // SỬA: Truyền displayYear vào hàm
                        data: getMonthlyCounts(trips, 'createdDate', displayYear),
                    };
                    newChartData.chart2 = {
                        // SỬA: Dùng displayYear
                        title: 'Số yêu cầu theo tháng (Năm ' + displayYear + ')',
                        labels: monthLabels,
                        // SỬA: Truyền displayYear vào hàm
                        data: getMonthlyCounts(requests, 'createdDate', displayYear),
                    };
                } else if (isDriver()) {
                    // SỬA: Dùng trực tiếp hàm isDriver()
                    newStats = {
                        'Số chuyến được giao': trips.length,
                        'Số chuyến đã hoàn thành': trips.filter((t) => t.tripStatusId === TRIP_STATUS.COMPLETED).length,
                        'Số chuyến đã nhận': trips.filter((t) => t.tripStatusId >= TRIP_STATUS.ACCEPTED).length,
                        'Số chuyến đã từ chối': trips.filter((t) => t.tripStatusId === TRIP_STATUS.REJECTED).length,
                    };
                    newChartData.chart1 = {
                        // SỬA: Dùng displayYear (tiêu đề có thể giữ nguyên nếu muốn)
                        title: 'Số chuyến được giao theo tháng (Năm ' + displayYear + ')',
                        labels: monthLabels,
                        // SỬA: Truyền displayYear vào hàm
                        data: getMonthlyCounts(trips, 'createdDate', displayYear),
                    };
                    newChartData.chart2 = {
                        // SỬA: Dùng displayYear
                        title: 'Số chuyến hoàn thành theo tháng (Năm ' + displayYear + ')',
                        labels: monthLabels,
                        // SỬA: Truyền displayYear vào hàm
                        data: getMonthlyCounts(
                            trips.filter((t) => t.tripStatusId === TRIP_STATUS.COMPLETED),
                            'actualEndTime',
                            displayYear,
                        ),
                    };
                } else {
                    // Mặc định là 'Người dùng' (isUser())
                    newStats = {
                        'Số yêu cầu đã tạo': requests.length,
                        'Số yêu cầu bị hủy': requests.filter(
                            (r) => r.tripRequestStatusId === TRIP_REQUEST_STATUS.CANCELLED,
                        ).length,
                        'Số yêu cầu đã duyệt': requests.filter(
                            (r) => r.tripRequestStatusId === TRIP_REQUEST_STATUS.APPROVED,
                        ).length,
                        'Số chuyến đã hoàn thành': trips.filter((t) => t.tripStatusId === TRIP_STATUS.COMPLETED).length,
                    };
                    newChartData.chart1 = {
                        // SỬA: Dùng displayYear
                        title: 'Số yêu cầu đã tạo theo tháng (Năm ' + displayYear + ')',
                        labels: monthLabels,
                        // SỬA: Truyền displayYear vào hàm
                        data: getMonthlyCounts(requests, 'createdDate', displayYear),
                    };
                    newChartData.chart2 = {
                        // SỬA: Dùng displayYear
                        title: 'Số chuyến hoàn thành theo tháng (Năm ' + displayYear + ')',
                        labels: monthLabels,
                        // SỬA: Truyền displayYear vào hàm
                        data: getMonthlyCounts(
                            trips.filter((t) => t.tripStatusId === TRIP_STATUS.COMPLETED),
                            'actualEndTime',
                            displayYear,
                        ),
                    };
                }

                setStats(newStats);
                setChartTitles({ chart1: newChartData.chart1.title, chart2: newChartData.chart2.title });

                // --- VẼ BIỂU ĐỒ ---
                if (!window.Chart) {
                    console.error('Chart.js chưa được load!');
                    setLoading(false);
                    return;
                }
                const ctx1 = document.getElementById('chart-1')?.getContext('2d');
                const ctx2 = document.getElementById('chart-2')?.getContext('2d');
                if (!ctx1 || !ctx2) {
                    setLoading(false);
                    return; // Thoát nếu canvas chưa sẵn sàng
                }

                // Hủy biểu đồ cũ (nếu có)
                if (chart1Ref.current) {
                    chart1Ref.current.destroy();
                }
                if (chart2Ref.current) {
                    chart2Ref.current.destroy();
                }

                // Vẽ biểu đồ 1
                chart1Ref.current = new window.Chart(ctx1, {
                    type: 'bar',
                    data: {
                        labels: newChartData.chart1.labels,
                        datasets: [
                            {
                                label: newChartData.chart1.title,
                                data: newChartData.chart1.data,
                                backgroundColor: 'rgba(0, 156, 255, .7)',
                            },
                        ],
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        scales: {
                            y: {
                                ticks: {
                                    stepSize: 1,
                                    precision: 0,
                                },
                            },
                        },
                    },
                });

                // Vẽ biểu đồ 2
                chart2Ref.current = new window.Chart(ctx2, {
                    type: 'line',
                    data: {
                        labels: newChartData.chart2.labels,
                        datasets: [
                            {
                                label: newChartData.chart2.title,
                                data: newChartData.chart2.data,
                                backgroundColor: 'rgba(0, 156, 255, .3)',
                                borderColor: 'rgba(0, 156, 255, .7)',
                                fill: true,
                                tension: 0.3,
                            },
                        ],
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        scales: {
                            y: {
                                ticks: {
                                    stepSize: 1,
                                    precision: 0,
                                },
                            },
                        },
                    },
                });
                // --- KẾT THÚC VẼ ---
            } catch (err) {
                console.error('Lỗi tải dữ liệu Dashboard:', err);
            } finally {
                setLoading(false);
            }
        };

        // 3. Gọi hàm
        fetchAndDrawData();

        // 4. Dọn dẹp khi unmount
        // Hàm return này sẽ chạy khi component bị hủy
        return () => {
            if (chart1Ref.current) {
                chart1Ref.current.destroy();
                chart1Ref.current = null;
            }
            if (chart2Ref.current) {
                chart2Ref.current.destroy();
                chart2Ref.current = null;
            }
        };
    }, []); // <-- Mảng rỗng, chỉ chạy 1 LẦN.

    // 5. JSX để render
    const renderStats = () => {
        if (loading) return <p className="p-4">Đang tải thống kê...</p>;

        const icons = ['fa-car', 'fa-file-text', 'fa-truck', 'fa-user-circle'];
        return Object.entries(stats).map(([title, value], index) => (
            <StatCard
                key={title}
                title={title}
                value={value}
                iconClass={icons[index % icons.length]} // Xoay vòng icon
            />
        ));
    };

    return (
        <div>
            <div className="container-fluid pt-4 px-4">
                <div className="row g-4">{renderStats()}</div>
            </div>

            <div className="container-fluid pt-4 px-4">
                <div className="row g-4">
                    <div className="col-sm-12 col-xl-6">
                        <div className="bg-light text-center rounded p-4" style={{ minHeight: '400px' }}>
                            <div className="d-flex align-items-center justify-content-between mb-4">
                                <h6 className="mb-0">{chartTitles.chart1}</h6>
                            </div>
                            <canvas id="chart-1" style={{ maxHeight: '450px' }}></canvas>
                        </div>
                    </div>
                    <div className="col-sm-12 col-xl-6">
                        <div className="bg-light text-center rounded p-4" style={{ minHeight: '400px' }}>
                            <div className="d-flex align-items-center justify-content-between mb-4">
                                <h6 className="mb-0">{chartTitles.chart2}</h6>
                            </div>
                            <canvas id="chart-2" style={{ maxHeight: '450px' }}></canvas>
                        </div>
                    </div>
                </div>
            </div>

            {/* Loại bỏ các phần không dùng đến */}
        </div>
    );
}
