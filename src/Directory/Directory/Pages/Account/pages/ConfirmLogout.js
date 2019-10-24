import React from "react";
import { Formik, Form } from "formik";
import { postObjectAsFormData, constants } from "js-forms";
import Layout from "../../Shared/Layout";
import { Text, Button, Flex } from "@chakra-ui/core";

const ConfirmLogout = () => (
  <Layout heading="Logout">
    <Flex flexDirection="column" alignItems="center">
      <Text p={4}>Would you like to logout of the UKCRC Tissue Directory?</Text>

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
            <Button
              variantColor="primary"
              type="submit"
              disabled={p.isSubmitting}
            >
              Logout
            </Button>
          </Form>
        )}
      />
    </Flex>
  </Layout>
);

export default ConfirmLogout;
