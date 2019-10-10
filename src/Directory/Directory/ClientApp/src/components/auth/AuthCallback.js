import React from "react";
import { Results } from "../../constants/oidc";
import { useAsync, IfPending, IfFulfilled, IfRejected } from "react-async";
import authorizeService from "../../services/authorize-service";
import GeneralError from "../GeneralError";
import { getReturnUrl } from "../../services/dom-service";

export const CallbackTypes = {
  Login: "Login",
  Logout: "Logout"
};

const AuthCallback = ({ callbackType }) => {
  const url = window.location.href;
  const state = useAsync(
    callbackType === CallbackTypes.Login
      ? authorizeService.completeSignIn
      : authorizeService.completeSignOut,
    { url }
  );

  return (
    <>
      <IfPending state={state}>
        <div>Completing {callbackType}...</div> {/* TODO: sexy */}
      </IfPending>
      <IfFulfilled state={state}>
        {({ status, state, message }) => {
          switch (status) {
            case Results.Redirect:
              throw new Error(`Invalid Auth Result for this flow: ${status}`);
            case Results.Success:
              window.location.replace(getReturnUrl(state));
              break;
            case Results.Fail:
              return <GeneralError message={message} />;
            default:
              throw new Error(`Invalid Auth Result: ${status}`);
          }
          return null;
        }}
      </IfFulfilled>
      <IfRejected state={state}>
        {error => (
          <GeneralError
            message="An authorization error occurred"
            error={error}
          />
        )}
      </IfRejected>
    </>
  );
};

export default AuthCallback;
