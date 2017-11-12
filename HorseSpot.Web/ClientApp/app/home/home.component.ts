import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Observable } from 'rxjs/Observable';
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
  templateUrl: './home.component.html'
})

export class HomeComponent implements OnInit {
  priceRanges: PriceRangeModel[];
  countries: string[];
  searchModel: SearchModel;
  priceRangeId: number;
  errorMessage: string;
  categoryId: number = 0;
  genderId: number = 0;
  typeaheadNoResults: boolean;
  utilDictionaries: UtilDictionaries = new UtilDictionaries();
  latestHorses: LatestHorsesModel = <LatestHorsesModel>{};
  selectedCountry: string;
  countryData: Observable<any[]>;

  constructor(private _router: Router,
    private _horseAdService: HorseAdsService,
    @Inject(PLATFORM_ID) private platformId: Object) {

    this._horseAdService.resetSearchModel();
    this.searchModel = this._horseAdService.getSearchModel();
    this.searchModel.Gender = "Gender";
    this.priceRangeId = this.searchModel.PriceRangeId
  }

  ngOnInit() {
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
    this._horseAdService.setSearchModel(this.searchModel);
    this._router.navigate(['/horses-for-sale/' + this.utilDictionaries.getUrlByCategoryId(this.categoryId) + '/1']);
  }

  getLatestHorses() {
    this._horseAdService.getLatestHorses()
      .subscribe(res => this.latestHorses = res,
      error => this.errorMessage = error);
  }
}
