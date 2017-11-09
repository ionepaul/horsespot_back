import { Component, OnInit, OnDestroy } from '@angular/core';
import { Location } from '@angular/common';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { Meta, Title } from '@angular/platform-browser';

import { HorseAdListModel } from '../../horse-advertisments/models/horseAdListModel';

@Component({
    templateUrl: './user-posts.component.html'
})

export class UserPostsComponent implements OnInit, OnDestroy {    
    pageNumber: number = 1;
    userId: string = "";
    totalNumber: number;
    userPosts: HorseAdListModel[];

    constructor(private _route: ActivatedRoute, 
                private _router: Router,
                private _location: Location,
                private _metaData: Meta, pageTitle: Title) {
        pageTitle.setTitle('Your Wishlist | Horse Spot');
        _metaData.addTags([ 
            { name: 'robots', content: 'NOINDEX, NOFOLLOW'}
        ]);

        this._router.events.subscribe((event) => {
            if(event instanceof NavigationEnd) {
                 this.userId = _route.snapshot.url[2].path;
                 this.pageNumber = parseInt(_route.snapshot.url[3].path);
                 this.totalNumber = this._route.snapshot.data['model'].TotalCount;
                 this.userPosts = this._route.snapshot.data['model'].HorseAdList;
            }
        });
    }

    ngOnInit() {
        this.totalNumber = this._route.snapshot.data['model'].TotalCount;
        this.userPosts = this._route.snapshot.data['model'].HorseAdList;
    }

    back() {
        this._location.back();
    }

    pageChanged(event: any) {
         this.pageNumber = event.page;
         this._router.navigate(['/account/horses-for-sale', this.userId, event.page]);             
    } 

    ngOnDestroy() {
        this._metaData.removeTag("name='robots'");
    } 
}