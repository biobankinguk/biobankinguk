import React from "react";
import ReactDOM from "react-dom";
import { Normalize } from "styled-normalize";
import Login from "../Account/pages/Login";
import LoginRedirect from "../Account/pages/LoginRedirect";

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
    case "login":
      app = (
        <>
          <Normalize />
          <LoginRedirect />
        </>
      );
      break;
  default:
    defaultRender = false; // no React to render
}

if (defaultRender)
  ReactDOM.render(<>{app}</>, document.getElementById("react-app"));
