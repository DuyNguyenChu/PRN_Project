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
import Vehicle from '../pages/Vehicle';
import VehicleRegistration from '../pages/VehicleRegistration';
import VehicleBranch from '../pages/VehicleBranch';
import VehicleStatus from '../pages/VehicleStatus';
import VehicleModel from '../pages/VehicleModel';
import VehicleInspection from '../pages/VehicleInspection';
import VehicleInsurance from '../pages/VehicleInsurance';
import VehicleType from '../pages/VehicleType';
import VehicleAccident from '../pages/VehicleAccident';
import VehicleAssignment from '../pages/VehicleAssignment';
import Profile from '../pages/Profile';
import User from '../pages/User';
import Menu from '../pages/Menu';
import FuelLog from '~/pages/FuelLog';
import MaintenanceRecord from '~/pages/MaintenanceRecord';
import TripRequestStatus from '~/pages/TripRequestStatus';
import TripStatus from '~/pages/TripStatus';
import TripRequest from '~/pages/TripRequest';
import Driver from '~/pages/Driver';
import TripExpense from '~/pages/TripExpense';

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
                <Route path="/Vehicle" element={<Vehicle />} />
                <Route path="/VehicleRegistration" element={<VehicleRegistration />} />
                <Route path="/VehicleBranch" element={<VehicleBranch />} />
                <Route path="/VehicleStatus" element={<VehicleStatus />} />
                <Route path="/VehicleModel" element={<VehicleModel />} />
                <Route path="/VehicleInspection" element={<VehicleInspection />} />
                <Route path="/VehicleInsurance" element={<VehicleInsurance />} />
                <Route path="/VehicleType" element={<VehicleType />} />
                <Route path="/VehicleAccident" element={<VehicleAccident />} />
                <Route path="/VehicleAssignment" element={<VehicleAssignment />} />
                <Route path="/profile" element={<Profile />} />
                <Route path="/user" element={<User />} />
                <Route path="/menu" element={<Menu />} />
                <Route path="/fuelLog" element={<FuelLog />} />
                <Route path="/maintenanceRecord" element={<MaintenanceRecord />} />
                <Route path="/tripRequestStatus" element={<TripRequestStatus />} />
                <Route path="/tripStatus" element={<TripStatus />} />
                <Route path="/tripRequest" element={<TripRequest />} />
                <Route path="/driver" element={<Driver />} />
                <Route path="/tripExpense" element={<TripExpense />} />
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
