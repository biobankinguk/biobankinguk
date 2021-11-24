import React from "react";
import Layout from "@theme/Layout";
import DocsLink from "@docusaurus/Link";
import {
  Heading,
  Stack,
  Text,
  SimpleGrid,
  Link,
  UnorderedList,
  ListItem,
  useColorModeValue,
} from "@chakra-ui/react";

const HeroBanner = () => {
  const responsiveChildWidths = { sm: "100%", md: "70%" };
  return (
    <Stack
      bgGradient="linear(to-br, yellow.700, orange.900)"
      py={10}
      alignItems="center"
      color="white"
      textShadow="2px 2px 2px rgba(0,0,0,.8)"
    >
      <Heading size="2xl" p={5} width={responsiveChildWidths}>
        <Text as="span" color="cyan.300">
          Catalogue
        </Text>
        <Text as="span"> &amp; </Text>
        <Text as="span" color="yellow.200">
          Search
        </Text>
        <Text> Tissue Samples.</Text>
      </Heading>

      <Heading
        as="h2"
        size="md"
        fontWeight="light"
        width={responsiveChildWidths}
        p={5}
      >
        The BiobankingUK Directory software allows organisations to easily
        provide details on samples they can make available for research, safely
        and anonymously.
      </Heading>
    </Stack>
  );
};

const Index = () => {
  return (
    <Layout>
      <Stack>
        <HeroBanner />

        <Stack p={5}>
          <Heading fontWeight="medium">📚 Documentation</Heading>
          <SimpleGrid p={8} columns={{ sm: 1, md: 2 }} spacing={8}>
            <DirectoryLinkCard />
            <ApiLinkCard />
            <InstallationLinkCard />
            <DeveloperLinkCard />
          </SimpleGrid>
        </Stack>
      </Stack>
    </Layout>
  );
};

const DirectoryLinkCard = () => (
  <LinkCard href="/directory" heading="🌐 Directory Guide">
    <Text>Documentation for using the Directory website.</Text>
    <UnorderedList pl={8}>
      <ListItem>Searching for samples or organisations</ListItem>
      <ListItem>
        Managing Organisation Data (Biobank or Network Administrators)
      </ListItem>
      <ListItem>
        Managing the Directory itself (Directory Administrators)
      </ListItem>
    </UnorderedList>
  </LinkCard>
);

const ApiLinkCard = () => (
  <LinkCard href="/api" heading="🖥 API Guide">
    <Text>
      Documentation for accessing and consuming the Directory Web API.
    </Text>
    <UnorderedList pl={8}>
      <ListItem>Authentication</ListItem>
      <ListItem>Bulk Data Submission</ListItem>
    </UnorderedList>
  </LinkCard>
);

const InstallationLinkCard = () => (
  <LinkCard href="/installation" heading="🛠 Installation Guide">
    <Text>
      Documentation for installing and maintaining an instance of the software
      stack.
    </Text>
    <UnorderedList pl={8}>
      <ListItem>Overview of the stack and its requirements</ListItem>
      <ListItem>Installing parts of the stack</ListItem>
      <ListItem>Configuring parts of the stack</ListItem>
    </UnorderedList>
  </LinkCard>
);

const DeveloperLinkCard = () => (
  <LinkCard href="/dev" heading="👩‍💻 Developer Guide">
    <Text>Documentation for Developers working with the source code.</Text>
    <UnorderedList pl={8}>
      <ListItem>Getting started with the code</ListItem>
      <ListItem>Running parts of the stack locally</ListItem>
      <ListItem>Modifying the codebase and contributing to it</ListItem>
    </UnorderedList>
  </LinkCard>
);

const LinkCard = ({ heading, children, href }) => {
  const bgColors = {
    light: {
      normal: "gray.200",
      hover: "gray.100",
    },
    dark: { normal: "gray.700", hover: "gray.600" },
  };
  const bg = useColorModeValue(bgColors.light, bgColors.dark);
  return (
    <Link
      as={DocsLink}
      to={href}
      _hover={{ textDecoration: "inherit", color: "inherit" }}
    >
      <Stack
        h="100%"
        bg={bg.normal}
        p={4}
        borderRadius={5}
        position="relative"
        transition="top ease .1s"
        top={0}
        _hover={{ boxShadow: "dark-lg", top: "-3px", bg: bg.hover }}
      >
        <Heading as="h3" size="md" fontWeight="medium" py={2}>
          {heading}
        </Heading>
        {children}
      </Stack>
    </Link>
  );
};

export default Index;
