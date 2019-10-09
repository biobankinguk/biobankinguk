import React from "react";
import { Route } from "react-router-dom";
import { Paths, Results } from "../../constants/oidc";
import { useAsync } from "react-async";
import authorizeService from "../../services/AuthorizeService";

const Login = ({ returnUrl }) => {
  const handleSignInResult = ({ status }) => {
    switch (status) {
      case Results.Redirect:
        break; // the Auth Service has directed us away
      case Results.Success:
        // navigate to returnUrl
        break;
      case Results.Fail:
        //navigate to login failure? pass along `message`?
        break;
      default:
        throw new Error(`Invalid Auth Result: ${status}`);
    }
  };

  useAsync(
    {
      promiseFn: authorizeService.signIn,
      onResolve: handleSignInResult
    },
    { returnUrl }
  );

  // we don't even care about isPending;
  // all outcomes of useAsync are to navigate elsewhere
  return <div>Processing Login...</div>; // TODO: sexy
};

const AuthRoutes = () => (
  <>
    <Route path={Paths.Login} component={Login} />
  </>
);

export default AuthRoutes;
