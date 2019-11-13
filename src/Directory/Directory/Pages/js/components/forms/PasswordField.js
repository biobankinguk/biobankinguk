import React from "react";
import { Field } from "formik";
import BasicInput from "./BasicInput";

const PasswordField = p => (
  <Field name="Password">
    {rp => (
      <BasicInput
        {...rp}
        label={rp.field.name}
        placeholder={rp.field.name}
        isRequired
        isPassword
        {...p}
      />
    )}
  </Field>
);

export default PasswordField;
