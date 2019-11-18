import React from "react";
import { Formik, Form } from "formik";
import { Stack, Box, Button } from "@chakra-ui/core";
import valSchema from "../validation/reset-form";
import { useAspForm } from "@/hooks/aspnet-interop";
import { postObjectAsFormData } from "js-forms";
import SetPasswordFieldGroup from "@/components/forms/SetPasswordFieldGroup";
import { hasErrors } from "@/services/modelstate-validation";

const ResetForm = ({ ModelState, Code, UserId }) => {
  const { action, csrf } = useAspForm();

  const handleSubmit = (values, actions) => {
    const obj = {
      ...values,
      ...csrf,
      Code,
      UserId
    };

    postObjectAsFormData(action, obj);
    actions.setSubmitting(false);
  };

  return (
    <Formik
      initialValues={{ Password: "", PasswordConfirm: "" }}
      onSubmit={handleSubmit}
      validationSchema={valSchema}
    >
      {({ isSubmitting }) => (
        <Form noValidate>
          <Stack spacing={3} my={3}>
            <Box>
              <SetPasswordFieldGroup initialHidden={!hasErrors(ModelState)} />
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

export default ResetForm;
