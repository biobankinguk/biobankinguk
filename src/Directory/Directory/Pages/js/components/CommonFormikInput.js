import React from "react";
import {
  FormControl,
  FormLabel,
  Input,
  FormErrorMessage
} from "@chakra-ui/core";

/**
 * Should account for 90% of text inputs inside Formik Fields.
 *
 * - Sets Input element id to the field name
 * - Has a label, indicating required status
 * - Allows text or password fields
 * - Uses placeholder text
 * - Displays validation errors
 * - forwards all other props to <Input />
 *
 * @param {*} props render props from Formik Field, and additional props
 */
const CommonFormikInput = ({
  field,
  form: { errors, touched },
  label,
  placeholder,
  isPassword = false,
  isRequired = false,
  ...p
}) => (
  <FormControl
    isRequired={isRequired}
    isInvalid={errors[field.name] && touched[field.name]}
  >
    <FormLabel htmlFor={field.name}>{label}</FormLabel>
    <Input
      {...field}
      id={field.name}
      placeholder={placeholder}
      type={isPassword ? "password" : "text"}
      {...p}
    />
    <FormErrorMessage>{errors[field.name]}</FormErrorMessage>
  </FormControl>
);

export default CommonFormikInput;
