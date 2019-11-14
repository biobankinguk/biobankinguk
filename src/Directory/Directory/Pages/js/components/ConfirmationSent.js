import React from "react";
import ResendConfirmationAlert from "./ResendConfirmationAlert";
import BasicAlert from "./BasicAlert";

const ConfirmationSent = ({ username }) => (
  <>
    <BasicAlert title="Almost there!">
      To complete your registration, please confirm your email address by
      clicking the link we've emailed you.
    </BasicAlert>

    <ResendConfirmationAlert username={username} />
  </>
);

export default ConfirmationSent;
