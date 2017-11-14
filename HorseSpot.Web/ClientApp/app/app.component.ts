import { Component, OnInit, OnDestroy, ViewEncapsulation } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Subscription } from 'rxjs/Subscription';
import { Meta, Title, DOCUMENT, MetaDefinition } from '@angular/platform-browser';

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

  private routerSub$: Subscription;

  constructor(public _accountService: AccountService,
    //public _appointmentsService: AppointmentsService,
    public _router: Router,
    private _activatedRoute: ActivatedRoute,
    public _notificationService: NotificationService,
    public _authService: AuthService,
    public _translateService: TranslateService,
    private title: Title,
    private meta: Meta,
    private linkService: LinkService,
    @Inject(PLATFORM_ID) private platformId: Object) { }

  ngOnInit() {
    this._changeTitleOnNavigation();

    this._translateService.setDefaultLang('en');
    this._translateService.use('en');

    if (this._authService.isRefreshTokenExpired()) {
      this._authService.removeUserStoredInfo();
    }

    //let userId = this._accountService.getUserId();       
    //if (userId != null) {
    //     this._appointmentsService.webSocketConnect(userId);        
    //}

    if (isPlatformBrowser(this.platformId)) {
      this.isBrowser = true;
      let browserLang = this._translateService.getBrowserLang();
      this._translateService.use(browserLang.match(/en/) ? browserLang : 'en');

      this._router.events.subscribe((event) => {
        setTimeout(function () {
          window.scrollTo(0, 1);
        }, 0);
      });
    }
  }

  ngOnDestroy() {
    this.routerSub$.unsubscribe();
  }

  private _changeTitleOnNavigation() {

    this.routerSub$ = this._router.events
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
      });
  }

  private _setMetaAndLinks(event) {
    const title = event['title']
      ? `${event['title']} | ${this.endPageTitle}`
      : `${this.defaultPageTitle} | ${this.endPageTitle}`;

    this.title.setTitle(title);

    const metaData = event['meta'] || [];
    const linksData = event['links'] || [];

    for (let i = 0; i < metaData.length; i++) {
      this.meta.updateTag(metaData[i]);
    }

    for (let i = 0; i < linksData.length; i++) {
      this.linkService.addTag(linksData[i]);
    }
  }
}
