import React from "react";
import {
  Alert,
  Flex,
  AlertIcon,
  AlertTitle,
  AlertDescription
} from "@chakra-ui/core";

const BasicAlert = ({
  title,
  noIcon,
  noChildWrapper,
  children,
  ...p
}) => {
  let Title = null;
  if (title)
    Title = (
      <Flex alignItems="center">
        {noIcon ? null : <AlertIcon />}
        <AlertTitle>Almost there!</AlertTitle>
      </Flex>
    );

  let Description = null;
  if (children) {
    if (noChildWrapper) Description = children;
    else Description = <AlertDescription>{children}</AlertDescription>;
  }

  return (
    <Alert status="info" variant="left-accent" flexDirection="column" {...p}>
      {Title}
      {Description}
    </Alert>
  );
};

export default BasicAlert;
