import React from "react";
import {
  Flex,
  Alert,
  AlertDescription,
  Link,
  AlertIcon
} from "@chakra-ui/core";
import Layout from "../../Shared/Layout";
import LoginForm from "../components/LoginForm";
import AspValidationSummary from "../components/AspValidationSummary";
import WrongFormAlert from "../components/WrongFormAlert";

const Login = () => {
  return (
    <Layout heading="Login">
      <Flex flexDirection="column">
        <WrongFormAlert
          text="Don't have an account?"
          linkText="Register one"
          href="Register"
        />

        <AspValidationSummary />
        <LoginForm />

        <Link color="primary.500" href="#">
          Forgot password?
        </Link>
      </Flex>
    </Layout>
  );
};

export default Login;
