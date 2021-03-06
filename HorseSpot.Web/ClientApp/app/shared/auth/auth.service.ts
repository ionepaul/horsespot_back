import { Injectable } from '@angular/core';
import { Http, URLSearchParams, Headers, RequestOptions, Response } from '@angular/http';
import { Router } from '@angular/router';
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { CONFIG } from '../../config';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/do';

//SERVICES
import { HttpWrapper } from '../http/http.wrapper';

//MODELS
import { LoginModel } from '../../account/models/login.model';
import { RegisterExternalModel } from '../../account/models/registerExternalModel';
import { ExternalUserModel } from '../../account/models/external-user.model';

@Injectable()
export class AuthService {
  private _checkPostOwner = CONFIG.baseUrls.apiUrl + 'horses/ispostowner/';

  constructor(private _httpWrapper: HttpWrapper,
    private _router: Router,
    @Inject(PLATFORM_ID) private platformId: Object) { }

  authenticateUser(model: LoginModel) {
    let body = "grant_type=" + CONFIG.login_grant_type + "&username=" + model.Email + "&password=" + encodeURIComponent(model.Password) + "&client_id=" + CONFIG.client_id;

    let headers = new Headers();

    headers.set('Content-Type', 'application/x-www-form-urlencoded');
    let requestOptions = new RequestOptions({
      headers: headers
    });

    return this._httpWrapper
      .post(CONFIG.authUrl, body, requestOptions)
      .map((res: Response) => res.json())
      .do(data => this.storeUserAccessInfo(data))
      .catch(this._handleAuthError);
  }

  updateExternalUser(provider: string, externalToken: string, externalUserModel: ExternalUserModel, clientId: string) {
    return this._httpWrapper
      .post(CONFIG.baseUrls.apiUrl + "account/updateExternalUser?provider=" + provider + "&externalToken=" + externalToken + "&clientId=" + clientId, JSON.stringify(externalUserModel))
      .map((res: Response) => res.json())
      .do(data => this.storeUserAccessInfo(data))
      .catch(this._handleAuthError);
  }

  obtainLocalAccessToken(provider: string, externalToken: string, clientId: string) {
    return this._httpWrapper
      .get(CONFIG.baseUrls.apiUrl + "account/obtainLocalAccessToken?provider=" + provider + "&externalAccessToken=" + externalToken + "&clientId=" + clientId)
      .map((res: Response) => res.json())
      .do(data => this.storeUserAccessInfo(data))
      .catch(this._handleAuthError);
  }

  deleteExternalUser(provider: string, externalToken: string) {
    return this._httpWrapper
               .post(CONFIG.baseUrls.apiUrl + "account/delete?provider=" + provider + "&externalToken=" + externalToken, "")
               .catch(this._handleAuthError);
  }

  refreshToken() {
    let data = new URLSearchParams();
    data.append('grant_type', CONFIG.refresh_token_grant_type);
    data.append('refresh_token', this.getItem('refresh_token'));
    data.append('client_id', CONFIG.client_id);

    let body = data.toString();
    let headers = new Headers();

    headers.set('Content-Type', 'application/x-www-form-urlencoded');
    let requestOptions = new RequestOptions({
      headers: headers
    });

    return this._httpWrapper
      .post(CONFIG.authUrl, body, requestOptions)
      .map((res: Response) => res.json())
      .catch(this._handleAuthError);
  }

  isTokenExpired(): boolean {
    var tokenExpires = this.getItem('token_expires');
    if (tokenExpires != null) {
      var tokenExpirationDate = parseInt(tokenExpires);

      if (new Date().getTime() > tokenExpirationDate) {
        return true;
      }
    }

    return false;
  }

  isRefreshTokenExpired(): boolean {
    var refreshTokenExpires = this.getItem('refresh_token_expires');
 
    if (refreshTokenExpires != null) {
      var refreshTokenExpirationDate = parseInt(refreshTokenExpires);

      if (new Date().getTime() > refreshTokenExpirationDate) {
        return true;
      }
    }

    return false;
  }

  storeUserAccessInfo(data: any) {
    this.storeItem('id', data.userId);
    this.storeItem('access_token', data.access_token);
    this.storeItem('refresh_token', data.refresh_token);
    this.storeItem('token_expires', new Date(data[".expires"]).getTime().toString());
    this.storeItem('refresh_token_expires', (new Date(data[".expires"]).getTime() + (CONFIG.refresh_token_lifetime_min * 60 * 1000)).toString());
    this.storeItem('horsespot_userName', data.fullName);
    this.storeItem('pic_name', data.profilePic);
  }

  removeUserStoredInfo() {
    this._removeItem('id');
    this._removeItem('access_token');
    this._removeItem('refresh_token');
    this._removeItem('token_expires');
    this._removeItem('refresh_token_expires');
    this._removeItem('horsespot_userName');
    this._removeItem('pic_name');
  }

  checkPostOwner(adId: string) {
    return this._httpWrapper
      .get(this._checkPostOwner + adId)
      .map((res) => res.json())
      .catch(error => {
        if (error && error.status === 401 && this.isTokenExpired()) {
          return this.refreshToken().mergeMap((data) => {
            this.storeUserAccessInfo(data);
            return this._httpWrapper.get(this._checkPostOwner + adId)
              .catch(this._handleError);
          })
        } else if (error && error.status === 401) {
          this._router.navigate(['/error/401']);
        }
        else {
          return this._handleError(error);
        }
      });
  }

  public getItem(key: string) {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem(key);
    }

    return null;
  }

  public storeItem(key: string, value: string) {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem(key, value);
    }
  }

  private _removeItem(key: string) {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem(key);
    }
  }

  private _handleAuthError(error: Response) {
    let errorMsg = error.json().error_description || "Server Error";
    return Observable.throw(errorMsg);
  }

  private _handleError(error: Response) {
    let errorMsg = error.json().Message || "Server Error";
    return Observable.throw(errorMsg);
  }
}
