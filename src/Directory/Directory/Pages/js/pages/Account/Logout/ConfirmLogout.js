import React from "react";
import { Formik, Form } from "formik";
import { postObjectAsFormData } from "js-forms";
import { Text, Button, Flex } from "@chakra-ui/core";
import Layout from "@/components/Layout";
import { useAspForm } from "@/hooks/aspnet-interop";

const ConfirmLogout = () => {
  const { action, csrf } = useAspForm();
  return (
    <Layout heading="Logout">
      <Flex flexDirection="column" alignItems="center">
        <Text p={4}>
          Would you like to logout of the UKCRC Tissue Directory?
        </Text>

        <Formik
          onSubmit={(values, actions) => {
            postObjectAsFormData(action, {
              ...values,
              ...csrf
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
};

export default ConfirmLogout;
