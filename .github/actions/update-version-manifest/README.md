# Update Version Manifest

This is a fairly generic action which reads a version manifest and updates it bsed on the release instructions within it.

The purpose is to make and record a semver change for use elsewhere in a workflow.

# Usage

### Inputs

- `manifest-path` - path to the manifest file; defaults to `./version.yml`

### Outputs

- `version` - the updated (if applicable) version in the manifest.
- `doRelease` - a boolean to indicate if a release is expected given the manifest's release type.
  - i.e. if `manifest.release` is not `none`

### Side effects

- the local working copy of the manifest is updated.
  - it should typically then be committed to git to prevent re-enacting the same version update in future workflow runs.

## The manifest

The manifest format is simple:

```yaml
version: "<a valid semver value for the current version>"
release: "<a release type to indicate how the manifest should be updated>"
```

`version` should rarely need amending manually; intended usage is for PRs to amend `release` with a valid type.

The action will then apply that operation to update the version within the manifest, and reset it.

### Release type values:
- `none` - don't do anything during release workflow
- `force` - no version update, but trigger a release attempt with the manifest's specified version
- `<semver release>` - update the manifest's version AND trigger a release attempt with the new version
  - anything valid for [semver's `inc()` function](https://github.com/npm/node-semver#functions)
    - a second value (space delimited from the first) is valid as a prerelease identifier for any `pre` values.
    - e.g. `preminor beta` applied to `1.0.0` would result in `1.1.0-beta.1`

# Development

The `action.yml` points to a single dist file, built with `@vercel/ncc`, committed to the repo.

So if you make changes to the source, don't forget you need to `npm run build` and commit the `dist/` directory for github to use the changes.