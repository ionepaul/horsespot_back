import { Component, OnInit, OnDestroy } from '@angular/core';
import { Location } from '@angular/common';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { CONFIG } from '../../config';

//MODELS
import { HorseAdListModel } from '../../horse-advertisments/models/horseAdListModel';
import { UserModel } from '../models/user.model';

//SERVICES
import { AccountService } from '../account.service';

@Component({
  templateUrl: './user-references.component.html'
})

export class UserReferencesComponent implements OnInit, OnDestroy {
  pageNumber: number = 1;
  userId: string = "";
  totalNumber: number;
  userReferences: HorseAdListModel[];
  userModel: UserModel = <UserModel>{};
  errorMessage: string;
  profileImageUrl: string = CONFIG.profileImagesUrl;

  private routerSub$: Subscription;

  constructor(private _route: ActivatedRoute,
    private _router: Router,
    private _accountService: AccountService,
    private _location: Location) {

    this.routerSub$ = this._router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.userId = _route.snapshot.url[2].path;
        this.pageNumber = parseInt(_route.snapshot.url[3].path);
        this.totalNumber = this._route.snapshot.data['model'].TotalCount;
        this.userReferences = this._route.snapshot.data['model'].HorseAdList;
      }
    });
  }

  ngOnInit() {
    this.totalNumber = this._route.snapshot.data['model'].TotalCount;
    this.userReferences = this._route.snapshot.data['model'].HorseAdList;

    this._accountService.getUserDetails(this.userId)
      .subscribe(res => {
        this.userModel = res;
        this.setProfilePicture(res.ImagePath);
      },
      error => this.errorMessage = error);
  }

  back() {
    this._location.back();
  }

  pageChanged(event: any) {
    this.pageNumber = event.page;
    this._router.navigate(['/account/sold-horses', this.userId, event.page]);
  }

  setProfilePicture(profilePicture) {
    if (profilePicture.indexOf('http') >= 0) {
      this.profileImageUrl = profilePicture;
    }
    else {
      this.profileImageUrl += profilePicture;
    }
  }

  ngOnDestroy() {
    this.routerSub$.unsubscribe();
  }
}
