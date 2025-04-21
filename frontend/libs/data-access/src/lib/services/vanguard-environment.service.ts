import { Injectable } from '@angular/core';

export interface Environment {
  production: boolean;
  apiUrl: string;
}

@Injectable({
  providedIn: 'root'
})
export class EnvironmentService {
  private _environment: Environment | null = null;

  setEnvironment(environment: Environment) {
    this._environment = environment;
  }

  get apiUrl(): string {
    if (!this._environment) {
      throw new Error('Environment not set. Call setEnvironment first.');
    }
    return this._environment.apiUrl;
  }

  get isProduction(): boolean {
    if (!this._environment) {
      throw new Error('Environment not set. Call setEnvironment first.');
    }
    return this._environment.production;
  }
}
