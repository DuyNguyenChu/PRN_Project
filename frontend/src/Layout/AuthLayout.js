import React from "react";
import Spinner from "../components/Spinner";
import { Outlet } from "react-router-dom";

export default function AuthLayout() {
  return (
    <div className="container-fluid position-relative bg-white d-flex p-0">
      <Spinner />
      <Outlet />
    </div>
  );
}
