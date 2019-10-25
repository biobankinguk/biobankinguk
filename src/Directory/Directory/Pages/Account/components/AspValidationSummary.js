import React from "react";
import {
  Alert,
  Flex,
  AlertIcon,
  AlertTitle,
  AlertDescription
} from "@chakra-ui/core";

const AspValidationSummary = ({
  elementId = "asp-validation-summary",
  errorClass = "validation-summary-errors"
}) => {
  const valSummary = document.getElementById(elementId);
  if (valSummary && valSummary.className.includes(errorClass))
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
          <AlertTitle>
            There were some errors with the form submission
          </AlertTitle>
        </Flex>
        <AlertDescription
          dangerouslySetInnerHTML={{ __html: valSummary.innerHTML }}
        ></AlertDescription>
      </Alert>
    );

  return null;
};

export default AspValidationSummary;
