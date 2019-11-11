import React from "react";
import Layout from "../../../components/Layout";
import ConfirmationSent from "../../../components/ConfirmationSent";

const RegisterResult = ({ Email }) => {
  return (
    <Layout heading="Register">
      <ConfirmationSent username={Email} />
    </Layout>
  );
};

export default RegisterResult;
