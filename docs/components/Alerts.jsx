import { Alert, AlertDescription, AlertIcon } from "@chakra-ui/react";

// Because writing JSX in MDX doesn't give intellisense, we wrap some of the more common cases with presets and simpler props
// Alerts are one such case

export const SimpleAlert = ({ status, children }) => (
  <Alert status={status}>
    <AlertIcon />
    <AlertDescription>{children}</AlertDescription>
  </Alert>
);

export const DifferentThemesAlert = () => (
  <SimpleAlert status="warning">
    Your Directory installation may look a little different.
  </SimpleAlert>
);
