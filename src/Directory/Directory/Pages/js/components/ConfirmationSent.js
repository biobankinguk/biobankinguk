import React from "react";
import {
  AlertTitle,
  AlertDescription,
  Alert,
  AlertIcon,
  Flex
} from "@chakra-ui/core";
import ResendConfirmationAlert from "./ResendConfirmationAlert";

const ConfirmationSent = ({ username }) => (
  <>
    <Alert status="info" variant="left-accent" flexDirection="column">
      <Flex alignItems="center">
        <AlertIcon />
        <AlertTitle>Almost there!</AlertTitle>
      </Flex>
      <AlertDescription>
        To complete your registration, please confirm your email address by
        clicking the link we've emailed you.
      </AlertDescription>
    </Alert>

    <ResendConfirmationAlert username={username} />
  </>
);

export default ConfirmationSent;
