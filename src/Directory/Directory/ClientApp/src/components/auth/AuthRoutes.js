import React from "react";
import { Route } from "react-router-dom";
import { Paths } from "../../constants/oidc";
import Login from "./Login";
import AuthCallback, { CallbackTypes } from "./AuthCallback";
import Logout from "./Logout";

const AuthRoutes = () => (
  <>
    <Route path={Paths.Login} component={Login} />
    <Route
      path={Paths.LoginCallback}
      render={p => <AuthCallback {...p} callbackType={CallbackTypes.Login} />}
    />
    <Route path={Paths.Logout} component={Logout} />
    <Route
      path={Paths.LogoutCallback}
      render={p => <AuthCallback {...p} callbackType={CallbackTypes.Logout} />}
    />
  </>
);

export default AuthRoutes;
