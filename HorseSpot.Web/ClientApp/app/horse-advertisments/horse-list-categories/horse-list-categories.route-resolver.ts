import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router, NavigationEnd, Event } from '@angular/router';
import { HorseAdsService } from '../horse-ads.service';
import { UtilDictionaries } from '../../shared/utils/util-dictionaries';
import { GetHorseAdListResultsModel } from '../models/getHorseAdListResultsModel';

@Injectable()
export class HorseListCategoriesResolver implements Resolve<GetHorseAdListResultsModel> {

  utilDictionaries: UtilDictionaries = new UtilDictionaries();
  private _categoryName: string;
  private isRedirectFromHome: boolean = false;
  private _homeUrl = "/home";

  constructor(private _horseAdService: HorseAdsService, private _router: Router) { 
      this._router.events.pairwise().subscribe((event: any) => {
          if (event[0].url == this._homeUrl) {
              this.isRedirectFromHome = true;
          } else {
            this.isRedirectFromHome = false;
          }
      });

      if (this._router.isActive(this._homeUrl, true)) {
          this.isRedirectFromHome = true;
      }
  }

  resolve(route: ActivatedRouteSnapshot) {        
    if (this._categoryName != route.url[1].path && !this.isRedirectFromHome) { 
        this._horseAdService.resetSearchModel();
    }
    
    this._categoryName = route.url[1].path;
    var pageNumber = parseInt(route.url[2].path);

    var searchModel = this._horseAdService.getSearchModel();     
    searchModel.PageNumber = pageNumber;
    searchModel.AbilityId = parseInt(this.utilDictionaries.getUrlByCategoryName(this._categoryName));
    searchModel.Gender = searchModel.GenderId == 0 ? null : this.utilDictionaries.getGenderNameById(searchModel.GenderId);

    return this._horseAdService.search(searchModel);
  }
}
