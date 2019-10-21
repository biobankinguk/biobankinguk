import React from "react";
import { Formik, Form } from "formik";
import { postObjectAsFormData, constants } from "js-forms";

const ConfirmLogout = () => (
  <>
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
  </>
);

export default ConfirmLogout;
