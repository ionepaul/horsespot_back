import { Http, Request, RequestOptionsArgs, Response, XHRBackend, RequestOptions, ConnectionBackend, Headers, URLSearchParams } from '@angular/http';
import { Router } from '@angular/router';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/delay';
import 'rxjs/add/observable/empty';

import { SpinnerService } from '../spinner/spinner.service';
import { StorageService } from '../auth/storage.service';

import { CONFIG } from '../../config';

export class HttpInterceptor extends Http {
 
    constructor(backend: ConnectionBackend, 
                defaultOptions: RequestOptions, 
                private _router: Router, 
                private _spinnerService: SpinnerService,
                private _storageService: StorageService) {
        super(backend, defaultOptions);
    }
 
    request(url: string | Request, options?: RequestOptionsArgs): Observable<Response> {
        return this.intercept(super.request(url, options));
    }
 
    get(url: string, options?: RequestOptionsArgs): Observable<Response> {
        this._spinnerService.isLoading = true;
        return this.intercept(super.get(url,this.getRequestOptionArgs(options)).map((res:Response) => { this._spinnerService.isLoading = false; return res; }));
    }
 
    post(url: string, body: string, options?: RequestOptionsArgs): Observable<Response> { 
        this._spinnerService.isLoading = true;
        return this.intercept(super.post(url, body, this.getRequestOptionArgs(options))).map((res:Response) => { this._spinnerService.isLoading = false; return res; });
    }
 
    put(url: string, body: string, options?: RequestOptionsArgs): Observable<Response> {
        this._spinnerService.isLoading = true;
        return this.intercept(super.put(url, body, this.getRequestOptionArgs(options))
        
        .map((res:Response) => { this._spinnerService.isLoading = false; return res; }));
    }
 
    delete(url: string, options?: RequestOptionsArgs): Observable<Response> {
        this._spinnerService.isLoading = true;
        return this.intercept(super.delete(url, this.getRequestOptionArgs(options)).map((res:Response) => { this._spinnerService.isLoading = false; return res; }));
    }

    getRequestOptionArgs(options?: RequestOptionsArgs) : RequestOptionsArgs {
        if (options == null) {
            options = new RequestOptions();
            options.headers = new Headers();

            var access_token = this._storageService.getItem('access_token');

            if (access_token != null) {
                options.headers.delete('Authorization');
                options.headers.append('Authorization', 'Bearer ' + access_token);
            }

            options.headers.append('Content-Type', 'application/json');
        }

        return options;
    }
 
    intercept(observable: Observable<Response>): Observable<Response> {
        return observable.catch((err, source) => {
            this._spinnerService.isLoading = false;
            if (err.status  === 403) {
                this._router.navigate(['/error/403']);
                return Observable.empty();
            } else if (err.status  === 404) {
                this._router.navigate(['/error/404']);
                return Observable.empty();
            } else if (err.status  === 500) {
                this._router.navigate(['/error/500']);
                return Observable.empty();
            } else {
                return Observable.throw(err);
            }
        });
    }
}