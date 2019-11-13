import React from "react";
import { AlertDescription, Text, Box, Link } from "@chakra-ui/core";
import ResendConfirmationAlert from "@/components/ResendConfirmationAlert";
import { hasErrors } from "@/services/modelstate-validation";
import BasicAlert from "@/components/BasicAlert";
import LinkErrorAlert from "@/components/LinkErrorAlert";
import ConditionalPage from "@/components/ConditionalPage";
import Layout from "@/layouts/Clean";

const ConfirmAccount = ({ ModelState, Username }) => (
  <ConditionalPage
    layout={<Layout heading="Register" />}
    condition={(() => {
      const e = hasErrors(ModelState);
      console.log(e);
      return e;
    })()}
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
);

export default ConfirmAccount;
