import React from "react";
import { AlertDescription, Text, Box, Link } from "@chakra-ui/core";
import { hasErrors } from "@/services/modelstate-validation";
import BasicAlert from "@/components/BasicAlert";
import LinkErrorAlert from "@/components/LinkErrorAlert";
import ConditionalPage from "@/components/ConditionalPage";
import Layout from "@/layouts/Clean";

const ResetPasswordResult = ({ ModelState }) => (
  <ConditionalPage
    layout={<Layout heading="Reset Password" />}
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
);

export default ResetPasswordResult;
