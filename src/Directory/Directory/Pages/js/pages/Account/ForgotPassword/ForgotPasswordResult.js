import React from "react";
import Layout from "@/components/Layout";
import BasicAlert from "@/components/BasicAlert";
import { AlertDescription, Text, Box, Link } from "@chakra-ui/core";

const ForgotPasswordResult = () => (
  <Layout heading="Forgot Password">
    <BasicAlert
      status="success"
      title="Password Reset Request received!"
      noChildWrapper
    >
      <AlertDescription flexDirection="column" textAlign="center">
        <Text>
          If a matching account was found, a reset password link will arrive at
          the email address you entered.
        </Text>
        <Text>Please check your email.</Text>
        <Box mt={3}>
          <Link color="primary.500" href="/">
            Return home
          </Link>
        </Box>
      </AlertDescription>
    </BasicAlert>
  </Layout>
);

export default ForgotPasswordResult;
