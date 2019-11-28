import React from "react";
import Layout from "layouts/Clean";
import { hasErrors } from "services/modelstate-validation";
import ConfirmationSent from "components/ConfirmationSent";
import BasicAlert from "components/BasicAlert";
import Conditional from "components/Conditional";

const ResendConfirm = ({ ModelState, Username }) => (
  <Layout heading="Register">
    <Conditional expression={hasErrors(ModelState, "")}>
      <BasicAlert status="error" title="There seems to be a problem.">
        Failed to resend confirmation link for this invalid User ID.
      </BasicAlert>

      <ConfirmationSent default username={Username} />
    </Conditional>
  </Layout>
);

export default ResendConfirm;
