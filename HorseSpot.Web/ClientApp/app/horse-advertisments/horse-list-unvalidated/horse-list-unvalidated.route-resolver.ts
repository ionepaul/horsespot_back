import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';

import { HorseAdsService } from '../horse-ads.service';

import { GetHorseAdListResultsModel } from '../../horse-advertisments/models/getHorseAdListResultsModel';

@Injectable()
export class UnvalidatedHorseListResolver implements Resolve<GetHorseAdListResultsModel> {

  constructor(private _horseAdService: HorseAdsService) { }

  resolve(route: ActivatedRouteSnapshot) {
    var pageNumber = parseInt(route.url[2].path);

    return this._horseAdService.getUnvalidatedHorsePosts(pageNumber);
  }
}