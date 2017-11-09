import { Component, OnInit, OnDestroy } from '@angular/core';
import { Location } from '@angular/common';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { Meta, Title } from '@angular/platform-browser';

import { HorseAdListModel } from '../../horse-advertisments/models/horseAdListModel';

@Component({
    templateUrl: './user-favorites.component.html'
})

export class UserFavoritesComponent implements OnInit, OnDestroy {
    pageNumber: number = 1;
    totalNumber: number;
    userFavoritesPosts: HorseAdListModel[];

    constructor(private _route: ActivatedRoute, 
                private _router: Router, 
                private _location: Location,
                private _metaData: Meta, pageTitle: Title) {
        pageTitle.setTitle('Your Horses | Horse Spot');
        _metaData.addTags([ 
            { name: 'robots', content: 'NOINDEX, NOFOLLOW'}
        ]);

        this._router.events.subscribe((event) => {
            if(event instanceof NavigationEnd) {
                 this.pageNumber = parseInt(_route.snapshot.url[2].path);
                 this.totalNumber = this._route.snapshot.data['model'].TotalCount;
                 this.userFavoritesPosts = this._route.snapshot.data['model'].HorseAdList;
            }
        });
    }

    ngOnInit() {
        this.totalNumber = this._route.snapshot.data['model'].TotalCount;
        this.userFavoritesPosts = this._route.snapshot.data['model'].HorseAdList;
    }

    back() {
        this._location.back();
    }

    pageChanged(event: any) {
         this.pageNumber = event.page;
         this._router.navigate(['/account/wishlist', event.page]);             
    }

    ngOnDestroy() {
        this._metaData.removeTag("name='robots'");
    }  
}