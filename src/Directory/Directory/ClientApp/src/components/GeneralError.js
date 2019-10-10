import React from "react";

const GeneralError = ({ message, error }) => {
  if (error) console.log(error);
  return <div>{message}</div>; // TODO: sexy
};

export default GeneralError;
