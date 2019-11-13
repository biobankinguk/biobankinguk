import React from "react";
import Layout from "@/layouts/Clean";
import UnconfirmedAccountFound from "@/components/UnconfirmedAccountFound";
import ModelValidationSummary from "@/components/ModelValidationSummary";
import RequestForm from "./components/RequestForm";
import BasicAlert from "@/components/BasicAlert";

const ForgotPassword = vm => {
  let failureAlert;
  if (vm.AllowResend)
    failureAlert = <UnconfirmedAccountFound username={vm.Email} />;
  else failureAlert = <ModelValidationSummary errors={vm.ModelState} />;

  return (
    <Layout heading="Forgot Password">
      <BasicAlert
        p={2}
        title="Please enter the email address associated with your account."
      >
        If a matching account is found, a reset password link will be sent via
        email.
      </BasicAlert>

      {failureAlert}
      <RequestForm {...vm} />
    </Layout>
  );
};

export default ForgotPassword;
