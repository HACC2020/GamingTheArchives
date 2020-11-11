type EnvironmentSettings =
  { production: boolean, apiUrl: string, apiCredentialMode: RequestCredentials } & any;

export const environment: EnvironmentSettings = {
  production: true,
  apiUrl: 'https://www.hiscribe.app/api',
  apiCredentialMode: 'same-origin',
  useHash: true
};
