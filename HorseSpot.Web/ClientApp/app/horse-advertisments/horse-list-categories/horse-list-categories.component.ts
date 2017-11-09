import { Component, OnInit, ViewChild, ElementRef, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router'; 
import { HorseAdListModel } from '../models/horseAdListModel';
import { SearchModel } from '../models/searchModel';
import { BetweenAge } from '../models/searchModel';
import { PriceRangeModel } from '../models/priceRangeModel';
import { CountryModel } from '../models/countryModel';
import { GenderModel } from '../models/genderModel';
import { RecommendedRiderModel } from '../models/recommendedRiderModel';
import { HorseAdsService } from '../horse-ads.service'; 
import { Meta, Title } from '@angular/platform-browser';
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { DOCUMENT } from '@angular/platform-browser';

@Component({
    templateUrl: "./horse-list-categories.component.html",
})

export class HorseListCategoriesComponent implements OnInit, OnDestroy {
    @ViewChild('advanceSearch') public advanceSearch: ElementRef;
    categoryHorseList: HorseAdListModel[];    
    categoryName: string;
    totalNumber: number;
    pageNumber: number = 1;
    errorMessage: string;
    genders: GenderModel[];
    genderId: number;
    priceRangeId: number;
    priceRanges: PriceRangeModel[];
    recommendedRiders: RecommendedRiderModel[];
    isAdvanceSearch: boolean = false;
    countries: string[];
    searchModel: SearchModel = new SearchModel();
    advanceSearchState: string = "out";
    sub: any;
    hideQuickSearchPanel: boolean = false;
    showHideSearchBar: boolean = false;
    ageRange: number[] = [0, 20];
    heightRange: number[] = [0, 200];
    price: number[] = [1.000, 3.000];

    constructor(private _route: ActivatedRoute, 
                private _router: Router, 
                private _horseAdService: HorseAdsService,
                @Inject(DOCUMENT) private document,
                @Inject(PLATFORM_ID) private platformId: Object, 
                private _metaData: Meta, pageTitle: Title) {

        this.sub = this._router.events.subscribe((event) => {
            if  (event instanceof NavigationEnd) {
                 this.categoryName = _route.snapshot.url[1].path;
                 this.pageNumber = parseInt(_route.snapshot.url[2].path);
                 this.totalNumber = this._route.snapshot.data['model'].TotalCount;
                 this.categoryHorseList = this._route.snapshot.data['model'].HorseAdList;

                let title = this.categoryName.toUpperCase() + ' | HorseSpot';
                pageTitle.setTitle(title);
            }
        });

        this.removeExistingTags();

        _metaData.addTags([ 
            { name: 'description', content: 'Choose from multiple horses for sale grouped by ability and use the advance serach to find the perfect one for you.'},
            { property: 'og:title', content: 'Horses For Sale | Horse Spot'},
            { property: 'og:description', content: 'Choose from multiple horses for sale grouped by ability and use the advance serach to find the perfect one for you.'},
            { name: 'twitter:card', content: "summary_large_image"},
            { name: 'twitter:title', content: "Horses For Sale | Horse Spot"},
            { name: 'twitter:description', content: 'Choose from multiple horses for sale grouped by ability and use the advance serach to find the perfect one for you.'},
        ]);

        if (isPlatformBrowser(this.platformId)) {
            this.hideQuickSearchPanel = window.screen.width <= 767;
        }

        this.searchModel = this._horseAdService.getSearchModel();
        //this.genderId = this.searchModel.Gender;
        this.priceRangeId = this.searchModel.PriceRangeId;
    }

    search(isRefresh: boolean) {
        this._horseAdService.setSearchModel(this.searchModel);

        if (this.pageNumber == 1) {
            this.searchingWhenFirstPage();
        }
        else {
            this.pageNumber = 1;
            this._router.navigate(['/horses-for-sale/' + this.categoryName + '/1']);
        }

        if (!isRefresh) {
            setTimeout(() => {
                this.advanceSearch.nativeElement.scrollIntoView({ behavior: 'smooth' });  
            }, 0);
        }  
    }

    searchingWhenFirstPage() { 
        this._horseAdService.search(this.searchModel)
                            .subscribe(res => {this.categoryHorseList = res.HorseAdList; this.totalNumber = res.TotalCount},
                                       error => this.errorMessage = error);
    }

    resetSearchCriteria() {
        this.searchModel = new SearchModel();
        this.genderId = 0;
        this.priceRangeId = 0;
        this.search(true);
    }

    checkPageNumberAndSearch() {
        if (this.pageNumber == 1) {
            this.searchingWhenFirstPage();
        }
        else {
            this.pageNumber = 1;
            this._router.navigate(['/horses-for-sale/' + this.categoryName + '/1']);
        } 
    }

    ngOnInit() {
        this.getGenders();
        this.getPriceRanges();
        this.getCountries();
        this.getRecommendedRiders();
        this.totalNumber = this._route.snapshot.data['model'].TotalCount;
        this.categoryHorseList = this._route.snapshot.data['model'].HorseAdList;
    }

    getGenders() {
        // this._horseAdService.getAllGenders()
        //                     .subscribe(res => this.genders = res,
        //                                error =>this.errorMessage = error);
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

    getRecommendedRiders() {
        this._horseAdService.getAllRecomendedRiders()
                            .subscribe(res => this.recommendedRiders = res,
                                       error =>this.errorMessage = error);
    }

    updateGender(event: any) {
       this.genderId = event;
       this.searchModel.GenderId = event;
    }

    updatePriceRange(event: any) {
       this.priceRangeId = event;
       this.searchModel.PriceRangeId = event;
    }

    minAgeChanged(event: any) {
        this.searchModel.AgeModel.MinAge = parseInt(event) || null;
    }

    maxAgeChanged(event: any) {
        this.searchModel.AgeModel.MaxAge = parseInt(event) || null;
    }

    minHeightChanged(event: any) {
        this.searchModel.HeightModel.MinHeight = parseInt(event) || null;
    }

    maxHeightChanged(event: any) {
        this.searchModel.HeightModel.MaxHeight = parseInt(event) || null;
    }

    toggleAdvanceSearch() {
        this.isAdvanceSearch = !this.isAdvanceSearch;
        this.advanceSearchState = (this.advanceSearchState == 'in' ? 'out' : 'in');
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

    ngOnDestroy() {
        this.sub.unsubscribe();
        this.removeExistingTags();
    }

    removeExistingTags() {
        this._metaData.removeTag("name='description'");
        this._metaData.removeTag("property='og:title'");
        this._metaData.removeTag("property='og:description'");
        this._metaData.removeTag("property='og:image'");
        this._metaData.removeTag("name='twitter:card'");
        this._metaData.removeTag("name='twitter:description'");
        this._metaData.removeTag("name='twitter:card'");
        this._metaData.removeTag("name='twitter:image'");
    }
}
