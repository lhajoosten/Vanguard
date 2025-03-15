export const environment = {
  production: false,
  apiUrl: 'https://vanguard-api-staging.herokuapp.com/api',
  appName: 'Vanguard Platform - Staging',
  version: '0.1.0',
  debugEnabled: true,
  authConfig: {
    domain: 'dev-yourdomain.auth0.com',
    clientId: 'your-staging-client-id',
    redirectUri: 'https://lhajoosten.github.io/Vanguard/staging/callback',
    audience: 'https://vanguard-api-staging/api'
  }
};
