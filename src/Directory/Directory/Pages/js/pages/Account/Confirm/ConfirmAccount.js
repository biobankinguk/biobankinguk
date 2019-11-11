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
import Layout from "../../../components/Layout";
import ResendConfirmationAlert from "../../../components/ResendConfirmationAlert";
import { hasErrors } from "../../../components/ModelValidationSummary";

const ConfirmAccount = ({ ModelState, Username }) => {
  let content;
  if (hasErrors(ModelState)) {
    content = (
      <>
        <Alert status="error" variant="left-accent" flexDirection="column">
          <Flex alignItems="center">
            <AlertIcon />
            <AlertTitle>
              There seems to be a problem with this confirmation link.
            </AlertTitle>
          </Flex>
          <AlertDescription>
            Your user ID or account confirmation token is invalid, or has
            expired.
          </AlertDescription>
        </Alert>

        <ResendConfirmationAlert username={Username} />
      </>
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
