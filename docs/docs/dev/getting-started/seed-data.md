---
sidebar_position: 3
---
# Seed Data

- This represent the tables you'd want to seed in a new installation
- All other tables are dynamic system or user data
- These sample records are based on Production at the time of writing
- Feel free to substitute your own data; these are samples
- In particular, curation of `MaterialTypes` and `Diagnosis` records is likely
- IDs typically shouldn't be imported, unless you feel the need to preserve them
- There are a few dependent entities (with FK constraints). You'll need to `INSERT` the dependencies first and ensure the FK IDs line up.
  - `Counties` depend on `Countries`.
  - `AnnualStatistics` depend on `AnnualStatisticGroups`
- Some tables may be redundant (e.g. `HtaStatus`) but should be populated as expected until such time as they are properly removed from the codebase.
- `Funders` weren't seeded by the old Directory seed process, but it makes sense to want to prepopulate a starting point for them in a new environment

See [`ref-data seed` documentation](/dev/cli/ref-data#seed).

## Additional notes on pending RefData changes

### Diagnoses (SNOMED Terms)

These should be seeded based on an existing environment, (especially since they are editable through the admin UI) or selected freshly from the full list of terms, if desirable for a new environment.

TODO: coming MIABIS and OMOP changes are likely to affect this notably including the move to the separate Core RefData Service

### Material Types

These should be seeded based on an existing environment or selected freshly.

For new environments, ID's need not be preserved.
When migrating environment data, ID's should be preserved, to maintain reign keys on the incoming data.

SortOrder should be alphabetical, and adjusted when new terms are added

### Macroscopic Assessments

These should be seeded based on Core RefData Services Seed Data.

### Preservation Types

These should be seeded based on Core RefData Services Seed Data.
Called "Storage Temperatures" in the new RefData.

### Collection Percentages

These should be seeded based on an existing environment or example dataset for now.

### Age Ranges

These should be seeded based on Core RefData Services Seed Data.

### Genders

These should be seeded based on an existing environment or example dataset for now
Called "Sex" in Core RefData Service.

### Donor Counts

These should be seeded based on Core RefData Services Seed Data.

### Access Conditions

These should be seeded based on Core RefData Services Seed Data.

### Collection Types

These should be seeded based on Core RefData Services Seed Data.

### Collection Statuses

These should be seeded based on Core RefData Services Seed Data.

### Collection Points

These should be seeded based on Core RefData Services Seed Data.

### Consent Restrictions

These should be seeded based on Core RefData Services Seed Data.

### Associated Data Types

These should be seeded based on Core RefData Services Seed Data.

### Annual Statistics

These should be seeded based on Core RefData Services Seed Data.
First "Annual Statistic Group", then "Annual Statistic" with foreign keys to oups

### SOP Statuses

These should be seeded based on Core RefData Services Seed Data.

### Sample Collection Mode

These should be seeded based on an existing environment or example dataset for now.

### Associated Data Procurement Timeframes

These should be seeded based on Core RefData Services Seed Data.

### HTA Statuses

These should be seeded based on Core RefData Services Seed Data.

### Organisation Service Offerings

These should be seeded based on Core RefData Services Seed Data.
Called "Service Offerings".

### Countries

These should be seeded based on Core RefData Services Seed Data.

### Counties

These should be seeded based on Core RefData Services Seed Data.

### Registration Reasons

These should be seeded based on an existing environment or example dataset for now.
