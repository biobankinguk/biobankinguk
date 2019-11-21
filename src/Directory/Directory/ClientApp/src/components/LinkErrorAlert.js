import React from "react";
import BasicAlert from "./BasicAlert";

const LinkErrorAlert = ({linkType}) => (
  <BasicAlert
    status="error"
    title={`There seems to be a problem with this ${linkType} link.`}
  >
    Your user ID or {linkType} token is invalid, or has expired.
  </BasicAlert>
);

export default LinkErrorAlert;
