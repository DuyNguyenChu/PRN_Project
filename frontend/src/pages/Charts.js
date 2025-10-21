import React, { useEffect, useRef } from "react";

export default function Charts() {
  const chart1Ref = useRef(null);
  const chart2Ref = useRef(null);
  const chart3Ref = useRef(null);
  const chart4Ref = useRef(null);
  const chart5Ref = useRef(null);
  const chart6Ref = useRef(null);

  useEffect(() => {
    // Kiểm tra Chart.js đã load chưa
    if (!window.Chart) {
      console.error("Chart.js chưa được load!");
      return;
    }

    const ctx1 = document.getElementById("single-line-chart");
    const ctx2 = document.getElementById("multiple-line-chart");
    const ctx3 = document.getElementById("single-bar-chart");
    const ctx4 = document.getElementById("multiple-bar-chart");
    const ctx5 = document.getElementById("pie-chart");
    const ctx6 = document.getElementById("doughnut-chart");

    // 🔹 Nếu biểu đồ đã tồn tại, hủy trước khi tạo mới
    if (chart1Ref.current) chart1Ref.current.destroy();
    if (chart2Ref.current) chart2Ref.current.destroy();
    if (chart3Ref.current) chart3Ref.current.destroy();
    if (chart4Ref.current) chart4Ref.current.destroy();
    if (chart5Ref.current) chart5Ref.current.destroy();
    if (chart6Ref.current) chart6Ref.current.destroy();

    // 🔸 Tạo biểu đồ 1
    chart1Ref.current = new window.Chart(ctx1, {
      type: "line",
      data: {
        labels: [50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150],
        datasets: [
          {
            label: "Salse",
            fill: false,
            backgroundColor: "rgba(0, 156, 255, .3)",
            data: [7, 8, 8, 9, 9, 9, 10, 11, 14, 14, 15],
          },
        ],
      },
      options: { responsive: true },
    });

    // 🔸 Tạo biểu đồ 2
    chart2Ref.current = new window.Chart(ctx2, {
      type: "line",
      data: {
        labels: [2016, 2017, 2018, 2019, 2020, 2021, 2022],
        datasets: [
          {
            label: "Salse",
            data: [15, 30, 55, 45, 70, 65, 85],
            backgroundColor: "rgba(0, 156, 255, .5)",
            fill: true,
          },
          {
            label: "Revenue",
            data: [99, 135, 170, 130, 190, 180, 270],
            backgroundColor: "rgba(0, 156, 255, .3)",
            fill: true,
          },
        ],
      },
      options: { responsive: true },
    });

    // 🔸 Tạo biểu đồ 3
    chart3Ref.current = new window.Chart(ctx3, {
      type: "bar",
      data: {
        labels: ["Italy", "France", "Spain", "USA", "Argentina"],
        datasets: [
          {
            backgroundColor: [
              "rgba(0, 156, 255, .7)",
              "rgba(0, 156, 255, .6)",
              "rgba(0, 156, 255, .5)",
              "rgba(0, 156, 255, .4)",
              "rgba(0, 156, 255, .3)",
            ],
            data: [55, 49, 44, 24, 15],
          },
        ],
      },
      options: { responsive: true },
    });

    // 🔸 Tạo biểu đồ 4
    chart4Ref.current = new window.Chart(ctx4, {
      type: "bar",
      data: {
        labels: ["2016", "2017", "2018", "2019", "2020", "2021", "2022"],
        datasets: [
          {
            label: "USA",
            data: [15, 30, 55, 65, 60, 80, 95],
            backgroundColor: "rgba(0, 156, 255, .7)",
          },
          {
            label: "UK",
            data: [8, 35, 40, 60, 70, 55, 75],
            backgroundColor: "rgba(0, 156, 255, .5)",
          },
          {
            label: "AU",
            data: [12, 25, 45, 55, 65, 70, 60],
            backgroundColor: "rgba(0, 156, 255, .3)",
          },
        ],
      },
      options: { responsive: true },
    });

    // 🔸 Tạo biểu đồ 5
    chart5Ref.current = new window.Chart(ctx5, {
      type: "pie",
      data: {
        labels: ["Italy", "France", "Spain", "USA", "Argentina"],
        datasets: [
          {
            backgroundColor: [
              "rgba(0, 156, 255, .7)",
              "rgba(0, 156, 255, .6)",
              "rgba(0, 156, 255, .5)",
              "rgba(0, 156, 255, .4)",
              "rgba(0, 156, 255, .3)",
            ],
            data: [55, 49, 44, 24, 15],
          },
        ],
      },
      options: { responsive: true },
    });

    // 🔸 Tạo biểu đồ 6
    chart6Ref.current = new window.Chart(ctx6, {
      type: "doughnut",
      data: {
        labels: ["Italy", "France", "Spain", "USA", "Argentina"],
        datasets: [
          {
            backgroundColor: [
              "rgba(0, 156, 255, .7)",
              "rgba(0, 156, 255, .6)",
              "rgba(0, 156, 255, .5)",
              "rgba(0, 156, 255, .4)",
              "rgba(0, 156, 255, .3)",
            ],
            data: [55, 49, 44, 24, 15],
          },
        ],
      },
      options: { responsive: true },
    });

    // 🔹 Dọn dẹp khi component unmount
    return () => {
      if (chart1Ref.current) {
        chart1Ref.current.destroy();
        chart1Ref.current = null;
      }
      if (chart2Ref.current) {
        chart2Ref.current.destroy();
        chart2Ref.current = null;
      }
      if (chart3Ref.current) {
        chart3Ref.current.destroy();
        chart3Ref.current = null;
      }
      if (chart4Ref.current) {
        chart4Ref.current.destroy();
        chart4Ref.current = null;
      }
      if (chart5Ref.current) {
        chart5Ref.current.destroy();
        chart5Ref.current = null;
      }
      if (chart6Ref.current) {
        chart6Ref.current.destroy();
        chart6Ref.current = null;
      }
    };
  }, []); // chỉ chạy 1 lần

  return (
    <div className="container-fluid pt-4 px-4">
      <div className="row g-4">
        <div className="col-sm-12 col-xl-6">
          <div className="bg-light rounded h-100 p-4">
            <h6 className="mb-4">Single Line Chart</h6>
            <canvas id="single-line-chart"></canvas>
          </div>
        </div>
        <div className="col-sm-12 col-xl-6">
          <div className="bg-light rounded h-100 p-4">
            <h6 className="mb-4">Multiple Line Chart</h6>
            <canvas id="multiple-line-chart"></canvas>
          </div>
        </div>
        <div className="col-sm-12 col-xl-6">
          <div className="bg-light rounded h-100 p-4">
            <h6 className="mb-4">Single Bar Chart</h6>
            <canvas id="single-bar-chart"></canvas>
          </div>
        </div>
        <div className="col-sm-12 col-xl-6">
          <div className="bg-light rounded h-100 p-4">
            <h6 className="mb-4">Multiple Bar Chart</h6>
            <canvas id="multiple-bar-chart"></canvas>
          </div>
        </div>
        <div className="col-sm-12 col-xl-6">
          <div className="bg-light rounded h-100 p-4">
            <h6 className="mb-4">Pie Chart</h6>
            <canvas id="pie-chart"></canvas>
          </div>
        </div>
        <div className="col-sm-12 col-xl-6">
          <div className="bg-light rounded h-100 p-4">
            <h6 className="mb-4">Doughnut Chart</h6>
            <canvas id="doughnut-chart"></canvas>
          </div>
        </div>
      </div>
    </div>
  );
}
