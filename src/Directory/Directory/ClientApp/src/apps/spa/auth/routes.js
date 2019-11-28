import React from "react";
import { Router } from "@reach/router";
import { Paths } from "./constants";
import Login from "./routes/Login";
import AuthCallback, { CallbackTypes } from "./routes/AuthCallback";
import Logout from "./routes/Logout";

const AuthRoutes = () => (
  <Router>
    <Login path={Paths.Login()} />
    <AuthCallback
      path={Paths.LoginCallback()}
      callbackType={CallbackTypes.Login}
    />
    <Logout path={Paths.Logout()} />
    <AuthCallback
      path={Paths.LogoutCallback()}
      callbackType={CallbackTypes.Logout}
    />
  </Router>
);

export default AuthRoutes;
