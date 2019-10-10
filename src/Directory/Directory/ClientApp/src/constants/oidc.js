export const ApplicationName = "UKCRC Tissue Directory";

export const Results = {
  Redirect: "redirect",
  Success: "success",
  Fail: "fail"
};

export const QueryParams = {
  ReturnUrl: "returnUrl",
  Message: "message"
};

// export const Actions = {
//   Login: "login",
//   LoginCallback: "signin-oidc"
// };

const local = "/auth";
const idp = "/Account";
export const Paths = {
  Prefix: local,
  Login: `${local}/login`,
  LoginCallback: `${local}/signin-oidc`,
  Logout: `${local}/logout`,
  LogoutCallback: `${local}/signout-callback-oidc`,
  IdpRegister: `${idp}/Register`
};
