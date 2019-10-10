import React from "react";
import { Results } from "../../constants/oidc";
import { useAsync, IfPending, IfFulfilled, IfRejected } from "react-async";
import authorizeService from "../../services/authorize-service";
import LoginFailure from "./LoginFailure";
import { getReturnUrl } from "../../services/dom-service";

const Login = () => {
  const returnUrl = getReturnUrl();
  const state = useAsync(authorizeService.signIn, { returnUrl });

  return (
    <>
      <IfPending state={state}>
        <div>Processing Login...</div>
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

export default Login;
