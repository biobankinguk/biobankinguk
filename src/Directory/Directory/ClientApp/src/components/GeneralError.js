import React from "react";

const GeneralError = ({ message, error }) => {
  if (error) console.error(error);
  return <div>{message}</div>; // TODO: sexy
};

export default GeneralError;
