This project was bootstrapped with [Create React App](https://github.com/facebook/create-react-app) (CRA).

It provides multiple route entrypoints to the React App within:

- The main SPA, rendered from `public/index.html`
- Individual multi-page apps rendered by Razor Pages in the parent ASP.NET Core app

## Available Scripts

In the project directory, you can run:

### `npm start`

As per CRA, this will start the SPA on `localhost:3000`, but this is basically useless without the ASP.NET Core backend.

In practice, the ASP.NET Core app will run this for you in `Development` environments to boot this React App.

Supports hot reloading on file updates.

### `npm test`

Launches the test runner in the interactive watch mode.<br />
See the section about [running tests](https://facebook.github.io/create-react-app/docs/running-tests) for more information.

### `npm run build`

Builds the app for production to the `build` folder.<br />
It correctly bundles React in production mode and optimizes the build for the best performance.

The build is minified and the filenames include the hashes.<br />
Your app is ready to be deployed!

The ASP.NET Core app **does not** run this command. It **must** be run as part of the build process **before** `dotnet publish`.

This will ensure the production build files are present, at which point `dotnet publish` will include them in the production build of the ASP.NET Core app.

This process should occur in CI as per `.azure/pipelines/directory.yml`.

See the section about [deployment](https://facebook.github.io/create-react-app/docs/deployment) for more information.

### `npm run eject`

**Note: this is a one-way operation. Once you `eject`, you can’t go back!**

If you aren’t satisfied with the build tool and configuration choices, you can `eject` at any time. This command will remove the single build dependency from your project.

Instead, it will copy all the configuration files and the transitive dependencies (Webpack, Babel, ESLint, etc) right into your project so you have full control over them. All of the commands except `eject` will still work, but they will point to the copied scripts so you can tweak them. At this point you’re on your own.

You don’t have to ever use `eject`. The curated feature set is suitable for small and middle deployments, and you shouldn’t feel obligated to use this feature. However we understand that this tool wouldn’t be useful if you couldn’t customize it when you are ready for it.

## Learn More

You can learn more in the [Create React App documentation](https://facebook.github.io/create-react-app/docs/getting-started).

To learn React, check out the [React documentation](https://reactjs.org/).

### Code Splitting

This app makes extensive use of code splitting around routes, particularly those rendered by Razor Pages.

This ensures lean-ness of dependent JS loads on a given page.

This section has moved here: https://facebook.github.io/create-react-app/docs/code-splitting

### Analyzing the Bundle Size

This section has moved here: https://facebook.github.io/create-react-app/docs/analyzing-the-bundle-size

### Making a Progressive Web App

This section has moved here: https://facebook.github.io/create-react-app/docs/making-a-progressive-web-app

### Advanced Configuration

This section has moved here: https://facebook.github.io/create-react-app/docs/advanced-configuration

### Deployment

This section has moved here: https://facebook.github.io/create-react-app/docs/deployment

### `npm run build` fails to minify

This section has moved here: https://facebook.github.io/create-react-app/docs/troubleshooting#npm-run-build-fails-to-minify
