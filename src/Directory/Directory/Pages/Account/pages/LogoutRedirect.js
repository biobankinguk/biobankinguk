import React, { useEffect } from "react";

const LogoutRedirect = () => {
  const { redirectUrl, clientName } = document.getElementById(
    "react-data"
  ).dataset;

  useEffect(() => {
    if (redirectUrl) window.location.href = redirectUrl;
  });

  return (
    <>
      <h1>You are now logged out.</h1>
      {!!redirectUrl ? (
        <div>
          Click <a href={redirectUrl}>here</a> to return to the {clientName}{" "}
          application.
        </div>
      ) : null}
    </>
  );
};

export default LogoutRedirect;
