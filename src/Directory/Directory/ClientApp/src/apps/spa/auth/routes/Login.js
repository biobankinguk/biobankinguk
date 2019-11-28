import React from "react";
import { Results } from "constants/oidc";
import { useAsync, IfPending, IfFulfilled, IfRejected } from "react-async";
import GeneralError from "components/GeneralError";
import { getReturnUrl, setTitle } from "services/dom-service";
import { useAuthService } from "auth";

const Login = () => {
  const returnUrl = getReturnUrl();
  const { signIn } = useAuthService();
  const state = useAsync(signIn, { returnUrl });
  setTitle("Login");

  return (
    <>
      <IfPending state={state}>
        <div>Processing Login...</div> {/* TODO: sexy */}
      </IfPending>
      <IfFulfilled state={state}>
        {({ status, message }) => {
          switch (status) {
            case Results.Redirect:
              break;
            case Results.Success:
              window.location.replace(returnUrl);
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

export default Login;
