import React from "react";
import LoginForm from "../components/LoginForm";
import Layout from "../../Shared/Layout";
import { Flex, Alert, AlertDescription } from "@chakra-ui/core";

const Login = () => {
  const valSummary = document.getElementById("asp-validation-summary");
  let valErrors = null;
  if (valSummary && valSummary.className.includes("validation-summary-errors"))
    valErrors = (
      <Alert status="error" my={2} p={2} variant="left-accent">
        <AlertDescription
          dangerouslySetInnerHTML={{ __html: valSummary.innerHTML }}
        ></AlertDescription>
      </Alert>
    );

  return (
    <Layout heading="Login">
      <Flex flexDirection="column">
        {valErrors}
        <LoginForm />
      </Flex>
    </Layout>
  );
};

export default Login;
