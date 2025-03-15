export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api',
  appName: 'Vanguard Platform - Dev',
  version: '0.1.0',
  debugEnabled: true,
  authConfig: {
    domain: 'dev-yourdomain.auth0.com',
    clientId: 'your-dev-client-id',
    redirectUri: 'http://localhost:4200/callback',
    audience: 'https://vanguard-api-dev/api'
  }
};
