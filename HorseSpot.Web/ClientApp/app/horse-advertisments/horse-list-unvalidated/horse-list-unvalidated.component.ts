import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { Location } from '@angular/common';
import { CONFIG } from '../../config';

//MODELS
import { HorseAdListModel } from '../models/horseAdListModel';

@Component({
  templateUrl: "./horse-list-unvalidated.component.html"
})

export class HorseListUnvalidatedComponent implements OnInit {
  unvalidatedHorseList: HorseAdListModel[];
  totalNumber: number;
  pageNumber: number = 1;
  adsPerPage: number = CONFIG.adsPerPage;

  constructor(private _route: ActivatedRoute,
    private _router: Router,
    private _location: Location) {

    this._router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.pageNumber = parseInt(_route.snapshot.url[2].path);
        this.totalNumber = this._route.snapshot.data['model'].TotalCount;
        this.unvalidatedHorseList = this._route.snapshot.data['model'].HorseAdList;
      }
    });
  }

  ngOnInit() {
    this.totalNumber = this._route.snapshot.data['model'].TotalCount;
    this.unvalidatedHorseList = this._route.snapshot.data['model'].HorseAdList;
  }

  pageChanged(event: any) {
    this.pageNumber = event.page;
    this._router.navigate(['/horses/unvalidated', event.page]);
  }

  back() {
    this._location.back();
  }
}
