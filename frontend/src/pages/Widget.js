import React, { useEffect } from "react";

export default function Widget() {
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
    // ✅ Kiểm tra jQuery và OwlCarousel đã load
    if (window.$ && window.$.fn.owlCarousel) {
      window.$(".testimonial-carousel").owlCarousel({
        autoplay: true,
        smartSpeed: 1000,
        items: 1,
        dots: true,
        loop: true,
        nav: false,
      });
    } else {
      console.warn("Owl Carousel hoặc jQuery chưa được load!");
    }
  }, []); // chỉ chạy 1 lần khi mount

  return (
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
            <div id="calender"></div>
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
        <div className="col-sm-12 col-xl-6">
          <div className="bg-light rounded h-100 p-4">
            <h6 className="mb-4">Testimonial</h6>
            <div className="owl-carousel testimonial-carousel">
              <div className="testimonial-item text-center">
                <img
                  className="img-fluid rounded-circle mx-auto mb-4"
                  src="/assets/img/testimonial-1.jpg"
                  style={{ width: "100px", height: "100px" }}
                />
                <h5 className="mb-1">Client Name</h5>
                <p>Profession</p>
                <p className="mb-0">
                  Dolor et eos labore, stet justo sed est sed. Diam sed sed
                  dolor stet amet eirmod eos labore diam
                </p>
              </div>
              <div className="testimonial-item text-center">
                <img
                  className="img-fluid rounded-circle mx-auto mb-4"
                  src="/assets/img/testimonial-2.jpg"
                  style={{ width: "100px", height: "100px" }}
                />
                <h5 className="mb-1">Client Name</h5>
                <p>Profession</p>
                <p className="mb-0">
                  Dolor et eos labore, stet justo sed est sed. Diam sed sed
                  dolor stet amet eirmod eos labore diam
                </p>
              </div>
            </div>
          </div>
        </div>
        <div className="col-sm-12 col-xl-6">
          <div className="bg-light rounded h-100 p-4">
            <iframe
              className="position-relative rounded w-100 h-100"
              src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3001156.4288297426!2d-78.01371936852176!3d42.72876761954724!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x4ccc4bf0f123a5a9%3A0xddcfc6c1de189567!2sNew%20York%2C%20USA!5e0!3m2!1sen!2sbd!4v1603794290143!5m2!1sen!2sbd"
              style={{ border: "none" }}
              allowFullScreen // ✅ đổi từ allowfullscreen="true" -> allowFullScreen (boolean)
              aria-hidden="false"
              tabIndex="0" // ✅ đổi từ tabindex -> tabIndex
              title="Google Map"
            ></iframe>
          </div>
        </div>
      </div>
    </div>
  );
}
