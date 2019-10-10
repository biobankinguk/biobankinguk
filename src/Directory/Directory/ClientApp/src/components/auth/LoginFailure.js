import React from "react";

const LoginFailure = ({ message, error }) => {
  if (error) console.log("Login error", error);
  return <div>{message}</div>; // TODO: sexy
};

export default LoginFailure;
