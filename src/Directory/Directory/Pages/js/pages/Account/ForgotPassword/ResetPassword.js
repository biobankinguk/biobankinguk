import React from "react";
import Layout from "@/layouts/Clean";
import ModelValidationSummary from "@/components/ModelValidationSummary";
import RequestForm from "./components/RequestForm";
import BasicAlert from "@/components/BasicAlert";

const ForgotPassword = vm => {
  return (
    <Layout heading="Reset Password">
      <BasicAlert
        p={2}
        title="Please enter the email address associated with your account."
      >
        If a matching account is found, a reset password link will be sent via
        email.
      </BasicAlert>

      <ModelValidationSummary errors={vm.ModelState} />
      <RequestForm {...vm} />
    </Layout>
  );
};

export default ForgotPassword;
