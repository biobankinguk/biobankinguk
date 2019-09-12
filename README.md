# biobankinguk

This repository aims to contain all code pertaining to the **UKCRC Tissue Directory and Co-ordination Centre**; everything that lives under the domain `biobankinguk.org` should be here if it is under source control.

In practice, there is ongoing work to migrate the live Tissue Directory and API codebase here.

This repository will contain a few applications, some shared code, and some other useful bits and pieces for development.

Distinct applications may have their own `README` files for guidance on getting started. There is a guide below.

# Guide to Repository Structure

The `src/` folder contains roughly project or app categorised folders, which also have Visual Studio Solutions in. Each Solution contains all the relevant projects including dev dependencies, so it's generally a good idea to use the solution of the project or app area you are doing work on.

## Directory structure:

- `.azure/pipelines/` - Azure Pipelines configurations for conditionally building the different applications and libraries in the repo
- `src/` - actual source code
    - `Common/` - class libraries containing code shared between some or all of the other apps
    - `Directory/` - the new Directory app (sometimes called "Core"; contains core functionality such as identity services)
    - `Upload/` - the new Upload API (the Submission API and its process dependencies such as WebJobs)

# Contribute

Currently contributions are not accepted from outside of the University of Nottingham Digital Research Service team.

This may change in future.