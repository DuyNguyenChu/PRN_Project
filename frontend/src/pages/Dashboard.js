import React, { useEffect, useRef, useState } from "react";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

export default function Dashboard() {
  const chart1Ref = useRef(null);
  const chart2Ref = useRef(null);
  //   const [selectedDate, setSelectedDate] = useState(new Date());

  useEffect(() => {
    // Load plugin jQuery datetimepicker
    if (window.$ && window.$.fn.datetimepicker) {
      window.$("#calender").datetimepicker({
        inline: true,
        format: "L",
      });
    }
  }, []);

  useEffect(() => {
    // Kiểm tra Chart.js đã load chưa
    if (!window.Chart) {
      console.error("Chart.js chưa được load!");
      return;
    }

    const ctx1 = document.getElementById("worldwide-sales");
    const ctx2 = document.getElementById("salse-revenue");

    // 🔹 Nếu biểu đồ đã tồn tại, hủy trước khi tạo mới
    if (chart1Ref.current) chart1Ref.current.destroy();
    if (chart2Ref.current) chart2Ref.current.destroy();

    // 🔸 Tạo biểu đồ 1
    chart1Ref.current = new window.Chart(ctx1, {
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

    // 🔸 Tạo biểu đồ 2
    chart2Ref.current = new window.Chart(ctx2, {
      type: "line",
      data: {
        labels: ["2016", "2017", "2018", "2019", "2020", "2021", "2022"],
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
    };
  }, []); // chỉ chạy 1 lần
  return (
    <div>
      <div className="container-fluid pt-4 px-4">
        <div className="row g-4">
          <div className="col-sm-6 col-xl-3">
            <div className="bg-light rounded d-flex align-items-center justify-content-between p-4">
              <i className="fa fa-chart-line fa-3x text-primary"></i>
              <div className="ms-3">
                <p className="mb-2">Today Sale</p>
                <h6 className="mb-0">$1234</h6>
              </div>
            </div>
          </div>
          <div className="col-sm-6 col-xl-3">
            <div className="bg-light rounded d-flex align-items-center justify-content-between p-4">
              <i className="fa fa-chart-bar fa-3x text-primary"></i>
              <div className="ms-3">
                <p className="mb-2">Total Sale</p>
                <h6 className="mb-0">$1234</h6>
              </div>
            </div>
          </div>
          <div className="col-sm-6 col-xl-3">
            <div className="bg-light rounded d-flex align-items-center justify-content-between p-4">
              <i className="fa fa-chart-area fa-3x text-primary"></i>
              <div className="ms-3">
                <p className="mb-2">Today Revenue</p>
                <h6 className="mb-0">$1234</h6>
              </div>
            </div>
          </div>
          <div className="col-sm-6 col-xl-3">
            <div className="bg-light rounded d-flex align-items-center justify-content-between p-4">
              <i className="fa fa-chart-pie fa-3x text-primary"></i>
              <div className="ms-3">
                <p className="mb-2">Total Revenue</p>
                <h6 className="mb-0">$1234</h6>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div className="container-fluid pt-4 px-4">
        <div className="row g-4">
          <div className="col-sm-12 col-xl-6">
            <div className="bg-light text-center rounded p-4">
              <div className="d-flex align-items-center justify-content-between mb-4">
                <h6 className="mb-0">Worldwide Sales</h6>
                <a href="">Show All</a>
              </div>
              <canvas id="worldwide-sales"></canvas>
            </div>
          </div>
          <div className="col-sm-12 col-xl-6">
            <div className="bg-light text-center rounded p-4">
              <div className="d-flex align-items-center justify-content-between mb-4">
                <h6 className="mb-0">Salse & Revenue</h6>
                <a href="">Show All</a>
              </div>
              <canvas id="salse-revenue"></canvas>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
{/* <div className="container-fluid pt-4 px-4">
        <div className="bg-light text-center rounded p-4">
          <div className="d-flex align-items-center justify-content-between mb-4">
            <h6 className="mb-0">Recent Salse</h6>
            <a href="">Show All</a>
          </div>
          <div className="table-responsive">
            <table className="table text-start align-middle table-bordered table-hover mb-0">
              <thead>
                <tr className="text-dark">
                  <th scope="col">
                    <input className="form-check-input" type="checkbox" />
                  </th>
                  <th scope="col">Date</th>
                  <th scope="col">Invoice</th>
                  <th scope="col">Customer</th>
                  <th scope="col">Amount</th>
                  <th scope="col">Status</th>
                  <th scope="col">Action</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td>
                    <input className="form-check-input" type="checkbox" />
                  </td>
                  <td>01 Jan 2045</td>
                  <td>INV-0123</td>
                  <td>Jhon Doe</td>
                  <td>$123</td>
                  <td>Paid</td>
                  <td>
                    <a className="btn btn-sm btn-primary" href="">
                      Detail
                    </a>
                  </td>
                </tr>
                <tr>
                  <td>
                    <input className="form-check-input" type="checkbox" />
                  </td>
                  <td>01 Jan 2045</td>
                  <td>INV-0123</td>
                  <td>Jhon Doe</td>
                  <td>$123</td>
                  <td>Paid</td>
                  <td>
                    <a className="btn btn-sm btn-primary" href="">
                      Detail
                    </a>
                  </td>
                </tr>
                <tr>
                  <td>
                    <input className="form-check-input" type="checkbox" />
                  </td>
                  <td>01 Jan 2045</td>
                  <td>INV-0123</td>
                  <td>Jhon Doe</td>
                  <td>$123</td>
                  <td>Paid</td>
                  <td>
                    <a className="btn btn-sm btn-primary" href="">
                      Detail
                    </a>
                  </td>
                </tr>
                <tr>
                  <td>
                    <input className="form-check-input" type="checkbox" />
                  </td>
                  <td>01 Jan 2045</td>
                  <td>INV-0123</td>
                  <td>Jhon Doe</td>
                  <td>$123</td>
                  <td>Paid</td>
                  <td>
                    <a className="btn btn-sm btn-primary" href="">
                      Detail
                    </a>
                  </td>
                </tr>
                <tr>
                  <td>
                    <input className="form-check-input" type="checkbox" />
                  </td>
                  <td>01 Jan 2045</td>
                  <td>INV-0123</td>
                  <td>Jhon Doe</td>
                  <td>$123</td>
                  <td>Paid</td>
                  <td>
                    <a className="btn btn-sm btn-primary" href="">
                      Detail
                    </a>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <div className="container-fluid pt-4 px-4">
        <div className="row g-4">
          <div className="col-sm-12 col-md-6 col-xl-4">
            <div className="h-100 bg-light rounded p-4">
              <div className="d-flex align-items-center justify-content-between mb-2">
                <h6 className="mb-0">Messages</h6>
                <a href="">Show All</a>
              </div>
              <div className="d-flex align-items-center border-bottom py-3">
                <img
                  className="rounded-circle flex-shrink-0"
                  src="/assets/img/user.jpg"
                  alt=""
                  style={{ width: "40px", height: "40px" }}
                />
                <div className="w-100 ms-3">
                  <div className="d-flex w-100 justify-content-between">
                    <h6 className="mb-0">Jhon Doe</h6>
                    <small>15 minutes ago</small>
                  </div>
                  <span>Short message goes here...</span>
                </div>
              </div>
              <div className="d-flex align-items-center border-bottom py-3">
                <img
                  className="rounded-circle flex-shrink-0"
                  src="/assets/img/user.jpg"
                  alt=""
                  style={{ width: "40px", height: "40px" }}
                />
                <div className="w-100 ms-3">
                  <div className="d-flex w-100 justify-content-between">
                    <h6 className="mb-0">Jhon Doe</h6>
                    <small>15 minutes ago</small>
                  </div>
                  <span>Short message goes here...</span>
                </div>
              </div>
              <div className="d-flex align-items-center border-bottom py-3">
                <img
                  className="rounded-circle flex-shrink-0"
                  src="/assets/img/user.jpg"
                  alt=""
                  style={{ width: "40px", height: "40px" }}
                />
                <div className="w-100 ms-3">
                  <div className="d-flex w-100 justify-content-between">
                    <h6 className="mb-0">Jhon Doe</h6>
                    <small>15 minutes ago</small>
                  </div>
                  <span>Short message goes here...</span>
                </div>
              </div>
              <div className="d-flex align-items-center pt-3">
                <img
                  className="rounded-circle flex-shrink-0"
                  src="/assets/img/user.jpg"
                  alt=""
                  style={{ width: "40px", height: "40px" }}
                />
                <div className="w-100 ms-3">
                  <div className="d-flex w-100 justify-content-between">
                    <h6 className="mb-0">Jhon Doe</h6>
                    <small>15 minutes ago</small>
                  </div>
                  <span>Short message goes here...</span>
                </div>
              </div>
            </div>
          </div>
          <div className="col-sm-12 col-md-6 col-xl-4">
            <div className="h-100 bg-light rounded p-4">
              <div className="d-flex align-items-center justify-content-between mb-4">
                <h6 className="mb-0">Calender</h6>
                <a href="">Show All</a>
              </div>
              <div id="calender">
                
              </div>
            </div>
          </div>
          <div className="col-sm-12 col-md-6 col-xl-4">
            <div className="h-100 bg-light rounded p-4">
              <div className="d-flex align-items-center justify-content-between mb-4">
                <h6 className="mb-0">To Do List</h6>
                <a href="">Show All</a>
              </div>
              <div className="d-flex mb-2">
                <input
                  className="form-control bg-transparent"
                  type="text"
                  placeholder="Enter task"
                />
                <button type="button" className="btn btn-primary ms-2">
                  Add
                </button>
              </div>
              <div className="d-flex align-items-center border-bottom py-2">
                <input className="form-check-input m-0" type="checkbox" />
                <div className="w-100 ms-3">
                  <div className="d-flex w-100 align-items-center justify-content-between">
                    <span>Short task goes here...</span>
                    <button className="btn btn-sm">
                      <i className="fa fa-times"></i>
                    </button>
                  </div>
                </div>
              </div>
              <div className="d-flex align-items-center border-bottom py-2">
                <input className="form-check-input m-0" type="checkbox" />
                <div className="w-100 ms-3">
                  <div className="d-flex w-100 align-items-center justify-content-between">
                    <span>Short task goes here...</span>
                    <button className="btn btn-sm">
                      <i className="fa fa-times"></i>
                    </button>
                  </div>
                </div>
              </div>
              <div className="d-flex align-items-center border-bottom py-2">
                <input
                  className="form-check-input m-0"
                  type="checkbox"
                  defaultChecked
                />
                <div className="w-100 ms-3">
                  <div className="d-flex w-100 align-items-center justify-content-between">
                    <span>
                      <del>Short task goes here...</del>
                    </span>
                    <button className="btn btn-sm text-primary">
                      <i className="fa fa-times"></i>
                    </button>
                  </div>
                </div>
              </div>
              <div className="d-flex align-items-center border-bottom py-2">
                <input className="form-check-input m-0" type="checkbox" />
                <div className="w-100 ms-3">
                  <div className="d-flex w-100 align-items-center justify-content-between">
                    <span>Short task goes here...</span>
                    <button className="btn btn-sm">
                      <i className="fa fa-times"></i>
                    </button>
                  </div>
                </div>
              </div>
              <div className="d-flex align-items-center pt-2">
                <input className="form-check-input m-0" type="checkbox" />
                <div className="w-100 ms-3">
                  <div className="d-flex w-100 align-items-center justify-content-between">
                    <span>Short task goes here...</span>
                    <button className="btn btn-sm">
                      <i className="fa fa-times"></i>
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div> */}