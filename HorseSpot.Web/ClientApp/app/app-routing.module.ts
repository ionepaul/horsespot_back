import { NgModule } from '@angular/core';
import { Routes, RouterModule, PreloadAllModules } from '@angular/router';

//COMPONENTS
import { HomeComponent } from './home/home.component';
import { ErrorComponent } from './shared/error/error.component';
import { AppointmentsListComponent } from './account/appointments/appointments-list/appointments-list.component';
import { ProfileComponent } from './account/profile/profile.component';
import { UserPostsComponent } from './account/user-posts/user-posts.component';
import { UserFavoritesComponent } from './account/user-favorites/user-favorites.component';
import { UserReferencesComponent } from './account/user-references/user-references.component';
import { AddHorseAdComponent } from './horse-advertisments/add/add-horse-ad.component';
import { EditHorseAdComponent } from './horse-advertisments/edit/edit-horse-ad.component';
import { HorseListCategoriesComponent } from './horse-advertisments/horse-list-categories/horse-list-categories.component';
import { HorseListUnvalidatedComponent } from './horse-advertisments/horse-list-unvalidated/horse-list-unvalidated.component';
import { ContactComponent } from './contact/contact.component';
import { HorseAdDetailComponent } from './horse-advertisments/horse-ad-detail/horse-ad-detail.component';

//GUARDS
import { LoggedInGuard } from './shared/guards/loggedInGuard';
import { AdminGuard } from './shared/guards/adminGuard';
import { IsPostOwnerGuard } from './shared/guards/isPostOwnerGuard';

// ROUTE RESOLVERS
import { UserPostsResolver } from './account/user-posts/user-posts.route-resolver';
import { UserFavoritesResolver } from './account/user-favorites/user-favorites.route-resolver';
import { UserReferencesResolver } from './account/user-references/user-references.route-resolver';
import { HorseListCategoriesResolver } from './horse-advertisments/horse-list-categories/horse-list-categories.route-resolver';
import { UnvalidatedHorseListResolver } from './horse-advertisments/horse-list-unvalidated/horse-list-unvalidated.route-resolver';


const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  {
      path: 'home', component: HomeComponent,
      data: {
          title: 'Home',
          meta: [
              { name: 'description', content: 'Search horses for sale from all over the world through an online platform build with empathy and respect for horses, owners and riders.' },
              { property: 'og:title', content: 'Home | Horse Spot' },
              { property: 'og:description', content: 'Search horses for sale from all over the world through an online platform build with empathy and respect for horses, owners and riders.' },
              { name: 'twitter:card', content: 'summary_large_image' },
              { name: 'twitter:title', content: 'Home | Horse Spot' },
              { name: 'twitter:description', content: 'Search horses for sale from all over the world through an online platform build with empathy and respect for horses, owners and riders.' }
          ],
          links: [
              { rel: 'canonical', href: 'http://horse-spot.com/' },
              { rel: 'canonical', href: 'https://horse-spot.com/' },
              //{ rel: 'alternate', hreflang: 'es', href: 'http://es.example.com/' }
          ]
      }
  },
  {
      path: 'horses-for-sale/showjumping/:page', component: HorseListCategoriesComponent,
      data: {
          title: 'Show Jumping | Horses For Sale',
          meta: [
              { name: 'description', content: 'Browse through thousands of quality show jumping horses for sale, offered to you by multiple stables, owners, dealers or riders.' },
              { property: 'og:title', content: 'Show Jumping | Horses For Sale' },
              { property: 'og:description', content: 'Browse through thousands of quality show jumping horses for sale, offered to you by multiple stables, owners, dealers or riders.' },
              { name: 'twitter:card', content: 'summary_large_image' },
              { name: 'twitter:title', content: 'Show Jumping | Horses For Sale' },
              { name: 'twitter:description', content: 'Browse through thousands of quality show jumping horses for sale, offered to you by multiple stables, owners, dealers or riders.' },
          ],
          links: [
              { rel: 'canonical', href: 'http://horse-spot.com/horses-for-sale/showjumping/1' },
              { rel: 'next', href: 'https://horse-spot.com/horses-for-sale/showjumping/2' },
              //{ rel: 'alternate', hreflang: 'es', href: 'http://es.example.com/' }
          ]
      },
      resolve: { model: HorseListCategoriesResolver }
  },
  {
      path: 'horses-for-sale/dressage/:page', component: HorseListCategoriesComponent,
      data: {
          title: 'Dressage | Horses For Sale',
          meta: [
              { name: 'description', content: 'Browse through thousands of quality dressage horses for sale, offered to you by multiple stables, owners, dealers or riders.' },
              { property: 'og:title', content: 'Dressage | Horses For Sale' },
              { property: 'og:description', content: 'Browse through thousands of quality dressage horses for sale, offered to you by multiple stables, owners, dealers or riders.' },
              { name: 'twitter:card', content: 'summary_large_image' },
              { name: 'twitter:title', content: 'Dressage | Horses For Sale' },
              { name: 'twitter:description', content: 'Browse through thousands of quality dressage horses for sale, offered to you by multiple stables, owners, dealers or riders.' },
          ],
          links: [
              //{ rel: 'canonical', href: 'http://horse-spot.com/horses-for-sale/dressage/1' },
              //{ rel: 'next', href: 'https://horse-spot.com/horses-for-sale/dressage/2' },
              //{ rel: 'alternate', hreflang: 'es', href: 'http://es.example.com/' }
          ]
      },
      resolve: { model: HorseListCategoriesResolver }
  },
  {
      path: 'horses-for-sale/eventing/:page', component: HorseListCategoriesComponent,
      data: {
          title: 'Eventing | Horses For Sale',
          meta: [
              { name: 'description', content: 'Browse through thousands of quality eventing horses for sale, offered to you by multiple stables, owners, dealers or riders.' },
              { property: 'og:title', content: 'Eventing | Horses For Sale' },
              { property: 'og:description', content: 'Browse through thousands of quality eventing horses for sale, offered to you by multiple stables, owners, dealers or riders.' },
              { name: 'twitter:card', content: 'summary_large_image' },
              { name: 'twitter:title', content: 'Eventing | Horses For Sale' },
              { name: 'twitter:description', content: 'Browse through thousands of quality eventing horses for sale, offered to you by multiple stables, owners, dealers or riders.' },
          ],
          links: [
              //{ rel: 'canonical', href: 'http://horse-spot.com/horses-for-sale/eventing/1' },
              //{ rel: 'next', href: 'https://horse-spot.com/horses-for-sale/eventing/' },
              //{ rel: 'alternate', hreflang: 'es', href: 'http://es.example.com/' }
          ]
      },
      resolve: { model: HorseListCategoriesResolver }
  },
  {
      path: 'horses-for-sale/endurance/:page', component: HorseListCategoriesComponent,
      data: {
          title: 'Endurance | Horses For Sale',
          meta: [
              { name: 'description', content: 'Browse through thousands of quality endurance horses for sale, offered to you by multiple stables, owners, dealers or riders.' },
              { property: 'og:title', content: 'Endurance | Horses For Sale' },
              { property: 'og:description', content: 'Browse through thousands of quality endurance horses for sale, offered to you by multiple stables, owners, dealers or riders.' },
              { name: 'twitter:card', content: 'summary_large_image' },
              { name: 'twitter:title', content: 'Endurance | Horses For Sale' },
              { name: 'twitter:description', content: 'Browse through thousands of quality endurance horses for sale, offered to you by multiple stables, owners, dealers or riders.' },
          ],
          links: [
              //{ rel: 'canonical', href: 'http://horse-spot.com/horses-for-sale/eventing/1' },
              //{ rel: 'next', href: 'https://horse-spot.com/horses-for-sale/eventing/' },
              //{ rel: 'alternate', hreflang: 'es', href: 'http://es.example.com/' }
          ]
      },
      resolve: { model: HorseListCategoriesResolver }
  },
  {
      path: 'horses-for-sale/driving/:page', component: HorseListCategoriesComponent,
      data: {
          title: 'Driving | Horses For Sale',
          meta: [
              { name: 'description', content: 'Browse through thousands of quality driving horses for sale, offered to you by multiple stables, owners, dealers or riders.' },
              { property: 'og:title', content: 'Driving | Horses For Sale' },
              { property: 'og:description', content: 'Browse through thousands of quality driving horses for sale, offered to you by multiple stables, owners, dealers or riders.' },
              { name: 'twitter:card', content: 'summary_large_image' },
              { name: 'twitter:title', content: 'Driving | Horses For Sale' },
              { name: 'twitter:description', content: 'Browse through thousands of quality driving horses for sale, offered to you by multiple stables, owners, dealers or riders.' },
          ],
          links: [
              //{ rel: 'canonical', href: 'http://horse-spot.com/horses-for-sale/eventing/1' },
              //{ rel: 'next', href: 'https://horse-spot.com/horses-for-sale/eventing/' },
              //{ rel: 'alternate', hreflang: 'es', href: 'http://es.example.com/' }
          ]
      },
      resolve: { model: HorseListCategoriesResolver }
  },
  {
      path: 'horses-for-sale/foals/:page', component: HorseListCategoriesComponent,
      data: {
          title: 'Foals | Horses For Sale',
          meta: [
              { name: 'description', content: 'Browse through thousands of quality foals horses for sale, offered to you by multiple stables, owners, dealers or riders.' },
              { property: 'og:title', content: 'Foals | Horses For Sale' },
              { property: 'og:description', content: 'Browse through thousands of quality foals horses for sale, offered to you by multiple stables, owners, dealers or riders.' },
              { name: 'twitter:card', content: 'summary_large_image' },
              { name: 'twitter:title', content: 'Foals | Horses For Sale' },
              { name: 'twitter:description', content: 'Browse through thousands of quality foals horses for sale, offered to you by multiple stables, owners, dealers or riders.' },
          ],
          links: [
              //{ rel: 'canonical', href: 'http://horse-spot.com/horses-for-sale/eventing/1' },
              //{ rel: 'next', href: 'https://horse-spot.com/horses-for-sale/eventing/' },
              //{ rel: 'alternate', hreflang: 'es', href: 'http://es.example.com/' }
          ]
      },
      resolve: { model: HorseListCategoriesResolver }
  },
  {
      path: 'horses-for-sale/leisure/:page', component: HorseListCategoriesComponent,
      data: {
          title: 'Leisure | Horses For Sale',
          meta: [
              { name: 'description', content: 'Browse through thousands of quality leisure horses for sale, offered to you by multiple stables, owners, dealers or riders.' },
              { property: 'og:title', content: 'Leisure | Horses For Sale' },
              { property: 'og:description', content: 'Browse through thousands of quality leisure horses for sale, offered to you by multiple stables, owners, dealers or riders.' },
              { name: 'twitter:card', content: 'summary_large_image' },
              { name: 'twitter:title', content: 'Leisure | Horses For Sale' },
              { name: 'twitter:description', content: 'Browse through thousands of quality leisure horses for sale, offered to you by multiple stables, owners, dealers or riders.' },
          ],
          links: [
              //{ rel: 'canonical', href: 'http://horse-spot.com/horses-for-sale/eventing/1' },
              //{ rel: 'next', href: 'https://horse-spot.com/horses-for-sale/eventing/' },
              //{ rel: 'alternate', hreflang: 'es', href: 'http://es.example.com/' }
          ]
      },
      resolve: { model: HorseListCategoriesResolver }
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
