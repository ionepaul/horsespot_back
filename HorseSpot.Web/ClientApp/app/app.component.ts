import { Component, OnInit, OnDestroy, ViewEncapsulation } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser, Location } from '@angular/common';
import { Subscription } from 'rxjs/Subscription';
import { Meta, Title, DOCUMENT, MetaDefinition } from '@angular/platform-browser';
import { FacebookService, InitParams } from 'ngx-facebook';
import { Angulartics2GoogleAnalytics } from 'angulartics2/ga';
import { CONFIG } from './config';

//SERVICES
import { NotificationService } from './shared/notifications/notification.service';
import { AccountService } from './account/account.service';
import { AuthService } from './shared/auth/auth.service';
//import { AppointmentsService } from './account/appointments/appointments.service';
import { LinkService } from './shared/link.service';

@Component({
    selector: 'app',
    template: `
        <app-notification *ngIf="isBrowser" [refresh]="_notificationService.getRefresh()" [text]="_notificationService.getText()"></app-notification>
        <spinner></spinner>
        <navbar></navbar>      
        <router-outlet></router-outlet>
        <horse-spot-footer></horse-spot-footer>
    `,
    styleUrls: [
        '../../wwwroot/css/bootstrap.min.css',
        '../../wwwroot/css/font-awesome.min.css',
        '../../wwwroot/css/styles.css',
        '../../wwwroot/css/nouislider.min.css'
    ],
    encapsulation: ViewEncapsulation.None
})
export class AppComponent implements OnInit, OnDestroy {
    isBrowser: boolean = false;

    private endPageTitle: string = 'Horse Spot';
    // If no Title is provided, we'll use a default one before the dash(-)
    private defaultPageTitle: string = 'Horses For Sale';

    private _routerSub$: Subscription;

    constructor(public _accountService: AccountService,
        //public _appointmentsService: AppointmentsService,
        public _router: Router,
        private _activatedRoute: ActivatedRoute,
        public _notificationService: NotificationService,
        public _authService: AuthService,
        public _translateService: TranslateService,
        private _title: Title,
        private _meta: Meta,
        private _linkService: LinkService,
        private _location: Location,
        private _facebookService: FacebookService,
        angulartics2GoogleAnalytics: Angulartics2GoogleAnalytics,
        @Inject(PLATFORM_ID) private platformId: Object) { }

    ngOnInit() {
        this._changeTitleOnNavigation();

        if (this._authService.isRefreshTokenExpired()) {
            this._authService.removeUserStoredInfo();
        }

        //let userId = this._accountService.getUserId();       
        //if (userId != null) {
        //     this._appointmentsService.webSocketConnect(userId);        
        //}
        
        this._translateService.setDefaultLang('en');
        this._translateService.use('en');
        
        if (isPlatformBrowser(this.platformId)) {
            this._initFacebookSdkConnection();
            this.isBrowser = true;
        }
    }

    ngOnDestroy() {
        this._routerSub$.unsubscribe();
    }

    private _changeTitleOnNavigation() {
        this._routerSub$ = this._router.events
            .filter(event => event instanceof NavigationEnd)
            .map(() => this._activatedRoute)
            .map(route => {

                while (route.firstChild) route = route.firstChild;

                return route;
            })
            .filter(route => route.outlet === 'primary')
            .mergeMap(route => route.data)
            .subscribe((event) => {
                this._setMetaAndLinks(event);
                window.scrollTo(0, 0);
            });
    }

    private _setMetaAndLinks(event) {
        const title = event['title']
            ? `${event['title']} | ${this.endPageTitle}`
            : `${this.defaultPageTitle} | ${this.endPageTitle}`;

        this._title.setTitle(title);

        const metaData = event['meta'] || [];
        const linksData = event['links'] || [];

        for (let i = 0; i < metaData.length; i++) {
            this._meta.updateTag(metaData[i]);
        }

        for (let i = 0; i < linksData.length; i++) {
            this._linkService.addTag(linksData[i]);
        }
    }

    private _initFacebookSdkConnection() {
        let fbInitParams: InitParams = {
            appId: CONFIG.fbAppId,
            xfbml: true,
            version: CONFIG.fbSdkVersion
        };

        this._facebookService.init(fbInitParams);
    }
}
