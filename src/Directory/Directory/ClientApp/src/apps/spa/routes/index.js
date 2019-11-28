import React from "react";
import { Router } from "@reach/router";
import { Paths } from "../auth/constants";
import Auth from "../auth/routes";
import NotFound from "../pages/NotFound";

const Routes = () => (
  <Router>
    <Auth path={`${Paths.Prefix(true)}/*`} />
    <NotFound default />
  </Router>
);

export default Routes;
