const lightCodeTheme = require("prism-react-renderer/themes/github");
const darkCodeTheme = require("prism-react-renderer/themes/dracula");

// With JSDoc @type annotations, IDEs can provide config autocompletion
/** @type {import('@docusaurus/types').DocusaurusConfig} */
(
  module.exports = {
    title: "BiobankingUK",
    url: "https://docs.biobaninguk.org",
    baseUrl: "/",
    onBrokenLinks: "throw",
    onBrokenMarkdownLinks: "warn",
    favicon: "i/tdcc.png",
    organizationName: "biobankinguk", // Usually your GitHub org/user name.
    projectName: "biobankinguk", // Usually your repo name.

    presets: [
      [
        "@docusaurus/preset-classic",
        /** @type {import('@docusaurus/preset-classic').Options} */
        ({
          docs: {
            routeBasePath: "/",
            sidebarPath: require.resolve("./sidebars.js"),
            editUrl:
              "https://github.com/biobankinguk/biobankinguk/edit/main/docs/",
          },
          theme: {
            customCss: [require.resolve("./src/css/custom.css")],
          },
        }),
      ],
    ],

    themeConfig:
      /** @type {import('@docusaurus/preset-classic').ThemeConfig} */
      ({
        announcementBar: {
          content: "🚨🚧 This documentation site is a work in progress. 🚧🚨",
          backgroundColor: "#ffdb80",
          isCloseable: false,
        },
        navbar: {
          title: "BiobankingUK Docs",
          logo: {
            alt: "My Site Logo",
            src: "i/tdcc.png",
          },
          items: [
            {
              type: "doc",
              docId: "directory-guide/index",
              position: "left",
              label: "Directory",
            },
            {
              type: "doc",
              docId: "api-guide/index",
              position: "left",
              label: "API",
            },
            {
              type: "doc",
              docId: "installation-guide/index",
              position: "left",
              label: "Installation",
            },
            {
              type: "doc",
              docId: "dev-guide/index",
              position: "left",
              label: "Developers",
            },
            {
              href: "https://github.com/biobankinguk/biobankinguk",
              // label: "GitHub",
              position: "right",
              className: "header-github-link",
              "aria-label": "GitHub repository",
            },
          ],
        },
        footer: {
          style: "dark",
          copyright: `Copyright © ${new Date().getFullYear()} UKCRC Tissue Directory and Coordination Centre. Built with Docusaurus.`,
        },
        prism: {
          theme: lightCodeTheme,
          darkTheme: darkCodeTheme,
        },
      }),
  }
);
