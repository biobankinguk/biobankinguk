import React from "react";
import {
  AlertTitle,
  AlertDescription,
  Alert,
  Link,
  AlertIcon,
  Flex
} from "@chakra-ui/core";
import Layout from "../../Shared/Layout";

const RegisterResult = () => {
  return (
    <Layout heading="Register">
      <Alert status="success" variant="left-accent" flexDirection="column">
        <Flex alignItems="center">
          <AlertIcon />
          <AlertTitle>Registration Successful!</AlertTitle>
        </Flex>
        <AlertDescription>
          You may now{" "}
          <Link color="primary.500" href="/auth/login">
            Log in
          </Link>
          .
        </AlertDescription>
      </Alert>
    </Layout>
  );
};

export default RegisterResult;
