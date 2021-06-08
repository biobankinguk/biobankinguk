const compose = require("compose-function");
const { withDokz } = require("dokz/dist/plugin");

const composed = compose(withDokz);

module.exports = composed({
  async redirects() {
    return [
      {
        // currently we redirect the root index to the directory guide as we have no homepage
        source: "/",
        destination: "/directory-guide",
        permanent: false,
      },
    ];
  },
  pageExtensions: ["js", "jsx", "md", "mdx", "ts", "tsx"],
  future: {
    webpack5: true,
  },
});
