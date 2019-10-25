import React from "react";
import {
  Alert,
  Flex,
  AlertIcon,
  AlertTitle,
  AlertDescription
} from "@chakra-ui/core";

export const defaultElementId = "asp-validation-summary";
export const defaultErrorClass = "validation-summary-errors";

export const useAspValidationSummary = (
  elementId = defaultElementId,
  errorClass = defaultErrorClass
) => {
  const element = document.getElementById(elementId);
  const hasErrors = element && element.className.includes(errorClass);
  const summaryHTML = element ? element.innerHTML : null;
  return { element, summaryHTML, hasErrors };
};

const AspValidationSummary = ({
  elementId = defaultElementId,
  errorClass = defaultErrorClass
}) => {
  const { hasErrors, summaryHTML } = useAspValidationSummary(
    elementId,
    errorClass
  );

  if (hasErrors)
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
          dangerouslySetInnerHTML={{ __html: summaryHTML }}
        ></AlertDescription>
      </Alert>
    );

  return null;
};

export default AspValidationSummary;
