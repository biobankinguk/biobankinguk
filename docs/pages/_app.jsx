import { DokzProvider, GithubLink, ColorModeSwitch } from "dokz";
import Head from "next/head";
import { ChakraProvider, Text } from "@chakra-ui/react";
import { Link } from "dokz";
import RouterLink from "next/link";
import { useRouter } from "next/router";
import { apiSidebarOrdering } from "./api-guide/sidebar-order";

const routes = {
  DIRECTORY: { path: "/directory-guide" },
  API: { path: "/api-guide", sidebarOrdering: apiSidebarOrdering },
  INSTALLATION: { path: "/installation-guide" },
  DEV: { path: "/dev-guide" },
};

export default function App(props) {
  const { Component, pageProps } = props;
  const { pathname } = useRouter();

  let routeProps = {};
  for (const route of Object.values(routes)) {
    const { path, ...props } = route;
    if (pathname.startsWith(path)) {
      routeProps = { docsRootPath: `pages${path}`, ...props };
      break;
    }
  }

  return (
    <ChakraProvider resetCSS>
      <Head>
        <link
          href="https://fonts.googleapis.com/css?family=Fira+Code"
          rel="stylesheet"
          key="google-font-Fira"
        />
      </Head>
      {routeProps.docsRootPath ? (
        <DokzProvider
          {...routeProps}
          headTitlePrefix="BiobankingUK Docs - "
          headerLogo={
            <RouterLink href="/">
              <Link>BiobankingUK Docs</Link>
            </RouterLink>
          }
          headerItems={[
            <Text>Guides: </Text>,
            <RouterLink href={routes.DIRECTORY.path}>
              <Link>Directory</Link>
            </RouterLink>,
            <RouterLink href={routes.API.path}>
              <Link>API</Link>
            </RouterLink>,
            <RouterLink href={routes.INSTALLATION.path}>
              <Link>Installation</Link>
            </RouterLink>,
            <RouterLink href={routes.DEV.path}>
              <Link>Developers</Link>
            </RouterLink>,
            <GithubLink
              key="0"
              url="https://github.com/biobankinguk/biobankinguk"
            />,
            <ColorModeSwitch key="1" />,
          ]}
        >
          <Component {...pageProps} />
        </DokzProvider>
      ) : (
        <Component {...pageProps} />
      )}
    </ChakraProvider>
  );
}
