import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PurchaseBillItem } from '../models/models';

@Injectable({ providedIn: 'root' })
export class PurchaseBillService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<PurchaseBillItem[]> {
    return this.http.get<PurchaseBillItem[]>(`${environment.apiBaseUrl}/purchasebill`);
  }

  add(item: PurchaseBillItem): Observable<PurchaseBillItem> {
    return this.http.post<PurchaseBillItem>(`${environment.apiBaseUrl}/purchasebill`, item);
  }
}
