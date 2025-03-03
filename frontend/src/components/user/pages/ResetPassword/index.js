import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import './ResetPassword.module.scss';

import classNames from 'classnames/bind';
import style from './ResetPassword.module.scss';
const cs = classNames.bind(style);
function ForgotPassword() {
    const [username, setUsername] = useState('');
    const [email, setEmail] = useState('');
    const [otp, setOtp] = useState('');
    const [oldPassword, setOldPassword] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [step, setStep] = useState(1);
    const [timeLeft, setTimeLeft] = useState(0);
    const [showPasswords, setShowPasswords] = useState({
        old: false,
        new: false,
        confirm: false,
    });
    const navigate = useNavigate();

    useEffect(() => {
        if (timeLeft > 0) {
            const timer = setInterval(() => {
                setTimeLeft((prev) => prev - 1);
            }, 1000);
            return () => clearInterval(timer);
        }
    }, [timeLeft]);

    const togglePasswordVisibility = (field) => {
        setShowPasswords((prev) => ({ ...prev, [field]: !prev[field] }));
    };

    const verifyUser = async () => {
        const response = await fetch('http://localhost:5180/api/account/send-verification-code', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, email }),
        });

        if (response.ok) {
            alert('Mã xác nhận đã được gửi!');
            setStep(2);
            setTimeLeft(180);
        } else {
            alert('Username hoặc Email không đúng!');
        }
    };

    const sendOtp = async () => {
        if (timeLeft > 0) return;
        await verifyUser();
    };

    const changePassword = async () => {
        if (newPassword !== confirmPassword) {
            alert('Mật khẩu xác nhận không khớp!');
            return;
        }

        const response = await fetch('http://localhost:5180/api/account/change-password', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, email, otpCode: otp, oldPassword, newPassword }),
        });

        if (response.ok) {
            alert('Đổi mật khẩu thành công!');
            navigate('/login');
        } else {
            alert('OTP không hợp lệ hoặc đã hết hạn!');
        }
    };
    return (
        <div className={cs('resetpassword')}>
            {step === 1 ? (
                <div className={cs('resetpassword-verify')}>
                    <h3 className={cs('resetpassword-title')}>Xác minh tài khoản</h3>
                    <input
                        className={cs('resetpassword-input')}
                        type="text"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        placeholder="Nhập Username"
                    />
                    <input
                        className={cs('resetpassword-input')}
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        placeholder="Nhập Email"
                    />
                    <button className={cs('resetpassword-button', 'primary')} onClick={verifyUser}>
                        Tiếp tục
                    </button>
                </div>
            ) : (
                <div className={cs('resetpassword-change')}>
                    <h3 className={cs('resetpassword-title')}>Đổi mật khẩu</h3>
                    <div className={cs('resetpassword-group')}>
                        <input
                            className={cs('resetpassword-input')}
                            type={showPasswords.old ? 'text' : 'password'}
                            value={oldPassword}
                            onChange={(e) => setOldPassword(e.target.value)}
                            placeholder="Mật khẩu cũ"
                        />
                        <span
                            className={cs('resetpassword-eye')}
                            onClick={() => togglePasswordVisibility('old')}
                        >
                            {showPasswords.old ? '🙈' : '👁'}
                        </span>
                    </div>
                    <div className={cs('resetpassword-group')}>
                        <input
                            className={cs('resetpassword-input')}
                            type={showPasswords.new ? 'text' : 'password'}
                            value={newPassword}
                            onChange={(e) => setNewPassword(e.target.value)}
                            placeholder="Mật khẩu mới"
                        />
                        <span
                            className={cs('resetpassword-eye')}
                            onClick={() => togglePasswordVisibility('new')}
                        >
                            {showPasswords.new ? '🙈' : '👁'}
                        </span>
                    </div>
                    <div className={cs('resetpassword-group')}>
                        <input
                            className={cs('resetpassword-input')}
                            type={showPasswords.confirm ? 'text' : 'password'}
                            value={confirmPassword}
                            onChange={(e) => setConfirmPassword(e.target.value)}
                            placeholder="Xác nhận mật khẩu mới"
                        />
                        <span
                            className={cs('resetpassword-eye')}
                            onClick={() => togglePasswordVisibility('confirm')}
                        >
                            {showPasswords.confirm ? '🙈' : '👁'}
                        </span>
                    </div>
                    <div className={cs('resetpassword-otp')}>
                        <input
                            className={cs('resetpassword-input')}
                            type="text"
                            value={otp}
                            onChange={(e) => setOtp(e.target.value)}
                            placeholder="Nhập OTP"
                        />
                        <button
                            className={cs('resetpassword-button', 'secondary')}
                            onClick={sendOtp}
                            disabled={timeLeft > 0}
                        >
                            {timeLeft > 0 ? `Gửi lại (${timeLeft}s)` : 'Gửi mã'}
                        </button>
                    </div>
                    <button
                        className={cs('resetpassword-button', 'primary')}
                        onClick={changePassword}
                    >
                        Đổi mật khẩu
                    </button>
                </div>
            )}
        </div>
    );
}

export default ForgotPassword;
