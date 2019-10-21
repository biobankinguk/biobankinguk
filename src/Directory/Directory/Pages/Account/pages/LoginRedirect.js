import React, { useEffect } from "react";
import { Normalize } from "styled-normalize";

const LoginRedirect = () => {
  useEffect(() => {
    window.location.href = document.getElementById("meta-refresh").dataset.url;
  });

  return (
    <>
      <Normalize />
      <h1>You are now being returned to the application.</h1>
      <p>Once complete, you may close this tab</p>
    </>
  );
};

export default LoginRedirect;
