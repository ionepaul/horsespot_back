import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';
import 'rxjs/add/observable/throw';
import { HttpWrapper } from '../shared/http/http.wrapper';
import { PriceRangeModel } from './models/priceRangeModel';
import { CountryModel } from './models/countryModel';
import { GenderModel } from './models/genderModel';
import { HorseAbilityModel } from './models/horseAbilityModel';
import { RecommendedRiderModel } from './models/recommendedRiderModel';
import { HorseAdModel } from './models/horseAdModel';
import { GetHorseAdListResultsModel } from './models/getHorseAdListResultsModel';
import { EmailModel } from './models/emailModel';
import { PedigreeModel } from './models/pedigree';
import { SearchModel } from './models/searchModel';
import { AuthService } from '../shared/auth/auth.service';
import { LatestHorsesModel } from './models/latestHorses.model';
import { TransferHttp } from '../../modules/transfer-http/transfer-http';
import { CONFIG } from '../config';

@Injectable()
export class HorseAdsService {
  private _priceRangesUrl = CONFIG.baseUrls.apiUrl + 'priceranges';
  private _horseAbilitiesUrl = CONFIG.baseUrls.apiUrl + 'abilities';
  private _recommendedRidersUrl = CONFIG.baseUrls.apiUrl + 'recommendedriders';
  private _countriesUrl = CONFIG.baseUrls.apiUrl + 'countries';
  private _gendersUrl = CONFIG.baseUrls.apiUrl + 'genders';
  private _postHorseAdUrl = CONFIG.baseUrls.apiUrl + 'horses/post';
  private _uploadHorseAdPhotosUrl = CONFIG.baseUrls.apiUrl + 'horses/images/upload';
  private _deleteHorseAdPhotoUrl = CONFIG.baseUrls.apiUrl + 'horses/images/delete/';
  private _getUnvalidatedHorsePosts = CONFIG.baseUrls.apiUrl + 'horses/unvalidated/';
  private _getHorseAdDetails = CONFIG.baseUrls.apiUrl + 'horses/get/';
  private _validateHorseAdUrl = CONFIG.baseUrls.apiUrl + 'horses/validate/';
  private _addHorseAdToFavoritesUrl = CONFIG.baseUrls.apiUrl + 'horses/favorite/';
  private _sendEmailUrl = CONFIG.baseUrls.apiUrl + 'sendemail';
  private _editHorseAdUrl = CONFIG.baseUrls.apiUrl + 'horses/update/';
  private _deleteHorseAdUrl = CONFIG.baseUrls.apiUrl + 'horses/delete/';
  private _increseViewsUrl = CONFIG.baseUrls.apiUrl + 'horses/views/';
  private _searchUrl = CONFIG.baseUrls.apiUrl + 'horses/search?searchModel=';
  private _setHorseAdProfilePicUrl = CONFIG.baseUrls.apiUrl + 'horses/images/profilepic/';
  private _deletaUnsavedImageUrl = CONFIG.baseUrls.apiUrl + 'horses/images/delete?imageName='
  private _getLatestHorses = CONFIG.baseUrls.apiUrl + 'horses/latest';
  private _saveHorseImageUrl = CONFIG.baseUrls.apiUrl + 'horses/images/save/';

  sharedSearchModel: SearchModel = new SearchModel();

  constructor(private _http: Http,
    private _httpWrapper: HttpWrapper,
    private transferHttp: TransferHttp,
    private _authService: AuthService,
    private _router: Router) { }

  getAllPriceRanges(): Observable<PriceRangeModel[]> {
    return this._http.get(this._priceRangesUrl)
      .map((res: Response) => res.json() as PriceRangeModel[])
      .catch(this.handleError);
  }

  getAllCountries(name?: string): Observable<string[]> {
    return this.transferHttp.get(CONFIG.restCountriesUrl + name).catch((error: any) => Observable.of([]));
  }

  getAllAbilities(): Observable<HorseAbilityModel[]> {
    return this._http.get(this._horseAbilitiesUrl)
      .map((res: Response) => res.json() as HorseAbilityModel[])
      .catch(this.handleError);
  }

  getAllRecomendedRiders(): Observable<RecommendedRiderModel[]> {
    return this.transferHttp.get(this._recommendedRidersUrl);
  }

  increaseViews(adId: number) {
    return this._http.post(this._increseViewsUrl + adId, "")
      .catch(this.handleError);
  }

  saveHorseAdImage(adId: number, imageName: string) {
    return this._httpWrapper.post(`${this._saveHorseImageUrl}${adId}?imageName=${imageName}`, "")
                            .map((res) => res.json())
                            .catch(this.handleError);
  }

  postHorseAd(model: HorseAdModel) {
    let body = JSON.stringify(model);

    return this._httpWrapper.post(this._postHorseAdUrl, body)
      .map((res: Response) => res.json())
      .catch(error => {
        if (error && error.status === 401 && this._authService.isTokenExpired()) {
          return this._authService.refreshToken().merge((data) => {
            this._authService.storeUserAccessInfo(data);
            return this._httpWrapper.post(this._postHorseAdUrl, body)
              .map((res: Response) => res.json())
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

  editHorseAd(model: HorseAdModel, id: number) {
    let body = JSON.stringify(model);

    return this._httpWrapper.post(this._editHorseAdUrl + id, body)
      .map((res: Response) => res)
      .catch(error => {
        if (error && error.status === 401 && this._authService.isTokenExpired()) {
          return this._authService.refreshToken().merge((data) => {
            this._authService.storeUserAccessInfo(data);
            return this._httpWrapper.post(this._editHorseAdUrl + id, body)
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

  deleteHorseAd(id: number, isSold: boolean) {
    return this._httpWrapper.post(this._deleteHorseAdUrl + id + "/" + isSold, "")
      .map((res: Response) => res)
      .catch(error => {
        if (error && error.status === 401 && this._authService.isTokenExpired()) {
          return this._authService.refreshToken().merge((data) => {
            this._authService.storeUserAccessInfo(data);
            return this._httpWrapper.delete(this._deleteHorseAdUrl + id)
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

  uploadHorseAdPhotos(photo: File) {
    let headers = new Headers();

    headers.set('Accept', 'application/json');
    headers.set('Authorization', 'Bearer ' + this._authService.getItem('access_token'));
    let requestOptions = new RequestOptions({
      headers: headers
    });

    let formData: FormData = new FormData();
    formData.append('uploadFile', photo, photo.name);

    return this._http
      .post(this._uploadHorseAdPhotosUrl, formData, requestOptions)
      .map((res: Response) => res.json())
      .catch(error => {
        if (error && error.status === 401 && this._authService.isTokenExpired()) {
          return this._authService.refreshToken().merge((data) => {
            this._authService.storeUserAccessInfo(data);
            requestOptions.headers.delete('Authorization');
            requestOptions.headers.append('Authorization', 'Bearer ' + data.access_token)
            return this._http.post(this._uploadHorseAdPhotosUrl, formData, requestOptions)
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

  deleteImage(imageId: number) {
    return this._httpWrapper.post(this._deleteHorseAdPhotoUrl + imageId, "")
      .catch(error => {
        if (error && error.status === 401 && this._authService.isTokenExpired()) {
          return this._authService.refreshToken().merge((data) => {
            this._authService.storeUserAccessInfo(data);
            return this._httpWrapper.post(this._deleteHorseAdPhotoUrl + imageId, "")
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

  deleteUnsavedImage(imageName: string) {
    return this._httpWrapper.post(this._deletaUnsavedImageUrl + imageName, "")
      .catch(error => {
        if (error && error.status === 401 && this._authService.isTokenExpired()) {
          return this._authService.refreshToken().merge((data) => {
            this._authService.storeUserAccessInfo(data);
            return this._httpWrapper.post(this._deletaUnsavedImageUrl + imageName, "")
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

  setHorseAdProfilePicture(imageId: number) {
    return this._httpWrapper.post(this._setHorseAdProfilePicUrl + imageId, "")
      .catch(error => {
        if (error && error.status === 401 && this._authService.isTokenExpired()) {
          return this._authService.refreshToken().merge((data) => {
            this._authService.storeUserAccessInfo(data);
            return this._httpWrapper.post(this._setHorseAdProfilePicUrl + imageId, "")
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

  getHorsesFromCategory(url: string, pageNumber: number): Observable<GetHorseAdListResultsModel> {
    return this._http.get(`${url + pageNumber}`)
      .map((res: Response) => res.json() as GetHorseAdListResultsModel)
      .catch(this.handleError);
  }

  getUnvalidatedHorsePosts(pageNumber: number): Observable<GetHorseAdListResultsModel> {
    return this._httpWrapper.get(`${this._getUnvalidatedHorsePosts + pageNumber}`)
      .map((res: Response) => res.json() as GetHorseAdListResultsModel)
      .catch(error => {
        if (error && error.status === 401 && this._authService.isTokenExpired()) {
          return this._authService.refreshToken().merge((data) => {
            this._authService.storeUserAccessInfo(data);
            return this._httpWrapper.get(`${this._getUnvalidatedHorsePosts + pageNumber}`)
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

  getHorseAdDetails(horseAdId: number): Observable<HorseAdModel> {
    return this.transferHttp.get(`${this._getHorseAdDetails + horseAdId}`)
      .catch(this.handleError);
  }

  validateHorseAd(horseAdId: number) {
    return this._httpWrapper.post(`${this._validateHorseAdUrl + horseAdId}`, "")
      .catch(error => {
        if (error && error.status === 401 && this._authService.isTokenExpired()) {
          return this._authService.refreshToken().merge((data) => {
            this._authService.storeUserAccessInfo(data);
            return this._httpWrapper.post(`${this._validateHorseAdUrl + horseAdId}`, "")
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

  addHorseAdToFavorites(horseAdId: number) {
    return this._httpWrapper.post(`${this._addHorseAdToFavoritesUrl + horseAdId}`, "")
      .catch(error => {
        if (error && error.status === 401 && this._authService.isTokenExpired()) {
          return this._authService.refreshToken().merge((data) => {
            this._authService.storeUserAccessInfo(data);
            return this._httpWrapper.post(`${this._addHorseAdToFavoritesUrl + horseAdId}`, "")
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

  sendEmail(emailModel: EmailModel) {
    let body = JSON.stringify(emailModel);

    return this._http.post(this._sendEmailUrl, body)
      .catch(this.handleError)
  }

  search(searchModel: SearchModel) {
    let body = JSON.stringify(searchModel);

    return this._http.get(this._searchUrl + body)
      .map((res) => res.json())
      .catch(this.handleError);
  }

  setSearchModel(searchModel: SearchModel) {
    this.sharedSearchModel = searchModel;
  }

  getSearchModel() {
    return this.sharedSearchModel;
  }

  resetSearchModel() {
    this.sharedSearchModel = new SearchModel();
  }

  getLatestHorses() {
    return this.transferHttp.get(this._getLatestHorses);
  }

  private handleError(error: Response) {
    let errorMsg = error.json().Message || "Server Error";
    return Observable.throw(errorMsg);
  }
}
