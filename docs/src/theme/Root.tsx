import React from "react";
import { ChakraProvider } from "@chakra-ui/react";

const Root = ({ children }) => {
  return <ChakraProvider>{children}</ChakraProvider>;
};

export default Root;
