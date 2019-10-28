import React from "react";
import {
  Alert,
  Flex,
  AlertIcon,
  AlertTitle,
  AlertDescription,
  Text,
  Box,
  Link
} from "@chakra-ui/core";
import { useAspValidationSummary } from "../components/AspValidationSummary";
import appSettings from "../../../appsettings.json";
import Layout from "../../Shared/Layout";

const ConfirmAccount = () => {
  const { hasErrors } = useAspValidationSummary();

  let content;
  if (hasErrors) {
    content = (
      <Alert status="error" variant="left-accent" flexDirection="column">
        <Flex alignItems="center">
          <AlertIcon />
          <AlertTitle>
            There seems to be a problem with this confirmation link.
          </AlertTitle>
        </Flex>
        <AlertDescription flexDirection="column" textAlign="center">
          <Text>
            Your user ID or account confirmation token is invalid, or has
            expired.
          </Text>
          <Box mt={3}>
            You can{" "}
            <Link color="primary.500" href="/Account/Confirm/Resend">
              resend your confirmation link,
            </Link>
          </Box>
          <Box>
            or contact{" "}
            <Link
              color="primary.500"
              href={`mailto:${appSettings.SupportEmail}`}
            >
              {appSettings.SupportEmail}
            </Link>{" "}
            if you're having trouble.
          </Box>
        </AlertDescription>
      </Alert>
    );
  } else {
    content = (
      <Alert status="success" variant="left-accent" flexDirection="column">
        <Flex alignItems="center">
          <AlertIcon />
          <AlertTitle>Success!</AlertTitle>
        </Flex>
        <AlertDescription flexDirection="column" textAlign="center">
          <Text>
            Account Confirmation was successful and your registration is now
            complete.
          </Text>
          <Box mt={3}>
            <Link color="primary.500" href="/auth/login">
              Return home
            </Link>
          </Box>
        </AlertDescription>
      </Alert>
    );
  }

  return <Layout heading="Register">{content}</Layout>;
};

export default ConfirmAccount;
