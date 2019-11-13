import React from "react";
import ModelValidationSummary from "@/components/ModelValidationSummary";
import BasicAlert from "@/components/BasicAlert";
import ResetForm from "./components/ResetForm";
import { hasErrors } from "@/services/modelstate-validation";
import LinkErrorAlert from "@/components/LinkErrorAlert";
import ConditionalPage from "@/components/ConditionalPage";
import Layout from "@/layouts/Clean";

const ResetPassword = vm => (
  <ConditionalPage
    layout={<Layout heading="Reset Password" />}
    condition={hasErrors(vm.ModelState, "Link")}
    trueRender={() => <LinkErrorAlert linkType="password reset" />}
    falseRender={() => (
      <>
        <BasicAlert
          p={2}
          title="Please enter a new password for your account."
        />
        <ModelValidationSummary errors={vm.ModelState} />
        <ResetForm {...vm} />
      </>
    )}
  />
);

export default ResetPassword;
