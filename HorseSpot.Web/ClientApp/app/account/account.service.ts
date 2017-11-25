import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { CONFIG } from '../config';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';
import 'rxjs/add/observable/throw';

//SERVICES
import { HttpWrapper } from '../shared/http/http.wrapper';
import { AuthService } from '../shared/auth/auth.service';
//import { AppointmentsService } from './appointments/appointments.service';

//MODELS
import { LoginModel } from './models/login.model';
import { UserModel } from './models/user.model';
import { RegisterExternalModel } from './models/registerExternalModel';
import { RegisterModel } from './models/register.model';
import { ChangePasswordModel } from './models/changePassword.model';
import { GetHorseAdListResultsModel } from '../horse-advertisments/models/getHorseAdListResultsModel';
import { UserFullProfile } from './models/userFullProfile.model';

@Injectable()
export class AccountService {
  private _loginUrl: string = CONFIG.authUrl;
  private _registerUrl: string = CONFIG.baseUrls.apiUrl + 'account/register'
  private _profilePhotoUploadUrl: string = CONFIG.baseUrls.apiUrl + 'user/profilephoto/upload/';
  private _detailsUrl: string = CONFIG.baseUrls.apiUrl + 'account/details/';
  private _editProfileUrl: string = CONFIG.baseUrls.apiUrl + 'account/edit/';
  private _changePasswordUrl: string = CONFIG.baseUrls.apiUrl + 'account/changepassword/';
  private _getUserHorsePostsUrl: string = CONFIG.baseUrls.apiUrl + 'account/userhorseposts/';
  private _getUserFavoritesPostsUrl: string = CONFIG.baseUrls.apiUrl + 'account/userhorsefavorites/';
  private _getUserReferencePostsUrl: string = CONFIG.baseUrls.apiUrl + 'account/userreferences/';
  private _forgotPasswordUrl: string = CONFIG.baseUrls.apiUrl + 'account/forgotpassword';
  private _newsletterSubscribeUrl: string = CONFIG.baseUrls.apiUrl + 'account/newsletter';
  public _profilePhotoGetUrl: string = CONFIG.imagesUrl + '/Images/ProfilePhotos/';
  private _isAdminUrl: string = CONFIG.baseUrls.apiUrl + 'account/isAdmin/';
  private _getUserFullProfileUrl: string = CONFIG.baseUrls.apiUrl + 'account/fullProfile/';

  constructor(private _httpWrapper: HttpWrapper,
    private _http: Http,
    private _authService: AuthService,
    private _router: Router) { }
  //private _appointmentsService: AppointmentsService) { }

  loginService(model: LoginModel) {
    return this._authService.authenticateUser(model);
  }

  updateExternalUser(provider, externalToken, phoneNumber) {
    return this._authService.updateExternalUser(provider, externalToken, phoneNumber);
  }

  obtainLocalAccessToken(provider: string, externalToken: string) {
    return this._authService.obtainLocalAccessToken(provider, externalToken);
  }

  registerService(model: RegisterModel) {
    let body = JSON.stringify(model);

    return this._httpWrapper
      .post(this._registerUrl, body)
      .map((res: Response) => res.json())
      .catch(this.handleError);
  }

  uploadProfilePhoto(profilePhoto: File, userId: string) {
    let headers = new Headers();

    headers.set('Accept', 'application/json');
    let requestOptions = new RequestOptions({
      headers: headers
    });

    let formData: FormData = new FormData();
    formData.append('uploadFile', profilePhoto, profilePhoto.name);

    return this._http
      .post(`${this._profilePhotoUploadUrl + userId}`, formData, requestOptions)
      .map((res: Response) => res.json())
      .do(data => this._authService.storeItem('pic_name', data))
      .catch(this.handleError);
  }

  getUserDetails(userId: string): Observable<UserModel> {
    return this._httpWrapper
      .get(`${this._detailsUrl + userId}`)
      .map((res: Response) => res.json() as UserModel)
      .catch(this.handleError);
  }

  editUserDetails(userId: string, model: UserModel) {
    let body = JSON.stringify(model);

    return this._httpWrapper
      .post(`${this._editProfileUrl + userId}`, body)
      .map((res: Response) => res.json() as UserModel)
      .do(data => this._authService.storeItem('user_name', data.FirstName))
      .catch(error => {
        if (error && error.status === 401 && this._authService.isTokenExpired()) {
          return this._authService.refreshToken().mergeMap((data) => {
            this._authService.storeUserAccessInfo(data);
            return this._httpWrapper.post(`${this._editProfileUrl + userId}`, body)
              .map((res: Response) => res.json() as UserModel)
              .do(data => this._authService.storeItem('user_name', data.FirstName))
              .catch(this.handleError);
          })
        } else if (error && error.status === 401) {
          this._router.navigate(['/error/401']);
        }
        else {
          return this.handleError(error);
        }
      });
  }

  changePassword(userId: string, model: ChangePasswordModel) {
    let body = JSON.stringify(model);

    return this._httpWrapper
      .post(`${this._changePasswordUrl + userId}`, body)
      .map((res: Response) => res)
      .catch(error => {
        if (error && error.status === 401 && this._authService.isTokenExpired()) {
          return this._authService.refreshToken().mergeMap((data) => {
            this._authService.storeUserAccessInfo(data);
            return this._httpWrapper.post(`${this._changePasswordUrl + userId}`, body)
              .map((res: Response) => res)
              .catch(this.handleError);
          })
        } else if (error && error.status === 401) {
          this._router.navigate(['/error/401']);
        }
        else {
          return this.handleError(error);
        }
      });
  }

  getUserHorsePosts(pageNumber: number, userId: string): Observable<GetHorseAdListResultsModel> {
    return this._httpWrapper.get(this._getUserHorsePostsUrl + pageNumber + "?userId=" + userId)
      .map((res: Response) => res.json() as GetHorseAdListResultsModel)
      .catch(this.handleError);
  }

  getUserFavoritesPosts(pageNumber: number, userId: string): Observable<GetHorseAdListResultsModel> {
    return this._httpWrapper.get(this._getUserFavoritesPostsUrl + pageNumber + "?userId=" + userId)
      .map((res: Response) => res.json() as GetHorseAdListResultsModel)
      .catch(error => {
        if (error && error.status === 401 && this._authService.isTokenExpired()) {
          return this._authService.refreshToken().mergeMap((data) => {
            this._authService.storeUserAccessInfo(data);
            return this._httpWrapper.get(`${this._getUserFavoritesPostsUrl + pageNumber}?userId=${userId}`)
              .map((res: Response) => res.json() as GetHorseAdListResultsModel)
              .catch(this.handleError);
          })
        } else if (error && error.status === 401) {
          this._router.navigate(['/error/401']);
        }
        else {
          return this.handleError(error);
        }
      });
  }

  getUserHorseReferences(pageNumber: number, userId: string): Observable<GetHorseAdListResultsModel> {
    return this._httpWrapper.get(this._getUserReferencePostsUrl + pageNumber + "?userId=" + userId)
      .map((res: Response) => res.json() as GetHorseAdListResultsModel)
      .catch(this.handleError);
  }

  forgotPassword(email: string) {
    return this._httpWrapper.post(this._forgotPasswordUrl + "?email=" + email, "")
      .catch(this.handleError);
  }

  registerToNewsletter(email: string) {
    return this._httpWrapper.post(this._newsletterSubscribeUrl + "?email=" + email, "")
      .catch(this.handleError);
  }

  isLoggedIn(): boolean {
    return this._authService.getItem('access_token') != null;
  }

  getProfilePicutreName() {
    return this._authService.getItem('pic_name');
  }

  isAdmin(userId: string) {
    return this._httpWrapper.get(this._isAdminUrl + userId)
      .map(res => res.json())
      .catch(error => {
        if (error && error.status === 401 && this._authService.isTokenExpired()) {
          return this._authService.refreshToken().mergeMap((data) => {
            this._authService.storeUserAccessInfo(data);
            return this._httpWrapper.get(this._isAdminUrl + userId)
              .catch(this.handleError);
          })
        } else if (error && error.status === 401) {
          this._router.navigate(['/error/401']);
        }
        else {
          return this.handleError(error);
        }
      });
  }

  getName() {
    return this._authService.getItem('user_name');
  }

  getUserId() {
    return this._authService.getItem('id');
  }

  logout(): void {
    //this._appointmentsService.appointments.complete();
    this._authService.removeUserStoredInfo();
  }

  getUserFullProfile(userId: string) {
    return this._httpWrapper.get(this._getUserFullProfileUrl + userId)
      .map((res: Response) => res.json() as UserFullProfile)
      .catch(this.handleError);
  }

  private handleError(error: Response) {
    let errorMsg = error.json().Message || "Server Error";
    return Observable.throw(errorMsg);
  }
} 
