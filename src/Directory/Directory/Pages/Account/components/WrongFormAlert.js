import React from "react";
import { Alert, AlertIcon, AlertDescription, Link } from "@chakra-ui/core";

const WrongFormAlert = ({ text, linkText, href = "#" }) => (
  <Alert status="info" variant="left-accent" my={2}>
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

export default WrongFormAlert;
