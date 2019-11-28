import React, { createElement } from "react";
import { Redirect } from "@reach/router";
import { QueryParams } from "constants/oidc";
import { useAuth, useAuthService } from "auth";
import Conditional, { False, True } from "components/Conditional";

/**
 * Build a URL with query string.
 * @param {string} path The absolute or relative URL path
 * @param {...string[]} [query] Query parameter arrays in the form `[param, value]`
 */
const buildQueryUrl = (path, ...query) =>
  `${path}${query.reduce(
    (queryString, q) =>
      queryString + `${!queryString ? "?" : "&"}${q[0]}=${q[1]}`,
    ""
  )}`;

/**
 * Require an Authorised user.
 *
 * Unauthenticated users are sent to login by default, or an otherwise configured `fallback`
 *
 * This is a routing component that mixes Reach and React Router's APIs as follows:
 *
 * - `children` take priority and behave as Reach Router route children
 *   - i.e. they should be components with a `path` prop.
 *   - This allows protecting multiple routes by the same criteria under a parent route in one go
 * - `component` is the next highest precedence, and specifies a single component to instantiate and render
 * - `render` is lowest precedence and should be a function to execute
 */
const Authorize = ({ children, fallback, render, component }) => {
  const { unauthorised_uri } = useAuthService();
  const auth = useAuth();
  const { ready, isAuthenticated } = auth;
  if (!ready) return null;

  const condition = isAuthenticated; // TODO: More complex conditions for authorisation later...

  const redirectUrl = buildQueryUrl(unauthorised_uri, [
    QueryParams.ReturnUrl,
    encodeURI(window.location.href)
  ]);

  // reach router version
  return (
    <Conditional expression={condition}>
      <True>
        {children ||
          (component && createElement(component)) ||
          (render && render()) ||
          null}
      </True>

      <False>{fallback || <Redirect to={redirectUrl} />}</False>
    </Conditional>
  );
};

export default Authorize;
