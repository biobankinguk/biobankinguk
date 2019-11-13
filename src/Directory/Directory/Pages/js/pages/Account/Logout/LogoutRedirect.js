import React, { useEffect } from "react";
import {
  Alert,
  AlertTitle,
  Link,
  AlertDescription,
  AlertIcon
} from "@chakra-ui/core";
import Layout from "Components/Layout";

const LogoutRedirect = ({ PostLogoutRedirectUri, ClientName }) => {
  useEffect(() => {
    if (PostLogoutRedirectUri) window.location.href = PostLogoutRedirectUri;
  });

  return (
    <Layout heading="Logout">
      <Alert status="info" variant="left-accent" flexDirection="column">
        <AlertIcon />
        <AlertTitle>You are now logged out.</AlertTitle>
        {!!PostLogoutRedirectUri ? (
          <AlertDescription>
            <Link color="primary.500" href={PostLogoutRedirectUri}>
              Click here
            </Link>{" "}
            to return to the {ClientName} application.
          </AlertDescription>
        ) : null}
      </Alert>
    </Layout>
  );
};

export default LogoutRedirect;
