import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { SharedModule } from '../shared/shared.module';

import { AccountService } from './account.service';
import { AppointmentsService } from './appointments/appointments.service';
import { WebSocketService } from './appointments/websocket.service';

import { UserPostsResolver } from './user-posts/user-posts.route-resolver';
import { UserFavoritesResolver } from './user-favorites/user-favorites.route-resolver';
import { UserReferencesResolver } from './user-references/user-references.route-resolver';

import { ProfileComponent } from './profile/profile.component';
import { UserPostsComponent } from './user-posts/user-posts.component';
import { UserFavoritesComponent } from './user-favorites/user-favorites.component';
import { UserReferencesComponent } from './user-references/user-references.component';
//import { AppointmentsListComponent } from './appointments/appointments-list/appointments-list.component';
import { LoggedInGuard } from '../shared/guards/loggedInGuard';

@NgModule({
    imports: [
        SharedModule,
        RouterModule
    ],
    declarations: [ 
        ProfileComponent,
        UserPostsComponent,
        UserFavoritesComponent,
        //AppointmentsListComponent,
        UserReferencesComponent
    ],
    providers: [AccountService, WebSocketService, UserPostsResolver, UserFavoritesResolver, UserReferencesResolver]
                //AppointmentsService, 
})

export class AccountModule { }
