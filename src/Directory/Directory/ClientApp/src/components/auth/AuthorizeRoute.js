import React, { useState, useEffect } from "react";
import { useAsync } from "react-async";
import { Route, Redirect } from "react-router-dom";
import authorizeService from "../../services/AuthorizeService";
import { Paths, QueryParams } from "../../constants/oidc";

const AuthorizeRoute = ({ component: Component, ...rest }) => {
  const [authenticated, setAuthenticated] = useState(false);
  const { isPending, reload } = useAsync({
    promiseFn: authorizeService.isAuthenticated,
    onResolve: data => {
      console.log(data);
      setAuthenticated(data);
    }
  });

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
        if (authenticated) return <Component {...p} />;
        return <Redirect to={redirectUrl} />;
      }}
    />
  );
};

export default AuthorizeRoute;
