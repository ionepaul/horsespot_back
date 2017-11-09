import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';

import { AuthService } from '../auth/auth.service';

@Injectable()
export class IsPostOwnerGuard implements CanActivate {
  
  constructor(private _authService: AuthService, private _router: Router) {}

  canActivate(route: ActivatedRouteSnapshot) {
      var postId = route.url[2].path;

      return this._authService.checkPostOwner(postId).map((res) => {
            if (res == true) {
                return true;
            }
            
            this._router.navigate(['/error/403']);
            return false;            
      });
  }
}