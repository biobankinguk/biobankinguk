import React from "react";
import { Flex, Link } from "@chakra-ui/core";
import Layout from "layouts/Clean";
import UnconfirmedAccountFound from "components/UnconfirmedAccountFound";
import ModelValidationSummary from "components/ModelValidationSummary";
import TryThisAlert from "components/TryThisAlert";
import LoginForm from "./components/LoginForm";
import Conditional from "components/Conditional";

const Login = vm => (
  <Layout heading="Login">
    <Flex flexDirection="column">
      <TryThisAlert
        my={2}
        text="Don't have an account?"
        linkText="Register one"
        href="Register"
      />

      <Conditional expression={vm.AllowResend}>
        <UnconfirmedAccountFound username={vm.Username} />
        <ModelValidationSummary default errors={vm.ModelState} />
      </Conditional>

      <LoginForm {...vm} />

      <Link color="primary.500" href="ForgotPassword">
        Forgot password?
      </Link>
    </Flex>
  </Layout>
);

export default Login;
