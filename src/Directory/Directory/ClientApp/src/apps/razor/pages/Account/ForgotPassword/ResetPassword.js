import React from "react";
import ModelValidationSummary from "components/ModelValidationSummary";
import BasicAlert from "components/BasicAlert";
import ResetForm from "./components/ResetForm";
import { hasErrors } from "services/modelstate-validation";
import LinkErrorAlert from "components/LinkErrorAlert";
import Layout from "layouts/Clean";
import Conditional, { Default } from "components/Conditional";

const ResetPassword = vm => (
  <Layout heading="Reset Password">
    <Conditional expression={hasErrors(vm.ModelState, "Link")}>
      <LinkErrorAlert linkType="password reset" />

      <Default>
        <BasicAlert
          p={2}
          title="Please enter a new password for your account."
        />
        <ModelValidationSummary errors={vm.ModelState} />
        <ResetForm {...vm} />
      </Default>
    </Conditional>
  </Layout>
);

export default ResetPassword;
