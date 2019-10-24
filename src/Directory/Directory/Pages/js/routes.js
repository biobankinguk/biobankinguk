import React from "react";
import ReactDOM from "react-dom";
import { ThemeProvider, CSSReset } from "@chakra-ui/core";
import Login from "../Account/pages/Login";
import LoginRedirect from "../Account/pages/LoginRedirect";
import ConfirmLogout from "../Account/pages/ConfirmLogout";
import LogoutRedirect from "../Account/pages/LogoutRedirect";
import theme from "../../../../theme/dist/theme";

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
      <ThemeProvider theme={theme}>
        <CSSReset />
        {routeComponents[route]}
      </ThemeProvider>,
      document.getElementById("react-app")
    );
  }
}
