import React, { useState } from "react";
import { Outlet } from "react-router-dom";
import Sidebar from "../components/Sidebar";
import Header from "../components/Header";
import Footer from "../components/Footer";
import Spinner from "../components/Spinner";
import ToTop from "../components/ToTop";

function MainLayout() {
  // ✅ State lưu trạng thái mở/tắt sidebar
  const [sidebarOpen, setSidebarOpen] = useState(false);

  // ✅ Hàm toggle
  const toggleSidebar = () => {
    setSidebarOpen((prev) => !prev);
  };

  return (
    <div className="container-fluid position-relative bg-white d-flex p-0">
      <Spinner />
      {/* ✅ Thêm class "open" nếu sidebar đang mở */}
      <div className={`sidebar pe-4 pb-3 ${sidebarOpen ? "open" : ""}`}>
        <Sidebar />
      </div>

      <div className={`content ${sidebarOpen ? "open" : ""}`}>
        {/* ✅ Truyền hàm toggle xuống Header */}
        <Header onToggleSidebar={toggleSidebar} />
        {/* ✅ Bọc nội dung chính vào div để áp dụng flex-grow */}
        <div className="page-content">
          <Outlet />
        </div>
        <Footer />
      </div>
      <ToTop />
    </div>
  );
}

export default MainLayout;
