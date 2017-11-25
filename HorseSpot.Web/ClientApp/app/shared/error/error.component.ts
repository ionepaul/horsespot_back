import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

import { Subscription } from 'rxjs/Subscription';
import { AuthService } from '../auth/auth.service';

@Component({
  templateUrl: './error.component.html'
})

export class ErrorComponent implements OnInit, OnDestroy {
  statusCode: string;

  private _routerSub$: Subscription;

  constructor(private _activatedRoute: ActivatedRoute,
    private _authService: AuthService,
    private _router: Router,
    private _translateService: TranslateService) {
    this._routerSub$ = this._activatedRoute
      .params
      .subscribe(params => {
        this.statusCode = params['statusCode'];
      });
  }
  ngOnInit() {
    if (this.statusCode == '403') {
      this._authService.removeUserStoredInfo();
    }
  }

  ngOnDestroy() {
    this._routerSub$.unsubscribe();
  }
}
