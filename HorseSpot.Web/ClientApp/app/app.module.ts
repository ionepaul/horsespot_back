import { NgModule } from '@angular/core';

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
        HorseAdsModule
    ],
    providers: [],
    bootstrap: [AppComponent]
})

export class AppModuleShared { }
