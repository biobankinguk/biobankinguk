require("@babel/register");
const stringify = require("json-stringify");
const fs = require("fs");

const theme = require("./src/theme").default;

fs.writeFile("./dist/theme.json", stringify(theme, null, 2), e =>
  e ? console.error(e) : console.log("✔ Theme Built!")
);
