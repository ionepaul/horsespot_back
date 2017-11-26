import { Component, Input, OnChanges } from '@angular/core';
import { Router,ActivatedRoute } from '@angular/router';
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { CONFIG } from '../../config';

//SERVICES
import { AccountService } from '../../account/account.service';

//MODELS
import { HorseAdListModel } from '../../horse-advertisments/models/horseAdListModel';
import { HorseAdsService } from '../../horse-advertisments/horse-ads.service';

@Component({
    selector: 'horse-list',
    templateUrl: "./horse-list.component.html"
})

export class HorseListComponent {
    @Input() horses: HorseAdListModel[];
    @Input() category?: string;
    @Input() threeOnRow?: boolean;
    horseImageUrl: string = CONFIG.horseAdsImagesUrl;
    currentUserId: string;

    constructor(private _router: Router,
                private _route: ActivatedRoute, 
                private _accountService: AccountService,
                private _horseAdService: HorseAdsService,
                @Inject(PLATFORM_ID) private platformId: Object) 
    { 
        this.currentUserId = this._accountService.getUserId();
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
}
