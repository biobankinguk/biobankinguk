// Conditionally import Page Components based on route key
// This needs to be a naive switch/if-else so webpack chunks correctly
const routes = route => {
  switch (route) {
    case "Login":
      return import("./pages/Account/Login/Login");
    case "LoginRedirect":
      return import("./pages/Account/Login/LoginRedirect");
    case "LogoutConfirm":
      return import("./pages/Account/Logout/ConfirmLogout");
    case "LogoutRedirect":
      return import("./pages/Account/Logout/LogoutRedirect");
    case "Register":
      return import("./pages/Account/Register/Register");
    case "RegisterResult":
      return import("./pages/Account/Register/RegisterResult");
    case "Confirm":
      return import("./pages/Account/Confirm/ConfirmAccount");
    case "ConfirmResend":
      return import("./pages/Account/Confirm/ResendConfirm");
    case "ForgotPassword":
      return import("./pages/Account/ForgotPassword/ForgotPassword");
    case "ForgotPasswordResult":
      return import("./pages/Account/ForgotPassword/ForgotPasswordResult");
    default:
      return null;
  }
};

export default routes;
