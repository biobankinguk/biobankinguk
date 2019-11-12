import React from "react";
import {
  Alert,
  AlertDescription,
  AlertIcon,
  AlertTitle,
  Flex
} from "@chakra-ui/core";
import Layout from "Components/Layout";
import RequestForm from "./components/RequestForm";


const ForgotPassword = () => (
  <Layout heading="Forgot Password">
    <Alert status="info" p={2} variant="left-accent" flexDirection="column">
      <Flex alignItems="center">
        <AlertIcon />
        <AlertTitle>
          Please enter the email address associated with your account.
        </AlertTitle>
      </Flex>
      <AlertDescription>
        If a matching account is found, a reset password link will be sent via
        email.
      </AlertDescription>
    </Alert>

    <RequestForm />
  </Layout>
);

export default ForgotPassword;
