import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';

import { AccountService } from '../account.service';

import { GetHorseAdListResultsModel } from '../../horse-advertisments/models/getHorseAdListResultsModel';

@Injectable()
export class UserFavoritesResolver implements Resolve<GetHorseAdListResultsModel> {

  constructor(private _accountService: AccountService) { }

  resolve(route: ActivatedRouteSnapshot) {
    var pageNumber = parseInt(route.url[2].path);

    return this._accountService.getUserFavoritesPosts(pageNumber);
  }
}