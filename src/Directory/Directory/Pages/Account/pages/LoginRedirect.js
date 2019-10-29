import React, { useEffect } from "react";
import { AlertTitle, AlertDescription, Alert } from "@chakra-ui/core";
import Layout from "../../Shared/Layout";

const LoginRedirect = ({ RedirectUrl }) => {
  useEffect(() => {
    window.location.href = RedirectUrl;
  });

  return (
    <Layout heading="Login">
      <Alert status="info" variant="left-accent" flexDirection="column">
        <AlertTitle>You are now being returned to the application.</AlertTitle>
        <AlertDescription>
          Once complete, you may close this tab
        </AlertDescription>
      </Alert>
    </Layout>
  );
};

export default LoginRedirect;
