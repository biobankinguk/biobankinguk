import React from "react";
import {
  AlertDescription,
  Text,
  Box,
  Link
} from "@chakra-ui/core";
import Layout from "@/components/Layout";
import ResendConfirmationAlert from "@/components/ResendConfirmationAlert";
import { hasErrors } from "@/services/modelstate-validation";
import BasicAlert from "@/components/BasicAlert";

const ConfirmAccount = ({ ModelState, Username }) => {
  let content;
  if (hasErrors(ModelState)) {
    content = (
      <>
        <BasicAlert
          status="error"
          title="There seems to be a problem with this confirmation link."
        >
          Your user ID or account confirmation token is invalid, or has expired.
        </BasicAlert>

        <ResendConfirmationAlert username={Username} />
      </>
    );
  } else {
    content = (
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
    );
  }

  return <Layout heading="Register">{content}</Layout>;
};

export default ConfirmAccount;
