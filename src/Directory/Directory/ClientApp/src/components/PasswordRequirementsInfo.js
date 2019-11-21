import React from "react";
import BasicAlert from "./BasicAlert";

const PasswordRequirementsInfo = p => (
  <BasicAlert title="Password Requirements" p={2} variant="top-accent" {...p}>
    <ul>
      <li>Use at least 8 characters</li>
      <li>Use at least one uppercase letter</li>
      <li>Use at least one lowercase letter</li>
      <li>Use at least one number</li>
      <li>Use at least one special character</li>
    </ul>
  </BasicAlert>
);

export default PasswordRequirementsInfo;
