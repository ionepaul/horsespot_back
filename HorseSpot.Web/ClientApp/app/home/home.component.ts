import { Component, OnInit, OnDestroy, trigger, state, transition, style, animate } from '@angular/core';
import { Router } from '@angular/router';
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Observable } from 'rxjs/Observable';
import { CONFIG } from '../config';
import 'rxjs/add/observable/of';

//UTILS
import { UtilDictionaries } from '../shared/utils/util-dictionaries';

//MODELS
import { PriceRangeModel } from '../horse-advertisments/models/priceRangeModel';
import { CountryModel } from '../horse-advertisments/models/countryModel';
import { SearchModel } from '../horse-advertisments/models/searchModel';
import { LatestHorsesModel } from '../horse-advertisments/models/latestHorses.model';

//SERVICES
import { HorseAdsService } from '../horse-advertisments/horse-ads.service';

@Component({
  templateUrl: './home.component.html',
  animations: [
    trigger('searchForm', [
      state('in', style({ display: 'block', height: '*', opacity: 1 })),
      transition('* => in', [
        animate('300ms ease-in')
      ]),
      state('out', style({ display: 'none', height: '0px', opacity: 0 })),
      transition('in => out', [
        animate('300ms ease-out')
      ]),
    ])
  ]
})

export class HomeComponent implements OnInit {
  priceRanges: PriceRangeModel[];
  searchFormState: string = "in";
  countries: string[];
  searchModel: SearchModel;
  priceRangeId: number = 0;
  errorMessage: string;
  categoryId: number = 0;
  genderId: number = 0;
  typeaheadNoResults: boolean;
  utilDictionaries: UtilDictionaries = new UtilDictionaries();
  latestHorses: LatestHorsesModel = <LatestHorsesModel>{};
  selectedCountry: string;
  countryData: Observable<any[]>;
  selectedPriceRangeValue: string;
  isMobile: boolean;

  constructor(private _router: Router,
    private _horseAdService: HorseAdsService,
    @Inject(PLATFORM_ID) private platformId: Object) {

    this._horseAdService.resetSearchModel();
    this.searchModel = this._horseAdService.getSearchModel();
    this.searchModel.Gender = CONFIG.gender;
    this.priceRangeId = this.searchModel.PriceRangeId;
  }

  ngOnInit() {
    this.isMobile = window.screen.width <= CONFIG.mobile_width;
    this.searchFormState = this.isMobile ? "out" : "in";

    this.countryData = Observable.create((observer: any) => {
      observer.next(this.selectedCountry);
    }).mergeMap((name: string) => this.getCountries(name));

    this.getLatestHorses();
    this.getPriceRanges();
  }

  getPriceRanges() {
    this._horseAdService.getAllPriceRanges()
      .subscribe(res => this.priceRanges = res,
      error => this.errorMessage = error);
  }

  getCountries(name: string) {
    return this._horseAdService.getAllCountries(name);
  }

  changeTypeaheadNoResults(e: boolean): void {
    this.typeaheadNoResults = e;
  }

  search() {
    this.searchModel.Country = this.selectedCountry;

    if (this.priceRangeId != 0) {
      let priceRangeArray = this.getPriceRangeModelIntValues(this.priceRanges.find(x => x.Id == this.priceRangeId).PriceRangeValue);
     
      if (priceRangeArray.length == 1) {
        priceRangeArray.push(CONFIG.frontMaxPriceRangeValue);
      }

      this.searchModel.PriceModel.MinPrice = priceRangeArray[0];
      this.searchModel.PriceModel.MaxPrice = priceRangeArray[1];
    }

    this._horseAdService.setSearchModel(this.searchModel);
    this._router.navigate(['/horses-for-sale/' + this.utilDictionaries.getUrlByCategoryId(this.categoryId) + '/1']);
  }

  getLatestHorses() {
    this._horseAdService.getLatestHorses()
      .subscribe(res => this.latestHorses = res,
      error => this.errorMessage = error);
  }

  getPriceRangeModelIntValues(priceRangeModelValue: string) {
    let priceRangeValues: number[] = [];

    if (priceRangeModelValue === CONFIG.dbMaxPriceRangeValue) {
      priceRangeValues.push(parseInt(priceRangeModelValue.replace(",", "").replace("+", "")));

      return priceRangeValues;
    }

    let values = priceRangeModelValue.trim().split('-');

    priceRangeValues.push(parseInt(values[0].trim().replace(",", "")));
    priceRangeValues.push(parseInt(values[1].trim().replace(",", "")));

    return priceRangeValues;
  }

  toggleSearchOnMobile() {
    this.searchFormState = this.searchFormState == 'in' ? 'out' : 'in';
  }
}
