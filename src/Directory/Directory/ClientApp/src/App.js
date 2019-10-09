import React from "react";
import { Route, Switch, useParams } from "react-router-dom";
import AuthorizeRoute from "./components/auth/AuthorizeRoute";
import { Paths as AuthPaths } from "./constants/oidc";
import AuthRoutes from "./components/auth/AuthRoutes";

// TODO: PoC only, remove
const Protected = () => <div>Hello protected route.</div>;
const Greeter = () => {
  const { name } = useParams();
  return <div>Hello {name}</div>;
};

const App = () => (
  <Switch>
    <AuthorizeRoute path="/protected" component={Protected} />
    <Route path={AuthPaths.Prefix} component={AuthRoutes} />
    <Route path="/Hello/:name" component={Greeter} />
    <Route exact path="/">Hello World</Route>
  </Switch>
);

export default App;
