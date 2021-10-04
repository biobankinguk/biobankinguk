import React from "react";

export const ImageBox = (props: { src: string }) =>
  props.src ? (
    <div style={{ padding: "1em" }}>
      <img
        alt="Unknown"
        {...props}
        style={{
          boxShadow: "0 0 30px rgba(.2,.2,.2,.6)",
        }}
      />
    </div>
  ) : null;
