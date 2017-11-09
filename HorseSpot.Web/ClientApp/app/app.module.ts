import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { ContactComponent } from './contact/contact.component';

import { SharedModule } from './shared/shared.module';
import { LayoutModule } from './layout/layout.module';
import { AccountModule } from './account/account.module';
import { HorseAdsModule } from './horse-advertisments/horse-ads.module';
import { TransferHttpModule } from '../modules/transfer-http/transfer-http.module';

@NgModule({
    declarations: [
        AppComponent,
        HomeComponent,
        ContactComponent
    ],
    imports: [
        //BrowserModule.withServerTransition({appId: 'horse-spot'}),
        AppRoutingModule,
        SharedModule,
        LayoutModule,
        AccountModule,
        HorseAdsModule,
        TransferHttpModule
    ],
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModuleShared { }
