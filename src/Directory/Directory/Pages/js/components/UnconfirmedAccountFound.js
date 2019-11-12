import React from "react";
import { Box, Alert, Flex, AlertIcon, AlertTitle } from "@chakra-ui/core";
import ResendConfirmationAlert from "./ResendConfirmationAlert";

const UnconfirmedAccountFound = ({
  message = "This account already exists, but seems to be unconfirmed.",
  username
}) => (
  <Box>
    <Alert status="error" variant="left-accent" flexDirection="column">
      <Flex alignItems="center">
        <AlertIcon />
        <AlertTitle>{message}</AlertTitle>
      </Flex>
    </Alert>

    <ResendConfirmationAlert username={username} />
  </Box>
);

export default UnconfirmedAccountFound;
