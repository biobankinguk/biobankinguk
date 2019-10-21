import React from "react";
import ReactDOM from "react-dom";
import { Normalize } from "styled-normalize";
import Login from "../Account/Login";

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
  default:
    defaultRender = false; // no React to render
}

if (defaultRender)
  ReactDOM.render(
    <React.Fragment>{app}</React.Fragment>,
    document.getElementById("react-app")
  );
