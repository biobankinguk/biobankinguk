import React, { cloneElement } from "react";
import ConditionalContent from "./ConditionalContent";

const ConditionalPage = ({ layout, ...p }) =>
  cloneElement(layout, null, <ConditionalContent {...p} />);

export default ConditionalPage;
