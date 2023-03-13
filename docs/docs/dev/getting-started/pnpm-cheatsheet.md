# 💡 pnpm cheatsheet

Most pnpm commands can be done recursively against all workspaces with `-r`

You can target a specific workspace by being inside its workspace directory

- or you can target a workspace by relative directory path `-C <dir>`
- or you can filter workspaces to target using `--filter <filter-spec>`
  - See the docs for more complex filtering than just package name

## Dependency management

To install current dependencies for the whole repo: `pnpm i`

> ℹ
>
> pnpm symlinks `node_modules` inside workspaces.
>
> If you need to clean out `node_modules` you can't just do the root one, so use `pnpm dlx npkill` which will let you delete them all :)

To add a new dependency `pnpm add <package-name>` with `-D` if you want it to be a dev dependency

## Script running

Run scripts with `pnpm <script-name>`

> ℹ
>
> If the name of the script conflicts with a pnpm command, do `pnpm run <script-name>`
