import { DokzProvider, GithubLink, ColorModeSwitch } from "dokz";
import Head from "next/head";
import { ChakraProvider, Text } from "@chakra-ui/react";
import { Link } from "dokz";
import RouterLink from "next/link";
import { useRouter } from "next/router";

const routes = {
  DIRECTORY: "/directory-guide",
  API: "/api-guide",
  INSTALLATION: "/installation-guide",
  DEV: "/dev-guide",
};

export default function App(props) {
  const { Component, pageProps } = props;
  const { pathname } = useRouter();

  let docsRoot;
  for (const route of Object.values(routes)) {
    if (pathname.startsWith(route)) {
      docsRoot = `pages${route}`;
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
      {docsRoot ? (
        <DokzProvider
          docsRootPath={docsRoot}
          headTitlePrefix="BiobankingUK Docs - "
          headerLogo={
            <RouterLink href="/">
              <Link>BiobankingUK Docs</Link>
            </RouterLink>
          }
          headerItems={[
            <Text>Guides: </Text>,
            <RouterLink href={routes.DIRECTORY}>
              <Link>Directory</Link>
            </RouterLink>,
            <RouterLink href={routes.API}>
              <Link>API</Link>
            </RouterLink>,
            <RouterLink href={routes.INSTALLATION}>
              <Link>Installation</Link>
            </RouterLink>,
            <RouterLink href={routes.DEV}>
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
