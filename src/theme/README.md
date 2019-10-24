Shared Theme for biobankinguk sites.

Currently only a javascript module is produced, intended for use with React.

# Getting started

A built version of the theme is kept in source control, to avoid adding a necessary local build step during development.

Therefore it's always acceptable to just import `dist/theme` from wherever.

If you change anything in `src/` you should rebuild (and commit the updated build if you're keeping the source changes).

## Building

### Prerequisites

- `npm` 5.x+
- `npm i` to install build dependencies

`npm run build` to build a new `dist/theme.json`.

> ℹ CI builds of dependent applications should always rebuild the theme from source.

# `src/` guide

The `src/` folder contains:

- `theme.js` - a theme entrypoint.
  - This must export a default object which will be serialized into the distributable theme file.
  - It should therefore be sensibly serializable: no circular references or functions please.
  - It should follow the [System UI Theme Specification](https://system-ui.com/theme/).
- `build/` - modules used to transform or otherwise produce actual theme data.
- `data/` - raw theme data e.g. dictionaries of colors, fonts, etc. to be used directly in the theme or by scripts in `builds/`.