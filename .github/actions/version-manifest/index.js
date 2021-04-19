const core = require("@actions/core");
const fs = require("fs");
const yaml = require("js-yaml");
const semver = require("semver");

/** path to the manifest YAML file */
const manifestPath = core.getInput("manifest-path");
const shouldUpdate = core.getInput("update");

/** fixed (non semver) release types */
const releaseTypes = {
  NONE: "none",
  FORCE: "force",
};

/**
 * bump the manifest by reference based on release type
 * @param {*} manifest The manifest object
 */
const manifestBump = (manifest) => {
  const [release, label] = manifest.release.split(" ");
  manifest.version = semver.inc(manifest.version, release, label);
};

/**
 * reset the working manifest and save changes to disk
 * @param {*} manifest The manifest object
 */
const persistChanges = (manifest) => {
  // reset the manifest release type
  manifest.release = releaseTypes.NONE;
  fs.writeFileSync(manifestPath, yaml.dump(manifest));
};

try {
  // try parse version.yml
  const manifest = yaml.load(fs.readFileSync(manifestPath));
  console.log(`Manifest:`, manifest);

  // check version is valid semver
  if (
    !semver.valid(manifest.version, {
      includePrerelease: true,
    })
  )
    throw Error("Manifest Version is not a valid SemVer");

  // record whether the manifest triggers a release
  const doRelease = manifest.release !== releaseTypes.NONE;
  core.setOutput("doRelease", doRelease);
  console.log("doRelease:", doRelease);

  // version bump manifest accordingly for release type
  switch (manifest.release) {
    case releaseTypes.NONE:
    case releaseTypes.FORCE:
      break;
    default:
      manifestBump(manifest);
  }

  // save the updated manifest, if any release
  if (doRelease && shouldUpdate) {
    persistChanges(manifest);
  }

  // record the updated version
  core.setOutput("version", manifest.version);
  console.log("version:", manifest.version);

  // and if it's a prerelease
  const isPrerelease = !!semver.prerelease(manifest.version);
  core.setOutput("isPrerelease", isPrerelease);
  console.log("isPrerelease:", isPrerelease);
} catch (e) {
  core.setFailed(e.message);
}
