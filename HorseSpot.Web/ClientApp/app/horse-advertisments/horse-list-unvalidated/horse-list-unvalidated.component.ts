import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { Title, Meta } from '@angular/platform-browser';
import { HorseAdListModel } from '../models/horseAdListModel';

@Component({
    templateUrl: "./horse-list-unvalidated.component.html"
})

export class HorseListUnvalidatedComponent implements OnInit {    
    unvalidatedHorseList: HorseAdListModel[];    
    totalNumber: number;
    pageNumber: number = 1;

    constructor(private _route: ActivatedRoute, private _router: Router, pageTitle: Title, private _metaData: Meta) {
        this._router.events.subscribe((event) => {
            if(event instanceof NavigationEnd) {
                 this.pageNumber = parseInt(_route.snapshot.url[2].path);
                 this.totalNumber = this._route.snapshot.data['model'].TotalCount;
                 this.unvalidatedHorseList = this._route.snapshot.data['model'].HorseAdList;
            }
        });

        pageTitle.setTitle('Unvalidated Horses | HorseSpot');
        _metaData.addTags([ 
                  { name: 'robots', content: 'NOINDEX, NOFOLLOW'}
        ]);
    }

    ngOnInit() {
        this.totalNumber = this._route.snapshot.data['model'].TotalCount;
        this.unvalidatedHorseList = this._route.snapshot.data['model'].HorseAdList;
    }

    pageChanged(event: any) {
         this.pageNumber = event.page;
         this._router.navigate(['/horses/unvalidated', event.page]);             
    }   
    
    ngOnDestroy() {
        this._metaData.removeTag("name='robots'"); 
    }
}