import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TagResponse } from '../models/tag.model';

@Injectable({ providedIn: 'root' })
export class TagService {
  private readonly baseUrl = '/api/tag';

  constructor(private http: HttpClient) {}

  getTags(
    page = 1,
    size = 20,
    sortBy: 'name' | 'count' | 'share' = 'name',
    order: 'asc' | 'desc' = 'asc'
  ): Observable<TagResponse> {
    const params = new HttpParams()
      .set('page', page)
      .set('size', size)
      .set('sortBy', sortBy)
      .set('order', order);
    return this.http.get<TagResponse>(this.baseUrl, { params });
  }

  refreshTags(minTags = 1000): Observable<any> {
    const params = new HttpParams().set('minTags', minTags);
    return this.http.post(`${this.baseUrl}/refresh`, {}, { params });
  }
}
