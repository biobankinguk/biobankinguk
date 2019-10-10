import React from "react";
import { Route } from "react-router-dom";
import { Paths } from "../../constants/oidc";
import Login from "./Login";
import LoginCallback from "./LoginCallback";

const AuthRoutes = () => (
  <>
    <Route path={Paths.Login} component={Login} />
    <Route path={Paths.LoginCallback} component={LoginCallback} />
  </>
);

export default AuthRoutes;
