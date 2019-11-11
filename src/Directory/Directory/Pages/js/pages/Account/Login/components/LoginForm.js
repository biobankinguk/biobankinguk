import React from "react";
import { Formik, Form, Field } from "formik";
import { postObjectAsFormData, constants } from "js-forms";
import { Button, Flex, Stack, Box } from "@chakra-ui/core";
import valSchema from "../login-form-validation";
import CommonFormikInput from "../../../../components/CommonFormikInput";

const LoginForm = ({ Username }) => {
  const aspForm = document.getElementById("asp-form");
  const csrfToken = aspForm.elements[constants.aspNetCoreCsrf].value;

  const post = values => {
    postObjectAsFormData(aspForm.action, {
      ...values,
      [constants.aspNetCoreCsrf]: csrfToken
    });
  };

  const handleCancel = () => post({ button: "" });
  const handleSubmit = (values, actions) => {
    post({ ...values, button: "login" });
    actions.setSubmitting(false);
  };

  return (
    <Formik
      initialValues={{ Username, Password: "" }}
      onSubmit={handleSubmit}
      validationSchema={valSchema}
    >
      {({ isSubmitting }) => (
        <Form noValidate>
          <Stack spacing={3} my={3}>
            <Box>
              <Field name="Username">
                {rp => (
                  <CommonFormikInput
                    {...rp}
                    label="Email Address"
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
                  />
                )}
              </Field>
            </Box>

            <Flex justifyContent="space-between">
              <Button
                width="2xs"
                variantColor="primary"
                type="submit"
                disabled={isSubmitting}
              >
                Login
              </Button>
              <Button
                variant="outline"
                variantColor="dark"
                width="2xs"
                type="button"
                onClick={handleCancel}
              >
                Cancel
              </Button>
            </Flex>
          </Stack>
        </Form>
      )}
    </Formik>
  );
};

export default LoginForm;
