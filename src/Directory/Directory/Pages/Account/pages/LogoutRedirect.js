import React, { useEffect } from "react";
import Layout from "../../Shared/Layout";
import { Alert, AlertTitle, Link, AlertDescription } from "@chakra-ui/core";

const LogoutRedirect = () => {
  const { redirectUrl, clientName } = document.getElementById(
    "react-data"
  ).dataset;

  useEffect(() => {
    if (redirectUrl) window.location.href = redirectUrl;
  });

  return (
    <Layout heading="Logout">
      <Alert status="info" variant="left-accent" flexDirection="column">
        <AlertTitle>You are now logged out.</AlertTitle>
        {!!redirectUrl ? (
          <AlertDescription>
            Click <Link href={redirectUrl}>here</Link> to return to the{" "}
            {clientName} application.
          </AlertDescription>
        ) : null}
      </Alert>
    </Layout>
  );
};

export default LogoutRedirect;
