import React from "react";
import BasicInput from "./BasicInput";
import { Field } from "formik";

const EmailField = ({ name = "Email", ...p }) => (
  <Field name={name}>
    {rp => (
      <BasicInput
        {...rp}
        label="Email Address"
        placeholder="john.smith@example.com"
        isRequired
        {...p}
      />
    )}
  </Field>
);

export default EmailField;
