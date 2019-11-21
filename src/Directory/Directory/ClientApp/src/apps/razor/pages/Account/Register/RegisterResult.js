import React from "react";
import Layout from "layouts/Clean";
import ConfirmationSent from "components/ConfirmationSent";

const RegisterResult = ({ Email }) => {
  return (
    <Layout heading="Register">
      <ConfirmationSent username={Email} />
    </Layout>
  );
};

export default RegisterResult;
