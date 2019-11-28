import React, { createContext, useMemo, useState, useEffect } from "react";
import AuthService from "./service";
import { useAsync } from "react-async";

export const AuthServiceContext = createContext();
export const AuthStateContext = createContext();

// The input and output should look identical,
// but for the input we are destructuring from `service`
// Must ONLY contain NON-stateful properties of `AuthService`
// e.g. `config` derived or methods
const getServiceExports = ({
  unauthorised_uri,
  signIn,
  signOut,
  completeSignOut,
  completeSignIn
}) => ({
  unauthorised_uri,
  signIn,
  signOut,
  completeSignOut,
  completeSignIn
});

/**
 * Creates and provides a global AuthService instance,
 * subscribing to its changes.
 *
 * This exposes the service directly; consumers are more likely
 * to want to use the provided Hooks rather than directly useContext
 */
export const AuthProvider = ({ children, config }) => {
  const service = useMemo(() => new AuthService(config), [config]);
  // These exports are stale unless `config` changes
  // This is by design to avoid meaningless re-rendering
  // the result is that `serviceExports` MUST only contain
  // non-stateful properties of `AuthService`
  const serviceExports = getServiceExports(service);

  // This will be all the stateful properties of AuthService
  // e.g. derived from `_user`
  const [state, setState] = useState({ ready: false });

  // Fetch `_user` when it changes and derive our Context State from it
  const { data: userState, isFulfilled, reload } = useAsync(service.getUser);
  useEffect(() => {
    reload();
  }, [reload, service._user]);
  useEffect(() => {
    if (isFulfilled) {
      setState({
        ready: true,
        userState,
        user: userState && userState.profile,
        accessToken: userState && userState.access_token,
        isAuthenticated: !!(userState && userState.profile)
      });
    }
  }, [userState, isFulfilled]);

  return (
    <AuthServiceContext.Provider value={serviceExports}>
      <AuthStateContext.Provider value={state}>
        {children}
      </AuthStateContext.Provider>
    </AuthServiceContext.Provider>
  );
};
