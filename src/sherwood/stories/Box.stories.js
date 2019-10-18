import React from "react";
import { storiesOf } from "@storybook/react";
import Box from "../Box";

storiesOf("Box", module)
  .add("default", () => <Box>Hello World</Box>)
  .add("styled-system", () => <Box color="red" ml={50}>Hello World</Box>);
