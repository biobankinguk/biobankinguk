import React from "react";
import { useAsync, IfPending, IfRejected, IfFulfilled } from "react-async";
import { getReturnUrl, setTitle } from "services/dom-service";
import GeneralError from "../../../../components/GeneralError";
import { Results } from "constants/oidc";
import { useAuth, useAuthService } from "auth";

// moving multiple async service calls out
// into a single async function
// greatly simplifies the react component
const logout = async ({ returnUrl, isAuthenticated, signOut }) =>
  isAuthenticated ? await signOut({ returnUrl }) : { status: Results.Success };

const Logout = () => {
  const returnUrl = getReturnUrl();
  const { signOut } = useAuthService();
  const { isAuthenticated } = useAuth();
  const state = useAsync(logout, { returnUrl, isAuthenticated, signOut });
  setTitle("Logout");

  return (
    <>
      <IfPending state={state}>
        <div>Processing Logout...</div> {/* TODO: sexy */}
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

export default Logout;
