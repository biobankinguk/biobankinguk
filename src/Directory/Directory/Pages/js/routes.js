import React from "react";
import ReactDOM from "react-dom";
import { ThemeProvider, CSSReset, theme } from "@chakra-ui/core";
import ukcrcTheme from "../../../../theme/dist/theme";
import merge from "lodash-es/merge";
import { Global } from "@emotion/core";
import Login from "../Account/pages/Login";
import LoginRedirect from "../Account/pages/LoginRedirect";
import ConfirmLogout from "../Account/pages/ConfirmLogout";
import LogoutRedirect from "../Account/pages/LogoutRedirect";
import Register from "../Account/pages/Register";
import RegisterResult from "../Account/pages/RegisterResult";
import ConfirmAccount from "../Account/pages/ConfirmAccount";

/*
 * Here we conditionally render small one page React apps
 * based on the server-side page loaded
 */

const routeComponents = {
  login: <Login />,
  ["login-redirect"]: <LoginRedirect />,
  ["confirm-logout"]: <ConfirmLogout />,
  ["logout-redirect"]: <LogoutRedirect />,
  register: <Register />,
  ["register-result"]: <RegisterResult />,
  confirm: <ConfirmAccount />
};

/*
 * Below is where the actual React bootstrapping happens.
 * It shouldn't need touching often
 */

const root = document.getElementById("react-app");

if (root && root.dataset.route) {
  const { route } = root.dataset;
  if (routeComponents.hasOwnProperty(route)) {
    merge(theme, ukcrcTheme);
    ReactDOM.render(
      <ThemeProvider theme={theme}>
        <CSSReset />
        <Global
          styles={{
            body: { backgroundColor: theme.colors.defaultBackground }
          }}
        />
        {routeComponents[route]}
      </ThemeProvider>,
      document.getElementById("react-app")
    );
  }
}
