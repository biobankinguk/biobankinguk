import React, { useEffect } from "react";
import { Link } from "@chakra-ui/core";
import Layout from "@/layouts/Clean";
import BasicAlert from "@/components/BasicAlert";

const LogoutRedirect = ({ PostLogoutRedirectUri, ClientName }) => {
  useEffect(() => {
    if (PostLogoutRedirectUri) window.location.href = PostLogoutRedirectUri;
  });

  return (
    <Layout heading="Logout">
      <BasicAlert title="You are now logged out.">
        {!!PostLogoutRedirectUri ? (
          <>
            <Link color="primary.500" href={PostLogoutRedirectUri}>
              Click here
            </Link>{" "}
            to return to the {ClientName} application.
          </>
        ) : null}
      </BasicAlert>
    </Layout>
  );
};

export default LogoutRedirect;
