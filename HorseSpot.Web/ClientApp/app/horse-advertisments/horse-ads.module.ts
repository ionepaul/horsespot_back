import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { RouterModule } from '@angular/router';

import { HorseAdsService } from './horse-ads.service';
import { HorseListCategoriesResolver } from './horse-list-categories/horse-list-categories.route-resolver';
import { UnvalidatedHorseListResolver } from './horse-list-unvalidated/horse-list-unvalidated.route-resolver';

import { AddHorseAdComponent } from './add/add-horse-ad.component';
import { HorseListCategoriesComponent } from './horse-list-categories/horse-list-categories.component';
import { HorseListUnvalidatedComponent } from './horse-list-unvalidated/horse-list-unvalidated.component';
import { HorseAdDetailComponent } from './horse-ad-detail/horse-ad-detail.component';
import { EditHorseAdComponent } from './edit/edit-horse-ad.component';

import { LoggedInGuard } from '../shared/guards/loggedInGuard';
import { AdminGuard } from '../shared/guards/adminGuard';
import { IsPostOwnerGuard } from '../shared/guards/isPostOwnerGuard';

@NgModule({
    imports: [
        SharedModule,
        RouterModule
    ],
    declarations: [
        AddHorseAdComponent,
        HorseListCategoriesComponent,
        HorseListUnvalidatedComponent,
        HorseAdDetailComponent,
        EditHorseAdComponent
    ],
    providers: [ HorseAdsService, HorseListCategoriesResolver, UnvalidatedHorseListResolver ]
})

export class HorseAdsModule { }