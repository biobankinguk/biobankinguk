import React from "react";
import { Formik, Form, Field, ErrorMessage } from "formik";
import * as Yup from "yup";
import { postObjectAsFormData, constants } from "js-forms";

const LoginSchema = Yup.object().shape({
  Username: Yup.string()
    .test("valid-username", "Invalid email", v => {
      const isEmail = Yup.string()
        .email()
        .isValidSync(v);
      console.log(isEmail);

      const isLocal = Yup.string()
        .matches(/@localhost$/)
        .isValidSync(v);
      console.log(isLocal);

      return isEmail || isLocal;
    })
    .required("Required"),
  Password: Yup.string().required("Required")
});

const Login = () => {
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
      validationSchema={LoginSchema}
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

export default Login;
