import React from "react";
import {
  AlertTitle,
  AlertDescription,
  Alert,
  Link,
  AlertIcon,
  Flex,
  Box
} from "@chakra-ui/core";
import appSettings from "../../../appsettings.json";

const ConfirmationSent = () => (
  <>
    <Alert status="info" variant="left-accent" flexDirection="column">
      <Flex alignItems="center">
        <AlertIcon />
        <AlertTitle>Almost there!</AlertTitle>
      </Flex>
      <AlertDescription textAlign="center">
        To complete your registration, please confirm your email address by
        clicking the link we've emailed you.
      </AlertDescription>
    </Alert>
    <Alert status="info" variant="left-accent" flexDirection="column">
      <Flex alignItems="center">
        <AlertIcon name="question" />
        <AlertTitle>Haven't received a link in your email?</AlertTitle>
      </Flex>
      <AlertDescription flexDirection="column" textAlign="center">
        <Link color="primary.500" href="/auth/login">
          {/* TODO: resend link */}
          Click here to resend it
        </Link>
        <Box>
          or contact{" "}
          <Link color="primary.500" href={`mailto:${appSettings.SupportEmail}`}>
            {appSettings.SupportEmail}
          </Link>{" "}
          if you're having trouble.
        </Box>
      </AlertDescription>
    </Alert>
  </>
);

export default ConfirmationSent;
