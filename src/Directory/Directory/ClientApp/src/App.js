import React from "react";
import { Route } from "react-router-dom";
import AuthorizeRoute from "./components/auth/AuthorizeRoute";

// TODO: PoC only, remove
const Protected = () => <div>Hello protected route.</div>;

const App = () => (
  <div>
    <Route exact path="/">
      Hello World
    </Route>
    <Route path="/2">Hello 2</Route>
    <AuthorizeRoute path="protected" component={Protected} />
    <Route path="/hello">Login page?</Route>
  </div>
);

export default App;
