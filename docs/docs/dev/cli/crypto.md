# `crypto`

## `generate-id`

Generate a Secure Random Unique ID.
This can be used to generate cryptographically strong 256bit Random Unique IDs. This tool uses it to create Client IDs and Secrets.

### Command

- `crypto generate-id`

### Options

- `-s` | `--hash` | `--sha` : Also output a SHA256 hash of the ID in Base64Url format.

### Usage Scenarios

Generate an ID for any purpose, including secure secrets.

- `crypto generate-id`

The generated ID (encoded as a Base64Url string) will be output.

---

## `hash`

Hash a string. This can be used to hash a string in the same consistent way that everything in the codebase that manually hashes does. It uses SHA256.

### Command

- `crypto hash <INPUT>`
- use `--help` to see all the options and arguments

### Usage Scenarios

Output the SHA256 hash of a string value, as well the original input value.

- `crypto hash <INPUT>`

The original input string and the generated hash (encoded as a Base64Url string) will be output.
