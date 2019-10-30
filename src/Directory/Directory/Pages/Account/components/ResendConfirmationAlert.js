import React from "react";
import TryThisAlert from "./TryThisAlert";
import appSettings from "../../../appsettings.json";

const ResendConfirmationAlert = ({ username }) => (
  <>
    <TryThisAlert
      text="Haven't received a link in your email?"
      linkText="Try sending it again"
      href={`/Account/Confirm/Resend?username=${username}`}
    />

    <TryThisAlert
      text="Still having trouble? Contact"
      linkText={appSettings.SupportEmail}
      href={`mailto:${appSettings.SupportEmail}`}
    />
  </>
);

export default ResendConfirmationAlert;
