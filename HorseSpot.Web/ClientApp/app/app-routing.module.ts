import { NgModule } from '@angular/core';
import { Routes, RouterModule, PreloadAllModules } from '@angular/router';

//COMPONENTS
import { HomeComponent } from './home/home.component';
import { ErrorComponent } from './shared/error/error.component';
//import { AppointmentsListComponent } from './account/appointments/appointments-list/appointments-list.component';
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
import { AdvertiseComponent } from './advertise/advertise.component';
import { HireUsComponent } from './hire-us/hire-us.component';
import { TermsAndConditionsComponent } from './terms-and-conditions/terms-and-conditions.component';
import { CookiePageComponent } from './cookie-page/cookie-page.component';
import { PrivacyPageComponent } from './privacy-page/privacy-page.component';

//GUARDS
import { LoggedInGuard } from './shared/guards/loggedInGuard';
import { AdminGuard } from './shared/guards/adminGuard';
import { IsPostOwnerGuard } from './shared/guards/isPostOwnerGuard';
import { WishListGuard } from './shared/guards/wishlist.guard';

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
      title: 'Horses for sale | Buy & Sell',
      meta: [
        { name: 'description', content: 'Buying or selling horses? Horse Spot is an online horse market where you can search through multiple horses for sale or list yours for FREE. Whether you are looking for a show jumping horse, dressage horse, eventing, endurance, driving horse or just a leisure horse we have it all covered. Colts and foals also available on Horse Spot. Ready to list your horse for sale or to buy your future star?' },
        { property: 'og:title', content: 'Horses for sale | Buy & Sell | Horse Spot' },
        { property: 'og:description', content: 'Buying or selling horses? Horse Spot is an online horse market where you can search through multiple horses for sale or list yours for FREE. Whether you are looking for a show jumping horse, dressage horse, eventing, endurance, driving horse or just a leisure horse we have it all covered. Colts and foals also available on Horse Spot. Ready to list your horse for sale or to buy your future star?' },
        { name: 'twitter:card', content: 'summary_large_image' },
        { name: 'twitter:title', content: 'Horses for sale | Buy & Sell | Horse Spot' },
        { name: 'twitter:description', content: 'Buying or selling horses? Horse Spot is an online horse market where you can search through multiple horses for sale or list yours for FREE. Whether you are looking for a show jumping horse, dressage horse, eventing, endurance, driving horse or just a leisure horse we have it all covered. Colts and foals also available on Horse Spot. Ready to list your horse for sale or to buy your future star?' }
      ],
      links: [
        { rel: 'canonical', href: 'http://horse-spot.com/' },
        { rel: 'canonical', href: 'https://horse-spot.com/' },
        { rel: 'next', href: 'http://horse-spot.com/horses-for-sale/showjumping/1' }
      ]
    }
  },
  {
    path: 'horses-for-sale/showjumping/:page', component: HorseListCategoriesComponent,
    data: {
      title: 'Show Jumping | Horses For Sale',
      meta: [
        { name: 'description', content: 'Browse through thousands of quality show jumping horses for sale, from youngsters to grand prix horses, offered to you by trusted stables, owners, dealers or riders.' },
        { property: 'og:title', content: 'Show Jumping | Horses For Sale | Horse Spot' },
        { property: 'og:description', content: 'Browse through thousands of quality show jumping horses for sale, from youngsters to grand prix horses, offered to you by trusted stables, owners, dealers or riders.' },
        { name: 'twitter:card', content: 'summary_large_image' },
        { name: 'twitter:title', content: 'Show Jumping | Horses For Sale | Horse Spot' },
        { name: 'twitter:description', content: 'Browse through thousands of quality show jumping horses for sale, from youngsters to grand prix horses, offered to you by trusted stables, owners, dealers or riders.' },
      ]
    },
    resolve: { model: HorseListCategoriesResolver }
  },
  {
    path: 'horses-for-sale/dressage/:page', component: HorseListCategoriesComponent,
    data: {
      title: 'Dressage | Horses For Sale',
      meta: [
        { name: 'description', content: 'Browse through thousands of quality dressage horses for sale, from youngsters to grand prix horses, offered to you by trusted stables, owners, dealers or riders.' },
        { property: 'og:title', content: 'Dressage | Horses For Sale | Horse Spot' },
        { property: 'og:description', content: 'Browse through thousands of quality dressage horses for sale, from youngsters to grand prix horses, offered to you by trusted stables, owners, dealers or riders.' },
        { name: 'twitter:card', content: 'summary_large_image' },
        { name: 'twitter:title', content: 'Dressage | Horses For Sale | Horse Spot' },
        { name: 'twitter:description', content: 'Browse through thousands of quality dressage horses for sale, from youngsters to grand prix horses, offered to you by trusted stables, owners, dealers or riders.' },
      ]
    },
    resolve: { model: HorseListCategoriesResolver }
  },
  {
    path: 'horses-for-sale/eventing/:page', component: HorseListCategoriesComponent,
    data: {
      title: 'Eventing | Horses For Sale',
      meta: [
        { name: 'description', content: 'Browse through thousands of quality eventing horses for sale, from youngsters to experienced horses, offered to you by trusted stables, owners, dealers or riders.' },
        { property: 'og:title', content: 'Eventing | Horses For Sale | Horse Spot' },
        { property: 'og:description', content: 'Browse through thousands of quality eventing horses for sale, from youngsters to experienced horses, offered to you by trusted stables, owners, dealers or riders.' },
        { name: 'twitter:card', content: 'summary_large_image' },
        { name: 'twitter:title', content: 'Eventing | Horses For Sale | Horse Spot' },
        { name: 'twitter:description', content: 'Browse through thousands of quality eventing horses for sale, from youngsters to experienced horses, offered to you by trusted stables, owners, dealers or riders.' },
      ]
    },
    resolve: { model: HorseListCategoriesResolver }
  },
  {
    path: 'horses-for-sale/endurance/:page', component: HorseListCategoriesComponent,
    data: {
      title: 'Endurance | Horses For Sale',
      meta: [
        { name: 'description', content: 'Browse through thousands of quality endurance horses for sale, from youngsters to experienced horses, offered to you by trusted stables, owners, dealers or riders.' },
        { property: 'og:title', content: 'Endurance | Horses For Sale | Horse Spot' },
        { property: 'og:description', content: 'Browse through thousands of quality endurance horses for sale, from youngsters to experienced horses, offered to you by trusted stables, owners, dealers or riders.' },
        { name: 'twitter:card', content: 'summary_large_image' },
        { name: 'twitter:title', content: 'Endurance | Horses For Sale | Horse Spot' },
        { name: 'twitter:description', content: 'Browse through thousands of quality endurance horses for sale, from youngsters to experienced horses, offered to you by trusted stables, owners, dealers or riders.' },
      ]
    },
    resolve: { model: HorseListCategoriesResolver }
  },
  {
    path: 'horses-for-sale/driving/:page', component: HorseListCategoriesComponent,
    data: {
      title: 'Driving | Horses For Sale',
      meta: [
        { name: 'description', content: 'Browse through thousands of quality driving horses for sale, from youngsters to experienced horses, offered to you by trusted stables, owners, dealers or riders.' },
        { property: 'og:title', content: 'Driving | Horses For Sale | Horse Spot' },
        { property: 'og:description', content: 'Browse through thousands of quality driving horses for sale, from youngsters to experienced horses, offered to you by trusted stables, owners, dealers or riders.' },
        { name: 'twitter:card', content: 'summary_large_image' },
        { name: 'twitter:title', content: 'Driving | Horses For Sale | Horse Spot' },
        { name: 'twitter:description', content: 'Browse through thousands of quality driving horses for sale, from youngsters to experienced horses, offered to you by trusted stables, owners, dealers or riders.' },
      ]
    },
    resolve: { model: HorseListCategoriesResolver }
  },
  {
    path: 'horses-for-sale/foals/:page', component: HorseListCategoriesComponent,
    data: {
      title: 'Foals | Horses For Sale',
      meta: [
        { name: 'description', content: 'Browse through thousands of quality foals for sale, offered to you by trusted stables, owners, dealers or riders.' },
        { property: 'og:title', content: 'Foals | Horses For Sale | Horse Spot' },
        { property: 'og:description', content: 'Browse through thousands of quality foals for sale, offered to you by trusted stables, owners, dealers or riders.' },
        { name: 'twitter:card', content: 'summary_large_image' },
        { name: 'twitter:title', content: 'Foals | Horses For Sale | Horse Spot' },
        { name: 'twitter:description', content: 'Browse through thousands of quality foals for sale, offered to you by trusted stables, owners, dealers or riders.' },
      ]
    },
    resolve: { model: HorseListCategoriesResolver }
  },
  {
    path: 'horses-for-sale/leisure/:page', component: HorseListCategoriesComponent,
    data: {
      title: 'Leisure | Horses For Sale',
      meta: [
        { name: 'description', content: 'Browse through thousands of quality leisure horses for sale, from youngsters to experienced horses, offered to you by trusted stables, owners, dealers or riders.' },
        { property: 'og:title', content: 'Leisure | Horses For Sale | Horse Spot' },
        { property: 'og:description', content: 'Browse through thousands of quality leisure horses for sale, from youngsters to experienced horses, offered to you by trusted stables, owners, dealers or riders.' },
        { name: 'twitter:card', content: 'summary_large_image' },
        { name: 'twitter:title', content: 'Leisure | Horses For Sale | Horse Spot' },
        { name: 'twitter:description', content: 'Browse through thousands of quality leisure horses for sale, from youngsters to experienced horses, offered to you by trusted stables, owners, dealers or riders.' },
      ]
    },
    resolve: { model: HorseListCategoriesResolver }
  },
  {
    path: 'horses-for-sale/:id/:category/:title', component: HorseAdDetailComponent
  },
  {
    path: 'horses/add', component: AddHorseAdComponent,
    data: {
      title: 'Sell Your Horse',
      meta: [
        { name: 'description', content: 'Selling your horse on Horse Spot it easy and free, just fill in the information and get in touch with the clients.' },
        { property: 'og:title', content: 'Sell Your Horse | Horses For Sale | Horse Spot' },
        { property: 'og:description', content: 'Selling your horse on Horse Spot it easy and free, just fill in the information and get in touch with the clients.' },
        { name: 'twitter:card', content: 'summary_large_image' },
        { name: 'twitter:title', content: 'Sell Your Horse | Horses For Sale | Horse Spot' },
        { name: 'twitter:description', content: 'Selling your horse on Horse Spot it easy and free, just fill in the information and get in touch with the clients.' },
      ]
    },
    canActivate: [LoggedInGuard]
  },
  {
    path: 'account/profile/:userId', component: ProfileComponent,
    data: {
      meta: [
        { name: 'description', content: 'Quick access to a Horse Spot user information including his references and current horses for sale.' },
        { property: 'og:title', content: 'Profile | Horse Spot' },
        { property: 'og:description', content: 'Quick access to a Horse Spot user information including his references and current horses for sale.' },
        { name: 'twitter:card', content: 'summary_large_image' },
        { name: 'twitter:title', content: 'Profile | Horse Spot' },
        { name: 'twitter:description', content: 'Quick access to a Horse Spot user information including his references and current horses for sale.' },
      ]
    },
  },
  {
    path: 'horses/edit/:id', component: EditHorseAdComponent,
    data: {
      title: 'Edit',
      meta: [
        { name: 'robots', content: 'NOINDEX, NOFOLLOW' },
      ],
    },
    canActivate: [LoggedInGuard, IsPostOwnerGuard]
  },
  {
    path: 'contact', component: ContactComponent,
    data: {
      title: 'Contact',
      meta: [
        { name: 'description', content: 'Please feel free to write us about any thought, suggestion, partnership offer or complain you have about our web site.' },
        { property: 'og:title', content: 'Contact | Horse Spot' },
        { property: 'og:description', content: 'Please feel free to write us about any thought, suggestion, partnership offer or complain you have about our web site.' },
        { name: 'twitter:card', content: 'summary_large_image' },
        { name: 'twitter:title', content: 'Contact | Horse Spot' },
        { name: 'twitter:description', content: 'Please feel free to write us about any thought, suggestion, partnership offer or complain you have about our web site.' },
      ]
    },
  },
  {
    path: 'error/:statusCode', component: ErrorComponent,
    data: {
      title: 'Error',
      meta: [
        { name: 'robots', content: 'NOINDEX, NOFOLLOW' },
      ],
    },
  },
  {
    path: 'account/horses-for-sale/:userId/:page', component: UserPostsComponent,
    data: {
      meta: [
        { name: 'robots', content: 'NOINDEX, NOFOLLOW' },
      ],
    },
    resolve: { model: UserPostsResolver }
  },
  {
    path: 'account/sold-horses/:userId/:page', component: UserReferencesComponent,
    data: {
      meta: [
        { name: 'robots', content: 'NOINDEX, NOFOLLOW' },
      ],
    },
    resolve: { model: UserReferencesResolver }
  },
  {
    path: 'account/wishlist/:userId/:page', component: UserFavoritesComponent,
    data: {
      meta: [
        { name: 'robots', content: 'NOINDEX, NOFOLLOW' },
      ],
    },
    canActivate: [LoggedInGuard, WishListGuard],
    resolve: { model: UserFavoritesResolver }
  },
  {
    path: 'horses/unvalidated/:page', component: HorseListUnvalidatedComponent,
    data: {
      title: 'Unvalidated',
      meta: [
        { name: 'robots', content: 'NOINDEX, NOFOLLOW' },
      ],
    },
    canActivate: [LoggedInGuard, AdminGuard],
    resolve: { model: UnvalidatedHorseListResolver }
  },
  {
    path: 'advertise', component: AdvertiseComponent,
    data: {
      title: 'Advertise',
      meta: [
        { name: 'description', content: 'We offer you the posibility to advertise your business on Horse Spot and get the attention of thousands of visitors per month.' },
        { property: 'og:title', content: 'Advertise | Horse Spot' },
        { property: 'og:description', content: 'We offer you the posibility to advertise your business on Horse Spot and get the attention of thousands of visitors per month.' },
        { name: 'twitter:card', content: 'summary_large_image' },
        { name: 'twitter:title', content: 'Advertise | Horse Spot' },
        { name: 'twitter:description', content: 'We offer you the posibility to advertise your business on Horse Spot and get the attention of thousands of visitors per month.' },
      ]
    },
  },
  {
    path: 'hire-us', component: HireUsComponent,
    data: {
      title: 'Hire Us',
      meta: [
        { name: 'description', content: 'Grow your equine business to the next level through Horse Spot management and marketing solutions. We build together with you. We build something that represents you.' },
        { property: 'og:title', content: 'Hire Us | Horse Spot' },
        { property: 'og:description', content: 'Grow your equine business to the next level through Horse Spot management and marketing solutions. We build together with you. We build something that represents you.' },
        { name: 'twitter:card', content: 'summary_large_image' },
        { name: 'twitter:title', content: 'Hire Us | Horse Spot' },
        { name: 'twitter:description', content: 'Grow your equine business to the next level through Horse Spot management and marketing solutions. We build together with you. We build something that represents you.' },
      ]
    },
  },
  {
    path: 'terms-and-conditions', component: TermsAndConditionsComponent
  },
  {
    path: 'cookies-usage', component: CookiePageComponent
  },
  {
    path: 'privacy-policy', component: PrivacyPageComponent
  }

  //{ path: 'account/appointments', component: AppointmentsListComponent, canActivate: [ LoggedInGuard ] },
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
