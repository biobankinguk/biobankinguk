import React from "react";
import { Formik, Form, Field } from "formik";
import { Stack, Box, Button } from "@chakra-ui/core";
import valSchema from "../validation/request-form";
import CommonFormikInput from "Components/CommonFormikInput";
import { useAspForm } from "Hooks/aspnet-interop";
import { postObjectAsFormData } from "js-forms";

const RequestForm = ({ Email }) => {
  const { action, csrf } = useAspForm();

  const handleSubmit = (actions, values) => {
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
                  <CommonFormikInput
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
