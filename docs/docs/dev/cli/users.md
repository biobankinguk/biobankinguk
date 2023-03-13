# `users`

## `add`

Add a new user to the Database.

### Command

- `add <EMAIL> <FULL-NAME>` : Add a new User

### Options

- `-p` | `--password` `<PASSWORD>` : The password to set for the new user. If omitted, the password reset journey can be used to set a password.
- `-r` | `--roles` `<ROLES>` : Role names to add the new user to.

### Usage Scenarios

Add an Admin user with the roles `SuperUser` and `DirectoryAdmin`:

- `users add admin@example.com Admin -r SuperUser DirectoryAdmin -p Password1!`

---

## `roles`

Manage roles for a User; listing, adding and/or removing

### Command

- `roles <EMAIL>`

### Options

- `--remove` | `--remove-roles` `<ROLES>` : Role names to remove the user from.
- `--add` | `--add-roles` `<ROLES>` : Role names to add the user to.

### Usage Scenarios

Add the `DirectoryAdmin` role to the Admin user:

- `users roles admin@example.com --add DirectoryAdmin`

---

## `list-roles`

List the names of user roles in the system

### Command

- `list-roles` : List roles
