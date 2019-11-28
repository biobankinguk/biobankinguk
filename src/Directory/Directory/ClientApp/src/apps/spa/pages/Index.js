import React from "react";
import { Link } from "@reach/router";
import ApiCall from "../auth/ApiCallTest";
import { Paths as AuthPaths } from "../auth/constants";

const Index = () => {
  //   setTitle(null);
  return (
    <>
      <div>Hello World</div>
      <div>The API Call should fail when not logged in</div>
      <div>
        <Link to={AuthPaths.Login(true)}>Login</Link> |{" "}
        <Link to={AuthPaths.Logout(true)}>Logout</Link> |{" "}
        <Link to="/protected">Protected route</Link>
      </div>
      <ApiCall />
    </>
  );
};

export default Index;
