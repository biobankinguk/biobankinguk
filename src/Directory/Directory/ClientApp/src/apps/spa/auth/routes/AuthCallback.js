import React from "react";
import { Results } from "constants/oidc";
import { useAsync, IfPending, IfFulfilled, IfRejected } from "react-async";
import GeneralError from "components/GeneralError";
import { getReturnUrl, setTitle } from "services/dom-service";
import { useAuthService } from "auth";

export const CallbackTypes = {
  Login: "Login",
  Logout: "Logout"
};

const AuthCallback = ({ callbackType }) => {
  const url = window.location.href;
  const { completeSignIn, completeSignOut } = useAuthService();
  const state = useAsync(
    callbackType === CallbackTypes.Login ? completeSignIn : completeSignOut,
    { url }
  );
  setTitle(callbackType);

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
