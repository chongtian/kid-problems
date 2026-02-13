import { HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';

export class BaseHttpServiceStub {

    constructor(stubValue = null) {
        if (stubValue) {
            this.stubValue = stubValue;
        } else {
            this.stubValue = {};
        }
    }

    navigateUrl: string;
    httpMethod: string;
    stubValue: any;

    download(url: string, httpParams?: HttpParams): Observable<Blob> {
        this.navigateUrl = url;
        this.httpMethod = 'GET';
        this.stubValue.Data = httpParams;
        return of(null);
    }

    get(getUrl: string, httpParams?: HttpParams): Observable<any> {
        this.navigateUrl = getUrl;
        this.httpMethod = 'GET';
        this.stubValue.Data = httpParams;
        return of(this.stubValue);
    }

    post(postUrl: string, body: any): Observable<any> {
        this.navigateUrl = postUrl;
        this.httpMethod = 'POST';
        this.stubValue.Data = body;
        return of(this.stubValue);
    }

    put(putUrl: string, body: any): Observable<any> {
        this.navigateUrl = putUrl;
        this.httpMethod = 'PUT';
        this.stubValue.Data = body;
        return of(this.stubValue);
    }

    delete(deleteUrl: string): Observable<any> {
        this.navigateUrl = deleteUrl;
        this.httpMethod = 'DELETE';
        this.stubValue.Data = null;
        return of(this.stubValue);
    }

}
