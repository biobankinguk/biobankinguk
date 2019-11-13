import React from "react";
import Layout from "@/layouts/Clean";
import ModelValidationSummary from "@/components/ModelValidationSummary";
import BasicAlert from "@/components/BasicAlert";
import ResetForm from "./components/ResetForm";

const ForgotPassword = vm => {
  return (
    <Layout heading="Reset Password">
      <BasicAlert p={2} title="Please enter a new password for your account." />

      <ModelValidationSummary errors={vm.ModelState} />
      <ResetForm {...vm} />
    </Layout>
  );
};

export default ForgotPassword;
