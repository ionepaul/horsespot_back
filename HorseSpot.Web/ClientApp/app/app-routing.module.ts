import { NgModule } from '@angular/core';
import { Routes, RouterModule, PreloadAllModules } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ErrorComponent } from './shared/error/error.component';
import { AppointmentsListComponent } from './account/appointments/appointments-list/appointments-list.component';
import { LoggedInGuard } from './shared/guards/loggedInGuard';
import { ProfileComponent } from './account/profile/profile.component';
import { UserPostsComponent } from './account/user-posts/user-posts.component';
import { UserFavoritesComponent } from './account/user-favorites/user-favorites.component';
import { UserReferencesComponent } from './account/user-references/user-references.component';
import { UserPostsResolver } from './account/user-posts/user-posts.route-resolver';
import { UserFavoritesResolver } from './account/user-favorites/user-favorites.route-resolver';
import { UserReferencesResolver } from './account/user-references/user-references.route-resolver';
import { AddHorseAdComponent } from './horse-advertisments/add/add-horse-ad.component';
import { EditHorseAdComponent } from './horse-advertisments/edit/edit-horse-ad.component';
import { AdminGuard } from './shared/guards/adminGuard';
import { IsPostOwnerGuard } from './shared/guards/isPostOwnerGuard';
import { HorseListCategoriesResolver } from './horse-advertisments/horse-list-categories/horse-list-categories.route-resolver';
import { HorseListCategoriesComponent } from './horse-advertisments/horse-list-categories/horse-list-categories.component';
import { UnvalidatedHorseListResolver } from './horse-advertisments/horse-list-unvalidated/horse-list-unvalidated.route-resolver';
import { HorseListUnvalidatedComponent } from './horse-advertisments/horse-list-unvalidated/horse-list-unvalidated.component';
import { ContactComponent } from './contact/contact.component';
import { HorseAdDetailComponent } from './horse-advertisments/horse-ad-detail/horse-ad-detail.component';

const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  {
      path: 'home', component: HomeComponent,
      data: {
          title: 'Homepage',
          meta: [
              { name: 'description', content: 'This is an example Description Meta tag!' }
          ],
          links: [
              { rel: 'canonical', href: 'http://horse-spot.com/' },
              { rel: 'canonical', href: 'https://horse-spot.com/' },
              { rel: 'alternate', hreflang: 'es', href: 'http://es.example.com/' }
          ]
      }
  },
  //{ path: 'contact', component: ContactComponent },
  //{ path: 'error/:statusCode', component: ErrorComponent },
  //{ path: 'account/appointments', component: AppointmentsListComponent, canActivate: [ LoggedInGuard ] },
  //{ path: 'account/profile/:userId', component: ProfileComponent },
  //{ path: 'account/horses-for-sale/:userId/:page', component: UserPostsComponent, resolve: { model: UserPostsResolver } },
  //{ path: 'account/sold-horses/:userId/:page', component: UserReferencesComponent, resolve: { model: UserReferencesResolver } },
  //{ path: 'account/wishlist/:page', component: UserFavoritesComponent, canActivate: [ LoggedInGuard ],resolve: { model: UserFavoritesResolver } },
  //{ path: 'horses/add', component: AddHorseAdComponent, canActivate: [ LoggedInGuard ] },
  //{ path: 'horses/edit/:id', component: EditHorseAdComponent, canActivate: [ LoggedInGuard, IsPostOwnerGuard ] },
  {
      path: 'horses-for-sale/showjumping/:page', component: HorseListCategoriesComponent,
      data: {
          title: 'Homepage',
          meta: [
              { name: 'description', content: 'This is an example Description Meta tag!' }
          ],
          links: [
              { rel: 'canonical', href: 'http://horse-spot.com/' },
              { rel: 'canonical', href: 'https://horse-spot.com/' },
              { rel: 'alternate', hreflang: 'es', href: 'http://es.example.com/' }
          ]
      },
      resolve: { model: HorseListCategoriesResolver }
  },
  //{ path: 'horses-for-sale/dressage/:page', component: HorseListCategoriesComponent, resolve: { model: HorseListCategoriesResolver } },
  //{ path: 'horses-for-sale/eventing/:page', component: HorseListCategoriesComponent, resolve: { model: HorseListCategoriesResolver } },
  //{ path: 'horses-for-sale/endurance/:page', component: HorseListCategoriesComponent, resolve: { model: HorseListCategoriesResolver } },
  //{ path: 'horses-for-sale/driving/:page', component: HorseListCategoriesComponent, resolve: { model: HorseListCategoriesResolver } },
  //{ path: 'horses-for-sale/leisure/:page', component: HorseListCategoriesComponent, resolve: { model: HorseListCategoriesResolver } }, 
  //{ path: 'horses/unvalidated/:page', component: HorseListUnvalidatedComponent, canActivate: [ LoggedInGuard, AdminGuard ], resolve: { model: UnvalidatedHorseListResolver } },
  //{ path: 'horses-for-sale/:id/:category/:title', component: HorseAdDetailComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    useHash: false,
    preloadingStrategy: PreloadAllModules,
    initialNavigation: 'enabled'
  })],
  exports: [RouterModule]
})

export class AppRoutingModule { }
