import { NgModule } from '@angular/core';
import { Http, Request, RequestOptionsArgs, Response, XHRBackend, RequestOptions, ConnectionBackend, Headers, HttpModule } from '@angular/http';
import { HttpInterceptor } from './http/customHttp';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { ModalModule } from 'ngx-modal';
import { Angular2FontawesomeModule } from 'angular2-fontawesome';
import { BsDropdownModule, CollapseModule, DatepickerModule, TimepickerModule, TypeaheadModule, PaginationModule } from 'ngx-bootstrap';
import { SpinnerComponent } from '../shared/spinner/spinner.component';
import { HorseListComponent } from './horse-list/horse-list.component';
import { ErrorComponent } from './error/error.component';
import { NotificationComponent } from './notifications/notification.component';
import { DateFormatPipe } from './pipes/dateFormat.pipe';
import { DateFormatHourPipe } from './pipes/dateFormatHour.pipe';
import { SpinnerService } from './spinner/spinner.service';
import { AuthService } from './auth/auth.service';
import { NotificationService } from './notifications/notification.service';
import { FileUploadModule } from 'ng2-file-upload';
import { LoggedInGuard } from '../shared/guards/loggedInGuard';
import { AdminGuard } from '../shared/guards/adminGuard';
import { IsPostOwnerGuard } from '../shared/guards/isPostOwnerGuard';
import { ImagePreview } from './utils/image.preview.directive';
import { EqualValidator } from './utils/equal-validator.directive';
import { FooterComponent } from '../layout/footer/footer.component';
import { AccountMenuComponent } from './account-menu/account-menu';
import { StorageService } from './auth/storage.service';
import { DescriptionFormatPipe } from './pipes/descriptionFormat.pipe';
import { LinkService } from './link.service';
import { TranslateModule, TranslateLoader, TranslatePipe } from "@ngx-translate/core";
import { TranslateHttpLoader } from "@ngx-translate/http-loader";

import { ORIGIN_URL } from './constants/baseurl.constants';

export function httpFactory(xhrBackend: XHRBackend, requestOptions: RequestOptions, router: Router, spinnerService: SpinnerService, storageService: StorageService) {
  return new HttpInterceptor(xhrBackend, requestOptions, router, spinnerService, storageService);
}

export function createTranslateLoader(http: Http, baseHref) {
  // Temporary Azure hack
  if (baseHref === null && typeof window !== 'undefined') {
    baseHref = window.location.origin;
  }
  // i18n files are in `wwwroot/assets/`
  return new TranslateHttpLoader(http, `${baseHref}/assets/internationalization/`, '.json');
}

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    HttpModule,
    Angular2FontawesomeModule,
    BsDropdownModule.forRoot(),
    CollapseModule.forRoot(),
    ModalModule,
    TypeaheadModule.forRoot(),
    PaginationModule.forRoot(),
    DatepickerModule.forRoot(),
    TimepickerModule.forRoot(),
    FileUploadModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: (createTranslateLoader),
        deps: [Http, [ORIGIN_URL]]
      }
    })
  ],
  declarations: [
    SpinnerComponent,
    ImagePreview,
    EqualValidator,
    HorseListComponent,
    ErrorComponent,
    DateFormatPipe,
    DateFormatHourPipe,
    DescriptionFormatPipe,
    NotificationComponent,
    ErrorComponent,
    AccountMenuComponent
  ],
  providers: [
    NotificationService, AuthService, LoggedInGuard, AdminGuard, IsPostOwnerGuard, SpinnerService, StorageService, TranslateModule,
    LinkService
    //{
    //  provide: Http, useFactory: httpFactory,
    //  deps: [XHRBackend, RequestOptions, Router, SpinnerService, StorageService]
    //},
  ],
  exports: [CommonModule, FormsModule, ReactiveFormsModule, NotificationComponent, ErrorComponent, SpinnerComponent, EqualValidator, ImagePreview,
    DateFormatPipe, DateFormatHourPipe, HttpModule, BsDropdownModule, CollapseModule,
    ModalModule, Angular2FontawesomeModule, DatepickerModule, TimepickerModule, TranslateModule, HorseListComponent, PaginationModule,
    TypeaheadModule, AccountMenuComponent, FileUploadModule, DescriptionFormatPipe]
})

export class SharedModule { }
