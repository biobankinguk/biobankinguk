import React, { useEffect } from "react";
import Layout from "../../Shared/Layout";
import { Alert, AlertTitle, Link, AlertDescription } from "@chakra-ui/core";

const LogoutRedirect = ({ RedirectUrl, ClientName }) => {
  useEffect(() => {
    if (RedirectUrl) window.location.href = RedirectUrl;
  });

  return (
    <Layout heading="Logout">
      <Alert status="info" variant="left-accent" flexDirection="column">
        <AlertTitle>You are now logged out.</AlertTitle>
        {!!RedirectUrl ? (
          <AlertDescription>
            Click <Link href={RedirectUrl}>here</Link> to return to the{" "}
            {ClientName} application.
          </AlertDescription>
        ) : null}
      </Alert>
    </Layout>
  );
};

export default LogoutRedirect;
