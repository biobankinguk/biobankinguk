import React from "react";
import { Router, Link } from "@reach/router";
import { AuthProvider, Authorize } from "auth";
import { Paths as AuthPaths } from "./auth/constants";
import config from "./auth/config";
import Index from "./pages/Index";
import Routes from "./routes";
import ApiCall from "./auth/ApiCallTest";
import { setTitle } from "services/dom-service";

// TODO: PoC only, remove
const Protected = () => {
  setTitle("Protected");
  return (
    <>
      <div>Hello protected route.</div>
      <ApiCall />
      <Link to={AuthPaths.Logout(true)}>Logout</Link>
    </>
  );
};
const Greeter = ({ name }) => {
  setTitle("Hello");
  return <div>Hello {name}</div>;
};

const App = () => {
  return (
    <AuthProvider config={config}>
      <Router>
        <Index path="/" />
        <Greeter path="/hello/:name" />
        <Authorize path="/protected" component={Protected} />
        <Routes path="/*" />
      </Router>
    </AuthProvider>
  );
};

export default App;
