import React from "react";
import ReactDOM from "react-dom";
import Login from "../Account/pages/Login";
import LoginRedirect from "../Account/pages/LoginRedirect";
import ConfirmLogout from "../Account/pages/ConfirmLogout";
import LogoutRedirect from "../Account/pages/LogoutRedirect";

/*
 * Here we conditionally render small one page React apps
 * based on the server-side page loaded
 */

const routeComponents = {
  login: <Login />,
  ["login-redirect"]: <LoginRedirect />,
  ["confirm-logout"]: <ConfirmLogout />,
  ["logout-redirect"]: <LogoutRedirect />
};

const root = document.getElementById("react-app");

if (root && root.dataset.route) {
  const { route } = root.dataset;
  if (routeComponents.hasOwnProperty(route)) {
    ReactDOM.render(
      <>{routeComponents[route]}</>,
      document.getElementById("react-app")
    );
  }
}