import React from "react";
import { Results } from "../../constants/oidc";
import { useAsync, IfPending, IfFulfilled, IfRejected } from "react-async";
import authorizeService from "../../services/authorize-service";
import LoginFailure from "./LoginFailure";
import { getReturnUrl } from "../../services/dom-service";

const LoginCallback = () => {
  const url = window.location.href;
  const state = useAsync(authorizeService.completeSignIn, { url });

  return (
    <>
      <IfPending state={state}>
        <div>Completing Login...</div>
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
              return <LoginFailure message={message} />;
            default:
              throw new Error(`Invalid Auth Result: ${status}`);
          }
          return null;
        }}
      </IfFulfilled>
      <IfRejected state={state}>
        {error => <LoginFailure message="An error occurred" error={error} />}
      </IfRejected>
    </>
  );
};

export default LoginCallback;
