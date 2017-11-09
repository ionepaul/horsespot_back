import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Meta, Title } from '@angular/platform-browser';
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { UtilDictionaries } from '../shared/utils/util-dictionaries';
import { PriceRangeModel } from '../horse-advertisments/models/priceRangeModel';
import { CountryModel } from '../horse-advertisments/models/countryModel';
import { SearchModel } from '../horse-advertisments/models/searchModel';
import { HorseAdsService } from '../horse-advertisments/horse-ads.service';
import { LatestHorsesModel } from '../horse-advertisments/models/latestHorses.model';

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

  constructor(private _router: Router,
    private _horseAdService: HorseAdsService,
    @Inject(PLATFORM_ID) private platformId: Object,
    private _metaData: Meta, pageTitle: Title) {

    this._horseAdService.resetSearchModel();
    this.searchModel = this._horseAdService.getSearchModel();
    this.searchModel.Gender = "Gender";
    this.priceRangeId = this.searchModel.PriceRangeId
  }

  ngOnInit() {
    this.getLatestHorses();
    this.getPriceRanges();
    this.getCountries();
  }

  getPriceRanges() {
    this._horseAdService.getAllPriceRanges()
      .subscribe(res => this.priceRanges = res,
      error => this.errorMessage = error);
  }

  getCountries() {
    this._horseAdService.getAllCountries()
      .subscribe(res => this.countries = res,
      error => this.errorMessage = error);
  }

  changeTypeaheadNoResults(e: boolean): void {
    this.typeaheadNoResults = e;
  }

  search() {
    this._horseAdService.setSearchModel(this.searchModel);
    this._router.navigate(['/horses-for-sale/' + this.utilDictionaries.getUrlByCategoryId(this.categoryId) + '/1']);
  }

  getLatestHorses() {
    this._horseAdService.getLatestHorses()
      .subscribe(res => this.latestHorses = res,
      error => this.errorMessage = error);
  }
}
