import React from "react";
import ReactDOM from "react-dom";
import { Normalize } from "styled-normalize";
import Login from "../Account/pages/Login";
import LoginRedirect from "../Account/pages/LoginRedirect";
import ConfirmLogout from "../Account/pages/ConfirmLogout";
import LogoutRedirect from "../Account/pages/LogoutRedirect";

/*
 * Here we conditionally render small one page React apps
 * based on the server-side page loaded
 */

let defaultRender = true;
let app;

switch (document.getElementById("react-app").dataset.route) {
  case "login":
    app = (
      <>
        <Normalize />
        <Login />
      </>
    );
    break;
  case "login-redirect":
    app = (
      <>
        <Normalize />
        <LoginRedirect />
      </>
    );
    break;
  case "confirm-logout":
    app = (
      <>
        <Normalize />
        <ConfirmLogout />
      </>
    );
    break;
  case "logout-redirect":
    app = (
      <>
        <Normalize />
        <LogoutRedirect />
      </>
    );
    break;
  default:
    defaultRender = false; // no React to render
}

if (defaultRender)
  ReactDOM.render(<>{app}</>, document.getElementById("react-app"));
