import React from "react";
import Login from "../Account/pages/Login";
import LoginRedirect from "../Account/pages/LoginRedirect";
import ConfirmLogout from "../Account/pages/ConfirmLogout";
import LogoutRedirect from "../Account/pages/LogoutRedirect";
import Register from "../Account/pages/Register";
import RegisterResult from "../Account/pages/RegisterResult";
import ConfirmAccount from "../Account/pages/ConfirmAccount";
import ResendConfirm from "../Account/pages/ResendConfirm";

// Here we map "page" components to keys
export default {
  Login: <Login />,
  LoginRedirect: <LoginRedirect />,
  LogoutConfirm: <ConfirmLogout />,
  LogoutRedirect: <LogoutRedirect />,
  Register: <Register />,
  RegisterResult: <RegisterResult />,
  Confirm: <ConfirmAccount />,
  ConfirmResend: <ResendConfirm />
};
