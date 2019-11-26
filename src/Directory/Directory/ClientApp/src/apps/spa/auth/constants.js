export const ApplicationName = "UKCRC Tissue Directory";

const local = "/auth";
const idp = "/Account";
export const Paths = {
  Prefix: local,
  Login: `${local}/login`,
  LoginCallback: `${local}/signin-oidc`,
  Logout: `${local}/logout`,
  LogoutCallback: `${local}/signout-callback-oidc`,
  IdpRegister: `${idp}/Register`,
  Origin: window.location.origin
};
