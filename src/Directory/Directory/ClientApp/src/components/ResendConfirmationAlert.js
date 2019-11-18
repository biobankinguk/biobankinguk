import React from "react";
import TryThisAlert from "./TryThisAlert";

const ResendConfirmationAlert = ({ username, supportEmail }) => (
  <>
    <TryThisAlert
      text="Haven't received a link in your email?"
      linkText="Try sending it again"
      href={`/Account/Confirm/Resend?username=${username}`}
    />

    <TryThisAlert
      text="Still having trouble? Contact"
      linkText={supportEmail}
      href={`mailto:${supportEmail}`}
    />
  </>
);

export default ResendConfirmationAlert;
