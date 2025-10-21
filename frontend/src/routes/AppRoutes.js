import { Routes, Route, Navigate, useLocation } from 'react-router-dom';
import Dashboard from '../pages/Dashboard';
import Charts from '../pages/Charts';
import Tables from '../pages/Tables';
import Forms from '../pages/Forms';
import Buttons from '../pages/Buttons';
import Login from '../pages/Login';
import Register from '../pages/Register';
import Typography from '../pages/Typography';
import Elements from '../pages/Elements';
import Widget from '../pages/Widget';
import Error from '../pages/Error';
import BlankPage from '../pages/BlankPage';
import UserStatus from '../pages/UserStatus';
import Action from '../pages/Action';
import Role from '../pages/Role';

import MainLayout from '../Layout/MainLayout';
import AuthLayout from '../Layout/AuthLayout';

function AppRoutes() {
    /**
     * Component PrivateRoute được định nghĩa ngay trong file này
     * để kiểm tra trạng thái đăng nhập của người dùng.
     * @param {{ children: JSX.Element }} props
     */
    const PrivateRoute = ({ children }) => {
        const location = useLocation();

        // Kiểm tra xem có dữ liệu người dùng trong localStorage không
        const isAuthenticated = !!localStorage.getItem('userData');

        if (!isAuthenticated) {
            // Nếu chưa đăng nhập, điều hướng người dùng về trang /login
            // và gửi kèm một thông báo để trang Login có thể hiển thị
            return (
                <Navigate
                    to="/login"
                    state={{ message: 'Bạn chưa đăng nhập. Vui lòng đăng nhập để tiếp tục.' }}
                    replace
                />
            );
        }

        // Nếu đã đăng nhập, cho phép render component con (chính là MainLayout)
        return children;
    };

    const PublicRoute = ({ children }) => {
        const isAuthenticated = !!localStorage.getItem('userData');

        if (isAuthenticated) {
            // Nếu đã đăng nhập, không cho phép truy cập lại trang login/register
            return <Navigate to="/dashboard" replace />;
        }

        return children;
    };
    return (
        <Routes>
            <Route
                element={
                    <PrivateRoute>
                        <MainLayout />
                    </PrivateRoute>
                }
            >
                <Route path="/" element={<Navigate to="/dashboard" replace />} />
                <Route path="/dashboard" element={<Dashboard />} />
                <Route path="/charts" element={<Charts />} />
                <Route path="/tables" element={<Tables />} />
                <Route path="/forms" element={<Forms />} />
                <Route path="/buttons" element={<Buttons />} />
                <Route path="/typography" element={<Typography />} />
                <Route path="/elements" element={<Elements />} />
                <Route path="/widget" element={<Widget />} />
                <Route path="/error" element={<Error />} />
                <Route path="/blank" element={<BlankPage />} />
                <Route path="/userStatus" element={<UserStatus />} />
                <Route path="/action" element={<Action />} />
                <Route path="/role" element={<Role />} />
            </Route>

            {/* Nếu người dùng gõ một đường dẫn không tồn tại, chuyển về trang login */}
            <Route path="*" element={<Navigate to="/error" replace />} />

            {/* --- CÁC ROUTE CÔNG KHAI (Public Routes) --- */}
            {/* Sử dụng PublicRoute để nếu đã đăng nhập thì tự động chuyển về dashboard */}
            <Route
                element={
                    <PublicRoute>
                        <AuthLayout />
                    </PublicRoute>
                }
            >
                <Route path="/" element={<Navigate to="/login" replace />} />
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
            </Route>
        </Routes>
    );
}

export default AppRoutes;
