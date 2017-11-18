import { Component, Input, HostListener, ViewChild, ElementRef, OnInit, TemplateRef, OnDestroy, OnChanges } from '@angular/core';
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

//MODELS
import { RegisterModel } from '../../account/models/register.model';
import { RegisterExternalModel } from '../../account/models/registerExternalModel';
import { LoginModel } from '../../account/models/login.model';

import { CONFIG } from '../../config';

@Component({
    selector: 'navbar',
    templateUrl: './navbar.component.html'
})
export class NavbarComponent implements OnInit, OnDestroy, OnChanges {
    @Input() langCode: string;

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
    errorMessage: string;
    isForgotPassword: boolean = false;
    forgotPasswordEmail: string;
    forgotPasswordErrorMessage: string;
    notificationRefresh: number;
    hasLocalAccount: boolean;

    private _activatedRouteSub$: Subscription;

    constructor(public _accountService: AccountService,
        public _router: Router,
        public _activatedRoute: ActivatedRoute,
        private _translateService: TranslateService,
        private _notificationService: NotificationService,
        private _location: Location,
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
            } else if (firstReg.toLocaleLowerCase() === CONFIG._true) {
                this.phoneNumberModal.show();
            }
            else if (firstReg.toLocaleLowerCase() === CONFIG._false) {
              this._accountService.obtainLocalAccessToken(this.provider, this.externalToken).subscribe(res => {
                    let basePath = this._location.path().slice(0, this._location.path().indexOf('?'));
                    this._location.replaceState(basePath);
                });
            }
        });
    }

    ngOnDestroy() {
        this._activatedRouteSub$.unsubscribe();
    }

    ngOnChanges() {
        let currentLangIndex = this.languages.findIndex(x => x.value == this.langCode);
        this.currentLang = this.languages[currentLangIndex];

        if (this.currentLang != undefined) {
            if (this.languages.length != CONFIG.languages.length) {
                this.languages.push(this.currentLang);
            }

            this.languages.splice(currentLangIndex, 1);
        }
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
        var redirectUri = isPlatformBrowser(this._platformId) ? window.location : "";
        var externalAuthUrl = CONFIG.baseUrls.apiUrl + "account/ExternalLogin?provider=" + provider
            + "&response_type=token&client_id=" + CONFIG.client_id
            + "&redirect_uri=" + redirectUri;

        if (isPlatformBrowser(this._platformId)) {
            window.location.href = externalAuthUrl;
        }
    }

    updateExternalUserPhoneNumber() {
        this._accountService.updateExternalUser(this.provider, this.externalToken, this.externalUserPhoneNumber)
            .subscribe(res => {
                this.phoneNumberModal.hide();
                let basePath = this._location.path().slice(0, this._location.path().indexOf('?'));
                this._location.replaceState(basePath);
            })
    }

    register() {
        var names = this.registerInput.FullName.split(" ");
        this.registerInput.FirstName = names[0];
        this.registerInput.LastName = names[1];
        this.registerInput.ConfirmPassword = this.registerInput.Password;

        this._accountService.registerService(this.registerInput)
            .subscribe(res => {
                this.signUpModal.hide();
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
                this.loginModal.hide();
            },
            error => this.errorMessage = error)
    }

    getNewPassword() {
        this._accountService.forgotPassword(this.forgotPasswordEmail)
            .subscribe(res => {
                this.isForgotPassword = !this.isForgotPassword;
                this.loginModal.hide();
                this.showNotification();
            },
            error => this.forgotPasswordErrorMessage = error)
    }

    skipPhoneNumberEntering() {
        this._accountService.obtainLocalAccessToken(this.provider, this.externalToken)
            .subscribe(res => {
                this.phoneNumberModal.hide();
                let basePath = this._location.path().slice(0, this._location.path().indexOf('?'));
                this._location.replaceState(basePath);
            });
    }

    showNotification() {
        this.notificationRefresh++;
        this._notificationService.setRefreshAndText(this.notificationRefresh, this._notificationService.passwordRecoverySuccessText());
    }
}
