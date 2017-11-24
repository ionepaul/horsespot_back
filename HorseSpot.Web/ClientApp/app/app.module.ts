import { NgModule } from '@angular/core';
import { AdsenseModule } from 'ng2-adsense';

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

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    ContactComponent
  ],
  imports: [
    AppRoutingModule,
    SharedModule,
    LayoutModule,
    AccountModule,
    HorseAdsModule,
    //AdsenseModule.forRoot({
    //  adClient: 'ca-pub-4911156518333270"',
    //  adSlot: 7259870550
    //})
  ],
  providers: [],
  bootstrap: [AppComponent]
})

export class AppModuleShared { }
