import React from "react";
import { hasErrors } from "services/modelstate-validation";
import BasicAlert from "./BasicAlert";

const ModelValidationSummary = ({ errors }) => {
  if (hasErrors(errors, ""))
    return (
      <BasicAlert
        status="error"
        my={2}
        py={2}
        title="There were some errors with your request"
      >
        <ul>
          {errors[""].map(e => (
            <li>{e}</li>
          ))}
        </ul>
      </BasicAlert>
    );

  return null;
};

export default ModelValidationSummary;
