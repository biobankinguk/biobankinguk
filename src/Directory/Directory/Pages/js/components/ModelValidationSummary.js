import React from "react";
import {
  Alert,
  Flex,
  AlertIcon,
  AlertTitle,
  AlertDescription
} from "@chakra-ui/core";
import { hasErrors } from "@/services/modelstate-validation";

const ModelValidationSummary = ({ errors }) => {
  if (hasErrors(errors, ""))
    return (
      <Alert
        status="error"
        my={2}
        p={2}
        variant="left-accent"
        flexDirection="column"
      >
        <Flex alignItems="center">
          <AlertIcon />
          <AlertTitle>There were some errors with your request</AlertTitle>
        </Flex>
        <AlertDescription>
          <ul>
            {errors[""].map(e => (
              <li>{e}</li>
            ))}
          </ul>
        </AlertDescription>
      </Alert>
    );

  return null;
};

export default ModelValidationSummary;
