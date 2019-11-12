import React from "react";
import { Alert, AlertIcon, AlertDescription, Link } from "@chakra-ui/core";

/**
 * Poses a question, and a link to try in response, in an alert
 *
 * e.g. "Feeling hungry? <Try Eating>" where <> links somewhere
 */
const TryThisAlert = ({ text, linkText, href = "#", ...p }) => (
  <Alert status="info" variant="left-accent" {...p}>
    <AlertIcon />
    <AlertDescription>
      {text && linkText ? <>{text} </> : text}
      {linkText ? (
        <Link color="primary.500" href={href}>
          {linkText}
        </Link>
      ) : null}
    </AlertDescription>
  </Alert>
);

export default TryThisAlert;
