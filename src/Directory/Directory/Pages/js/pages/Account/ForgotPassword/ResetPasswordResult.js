import React from "react";
import { AlertDescription, Text, Box, Link } from "@chakra-ui/core";
import { hasErrors } from "@/services/modelstate-validation";
import BasicAlert from "@/components/BasicAlert";
import LinkErrorAlert from "@/components/LinkErrorAlert";
import Layout from "@/layouts/Clean";
import ConditionalContent from "@/components/ConditionalContent";

const ResetPasswordResult = ({ ModelState }) => (
  <Layout heading="Reset Password">
  <ConditionalContent
    condition={hasErrors(ModelState)}
    trueRender={() => <LinkErrorAlert linkType="password reset" />}
    falseRender={() => (
      <BasicAlert status="success" title="Success!" noChildWrapper>
        <AlertDescription flexDirection="column" textAlign="center">
          <Text>Your new password has been successfully set!</Text>
          <Box mt={3}>
            <Link color="primary.500" href="/auth/login">
              Return home
            </Link>
          </Box>
        </AlertDescription>
      </BasicAlert>
    )}
  />
  </Layout>
);

export default ResetPasswordResult;
