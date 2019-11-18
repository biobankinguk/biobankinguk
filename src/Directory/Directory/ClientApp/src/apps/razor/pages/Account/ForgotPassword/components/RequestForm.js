import React from "react";
import { Formik, Form } from "formik";
import { Stack, Button, Box } from "@chakra-ui/core";
import valSchema from "../validation/request-form";
import { useAspForm } from "hooks/aspnet-interop";
import { postObjectAsFormData } from "js-forms";
import EmailField from "components/forms/EmailField";

const RequestForm = ({ Email }) => {
  const { action, csrf } = useAspForm();

  const handleSubmit = (values, actions) => {
    postObjectAsFormData(action, {
      ...values,
      ...csrf
    });
    actions.setSubmitting(false);
  };

  return (
    <Formik
      initialValues={{ Email }}
      onSubmit={handleSubmit}
      validationSchema={valSchema}
    >
      {({ isSubmitting }) => (
        <Form noValidate>
          <Stack spacing={3} my={3}>
            <Box>
              <EmailField />
            </Box>

            <Button
              width="2xs"
              variantColor="primary"
              type="submit"
              disabled={isSubmitting}
            >
              Submit
            </Button>
          </Stack>
        </Form>
      )}
    </Formik>
  );
};

export default RequestForm;
