import React, { useState, useEffect } from "react";
import { useAsync } from "react-async";
import { Route, Redirect } from "react-router-dom";
import authorizeService from "../../services/AuthorizeService";

const AuthorizeRoute = ({ component: Component, ...props }) => {
  const [authenticated, setAuthenticated] = useState(false);
  const { data, isPending, reload } = useAsync(
    authorizeService.isAuthenticated
  );

  useEffect(() => {
    const subId = authorizeService.subscribe(() => {
      reload();
      setAuthenticated(data);
    });
    return () => authorizeService.unsubscribe(subId);
  }, [reload, data]);

  const redirectUrl = "/hello"; // TODO:

  return isPending ? (
    <div>Loading...</div>
  ) : (
    <Route
      {...props}
      render={props =>
        authenticated ? <Component {...props} /> : <Redirect to={redirectUrl} />
      }
    />
  );
};

export default AuthorizeRoute;
