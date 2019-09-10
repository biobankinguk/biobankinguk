# biobankinguk

This repository aims to contain all code pertaining to the **UKCRC Tissue Directory and Co-ordination Centre**; everything that lives under the domain `biobankinguk.org` should be here if it is under source control.

In practice, there is ongoing work to migrate the live Tissue Directory and API codebase here.

This repository will contain a few applications, some shared code, and some other useful bits and pieces for development.

Distinct applications may have their own `README` files for guidance on getting started. There is a guide below.

# Guide to Repository Structure

- `/.azure/pipelines` - Azure Pipelines configurations for conditionally building the different applications and libraries in the repo
- `/src` - actual source code
    - `/Directory` - the new Directory app (sometimes called "Core", contains core functionality such as identity services)

# Contribute

Currently contributions are not accepted from outside of the University of Nottingham Digital Research Service team.

This may change in future.