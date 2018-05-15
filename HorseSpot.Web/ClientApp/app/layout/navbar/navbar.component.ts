import { Component, Input, HostListener, ViewChild, ElementRef, OnInit, TemplateRef, OnDestroy } from '@angular/core';
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser, Location } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { NgForm } from '@angular/forms';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs/Subscription';

//SERVICES
import { NotificationService } from '../../shared/notifications/notification.service';
import { AccountService } from '../../account/account.service';
import { SpinnerService } from '../../shared/spinner/spinner.service';

//MODELS
import { RegisterModel } from '../../account/models/register.model';
import { RegisterExternalModel } from '../../account/models/registerExternalModel';
import { LoginModel } from '../../account/models/login.model';
import { ExternalUserModel } from '../../account/models/external-user.model'; 

import { CONFIG } from '../../config';

@Component({
  selector: 'navbar',
  templateUrl: './navbar.component.html'
})
export class NavbarComponent implements OnInit, OnDestroy {
  @ViewChild('phoneNumberModal') public phoneNumberModal: ModalDirective;
  @ViewChild('signUpModal') public signUpModal: ModalDirective;
  @ViewChild('loginModal') public loginModal: ModalDirective;
  @ViewChild('registerForm') public registerForm: NgForm;
  @ViewChild('logInForm') public logInForm: NgForm;
  @ViewChild('phoneNumberForm') public phoneNumberForm: NgForm;
  isBurgerMenuCollapsed: boolean = true;
  isLangDropdownCollapsed: boolean = true;
  notificationsNumber: number = 0;
  isMobileDevice: boolean;
  languages = CONFIG.languages;
  currentLang: any;
  provider: string;
  externalToken: string;
  externalUserPhoneNumber: string;
  loginInput: LoginModel = <LoginModel>{};
  registerInput: RegisterModel = <RegisterModel>{};
  externalUserModel: ExternalUserModel = <ExternalUserModel>{};
  errorMessage: string;
  isForgotPassword: boolean = false;
  forgotPasswordEmail: string;
  forgotPasswordErrorMessage: string;
  notificationRefresh: number;
  hasLocalAccount: boolean;
  lang: string;
  userId: string;
  userName: string;
  termsAccepted: boolean = false;

  private _activatedRouteSub$: Subscription;

  constructor(public _accountService: AccountService,
    public _router: Router,
    public _activatedRoute: ActivatedRoute,
    private _translateService: TranslateService,
    private _notificationService: NotificationService,
    private _location: Location,
    private _spinnerService: SpinnerService,
    @Inject(PLATFORM_ID) private _platformId: Object) {
    this.notificationRefresh = this._notificationService.getRefresh();
    this.isMobileDevice = isPlatformBrowser(this._platformId) ? window.screen.width <= CONFIG.mobile_width : false;
  }

  ngOnInit() {
    this._activatedRouteSub$ = this._activatedRoute.queryParams.subscribe(params => {
      let firstReg: string = params['first_reg'] != undefined ? params['first_reg'] : "";
      let extEmailLocallyReg: string = params['haslocalaccount'] != undefined ? params['haslocalaccount'] : "";
      this.externalToken = params['external_token'] != undefined ? params['external_token'] : "";
      this.provider = params['provider'] != undefined ? params['provider'] : "";

      if (extEmailLocallyReg.toLocaleLowerCase() === CONFIG._true) {
        this.loginInput.Email = params['email'];
        this.loginModal.show();
        this.hasLocalAccount = true;
        let basePath = this._location.path().slice(0, this._location.path().indexOf('?'));
        this._location.replaceState(basePath);
      } else if (firstReg.toLocaleLowerCase() === CONFIG._true) {
        this.handleExternalFirstAuth();
      }
      else if (firstReg.toLocaleLowerCase() === CONFIG._false) {
        this._accountService.obtainLocalAccessToken(this.provider, this.externalToken, CONFIG.client_id).subscribe(res => {
          let basePath = this._location.path().slice(0, this._location.path().indexOf('?'));
          this._location.replaceState(basePath);
          this.setUserNameAndId();
        });
      }

      this.lang = this._translateService.currentLang != undefined ? this._translateService.currentLang : 'en';

      if (params['lang'] != undefined) {
        this.lang = params['lang'];

        let basePath = this._location.path().slice(0, this._location.path().indexOf('?'));
        this._location.replaceState(basePath);
      }

      this.changeLanguage(this.lang);
      this.setUserNameAndId();
    });
  }

  ngOnDestroy() {
    this._activatedRouteSub$.unsubscribe();
  }

  goToForgotPasswordScreen() {
    this.loginInput = <LoginModel>{};
    this.errorMessage = "";
    this.forgotPasswordErrorMessage = "";
    this.forgotPasswordEmail = "";
    this.isForgotPassword = !this.isForgotPassword;
  }

  burgerMenuItemClick() {
    if (this.isMobileDevice) {
      this.isBurgerMenuCollapsed = !this.isBurgerMenuCollapsed;
    }
  }

  burgerIconClick() {
    this.isBurgerMenuCollapsed = !this.isBurgerMenuCollapsed;
  }

  setNotifNumber(event: any) {
    this.notificationsNumber = event;
  }

  changeLanguage(lang: string) {
    if (this.currentLang != undefined) {
      this.languages.push(this.currentLang);
    }
    let selectedLanguage = this.languages.findIndex(x => x.value == lang);
    this.currentLang = this.languages[selectedLanguage];
    this.languages.splice(selectedLanguage, 1);
    this.isLangDropdownCollapsed = true;
    this._translateService.use(lang);
  }

  authExternal(provider: string) {
    this._spinnerService.isLoading = true;
    var redirectUri = isPlatformBrowser(this._platformId) ? window.location : "";
    var externalAuthUrl = CONFIG.baseUrls.apiUrl + "account/ExternalLogin?provider=" + provider
      + "&response_type=token&client_id=" + CONFIG.client_id
      + "&redirect_uri=" + redirectUri;

    if (isPlatformBrowser(this._platformId)) {
      window.location.href = externalAuthUrl;
    }
  }

  updateExternalUser() {
    this._accountService.updateExternalUser(this.provider, this.externalToken, this.externalUserModel, CONFIG.client_id)
      .subscribe(res => {
        window.scrollTo(0, 0);
        this.setUserNameAndId();
        this.phoneNumberModal.hide();
      });
  }

  register() {
    var names = this.registerInput.FullName.split(" ");
    this.registerInput.FirstName = names[0];
    this.registerInput.LastName = names[1];
    this.registerInput.ConfirmPassword = this.registerInput.Password;

    this._accountService.registerService(this.registerInput)
      .subscribe(res => {
        this.signUpModal.hide();
        this.loginInput.Email = res.Email
        this.loginModal.show();
      },
      error => this.errorMessage = error);
  }

  onSignUpModalClose() {
    this.errorMessage = "";
    this.registerForm.resetForm();
  }

  onLogInModalClose() {
    this.errorMessage = "";
    this.isForgotPassword = false;
    this.logInForm.resetForm();

    if (this.hasLocalAccount) {
      this._router.navigate([this._location.path()]);
    }

    this.hasLocalAccount = false;
  }

  onPhoneNumberModalClose() {
    this.phoneNumberForm.resetForm();
  }

  login(): void {
    this._accountService.loginService(this.loginInput)
      .subscribe(response => {
        window.scrollTo(0, 0);
        this.loginModal.hide();
        this.setUserNameAndId();
      },
      error => this.errorMessage = error)
  }

  setUserNameAndId() {
    this.userId = this._accountService.getUserId();
    this.userName = this._accountService.getName();
  }

  getNewPassword() {
    this._accountService.forgotPassword(this.forgotPasswordEmail)
      .subscribe(res => {
        this.isForgotPassword = !this.isForgotPassword;
        window.scrollTo(0, 0);
        this.loginModal.hide();
        this.showNotification();
      },
      error => this.forgotPasswordErrorMessage = error)
  }

  skipPhoneNumberEntering() {
    this._accountService.obtainLocalAccessToken(this.provider, this.externalToken, CONFIG.client_id)
      .subscribe(res => {
        this.setUserNameAndId();
        window.scrollTo(0, 0);
        this.phoneNumberModal.hide();
      });
  }

  showNotification() {
    this.notificationRefresh++;
    this._notificationService.setRefreshAndText(this.notificationRefresh, this._notificationService.passwordRecoverySuccessText());
  }

  handleExternalFirstAuth() {
    this._spinnerService.isLoading = true;
    let basePath = this._location.path().slice(0, this._location.path().indexOf('?'));
    this._location.replaceState(basePath);

    if (isPlatformBrowser(this._platformId)) {
      setTimeout(() => {
        this._spinnerService.isLoading = false;
        this.phoneNumberModal.show();
      }, 0);
    }
  }

  onTermsCheckboxChange($event) {
    this.termsAccepted = $event.target.checked;
  }

  deleteExternalUser() {
    this._accountService.deleteExternalUser(this.provider, this.externalToken)
      .subscribe(res => {
        window.scrollTo(0, 0);
        this.phoneNumberModal.hide();
      },
      error => {
        window.scrollTo(0, 0);
        this.phoneNumberModal.hide();
      });
  }
}
