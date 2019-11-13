import React from "react";
import { Formik, Form, Field } from "formik";
import { Stack, Box, Button } from "@chakra-ui/core";
import valSchema from "../validation/request-form";
import BasicInput from "@/components/forms/BasicInput";
import { useAspForm } from "@/hooks/aspnet-interop";
import { postObjectAsFormData } from "js-forms";

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
              <Field name="Email">
                {rp => (
                  <BasicInput
                    {...rp}
                    label="Email Address"
                    placeholder="john.smith@example.com"
                    isRequired
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
              Submit
            </Button>
          </Stack>
        </Form>
      )}
    </Formik>
  );
};

export default RequestForm;
