import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';

import { AccountService } from '../../account/account.service';

@Injectable()
export class WishListGuard implements CanActivate {

  constructor(private _accountService: AccountService,
    private _router: Router) { }

  canActivate(route: ActivatedRouteSnapshot) {
    var userId = route.url[2].path;

    return userId === this._accountService.getUserId();
  }
}
