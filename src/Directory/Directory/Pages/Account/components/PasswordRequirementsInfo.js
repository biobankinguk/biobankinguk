import React from "react";
import {
  Box,
  Flex,
  Icon,
  Text,
  Alert,
  AlertTitle,
  AlertIcon,
  AlertDescription
} from "@chakra-ui/core";

const PasswordRequirementsInfo = p => (
  <Alert status="info" p={2} variant="top-accent" flexDirection="column" {...p}>
    <Flex alignItems="center">
      <AlertIcon />
      <AlertTitle>Password requirements</AlertTitle>
    </Flex>
    <AlertDescription>
      <ul>
        <li>Use at least 8 characters</li>
        <li>Use at least one uppercase letter</li>
        <li>Use at least one lowercase letter</li>
        <li>Use at least one number</li>
        <li>Use at least one special character</li>
      </ul>
    </AlertDescription>
  </Alert>
);

export default PasswordRequirementsInfo;
