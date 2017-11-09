import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';

import { AccountService } from '../../account/account.service';

@Injectable()
export class AdminGuard implements CanActivate {

  constructor(private _accountService: AccountService, private _router: Router) {}

  canActivate() {
      let currentUserId = this._accountService.getUserId();
        
      if (currentUserId) {
      
        return this._accountService.isAdmin(currentUserId).map((res) => { 
              if (res == true) {
                return true;
              }

            this._router.navigate(['/error/403']);
            return false; 
        });
       }
  }
}