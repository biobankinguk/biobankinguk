import React from "react";
import Layout from "Components/Layout";
import ConfirmationSent from "Components/ConfirmationSent";

const RegisterResult = ({ Email }) => {
  return (
    <Layout heading="Register">
      <ConfirmationSent username={Email} />
    </Layout>
  );
};

export default RegisterResult;
