import React, { useState, useEffect } from "react";

export default function ToTop() {
    useEffect(() => {
        // ✅ Đảm bảo jQuery đã được load (từ public/index.html)
        const $ = window.$;
    
        if ($) {
          // Khi cuộn trang
          $(window).on("scroll.backToTop", function () {
            if ($(this).scrollTop() > 300) {
              $(".back-to-top").fadeIn("slow");
            } else {
              $(".back-to-top").fadeOut("slow");
            }
          });
    
          // Khi click nút Back-to-Top
          $(".back-to-top").on("click.backToTop", function (e) {
            e.preventDefault();
            $("html, body").animate({ scrollTop: 0 }, 1500, "easeInOutExpo");
          });
        } else {
          console.warn("⚠️ jQuery chưa được load — hãy thêm script vào public/index.html");
        }
    
        // ✅ Cleanup để tránh double binding trong React StrictMode
        return () => {
          if ($) {
            $(window).off(".backToTop");
            $(".back-to-top").off(".backToTop");
          }
        };
      }, []);
    return(
        <a
          href="#"
          className="btn btn-lg btn-primary btn-lg-square back-to-top"
        >
          <i className="bi bi-arrow-up"></i>
        </a>
    );
}

