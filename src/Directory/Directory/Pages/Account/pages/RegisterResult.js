import React from "react";
import { AlertTitle, AlertDescription, Alert } from "@chakra-ui/core";
import Layout from "../../Shared/Layout";

const RegisterResult = () => {
  return (
    <Layout heading="Register">
      <Alert status="success" variant="left-accent" flexDirection="column">
        <AlertTitle>Registration Successful!</AlertTitle>
        <AlertDescription>
          You may now{" "}
          <Link color="primary" href="/auth/login">
            Log in
          </Link>
          .
        </AlertDescription>
      </Alert>
    </Layout>
  );
};

export default RegisterResult;
