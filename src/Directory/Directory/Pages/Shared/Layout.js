import React from "react";
import { Box } from "@chakra-ui/core";

const Layout = ({ children }) => (
  <>
    <Box>Navbar...</Box>
    {children}
    <Box>Footer</Box>
  </>
);

export default Layout;
