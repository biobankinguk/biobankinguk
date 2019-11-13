import React from "react";
import { Flex, Link } from "@chakra-ui/core";
import Layout from "@/components/Layout";
import UnconfirmedAccountFound from "@/components/UnconfirmedAccountFound";
import ModelValidationSummary from "@/components/ModelValidationSummary";
import TryThisAlert from "@/components/TryThisAlert";
import LoginForm from "./components/LoginForm";

const Login = vm => {
  let failureAlert;
  if (vm.AllowResend)
    failureAlert = <UnconfirmedAccountFound username={vm.Username} />;
  else failureAlert = <ModelValidationSummary errors={vm.ModelState} />;

  return (
    <Layout heading="Login">
      <Flex flexDirection="column">
        <TryThisAlert
          my={2}
          text="Don't have an account?"
          linkText="Register one"
          href="Register"
        />

        {failureAlert}
        <LoginForm {...vm} />

        <Link color="primary.500" href="ForgotPassword">
          Forgot password?
        </Link>
      </Flex>
    </Layout>
  );
};

export default Login;
