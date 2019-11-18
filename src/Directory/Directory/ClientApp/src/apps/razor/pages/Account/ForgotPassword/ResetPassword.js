import React from "react";
import ModelValidationSummary from "components/ModelValidationSummary";
import BasicAlert from "components/BasicAlert";
import ResetForm from "./components/ResetForm";
import { hasErrors } from "services/modelstate-validation";
import LinkErrorAlert from "components/LinkErrorAlert";
import Layout from "layouts/Clean";
import ConditionalContent from "components/ConditionalContent";

const ResetPassword = vm => (
  <Layout heading="Reset Password">
    <ConditionalContent
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
  </Layout>
);

export default ResetPassword;
