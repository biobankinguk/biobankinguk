import React from "react";
import {
  Flex,
  Link,
  AlertIcon,
  AlertTitle,
  AlertDescription,
  Box,
  Alert
} from "@chakra-ui/core";
import Layout from "../../Shared/Layout";
import LoginForm from "../components/LoginForm";
import ModelValidationSummary from "../components/ModelValidationSummary";
import TryThisAlert from "../components/TryThisAlert";
import appSettings from "../../../appsettings.json";

const ResendConfirmation = ({ username }) => (
  <Alert
    status="error"
    my={2}
    p={2}
    variant="left-accent"
    flexDirection="column"
  >
    <Flex alignItems="center">
      <AlertIcon />
      <AlertTitle>This account seems to be unconfirmed.</AlertTitle>
    </Flex>
    <AlertDescription flexDirection="column" textAlign="center">
      <Box mt={3}>
        You can{" "}
        <Link
          color="primary.500"
          href={`/Account/Confirm/Resend?username=${username}`}
        >
          resend your confirmation link,
        </Link>
      </Box>
      <Box>
        or contact{" "}
        <Link color="primary.500" href={`mailto:${appSettings.SupportEmail}`}>
          {appSettings.SupportEmail}
        </Link>{" "}
        if you're having trouble.
      </Box>
    </AlertDescription>
  </Alert>
);

const Login = vm => {
  let failureAlert;
  if (vm.AllowResend)
    failureAlert = <ResendConfirmation username={vm.Username} />;
  else failureAlert = <ModelValidationSummary errors={vm.ModelState} />;

  return (
    <Layout heading="Login">
      <Flex flexDirection="column">
        <TryThisAlert
          text="Don't have an account?"
          linkText="Register one"
          href="Register"
        />

        {failureAlert}
        <LoginForm {...vm} />

        <Link color="primary.500" href="#">
          Forgot password?
        </Link>
      </Flex>
    </Layout>
  );
};

export default Login;
