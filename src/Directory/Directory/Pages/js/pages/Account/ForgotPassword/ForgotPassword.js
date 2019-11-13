import React from "react";
import {
  Alert,
  AlertDescription,
  AlertIcon,
  AlertTitle,
  Flex
} from "@chakra-ui/core";
import Layout from "@/components/Layout";
import UnconfirmedAccountFound from "@/components/UnconfirmedAccountFound";
import ModelValidationSummary from "@/components/ModelValidationSummary";
import RequestForm from "./components/RequestForm";

const ForgotPassword = vm => {
  let failureAlert;
  if (vm.AllowResend)
    failureAlert = <UnconfirmedAccountFound username={vm.Email} />;
  else failureAlert = <ModelValidationSummary errors={vm.ModelState} />;

  return (
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

      {failureAlert}
      <RequestForm {...vm} />
    </Layout>
  );
};

export default ForgotPassword;
