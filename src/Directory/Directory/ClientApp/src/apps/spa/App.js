import React from "react";
import { Route, Switch, Link, useParams } from "react-router-dom";
import AuthorizeRoute from "components/AuthorizeRoute";
import { Paths as AuthPaths } from "./auth/constants";
import AuthRoutes from "apps/spa/auth/AuthRoutes";
import ApiCall from "apps/spa/auth/ApiCallTest";

// TODO: PoC only, remove
const Protected = () => (
  <>
    <div>Hello protected route.</div>
    <ApiCall />
    <Link to={AuthPaths.Logout}>Logout</Link>
  </>
);
const Greeter = () => {
  const { name } = useParams();
  return <div>Hello {name}</div>;
};

const App = () => (
  <Switch>
    <Route exact path="/">
      <div>Hello World</div>
      <div>The API Call should fail when not logged in</div>
      <div>
        <Link to={AuthPaths.Login}>Login</Link> |{" "}
        <Link to={AuthPaths.Logout}>Logout</Link>
      </div>
      <ApiCall />
    </Route>
    <AuthorizeRoute path="/protected" component={Protected} />
    <Route path={AuthPaths.Prefix} component={AuthRoutes} />
    <Route path="/Hello/:name" component={Greeter} />
    <Route path="*">404: Not Found</Route> {/* TODO: sexy */}
  </Switch>
);

export default App;
