import React from "react";
import { Formik, Form, Field } from "formik";
import { postObjectAsFormData, constants } from "js-forms";
import { Button, Stack, Box } from "@chakra-ui/core";
import valSchema from "./register-form-validation";
import CommonFormikInput from "./CommonFormikInput";

const RegisterForm = () => {
  const aspForm = document.getElementById("asp-form");
  const csrfToken = aspForm.elements[constants.aspNetCoreCsrf].value;

  const handleSubmit = (values, actions) => {
    postObjectAsFormData(aspForm.action, {
      ...values,
      [constants.aspNetCoreCsrf]: csrfToken
    });
    actions.setSubmitting(false);
  };

  const touch = ({ form: { setFieldTouched }, field: { name } }) => () =>
    setFieldTouched(name, true, false);

  return (
    <Formik
      initialValues={{
        FullName: aspForm.dataset.fullName,
        Email: aspForm.dataset.email,
        EmailConfirm: aspForm.dataset.emailConfirm,
        Password: "",
        PasswordConfirm: ""
      }}
      onSubmit={handleSubmit}
      validationSchema={valSchema}
    >
      {({ touched, isSubmitting }) => (
        <Form noValidate>
          <Stack spacing={3} my={3}>
            <Box>
              <Field name="FullName">
                {rp => (
                  <CommonFormikInput
                    {...rp}
                    label="Name"
                    placeholder="John Smith"
                    isRequired
                  />
                )}
              </Field>
            </Box>

            <Box>
              <Field name="Email">
                {rp => (
                  <CommonFormikInput
                    {...rp}
                    label="Email Address"
                    placeholder="john.smith@example.com"
                    isRequired
                    onFocus={touch(rp)}
                  />
                )}
              </Field>
            </Box>

            <Box hidden={!touched.Email}>
              <Field name="EmailConfirm">
                {rp => (
                  <CommonFormikInput
                    {...rp}
                    label="Confirm Email Address"
                    placeholder="john.smith@example.com"
                    isRequired
                  />
                )}
              </Field>
            </Box>
            <Box>
              <Field name="Password">
                {rp => (
                  <CommonFormikInput
                    {...rp}
                    label={rp.field.name}
                    placeholder={rp.field.name}
                    isRequired
                    isPassword
                    onFocus={touch(rp)}
                  />
                )}
              </Field>
            </Box>

            <Box hidden={!touched.Password}>
              <Field name="PasswordConfirm">
                {rp => (
                  <CommonFormikInput
                    {...rp}
                    label="Confirm Password"
                    placeholder="Password"
                    isRequired
                    isPassword
                  />
                )}
              </Field>
            </Box>

            <Button
              width="2xs"
              variantColor="primary"
              type="submit"
              disabled={isSubmitting}
            >
              Register
            </Button>
          </Stack>
        </Form>
      )}
    </Formik>
  );
};

export default RegisterForm;
