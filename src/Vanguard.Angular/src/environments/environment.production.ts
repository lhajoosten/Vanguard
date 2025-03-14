export const environment = {
  production: true,
  apiUrl: 'https://vanguard-api-prod.herokuapp.com/api',
  appName: 'Vanguard Platform',
  version: '0.1.0',
  debugEnabled: false,
  authConfig: {
    domain: 'vanguard.auth0.com',
    clientId: 'your-prod-client-id',
    redirectUri: 'https://lhajoosten.github.io/Vanguard/production/callback',
    audience: 'https://vanguard-api-prod/api'
  }
};
