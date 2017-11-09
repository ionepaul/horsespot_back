import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

import { Subscription } from 'rxjs/Subscription';
import { AuthService } from '../auth/auth.service';

@Component({
    templateUrl: './error.component.html'
})

export class ErrorComponent  implements OnInit {
  statusCode: string;
  statusCodeInfo: string;
  additionalInfo: string;
  buttonText: string;
  sub: Subscription;
  
  constructor(private _activatedRoute: ActivatedRoute, 
              private _authService: AuthService,
              private _router: Router,
              private _translateService: TranslateService) {
        this.sub = this._activatedRoute
        .params
        .subscribe(params => {
            this.statusCode = params['statusCode'];
    });
   }

  ngOnInit() {
      if (this.statusCode == "403") {
        this._authService.removeUserStoredInfo();
        this.statusCodeInfo = this.getTranslationMessage("ERROR_PAGE.FORBBIDEN_TITLE");
        this.additionalInfo = this.getTranslationMessage("ERROR_PAGE.FORBBIDEN_TEXT");
        this.buttonText = this.getTranslationMessage("ERROR_PAGE.FORBBIDEN_BTN");
      } 
      else if (this.statusCode == "401") {
        this.statusCodeInfo = this.getTranslationMessage("ERROR_PAGE.UNAUTHORIZED_TITLE");
        this.additionalInfo = this.getTranslationMessage("ERROR_PAGE.UNAUTHORIZED_TEXT");
        this.buttonText = this.getTranslationMessage("ERROR_PAGE.HOME_BTN");
      }
      else if (this.statusCode == "500") {
        this.statusCodeInfo = this.getTranslationMessage("ERROR_PAGE.INTERNAL_TITLE");
        this.additionalInfo = this.getTranslationMessage("ERROR_PAGE.INTERNAL_TEXT");
        this.buttonText = this.getTranslationMessage("ERROR_PAGE.HOME_BTN");
      }
      else {
        this.statusCode = "404";
        this.statusCodeInfo = this.getTranslationMessage("ERROR_PAGE.NOT_FOUND_TITLE");
        this.additionalInfo = this.getTranslationMessage("ERROR_PAGE.NOT_FOUND_TEXT");
        this.buttonText = this.getTranslationMessage("ERROR_PAGE.HOME_BTN");
      }
  }

  navigateTo() {
      if (this.statusCode == "403") {
        this._router.navigate(['/account/login']);
      } else {
        this._router.navigate(['/home']);
      }
  }

    getTranslationMessage(key: string): string {
        var value = "";
        this._translateService.get(key).subscribe((res: string) => value = res);
        return value;
    }
}
 