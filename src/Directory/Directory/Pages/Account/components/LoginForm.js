import React from "react";
import { Formik, Form, Field } from "formik";
import { postObjectAsFormData, constants } from "js-forms";
import valSchema from "./login-form-validation";
import {
  FormLabel,
  FormControl,
  Input,
  FormErrorMessage,
  Button,
  Link,
  Flex
} from "@chakra-ui/core";

const LoginForm = () => {
  const aspForm = document.getElementById("asp-form");
  const csrfToken = aspForm.elements[constants.aspNetCoreCsrf].value;

  const loginPost = values => {
    postObjectAsFormData(aspForm.action, {
      ...values,
      [constants.aspNetCoreCsrf]: csrfToken
    });
  };

  const handleCancel = () => loginPost({ button: "" });

  return (
    <Formik
      initialValues={{ Username: aspForm.dataset.username, Password: "" }}
      onSubmit={(values, actions) => {
        loginPost({ ...values, button: "login" });
        actions.setSubmitting(false);
      }}
      validationSchema={valSchema}
      render={p => (
        <Form>
          <Field name="Username">
            {({ field, form }) => (
              <FormControl
                isRequired
                isInvalid={form.errors.Username && form.touched.Username}
              >
                <FormLabel htmlFor="Username">Username</FormLabel>
                <Input {...field} id="Username" placeholder="Username" />
                <FormErrorMessage>{form.errors.Username}</FormErrorMessage>
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

          <Flex justifyContent="space-between" mb={2} mt={3}>
            <Button
              width="2xs"
              variantColor="primary"
              type="submit"
              disabled={p.isSubmitting}
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

          <Link color="primary.500" href="#">
            Forgot password?
          </Link>
        </Form>
      )}
    />
  );
};

export default LoginForm;
