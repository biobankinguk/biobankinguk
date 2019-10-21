import React from "react";
import { Formik, Form } from "formik";
import { postObjectAsFormData, constants } from "js-forms";
import Layout from "../../Shared/Layout";

const ConfirmLogout = () => (
  <Layout>
    <h1>Logout</h1>

    <p>Would you like to logout of the UKCRC Tissue Directory?</p>

    <Formik
      onSubmit={(values, actions) => {
        const aspForm = document.getElementById("asp-form");
        postObjectAsFormData(aspForm.action, {
          ...values,
          [constants.aspNetCoreCsrf]:
            aspForm.elements[constants.aspNetCoreCsrf].value
        });
        actions.setSubmitting(false);
      }}
      render={p => (
        <Form>
          <button type="submit" disabled={p.isSubmitting}>
            Yes
          </button>
        </Form>
      )}
    />
  </Layout>
);

export default ConfirmLogout;
