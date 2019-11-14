import React from "react";
import { Box } from "@chakra-ui/core";
import ResendConfirmationAlert from "./ResendConfirmationAlert";
import BasicAlert from "./BasicAlert";

const UnconfirmedAccountFound = ({
  message = "This account already exists, but seems to be unconfirmed.",
  username
}) => (
  <Box>
    <BasicAlert status="error" title={message} />
    <ResendConfirmationAlert username={username} />
  </Box>
);

export default UnconfirmedAccountFound;
