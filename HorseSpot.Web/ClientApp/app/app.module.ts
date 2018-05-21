import { NgModule } from '@angular/core';
import { Angulartics2Module } from 'angulartics2';
import { Angulartics2GoogleAnalytics } from 'angulartics2/ga';

//MODULES
import { SharedModule } from './shared/shared.module';
import { LayoutModule } from './layout/layout.module';
import { AccountModule } from './account/account.module';
import { HorseAdsModule } from './horse-advertisments/horse-ads.module';
import { AppRoutingModule } from './app-routing.module';

//COMPONENTS
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { ContactComponent } from './contact/contact.component';
import { AdvertiseComponent } from './advertise/advertise.component';
import { HireUsComponent } from './hire-us/hire-us.component';
import { TermsAndConditionsComponent } from './terms-and-conditions/terms-and-conditions.component';
import { CookiePageComponent } from './cookie-page/cookie-page.component';

//PROVIDERS
import { CookieService } from 'ngx-cookie-service';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    ContactComponent,
    AdvertiseComponent,
    HireUsComponent,
    TermsAndConditionsComponent,
    CookiePageComponent
  ],
  imports: [
    Angulartics2Module.forRoot([Angulartics2GoogleAnalytics]),
    AppRoutingModule,
    SharedModule,
    LayoutModule,
    AccountModule,
    HorseAdsModule,
  ],
  providers: [CookieService],
  bootstrap: [AppComponent]
})

export class AppModuleShared { }
