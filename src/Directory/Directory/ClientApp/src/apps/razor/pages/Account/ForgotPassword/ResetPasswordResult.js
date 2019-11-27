import React from "react";
import { AlertDescription, Text, Box, Link } from "@chakra-ui/core";
import { hasErrors } from "services/modelstate-validation";
import BasicAlert from "components/BasicAlert";
import LinkErrorAlert from "components/LinkErrorAlert";
import Layout from "layouts/Clean";
import Conditional from "components/Conditional";

const ResetPasswordResult = ({ ModelState }) => (
  <Layout heading="Reset Password">
    <Conditional expression={ModelState}>
      <LinkErrorAlert condition={hasErrors} linkType="password reset" />

      <BasicAlert default status="success" title="Success!" noChildWrapper>
        <AlertDescription flexDirection="column" textAlign="center">
          <Text>Your new password has been successfully set!</Text>
          <Box mt={3}>
            <Link color="primary.500" href="/auth/login">
              Return home
            </Link>
          </Box>
        </AlertDescription>
      </BasicAlert>
    </Conditional>
  </Layout>
);

export default ResetPasswordResult;
