import { NgModule } from '@angular/core';
import { Http, Request, RequestOptionsArgs, Response, XHRBackend, RequestOptions, ConnectionBackend, Headers, HttpModule } from '@angular/http';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule, TranslateLoader, TranslatePipe } from "@ngx-translate/core";
import { TranslateHttpLoader } from "@ngx-translate/http-loader";
import { Angular2FontawesomeModule } from 'angular2-fontawesome';
import { ModalModule, TypeaheadModule, PaginationModule, BsDropdownModule, CollapseModule, CarouselModule } from 'ngx-bootstrap';
import { FileUploadModule } from 'ng2-file-upload';
import { NouisliderModule } from 'ng2-nouislider';
import { FacebookModule } from 'ngx-facebook';

//MODULES
import { TransferHttpModule } from '../../modules/transfer-http/transfer-http.module';

//COMPONENTS
import { SpinnerComponent } from '../shared/spinner/spinner.component';
import { HorseListComponent } from './horse-list/horse-list.component';
import { ErrorComponent } from './error/error.component';
import { NotificationComponent } from './notifications/notification.component';
import { RightSideAdvertismentsComponent } from './advertisments/right-side.advertisments.component';

//SERVICES
import { SpinnerService } from './spinner/spinner.service';
import { AuthService } from './auth/auth.service';
import { NotificationService } from './notifications/notification.service';
import { StorageService } from './auth/storage.service';
import { LinkService } from './link.service';
import { HttpWrapper } from './http/http.wrapper';

//PIPES
import { DateFormatPipe } from './pipes/dateFormat.pipe';
import { DateFormatHourPipe } from './pipes/dateFormatHour.pipe';
import { DescriptionFormatPipe } from './pipes/descriptionFormat.pipe';

//GUARDS
import { LoggedInGuard } from '../shared/guards/loggedInGuard';
import { AdminGuard } from '../shared/guards/adminGuard';
import { IsPostOwnerGuard } from '../shared/guards/isPostOwnerGuard';

//DIRECTIVES
import { ImagePreview } from './utils/image.preview.directive';
import { EqualValidator } from './utils/equal-validator.directive';

//CONSTANTS
import { ORIGIN_URL } from './constants/baseurl.constants';

export function httpFactory(xhrBackend: XHRBackend, requestOptions: RequestOptions, router: Router, spinnerService: SpinnerService, storageService: StorageService) {
    return new HttpWrapper(xhrBackend, requestOptions, router, spinnerService, storageService);
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
    ModalModule.forRoot(),
    TypeaheadModule.forRoot(),
    PaginationModule.forRoot(),
    BsDropdownModule.forRoot(),
    CollapseModule.forRoot(),
    FileUploadModule,
    TransferHttpModule,
    NouisliderModule,
    FacebookModule.forRoot(),
    CarouselModule.forRoot(),
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
    RightSideAdvertismentsComponent
  ],
  providers: [
    NotificationService, AuthService, LoggedInGuard, AdminGuard, IsPostOwnerGuard, SpinnerService, StorageService, TranslateModule,
    LinkService,
    {
      provide: HttpWrapper, useFactory: httpFactory,
      deps: [XHRBackend, RequestOptions, Router, SpinnerService, StorageService]
    },
  ],
  exports: [CommonModule, FormsModule, ReactiveFormsModule, NotificationComponent, ErrorComponent, SpinnerComponent, EqualValidator, ImagePreview,
    DateFormatPipe, DateFormatHourPipe, HttpModule, ModalModule, Angular2FontawesomeModule, TranslateModule, HorseListComponent, PaginationModule,
    TypeaheadModule, FileUploadModule, DescriptionFormatPipe, BsDropdownModule, CollapseModule, NouisliderModule, RightSideAdvertismentsComponent, FacebookModule, CarouselModule ]
})

export class SharedModule { }
