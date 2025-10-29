import React from 'react';

export default function Profile() {
    return (
        <div className="container-fluid pt-4 px-4">
            <div className="row g-4">
                <div className="col-sm-12 col-xl-12">
                    <div className="bg-light rounded h-100 p-4">
                        <nav>
                            <div className="nav nav-tabs" id="nav-tab" role="tablist">
                                {/* <button
                                    className="nav-link active"
                                    id="nav-home-tab"
                                    data-bs-toggle="tab"
                                    data-bs-target="#nav-home"
                                    type="button"
                                    role="tab"
                                    aria-controls="nav-home"
                                    aria-selected="true"
                                >
                                    Home
                                </button> */}
                                <button
                                    className="nav-link active"
                                    id="nav-profile-tab"
                                    data-bs-toggle="tab"
                                    data-bs-target="#nav-profile"
                                    type="button"
                                    role="tab"
                                    aria-controls="nav-profile"
                                    aria-selected="true"
                                >
                                    Profile
                                </button>
                                <button
                                    className="nav-link"
                                    id="nav-contact-tab"
                                    data-bs-toggle="tab"
                                    data-bs-target="#nav-contact"
                                    type="button"
                                    role="tab"
                                    aria-controls="nav-contact"
                                    aria-selected="false"
                                >
                                    Đổi mật khẩu
                                </button>
                            </div>
                        </nav>
                        <div className="tab-content pt-3" id="nav-tabContent">
                            {/* <div
                                className="tab-pane fade show active"
                                id="nav-home"
                                role="tabpanel"
                                aria-labelledby="nav-home-tab"
                            >
                                Diam sea sanctus amet clita lorem sit sanctus ea elitr. Lorem rebum est elitr eos.
                                Dolores aliquyam sadipscing dolor sadipscing accusam voluptua tempor. Sanctus elitr
                                sanctus diam tempor diam aliquyam et labore clita, ipsum takimata amet est erat, accusam
                                takimata gubergren sea sanctus duo nonumy. Ipsum diam ipsum sit kasd.
                            </div> */}
                            <div
                                className="tab-pane fade show active"
                                id="nav-profile"
                                role="tabpanel"
                                aria-labelledby="nav-profile-tab"
                            >
                                <h6 className="mb-4">Thông tin người dùng</h6>
                                <div className="form-floating mb-3">
                                    <input type="text" className="form-control" id="floatingInput" placeholder="Họ" />
                                    <label htmlFor="floatingInput">Họ</label>
                                </div>
                                <div className="form-floating mb-3">
                                    <input type="text" className="form-control" id="floatingInput" placeholder="Tên" />
                                    <label htmlFor="floatingInput">Tên</label>
                                </div>
                                <div className="mb-3">
                                    <div className="btn-group" role="group">
                                        <input type="radio" className="btn-check" name="gender" id="btncheck1" autoComplete="off" />
                                        <label className="btn btn-outline-secondary" htmlFor="btncheck1">
                                            Nam
                                        </label>

                                        <input type="radio" className="btn-check" name="gender" id="btncheck2" autoComplete="off" />
                                        <label className="btn btn-outline-secondary" htmlFor="btncheck2">
                                            Nữ
                                        </label>
                                    </div>
                                </div>
                                <div className="form-floating mb-3">
                                    <input
                                        type="email"
                                        className="form-control"
                                        id="floatingInput"
                                        placeholder="name@example.com"
                                    />
                                    <label htmlFor="floatingInput">Email</label>
                                </div>
                                <div className="form-floating mb-3">
                                    <input
                                        type="text"
                                        className="form-control"
                                        id="floatingInput"
                                        placeholder="Ngày sinh"
                                    />
                                    <label htmlFor="floatingInput">Ngày sinh</label>
                                </div>
                                <div className="form-floating mb-3">
                                    <input
                                        type="text"
                                        className="form-control"
                                        id="floatingInput"
                                        placeholder="Số điện thoại"
                                    />
                                    <label htmlFor="floatingInput">Số điện thoại</label>
                                </div>
                                <button type="button" className="btn btn-primary">
                                    Cập nhật thông tin
                                </button>
                            </div>

                            <div
                                className="tab-pane fade"
                                id="nav-contact"
                                role="tabpanel"
                                aria-labelledby="nav-contact-tab"
                            >
                                <h6 className="mb-4">Đổi mật khẩu</h6>
                                <div className="form-floating mb-3">
                                    <input
                                        type="password"
                                        className="form-control"
                                        id="floatingOldPassword"
                                        placeholder="Mật khẩu cũ"
                                    />
                                    <label htmlFor="floatingOldPassword">Mật khẩu cũ</label>
                                </div>
                                <div className="form-floating mb-3">
                                    <input
                                        type="password"
                                        className="form-control"
                                        id="floatingNewPassword"
                                        placeholder="Mật khẩu mới"
                                    />
                                    <label htmlFor="floatingNewPassword">Mật khẩu mới</label>
                                </div>
                                <div className="form-floating mb-3">
                                    <input
                                        type="password"
                                        className="form-control"
                                        id="floatingPassword"
                                        placeholder="Xác nhận mật khẩu"
                                    />
                                    <label htmlFor="floatingPassword">Xác nhận mật khẩu</label>
                                </div>
                                <button type="button" className="btn btn-primary">
                                    Cập nhật mật khẩu
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
