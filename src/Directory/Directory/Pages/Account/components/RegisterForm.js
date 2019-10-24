import React from "react";
import { Formik, Form, Field } from "formik";
import { postObjectAsFormData, constants } from "js-forms";
import valSchema from "./register-form-validation";
import {
  FormLabel,
  FormControl,
  Input,
  FormErrorMessage,
  Button,
  Link,
  Flex,
  Box
} from "@chakra-ui/core";

const RegisterForm = () => {
  const aspForm = document.getElementById("asp-form");
  const csrfToken = aspForm.elements[constants.aspNetCoreCsrf].value;

  const post = values => {
    postObjectAsFormData(aspForm.action, {
      ...values,
      [constants.aspNetCoreCsrf]: csrfToken
    });
  };

  return (
    <Formik
      initialValues={{
        FullName: aspForm.dataset.fullName,
        Email: aspForm.dataset.email,
        EmailConfirm: aspForm.dataset.emailConfirm,
        Password: "",
        PasswordConfirm: ""
      }}
      onSubmit={(values, actions) => {
        loginPost({ ...values, button: "login" });
        actions.setSubmitting(false);
      }}
      validationSchema={valSchema}
      render={p => (
        <Form>
          <Field name="FullName">
            {({ field, form }) => (
              <FormControl
                isRequired
                isInvalid={form.errors.FullName && form.touched.FullName}
              >
                <FormLabel htmlFor="FullName">Name</FormLabel>
                <Input {...field} id="FullName" placeholder="John Smith" />
                <FormErrorMessage>{form.errors.FullName}</FormErrorMessage>
              </FormControl>
            )}
          </Field>

          <Field name="Email">
            {({ field, form }) => (
              <FormControl
                isRequired
                isInvalid={form.errors.Email && form.touched.Email}
              >
                <FormLabel htmlFor="Email">Email Address</FormLabel>
                <Input
                  {...field}
                  id="Email"
                  placeholder="john.smith@example.com"
                />
                <FormErrorMessage>{form.errors.Email}</FormErrorMessage>
              </FormControl>
            )}
          </Field>

          <Field name="EmailConfirm">
            {({ field, form }) => (
              <FormControl
                isRequired
                isInvalid={
                  form.errors.EmailConfirm && form.touched.EmailConfirm
                }
              >
                <FormLabel htmlFor="EmailConfirm">
                  Confirm Email Address
                </FormLabel>
                <Input
                  {...field}
                  id="EmailConfirm"
                  placeholder="john.smith@example.com"
                />
                <FormErrorMessage>{form.errors.EmailConfirm}</FormErrorMessage>
              </FormControl>
            )}
          </Field>

          <Field name="Password">
            {({ field, form }) => (
              <FormControl
                mt={3}
                isRequired
                isInvalid={form.errors.Password && form.touched.Password}
              >
                <FormLabel htmlFor="Password">Password</FormLabel>
                <Input
                  {...field}
                  type="password"
                  id="Password"
                  placeholder="Password"
                />
                <FormErrorMessage>{form.errors.Password}</FormErrorMessage>
              </FormControl>
            )}
          </Field>

          <Field name="PasswordConfirm">
            {({ field, form }) => (
              <FormControl
                mt={3}
                isRequired
                isInvalid={form.errors.PasswordConfirm && form.touched.PasswordConfirm}
              >
                <FormLabel htmlFor="PasswordConfirm">Confirm Password</FormLabel>
                <Input
                  {...field}
                  type="password"
                  id="PasswordConfirm"
                  placeholder="Confirm Password"
                />
                <FormErrorMessage>{form.errors.PasswordConfirm}</FormErrorMessage>
              </FormControl>
            )}
          </Field>

          <Button
            mb={2}
            mt={3}
            width="2xs"
            variantColor="primary"
            type="submit"
            disabled={p.isSubmitting}
          >
            Register
          </Button>

          <Box>
            Already a user?
            <Link color="primary.500" href="#">
              Log in
            </Link>
          </Box>
        </Form>
      )}
    />
  );
};

export default RegisterForm;
