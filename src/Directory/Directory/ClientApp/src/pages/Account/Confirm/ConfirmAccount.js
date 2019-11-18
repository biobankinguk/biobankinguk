import React from "react";
import { AlertDescription, Text, Box, Link } from "@chakra-ui/core";
import ResendConfirmationAlert from "@/components/ResendConfirmationAlert";
import { hasErrors } from "@/services/modelstate-validation";
import BasicAlert from "@/components/BasicAlert";
import LinkErrorAlert from "@/components/LinkErrorAlert";
import Layout from "@/layouts/Clean";
import ConditionalContent from "@/components/ConditionalContent";

const ConfirmAccount = ({ ModelState, Username }) => (
  <Layout heading="Register">
    <ConditionalContent
      condition={hasErrors(ModelState)}
      trueRender={() => (
        <>
          <LinkErrorAlert linkType="account confirmation" />
          <ResendConfirmationAlert username={Username} />
        </>
      )}
      falseRender={() => (
        <BasicAlert status="success" title="Success!" noChildWrapper>
          <AlertDescription flexDirection="column" textAlign="center">
            <Text>
              Account Confirmation was successful and your registration is now
              complete.
            </Text>
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

export default ConfirmAccount;
