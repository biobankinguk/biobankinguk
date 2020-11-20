# Notes

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

## Additional notes on pending RefData changes

### Diagnoses (SNOMED Terms)

These should be seeded based on an existing environment,
(especially since they are editable through the admin UI)
or selected freshly from the full list of terms, if desirable
for a new environment.

TODO: coming MIABIS and OMOP changes are likely to affect this
notably including the move to the separate Core RefData Service

### Material Types

These should be seeded based on an existing environment
or selected freshly.

For new environments, ID's need not be preserved.
When migrating environment data, ID's should be preserved, to maintain reign keys on the incoming data.

SortOrder should be alphabetical, and adjusted when new terms are added

TODO: Core RefData Service will take over responsibility of this data soon.

### Macroscopic Assessments

These should be seeded based on Core RefData Services Seed Data.

TODO: Core RefData Service will take over responsibility of this data soon.

### Preservation Types

These should be seeded based on Core RefData Services Seed Data.
Called "Storage Temperatures" in the new RefData.

TODO: Core RefData Service will take over responsibility of this data soon.

### Collection Percentages

These should be seeded based on an existing environment or example dataset r now.

TODO: Core RefData Service will take over responsibility of this data soon.
Upper and Lower Bounds may change when that happens (they are used by the readSheetConversionService)

### Age Ranges

These should be seeded based on Core RefData Services Seed Data.

TODO: Core RefData Service will take over responsibility of this data soon.

### Genders

These should be seeded based on an existing environment or example dataset r now
Called "Sex" in Core RefData Service.

TOOD: Core RefData Service will take over responsibility of this data soon.
However that will require data updates for existing environments
as Gender becomes Sex and the terms are slightly different...

### Donor Counts

These should be seeded based on Core RefData Services Seed Data.

TODO: Core RefData Service will take over responsibility of this data soon.

### Access Conditions

These should be seeded based on Core RefData Services Seed Data.

TODO: Core RefData Service will take over responsibility of this data soon.
### Collection Types

These should be seeded based on Core RefData Services Seed Data.

TODO: Core RefData Service will take over responsibility of this data soon.

### Collection Statuses

These should be seeded based on Core RefData Services Seed Data.

TODO: Core RefData Service will take over responsibility of this data soon.

### Collection Points

These should be seeded based on Core RefData Services Seed Data.

TODO: Core RefData Service will take over responsibility of this data soon.

### Consent Restrictions

These should be seeded based on Core RefData Services Seed Data.

TODO: Core RefData Service will take over responsibility of this data soon.

### Associated Data Types

These should be seeded based on Core RefData Services Seed Data.

TODO: Core RefData Service will take over responsibility of this data soon.
TODO: Sort Order? Rejig the editortemplate / viewmodels too

### Annual Statistics

These should be seeded based on Core RefData Services Seed Data.
First "Annual Statistic Group", then "Annual Statistic" with foreign keys to oups

TODO: Core RefData Service will take over responsibility of this data soon.

### SOP Statuses

These should be seeded based on Core RefData Services Seed Data.

TODO: Core RefData Service will take over responsibility of this data soon.

### Sample Collection Mode

These should be seeded based on an existing environment or example dataset r now.

TOOD: Core RefData Service should eventually take over responsibility of this ta
However it won't immediately as it is only concerned with Collection / Sample fData at this time.
This is Capability RefData, which won't move until the Upload API handles pability submission.

### Associated Data Procurement Timeframes

These should be seeded based on Core RefData Services Seed Data.

TODO: Core RefData Service will take over responsibility of this data soon.

### HTA Statuses

These should be seeded based on Core RefData Services Seed Data.

TODO: Core RefData Service will take over responsibility of this data soon.

### Organisation Service Offerings

These should be seeded based on Core RefData Services Seed Data.
Called "Service Offerings".

TODO: Core RefData Service will take over responsibility of this data soon.
TODO: There's a reasonable argument to be amde for this being Directory Data t shared RefData...

### Countries

These should be seeded based on Core RefData Services Seed Data.

TODO: Core RefData Service will take over responsibility of this data soon.
TODO: This could be a legitimately problematic approach to this in future...

### Counties

These should be seeded based on Core RefData Services Seed Data.

TODO: Core RefData Service will take over responsibility of this data soon.
TODO: This could be a legitimately problematic approach to this in future...

### Registration Reasons

These should be seeded based on an existing environment or example dataset r now.

It can be treated as Directory Data only for now,
since it won't be used by any API's or other non-Directory driven processes...

TOOD: Core RefData Service may eventually take over responsibility of this ta
as there's an argument to be made that it is RefData and should live with her RefData...
