import React from "react";
import { Normalize } from "styled-normalize";

const Layout = children => (
  <>
    <Normalize />
    {children}
  </>
);

export default Layout;
