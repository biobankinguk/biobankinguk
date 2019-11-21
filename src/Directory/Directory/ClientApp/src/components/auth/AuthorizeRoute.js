import React, { useEffect } from "react";
import { useAsync } from "react-async";
import { Route, Redirect } from "react-router-dom";
import authorizeService from "services/authorize-service";
import { Paths, QueryParams } from "constants/oidc";

const AuthorizeRoute = ({ component: Component, ...rest }) => {
  const { data, isPending, reload } = useAsync(
    authorizeService.isAuthenticated
  );

  useEffect(() => {
    const subId = authorizeService.subscribe(() => {
      reload();
    });
    return () => authorizeService.unsubscribe(subId);
  }, [reload]);

  const redirectUrl = `${Paths.Login}?${QueryParams.ReturnUrl}=${encodeURI(
    window.location.href
  )}`;

  if (isPending) return <div>Loading...</div>; // TODO: make sexier

  return (
    <Route
      {...rest}
      render={p => {
        if (data) return <Component {...p} />;
        return <Redirect to={redirectUrl} />;
      }}
    />
  );
};

export default AuthorizeRoute;
