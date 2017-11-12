import { Component, OnInit, ViewChild, ElementRef, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { DOCUMENT } from '@angular/platform-browser';
import { Subscription } from 'rxjs/Subscription';
import { CONFIG } from '../../config';

//SERVICES
import { HorseAdsService } from '../horse-ads.service';

//MODELS
import { HorseAdListModel } from '../models/horseAdListModel';
import { SearchModel, BetweenAge, BetweenHeight } from '../models/searchModel';
import { PriceRangeModel } from '../models/priceRangeModel';
import { CountryModel } from '../models/countryModel';
import { GenderModel } from '../models/genderModel';
import { RecommendedRiderModel } from '../models/recommendedRiderModel';

@Component({
  templateUrl: "./horse-list-categories.component.html",
})

export class HorseListCategoriesComponent implements OnInit, OnDestroy {
  categoryHorseList: HorseAdListModel[];
  categoryName: string;
  totalNumber: number;
  pageNumber: number = 1;
  errorMessage: string;
  recommendedRiders: RecommendedRiderModel[];
  countries: string[];
  searchModel: SearchModel = new SearchModel();
  typeaheadNoResults: boolean;  
  hideQuickSearchPanel: boolean = false;
  showHideSearchBar: boolean = false;
  ageRange: number[] = [4, 13];
  heightRange: number[] = [160, 178];
  priceRange: number[] = [10000, 30000];

  private routerSub$: Subscription;

  constructor(private _route: ActivatedRoute,
    private _router: Router,
    private _horseAdService: HorseAdsService,
    @Inject(DOCUMENT) private document,
    @Inject(PLATFORM_ID) private platformId: Object) {

    this.routerSub$ = this._router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.categoryName = _route.snapshot.url[1].path;
        this.pageNumber = parseInt(_route.snapshot.url[2].path);
        this.totalNumber = this._route.snapshot.data['model'].TotalCount;
        this.categoryHorseList = this._route.snapshot.data['model'].HorseAdList;
      }
    });

    if (isPlatformBrowser(this.platformId)) {
      this.hideQuickSearchPanel = window.screen.width <= CONFIG.mobile_width;
    }

    this.searchModel = this._horseAdService.getSearchModel();
  }

  ngOnInit() {
    this.getCountries();
    this.getRecommendedRiders();
    this.totalNumber = this._route.snapshot.data['model'].TotalCount;
    this.categoryHorseList = this._route.snapshot.data['model'].HorseAdList;
  }

  search() {
    this.searchModel.AgeModel = new BetweenAge(this.ageRange[0], this.ageRange[1]);
    this.searchModel.HeightModel = new BetweenHeight(this.heightRange[0], this.heightRange[1]);

    this._horseAdService.setSearchModel(this.searchModel);

    if (this.pageNumber == 1) {
      this.searchingWhenFirstPage();
    }
    else {
      this.pageNumber = 1;
      this._router.navigate(['/horses-for-sale/' + this.categoryName + '/1']);
    }
  }

  searchingWhenFirstPage() {
    this._horseAdService.search(this.searchModel)
      .subscribe(res => {
        this.categoryHorseList = res.HorseAdList;
        this.totalNumber = res.TotalCount
      },
      error => this.errorMessage = error);
  }

  resetSearchCriteria() {
    this.searchModel = new SearchModel();
    this.search();
  }

  getCountries() {
    this._horseAdService.getAllCountries()
      .subscribe(res => this.countries = res,
      error => this.errorMessage = error);
  }

  getRecommendedRiders() {
    this._horseAdService.getAllRecomendedRiders()
      .subscribe(res => this.recommendedRiders = res,
      error => this.errorMessage = error);
  }

  isCheckedRecommendedRider(value: RecommendedRiderModel) {
    return this.searchModel.SuitableFor.findIndex(y => value.Id == y) > -1;
  }

  addHorseRecommendedRider(value: RecommendedRiderModel) {
    if (isPlatformBrowser(this.platformId)) {
      if ((<HTMLInputElement>this.document.getElementById(value.Rider)).checked === true) {
        this.searchModel.SuitableFor.push(value.Id);
      } else if ((<HTMLInputElement>this.document.getElementById(value.Rider)).checked === false) {
        let position = this.searchModel.SuitableFor.indexOf(value.Id);
        this.searchModel.SuitableFor.splice(position, 1);
      }
    }
  }

  pageChanged(event: any) {
    this.pageNumber = event.page;
    this._router.navigate(['/horses-for-sale/' + this.categoryName, event.page]);
  }

  showSearchPanel() {
    this.hideQuickSearchPanel = false;
    this.showHideSearchBar = true;
    if (isPlatformBrowser(this.platformId)) {
      window.scrollTo(0, 0);
    }
  }

  hideSearchPanel() {
    this.hideQuickSearchPanel = true;
    this.showHideSearchBar = false;
    if (isPlatformBrowser(this.platformId)) {
      window.scrollTo(0, 0);
    }
  }

  changeTypeaheadNoResults(e: boolean): void {
    this.typeaheadNoResults = e;
  }

  ngOnDestroy() {
    this.routerSub$.unsubscribe();
  }
}
