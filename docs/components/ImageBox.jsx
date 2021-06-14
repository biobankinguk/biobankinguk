import { Box } from "@chakra-ui/react";

export const ImageBox = (props) =>
  props.src ? (
    <Box m={6}>
      <img
        alt="Unknown"
        {...props}
        style={{
          boxShadow: "0 0 30px rgba(.2,.2,.2,.6)",
        }}
      />
    </Box>
  ) : null;
