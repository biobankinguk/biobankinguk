import React from "react";
import { AlertDescription, Text, Box, Link } from "@chakra-ui/core";
import ResendConfirmationAlert from "components/ResendConfirmationAlert";
import { hasErrors } from "services/modelstate-validation";
import BasicAlert from "components/BasicAlert";
import LinkErrorAlert from "components/LinkErrorAlert";
import Layout from "layouts/Clean";
import Conditional, { Default, When } from "components/Conditional";

const ConfirmAccount = ({ ModelState, Username, SupportEmail }) => (
  <Layout heading="Register">
    <Conditional expression={ModelState}>
      <When condition={hasErrors}>
        <LinkErrorAlert linkType="account confirmation" />
        <ResendConfirmationAlert
          username={Username}
          supportEmail={SupportEmail}
        />
      </When>

      <Default>
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
      </Default>
    </Conditional>
  </Layout>
);

export default ConfirmAccount;
