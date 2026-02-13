import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Auth } from 'aws-amplify';
import { MessageService } from '@app/_services';


/**
 * This is used to logout the user, when the server responds with an unathorized status code.
 * Especially when the session token expires.
 * @export
 * @class ErrorInterceptor
 * @implements {HttpInterceptor}
 */
@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

    constructor(private messageService: MessageService) { }


    /**
     * Intercepter intercepts the responses, and then process based on the recieved status code
     * Read more : http://jasonwatmore.com/post/2018/05/23/angular-6-jwt-authentication-example-tutorial
     * @param {HttpRequest<any>} request
     * @param {HttpHandler} next
     * @returns {Observable<HttpEvent<any>>}
     * @memberof ErrorInterceptor
     */
    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        return next.handle(request).pipe(catchError(err => {

            switch (err.status) {
                case 0:
                case 500:
                    this.messageService.add(`Internal Server Error.`);
                    break;
                case 400:
                    this.messageService.add(`The client made an invalid call.`);
                    break;
                case 401:
                    // auto logout if 401 response returned from api
                    Auth.signOut({ global: true })
                        .then(data => console.log(data))
                        .catch(err => console.log(err));
                    this.messageService.add(`Unauthorized service call. The user has been auto logout.`);
                    // this.messageService.add(`Unauthorized service call.`);
                    break;
                case 404:
                    // this.messageService.add(`Not found: 404.`);
                    console.log(`Not found: 404.`);
                    break;
                default:
                    this.messageService.add(`Status Code = ${err.status}, Message = ${err.message}`);
                    break;
            }

            // err.error is not null, if the ResponsenEntity contains an Exception
            // err.error.message will give the custom message send from the server
            // const error = err.error.message || err.statusText;
            // return throwError(() => error);
            return of(null);
        }))
    }
}