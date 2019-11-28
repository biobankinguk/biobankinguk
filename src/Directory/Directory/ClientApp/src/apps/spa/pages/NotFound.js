import React from "react";
import { setTitle } from "services/dom-service";

const NotFound = () => {
  setTitle("Error");
  return <div>404: Not Found</div>;
}; // TODO: sexy

export default NotFound;
