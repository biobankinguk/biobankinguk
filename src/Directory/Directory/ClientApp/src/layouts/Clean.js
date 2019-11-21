import React from "react";
import { Box, Flex, Heading } from "@chakra-ui/core";
import Logo from "./components/Logo";

const Layout = ({ heading, children }) => (
  <Flex justifyContent="center" mt={[0, 16]}>
    <Box w={["100%", "80%", "70%", "50%"]} p={[0, 0, 4]}>
      <Flex
        rounded="lg"
        bg="white"
        p={2}
        alignItems="center"
        flexDirection={["column", "row"]}
      >
        <Logo />
        <Heading ml={[0, 4]} mt={[2, 0]}>
          {heading}
        </Heading>
      </Flex>
      <Box p={4}>{children}</Box>
    </Box>
  </Flex>
);

export default Layout;
