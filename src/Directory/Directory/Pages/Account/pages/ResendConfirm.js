import React from "react";
import {
  Alert,
  Flex,
  AlertIcon,
  AlertTitle,
  AlertDescription
} from "@chakra-ui/core";
import { useAspValidationSummary } from "../components/AspValidationSummary";
import Layout from "../../Shared/Layout";
import ConfirmationSent from "../components/ConfirmationSent";

const ResendConfirm = () => {
  const { hasErrors } = useAspValidationSummary();

  let content;
  if (hasErrors) {
    content = (
      <Alert status="error" variant="left-accent" flexDirection="column">
        <Flex alignItems="center">
          <AlertIcon />
          <AlertTitle>There seems to be a problem.</AlertTitle>
        </Flex>
        <AlertDescription flexDirection="column" textAlign="center">
          Failed to resend confirmation link for this invalid User ID.
        </AlertDescription>
      </Alert>
    );
  } else {
    content = <ConfirmationSent />;
  }

  return <Layout heading="Register">{content}</Layout>;
};

export default ResendConfirm;
