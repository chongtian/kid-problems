import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
  providedIn: 'root'
})
export class BaseHttpService {

  constructor(private http: HttpClient) { }

  // download(url: string, httpParams?: HttpParams): Observable<Blob> {
  //   if (httpParams === null || httpParams === undefined) {
  //     httpParams = null;
  //   }

  //   const file = this.http.get<Blob>(this.getFullUrl(url),
  //     { params: httpParams, responseType: 'blob' as 'json' });
  //   return file;

  // }

  get(getUrl: string, httpParams?: HttpParams): Observable<any> {

    if (httpParams === null || httpParams === undefined) {
      httpParams = null;
    }

    return this.http.get<any>(this.getFullUrl(getUrl), { params: httpParams });
  }

  post(postUrl: string, body: any): Observable<any> {
    return this.http.post<any>(this.getFullUrl(postUrl), body, {});
  }

  put(putUrl: string, body: any): Observable<any> {
    return this.http.put<any>(this.getFullUrl(putUrl), body, {});
  }

  delete(deleteUrl: string): Observable<any> {
    return this.http.delete<any>(this.getFullUrl(deleteUrl), {});
  }

  private getFullUrl(url: string): string {
    return `${environment.apiBaseUrl}${url}`;
  }
  
}
