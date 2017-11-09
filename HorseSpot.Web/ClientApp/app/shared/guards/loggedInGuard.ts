import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';

import { AccountService } from '../../account/account.service';

@Injectable()
export class LoggedInGuard implements CanActivate {

  constructor(private _accountService: AccountService, private _router: Router) {}

  canActivate() {
      if (!this._accountService.isLoggedIn()) {
          this._router.navigate(['/account/login']);
          return false;
      }
      else {
          return true;
      }
  }
}