import React from "react";
import { Route, Switch, Link, useParams } from "react-router-dom";
import AuthorizeRoute from "./components/auth/AuthorizeRoute";
import { Paths as AuthPaths } from "./constants/oidc";
import AuthRoutes from "./components/auth/AuthRoutes";

// TODO: PoC only, remove
const Protected = () => (
  <>
    <div>Hello protected route.</div>
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
      Hello World
    </Route>
    <AuthorizeRoute path="/protected" component={Protected} />
    <Route path={AuthPaths.Prefix} component={AuthRoutes} />
    <Route path="/Hello/:name" component={Greeter} />
    <Route path="*">404: Not Found</Route> {/* TODO: sexy */}
  </Switch>
);

export default App;
