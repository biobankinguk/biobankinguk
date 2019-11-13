import React from "react";
import Layout from "@/layouts/Clean";
import { hasErrors } from "@/services/modelstate-validation";
import ConfirmationSent from "@/components/ConfirmationSent";
import BasicAlert from "@/components/BasicAlert";
import ConditionalPage from "@/components/ConditionalPage";

const ResendConfirm = ({ ModelState, Username }) => (
  <ConditionalPage
    layout={<Layout heading="Register" />}
    condition={hasErrors(ModelState, "")}
    trueRender={() => (
      <BasicAlert status="error" title="There seems to be a problem.">
        Failed to resend confirmation link for this invalid User ID.
      </BasicAlert>
    )}
    falseRender={() => <ConfirmationSent username={Username} />}
  />
);

export default ResendConfirm;
