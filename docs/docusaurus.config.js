const lightCodeTheme = require("prism-react-renderer/themes/github");
const darkCodeTheme = require("prism-react-renderer/themes/dracula");

// With JSDoc @type annotations, IDEs can provide config autocompletion
/** @type {import('@docusaurus/types').DocusaurusConfig} */
(
  module.exports = {
    title: "BiobankingUK",
    url: "https://docs.biobankinguk.org",
    baseUrl: "/",
    trailingSlash: false,
    onBrokenLinks: "throw",
    onBrokenMarkdownLinks: "throw",
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
              label: "Guides:",
              href: "#",
              position: "right",
              "aria-label": "Navbar Group Label",
            },
            {
              type: "doc",
              docId: "directory/index",
              position: "right",
              label: "Directory",
            },
            {
              type: "doc",
              docId: "api/index",
              position: "right",
              label: "API",
            },
            {
              type: "doc",
              docId: "installation/index",
              position: "right",
              label: "Installation",
            },
            {
              type: "doc",
              docId: "dev/index",
              position: "right",
              label: "Developers",
            },
            {
              label: "|",
              href: "#",
              position: "right",
              "aria-label": "Navbar Group Separator",
            },
            {
              href: "https://github.com/biobankinguk/biobankinguk",
              position: "right",
              className: "header-github-link",
              "aria-label": "GitHub repository",
            },
          ],
        },
        footer: {
          style: "dark",
          copyright: `Copyright © ${new Date().getFullYear()} University of Nottingham. Built with Docusaurus.`,
        },
        prism: {
          theme: lightCodeTheme,
          darkTheme: darkCodeTheme,
        },
      }),
  }
);
