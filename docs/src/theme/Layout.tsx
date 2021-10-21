import React, { useEffect } from "react";
import OriginalLayout from "@theme-original/Layout";
import useThemeContext from "@theme/hooks/useThemeContext";
import { useColorMode } from "@chakra-ui/react";

const ColorModeSync = () => {
  const { isDarkTheme } = useThemeContext();
  const { colorMode, toggleColorMode } = useColorMode();

  useEffect(() => {
    switch (colorMode) {
      case "dark":
        if (!isDarkTheme) toggleColorMode();
        break;
      default:
        if (isDarkTheme) toggleColorMode();
    }
  }, [isDarkTheme, colorMode, toggleColorMode]);

  return null;
};

const Layout = ({ children }) => (
  <OriginalLayout>
    <ColorModeSync />
    {children}
  </OriginalLayout>
);

export default Layout;
