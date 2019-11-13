import React from "react";
import Layout from "@/layouts/Clean";
import ModelValidationSummary from "@/components/ModelValidationSummary";
import BasicAlert from "@/components/BasicAlert";
import ResetForm from "./components/ResetForm";
import { hasErrors } from "@/services/modelstate-validation";

const ResetPassword = vm => {
  let content;
  if (hasErrors(vm.ModelState, "Link")) {
    content = (
      <>
        <BasicAlert
          status="error"
          title="There seems to be a problem with this reset link."
        >
          Your user ID or password reset token is invalid, or has expired.
        </BasicAlert>
      </>
    );
  } else {
    content = (
      <>
        <BasicAlert
          p={2}
          title="Please enter a new password for your account."
        />
        <ModelValidationSummary errors={vm.ModelState} />
        <ResetForm {...vm} />
      </>
    );
  }
  return <Layout heading="Reset Password">{content}</Layout>;
};

export default ResetPassword;
