import { Component, Input, OnChanges } from '@angular/core';
import { Router,ActivatedRoute } from '@angular/router';
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

import { HorseAdListModel } from '../../horse-advertisments/models/horseAdListModel';
import { AccountService } from '../../account/account.service';
import { HorseAdsService } from '../../horse-advertisments/horse-ads.service';

import { CONFIG } from '../../config';

@Component({
    selector: 'horse-list',
    templateUrl: "./horse-list.component.html"
})

export class HorseListComponent implements OnChanges {
    @Input() horses: HorseAdListModel[];
    @Input() category?: string;
    @Input() threeOnRow? : boolean;
    horseImageUrl: string = CONFIG.imagesUrl + '/Images/HorseAdsImg/';
    currentUserId: string;

    constructor(private _router: Router,
                private _route: ActivatedRoute, 
                private _accountService: AccountService,
                private _horseAdService: HorseAdsService,
                @Inject(PLATFORM_ID) private platformId: Object) 
    { 
        this.currentUserId = this._accountService.getUserId();
    }

    ngOnChanges() {
        //this.cutText();
        //this.imageBackground();
    }

    getHorseDetails(horseAdId: string, title: string, gender: string) {
        let formatedTitle = title.replace(/ /g,"-");
        let category = this.category ? this.category : this._route.snapshot.url[1].path;
        let url = formatedTitle.toLocaleLowerCase() + "-" + gender.toLocaleLowerCase();

        this._router.navigate(['/horses-for-sale', horseAdId, category, url]);
    }

    redirectToEdit(horseAdId: string) {
        this._router.navigate(['/horses/edit', horseAdId]);
    }

    cutText() {
        var para, paraArr, length, trimText;

        //if (isPlatformBrowser(this.platformId)) {
            //para = document.querySelectorAll('.home-container article .desc p');
            //paraArr = Array.prototype.slice.call(para);
            //paraArr.forEach(function(el) {
            //    length = 100;
            //    trimText = el.innerHTML.substring(0, length) + '...';
            //    el.innerHTML = trimText;
            //});
        ///}
    }

    //imageBackground() {
    //    var cards, cardsArr;

    //    //if (isPlatformBrowser(this.platformId)) {
    //        // select all the cards
    //        cards = document.querySelectorAll('.category-row article');
    //        // convert them into an array
    //        cardsArr = Array.prototype.slice.call(cards);
    //        // loop inside them
    //        cardsArr.forEach(function(el) {
    //            var imgSource;
    //            // find the img inside .featured-img
    //            imgSource = el.querySelector('.featured-img img').src;
    //            // add the image inside the featured image as a background to the featured-img div
    //            el.querySelector('.featured-img').style.background = 'linear-gradient( rgba(142, 14, 0, 0.6), rgba(31, 28, 24, 0.6) ), center center no-repeat, url('+imgSource+')';
    //        });
    //    //}
    //}
}
