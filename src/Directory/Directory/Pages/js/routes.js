import React from "react";
import {
  Login,
  LoginRedirect,
  ConfirmLogout,
  LogoutRedirect,
  Register,
  RegisterResult,
  ConfirmAccount,
  ResendConfirm,
  ForgotPassword
} from "./pages/Account";

// Here we map "page" components to keys
export default {
  Login: <Login />,
  LoginRedirect: <LoginRedirect />,
  LogoutConfirm: <ConfirmLogout />,
  LogoutRedirect: <LogoutRedirect />,
  Register: <Register />,
  RegisterResult: <RegisterResult />,
  Confirm: <ConfirmAccount />,
  ConfirmResend: <ResendConfirm />,
  ForgotPassword: <ForgotPassword />
};
