import React from "react";
import { ThemeProvider, CSSReset } from "@chakra-ui/core";
import { Global } from "@emotion/core";

const App = ({ theme, page: Page }) => {
  return (
    <ThemeProvider theme={theme}>
      <CSSReset />
      <Global
        styles={{
          body: { backgroundColor: theme.colors.defaultBackground }
        }}
      />
      <Page {...window.__TDCC__.ViewModel} />
    </ThemeProvider>
  );
};

export default App;
