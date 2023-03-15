import React, { useEffect } from "react";
import OriginalLayout from "@theme-original/Layout";
import { useColorMode as useDocsColorMode } from "@docusaurus/theme-common";
import { useColorMode as useChakraColorMode } from "@chakra-ui/react";

const ColorModeSync = () => {
  const { colorMode: docsColorMode } = useDocsColorMode();
  const { colorMode: chakraColorMode, toggleColorMode: toggleChakraColorMode } =
    useChakraColorMode();

  useEffect(() => {
    if (docsColorMode !== chakraColorMode) toggleChakraColorMode();
  }, [docsColorMode, chakraColorMode, toggleChakraColorMode]);

  return null;
};

const Layout = ({ children }) => (
  <OriginalLayout>
    <ColorModeSync />
    {children}
  </OriginalLayout>
);

export default Layout;
