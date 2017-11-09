import { Component, Input, HostListener, ViewChild, ElementRef, OnInit, TemplateRef } from '@angular/core';
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { NgForm } from '@angular/forms';
import { BsModalService } from 'ngx-bootstrap/modal';
import { BsModalRef } from 'ngx-bootstrap/modal';

//SERVICES
import { NotificationService } from '../../shared/notifications/notification.service';
import { AccountService } from '../../account/account.service';

//MODELS
import { RegisterModel } from '../../account/models/register.model';
import { RegisterExternalModel } from '../../account/models/registerExternalModel';
import { LoginModel } from '../../account/models/login.model';

import { CONFIG } from '../../config';

@Component({
  selector: 'navbar',
  templateUrl: './navbar.component.html'
})
export class NavbarComponent implements OnInit {
  //@ViewChild('phoneNumberModal') public phoneNumberModal: Modal;
  //@ViewChild('signUpModal') public signUpModal: Modal;
  //@ViewChild('loginModal') public loginModal: Modal;
  signUpModal: BsModalRef;
  @ViewChild('registerForm') public registerForm: NgForm;
  @ViewChild('logInForm') public logInForm: NgForm;
  @ViewChild('phoneNumberForm') public phoneNumberForm: NgForm;
  isBurgerMenuCollapsed: boolean = true;
  isLangDropdownCollapsed: boolean = true;
  notificationsNumber: number = 0;
  isMobileDevice: boolean;
  languages = CONFIG.languages;
  currentLang: any = new Object({ imgUrl: "", value: "", displayText: "" });
  provider: string;
  externalToken: string;
  externalUserPhoneNumber: string;
  loginInput: LoginModel = <LoginModel>{};
  registerInput: RegisterModel = <RegisterModel>{};
  errorMessage: string = "";
  isForgotPassword: boolean = false;
  forgotPasswordEmail: string;
  forgotPasswordErrorMessage: string = "";
  notificationRefresh: number;
  hasLocalAccount: boolean;
  isOnHome: boolean = false;
  config = {
    animated: true,
    keyboard: true,
    backdrop: true,
    ignoreBackdropClick: false
  }

  constructor(public _accountService: AccountService,
    public _router: Router,
    public _activatedRoute: ActivatedRoute,
    private _translateService: TranslateService,
    private _notificationService: NotificationService,
    private _modalService: BsModalService,
    @Inject(PLATFORM_ID) private platformId: Object) {
    this.notificationRefresh = this._notificationService.getRefresh();
    this.isMobileDevice = isPlatformBrowser(this.platformId) ? window.screen.width <= 767 : false;
  }

  ngOnInit() {
    this._activatedRoute.queryParams.subscribe(params => {
      let firstReg = params['first_reg'];
      let extEmailLocallyReg = params['haslocalaccount'];
      this.externalToken = params['external_token'];
      this.provider = params['provider'];

      if (extEmailLocallyReg == "True") {
        this.loginInput.Email = params['email'];
        //        this.loginModal.open();
        this.hasLocalAccount = true;
      } else if (firstReg == "True") {
        //         this.phoneNumberModal.open();
      }
      else if (firstReg == "False") {
        this._accountService.obtainLocalAccessToken(this.provider, this.externalToken).subscribe(res => {
          this._router.navigate(['/home']);
        });
      }
    });

    if (this._router.isActive('/home', true)) {
      this.isOnHome = true;
    }

    let currentLangIndex = this.languages.findIndex(x => x.value == this._translateService.currentLang);
    this.currentLang = this.languages[currentLangIndex];
    this.languages.splice(currentLangIndex, 1);
  }

  openSignUpModal(template: TemplateRef<any>) {
    this.signUpModal = this._modalService.show(template);
    Object.assign({}, this.config, { class: 'navbar-modals' })
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

  toggle() {
    if (!this.isBurgerMenuCollapsed) {
      this.isBurgerMenuCollapsed = true;
    }
  }

  setNotifNumber(event: any) {
    this.notificationsNumber = event;
  }

  changeLanguage(lang: string) {
    this.languages.push(this.currentLang);
    let selectedLanguage = this.languages.findIndex(x => x.value == lang);
    this.currentLang = this.languages[selectedLanguage];
    this.languages.splice(selectedLanguage, 1);
    this.isLangDropdownCollapsed = true;
    this._translateService.use(lang);
  }

  authExternal(provider: string) {
    var redirectUri = isPlatformBrowser(this.platformId) ? window.location : "";
    var externalAuthUrl = CONFIG.baseUrls.apiUrl + "account/ExternalLogin?provider=" + provider
      + "&response_type=token&client_id=" + CONFIG.client_id
      + "&redirect_uri=" + redirectUri;

    if (isPlatformBrowser(this.platformId)) {
      window.location.href = externalAuthUrl;
    }
  }

  updateExternalUserPhoneNumber() {
    this._accountService.updateExternalUser(this.provider, this.externalToken, this.externalUserPhoneNumber).subscribe(res => {
      //this.phoneNumberModal.close();
      this._router.navigate(['/home']);
    })
  }

  register() {
    var names = this.registerInput.FullName.split(" ");
    this.registerInput.FirstName = names[0];
    this.registerInput.LastName = names[1];
    this.registerInput.ConfirmPassword = this.registerInput.Password;

    this._accountService.registerService(this.registerInput)
      .subscribe(res => { //this.signUpModal.close(); 
        //this.loginModal.open(); },
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
      this._router.navigate(['/home']);
    }

    this.hasLocalAccount = false;
  }

  onPhoneNumberModalClose() {
    this.phoneNumberForm.resetForm();
  }

  login(): void {
    this._accountService.loginService(this.loginInput)
      .subscribe(response => response,//this.loginModal.close(),
      error => this.errorMessage = error)
  }

  getNewPassword() {
    this._accountService.forgotPassword(this.forgotPasswordEmail)
      .subscribe(res => {
        this.isForgotPassword = !this.isForgotPassword;
        //this.loginModal.close();
        this.showNotification();
      },
      error => this.forgotPasswordErrorMessage = error)
  }

  skipPhoneNumberEntering() {
    this._accountService.obtainLocalAccessToken(this.provider, this.externalToken).subscribe(res => {
      //this.phoneNumberModal.close();
      this._router.navigate(['/home']);
    });
  }

  showNotification() {
    this.notificationRefresh++;
    this._notificationService.setRefreshAndText(this.notificationRefresh, this._notificationService.passwordRecoverySuccessText());
  }
}
