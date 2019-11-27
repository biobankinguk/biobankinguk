import React from "react";
import Layout from "layouts/Clean";
import UnconfirmedAccountFound from "components/UnconfirmedAccountFound";
import ModelValidationSummary from "components/ModelValidationSummary";
import RequestForm from "./components/RequestForm";
import BasicAlert from "components/BasicAlert";
import Conditional from "components/Conditional";

const ForgotPassword = vm => (
  <Layout heading="Forgot Password">
    <BasicAlert
      p={2}
      title="Please enter the email address associated with your account."
    >
      If a matching account is found, a reset password link will be sent via
      email.
    </BasicAlert>

    <Conditional expression={vm.AllowResend}>
      <UnconfirmedAccountFound username={vm.Email} />
      <ModelValidationSummary default errors={vm.ModelState} />
    </Conditional>

    <RequestForm {...vm} />
  </Layout>
);

export default ForgotPassword;
