// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  applicationName: 'KidProblem',
  applicationVersion: '0.5D',
  apiBaseUrl: 'https://qajvrb7w7waa5icxlfm6tqustm0wspyy.lambda-url.us-east-2.on.aws/api',
  // apiBaseUrl: 'http://localhost:64800/api'
  // apiBaseUrl: 'https://localhost:5001/api',
  cognito: {
    userPoolId: 'us-east-2_pKH1HdXM9',
    userPoolWebClientId: '6bgffqhdrg8a8d6mrqtc2e4plr',
  }
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/plugins/zone-error';  // Included with Angular CLI.
