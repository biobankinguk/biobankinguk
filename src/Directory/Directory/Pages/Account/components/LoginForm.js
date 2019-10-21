import React from "react";
import { Formik, Form, Field, ErrorMessage } from "formik";
import { postObjectAsFormData, constants } from "js-forms";
import valSchema from "./login-form-validation";

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
          <label htmlFor="Username">Username</label>
          <Field name="Username" />
          <ErrorMessage name="Username" />

          <label htmlFor="password">Password</label>
          <Field type="password" name="Password" />
          <ErrorMessage name="Password" />

          <button type="submit" disabled={p.isSubmitting}>
            Login
          </button>
          <button type="button" onClick={handleCancel}>
            Cancel
          </button>

          <a href="#">Forgot password?</a>
        </Form>
      )}
    />
  );
};

export default LoginForm;
