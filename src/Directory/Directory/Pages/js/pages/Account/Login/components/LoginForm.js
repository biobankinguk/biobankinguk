import React from "react";
import { Formik, Form } from "formik";
import { postObjectAsFormData } from "js-forms";
import { Button, Flex, Stack, Box } from "@chakra-ui/core";
import valSchema from "../validation/login-form";
import { useAspForm } from "@/hooks/aspnet-interop";
import EmailField from "@/components/forms/EmailField";
import PasswordField from "@/components/forms/PasswordField";

const LoginForm = ({ Username }) => {
  const { action, csrf } = useAspForm();

  const post = values => {
    postObjectAsFormData(action, {
      ...values,
      ...csrf
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
              <EmailField name="Username" />
            </Box>

            <Box>
              <PasswordField />
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
