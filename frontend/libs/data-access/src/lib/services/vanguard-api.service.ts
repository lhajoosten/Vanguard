import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  // We'll inject the apiUrl from the component that uses this service
  constructor(private http: HttpClient) { }

  // Base method to set API URL from the environment
  configureBaseUrl(baseUrl: string) {
    this._baseUrl = baseUrl;
  }

  private _baseUrl = '';

  getItems() {
    return this.http.get(`${this._baseUrl}/api/items`);
  }
}
