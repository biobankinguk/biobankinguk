import React, { useState } from "react";
import { Formik, Form, Field } from "formik";
import { postObjectAsFormData } from "js-forms";
import { Button, Stack, Box, SimpleGrid } from "@chakra-ui/core";
import valSchema from "../validation/register-form";
import { hasErrors } from "@/services/modelstate-validation";
import BasicInput from "@/components/forms/BasicInput";
import PasswordRequirementsInfo from "@/components/PasswordRequirementsInfo";
import { useAspForm } from "@/hooks/aspnet-interop";

const RegisterForm = ({ ModelState, FullName, Email, EmailConfirm }) => {
  const [hideEmailConfirm, setHideEmailConfirm] = useState(
    !hasErrors(ModelState)
  );
  const [hidePasswordConfirm, setHidePasswordConfirm] = useState(
    !hasErrors(ModelState)
  );
  const touchEmail = () => setHideEmailConfirm(false);
  const touchPassword = () => setHidePasswordConfirm(false);

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
      initialValues={{
        FullName,
        Email,
        EmailConfirm,
        Password: "",
        PasswordConfirm: ""
      }}
      onSubmit={handleSubmit}
      validationSchema={valSchema}
    >
      {({ isSubmitting }) => (
        <Form noValidate>
          <Stack spacing={3} my={3}>
            <Box>
              <Field name="FullName">
                {rp => (
                  <BasicInput
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
                  <BasicInput
                    {...rp}
                    label="Email Address"
                    placeholder="john.smith@example.com"
                    isRequired
                    onFocus={touchEmail}
                  />
                )}
              </Field>
            </Box>

            <Box hidden={hideEmailConfirm}>
              <Field name="EmailConfirm">
                {rp => (
                  <BasicInput
                    {...rp}
                    label="Confirm Email Address"
                    placeholder="john.smith@example.com"
                    isRequired
                  />
                )}
              </Field>
            </Box>

            <SimpleGrid minChildWidth="300px">
              <Stack spacing={3} flexGrow={1} flexBasis="50%">
                <Box>
                  <Field name="Password">
                    {rp => (
                      <BasicInput
                        {...rp}
                        label={rp.field.name}
                        placeholder={rp.field.name}
                        isRequired
                        isPassword
                        onFocus={touchPassword}
                      />
                    )}
                  </Field>
                </Box>
                <Box hidden={hidePasswordConfirm}>
                  <Field name="PasswordConfirm">
                    {rp => (
                      <BasicInput
                        {...rp}
                        label="Confirm Password"
                        placeholder="Password"
                        isRequired
                        isPassword
                      />
                    )}
                  </Field>
                </Box>
              </Stack>

              <PasswordRequirementsInfo m={2} flexBasis="40%" />
            </SimpleGrid>

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
