import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Location } from '../models/models';

@Injectable({ providedIn: 'root' })
export class LocationService {
  constructor(private http: HttpClient) {}

  /** Populates the Batch dropdown from Location_Details saved at login. */
  getLocations(): Observable<Location[]> {
    return this.http.get<Location[]>(`${environment.apiBaseUrl}/location`);
  }
}
