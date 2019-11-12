import React from "react";
import {
  Alert,
  Flex,
  AlertIcon,
  AlertTitle,
  AlertDescription
} from "@chakra-ui/core";
import Layout from "Components/Layout";
import { hasErrors } from "Services/modelstate-validation";
import ConfirmationSent from "Components/ConfirmationSent";

const ResendConfirm = ({ ModelState, Username }) => {
  let content;
  if (hasErrors(ModelState, "")) {
    content = (
      <Alert status="error" variant="left-accent" flexDirection="column">
        <Flex alignItems="center">
          <AlertIcon />
          <AlertTitle>There seems to be a problem.</AlertTitle>
        </Flex>
        <AlertDescription>
          Failed to resend confirmation link for this invalid User ID.
        </AlertDescription>
      </Alert>
    );
  } else {
    content = <ConfirmationSent username={Username} />;
  }

  return <Layout heading="Register">{content}</Layout>;
};

export default ResendConfirm;
