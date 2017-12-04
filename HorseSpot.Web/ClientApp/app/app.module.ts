import { NgModule } from '@angular/core';
import { AdsenseModule } from 'ng2-adsense';
import { Angulartics2Module, Angulartics2GoogleAnalytics } from 'angulartics2';

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

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    ContactComponent,
    AdvertiseComponent,
    HireUsComponent
  ],
  imports: [
    AppRoutingModule,
    SharedModule,
    LayoutModule,
    AccountModule,
    HorseAdsModule,
    Angulartics2Module.forRoot([Angulartics2GoogleAnalytics])
    //AdsenseModule.forRoot({
    //  adClient: 'ca-pub-4911156518333270"',
    //  adSlot: 7259870550
    //})
  ],
  providers: [],
  bootstrap: [AppComponent]
})

export class AppModuleShared { }
