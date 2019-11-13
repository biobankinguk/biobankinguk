import React from "react";
import Layout from "@/components/Layout";
import { hasErrors } from "@/services/modelstate-validation";
import ConfirmationSent from "@/components/ConfirmationSent";
import BasicAlert from "@/components/BasicAlert";

const ResendConfirm = ({ ModelState, Username }) => {
  let content;
  if (hasErrors(ModelState, "")) {
    content = (
      <BasicAlert status="error" title="There seems to be a problem.">
        Failed to resend confirmation link for this invalid User ID.
      </BasicAlert>
    );
  } else {
    content = <ConfirmationSent username={Username} />;
  }

  return <Layout heading="Register">{content}</Layout>;
};

export default ResendConfirm;
