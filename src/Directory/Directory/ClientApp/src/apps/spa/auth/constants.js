export const ApplicationName = "UKCRC Tissue Directory";

const localPaths = {
  Login: "login",
  LoginCallback: "signin-oidc",
  Logout: "logout",
  LogoutCallback: "signout-callback-oidc"
};
export const Paths = {
  Origin: window.location.origin,
  IdpRegister: `/Account/Register`,

  // build absolute/relative Local Paths
  ...(() => {
    const prefix = "auth";
    return {
      Prefix: (absolute = false) => (absolute ? `/${prefix}` : prefix),
      ...Object.keys(localPaths).reduce((a, v) => {
        a[v] = (absolute = false) =>
          absolute ? `/${prefix}/${localPaths[v]}` : localPaths[v];
        return a;
      }, {})
    };
  })()
};
