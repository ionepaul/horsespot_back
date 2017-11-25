import { Component, OnInit, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import { NgForm } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { DOCUMENT, Title } from '@angular/platform-browser';
import { ModalDirective } from 'ngx-bootstrap/modal';

//SERVICES
import { AccountService } from '../account.service';
import { NotificationService } from '../../shared/notifications/notification.service';

//MODELS
import { ChangePasswordModel } from '../models/changePassword.model';
import { UserFullProfile } from '../models/userFullProfile.model';
import { UserModel } from '../models/user.model';
import { CONFIG } from '../../config';
import { EmailModel } from '../../horse-advertisments/models/emailModel';
import { HorseAdsService } from '../../horse-advertisments/horse-ads.service';

@Component({
  templateUrl: './profile.component.html'
})

export class ProfileComponent implements OnInit {
  @ViewChild('changePasswordModal') public changePasswordModal: ModalDirective;
  @ViewChild('sendEmailModal') public sendEmailModal: ModalDirective;

  @ViewChild('changePasswordForm') public changePasswordForm: NgForm;
  @ViewChild('sendEmailForm') public sendEmailForm: NgForm;

  userModel: UserFullProfile = <UserFullProfile>{};
  editModel: UserModel = <UserModel>{};
  changePasswordModel: ChangePasswordModel = <ChangePasswordModel>{};
  editErrorMessage: string;
  changePasswordErrorMessage: string;
  uploadImageErrorMessage: string;
  userId: string;
  profilePhotoUrl: string = CONFIG.profileImagesUrl;
  validUpload: boolean = true;
  notificationRefresh: number;
  profilePhoto: File;
  isEditMode: boolean = false;
  currentUserId: string;
  errorMessage: string;
  emailModel: EmailModel = <EmailModel>{};
  profileNameError: boolean = false;

  constructor(private _router: Router,
    private _activatedRoute: ActivatedRoute,
    private _accountService: AccountService,
    private _notificationService: NotificationService,
    private _location: Location,
    private _horseAdService: HorseAdsService,
    private _titleService: Title,
    @Inject(DOCUMENT) private document,
    @Inject(PLATFORM_ID) private platformId: Object) { }

  ngOnInit() {
    this.currentUserId = this._accountService.getUserId();
    this._activatedRoute.params.subscribe(params => {
      this.userId = params['userId'];
    });

    this.notificationRefresh = this._notificationService.getRefresh();

    this._accountService.getUserFullProfile(this.userId)
      .subscribe(response => {
        this._titleService.setTitle(`${response.FullName} | Horse Spot`);
        this.userModel = response;
        this.setProfilePicture(response.ImagePath);
      },
      error => this.editErrorMessage = error);
  }

  setProfilePicture(profilePicture) {
    if (profilePicture.indexOf('http') >= 0) {
      this.profilePhotoUrl = profilePicture;
    }
    else {
      this.profilePhotoUrl += profilePicture;
    }
  }

  back() {
    this._location.back();
  }

  saveChanges() {
    this.editModel.PhoneNumber = this.userModel.PhoneNumber;
    this.editModel.Email = this.userModel.Email;
    let names = this.userModel.FullName.split(" ");

    if (this.userModel.FullName == "") {
      this.profileNameError = true;
    } else {
      this.editModel.FirstName = names[0];
      this.editModel.LastName = names[1] != undefined ? names[1] : "";

      this._accountService.editUserDetails(this.userId, this.editModel)
        .subscribe(response => {
          this.isEditMode = false;
          this.editErrorMessage = "";
          this.notificationRefresh++;
          this.profileNameError = false;
          this._notificationService.setRefreshAndText(this.notificationRefresh, this._notificationService.profileChangesSuccessText());
        },
        error => this.editErrorMessage = error);
    }
  }

  changePassword() {
    this._accountService.changePassword(this.userId, this.changePasswordModel)
      .subscribe(response => {
        this.changePasswordModal.hide();
        this.notificationRefresh++;
        this._notificationService.setRefreshAndText(this.notificationRefresh, this._notificationService.changePasswordSuccessText());
      },
      error => this.changePasswordErrorMessage = error);
  }

  uploadProfilePicture() {
    if (this.profilePhoto != null) {
      this._accountService.uploadProfilePhoto(this.profilePhoto, this.userId)
        .subscribe(result => {
          this.profilePhotoUrl = "";
          this.profilePhotoUrl = CONFIG.profileImagesUrl;
          this.setProfilePicture(result);
          this.refresh();
        },
        error => this.uploadImageErrorMessage = error);
    }
  }

  refresh() {
    if (isPlatformBrowser(this.platformId)) {
      window.location.reload();
    }
  }

  onFileInputEventChange(fileInput: any) {
    if (fileInput.target.files.length > 0) {
      var files = fileInput.target.files;

      this.profilePhoto = files[0];
      let extensionStart = this.profilePhoto.name.lastIndexOf(".");
      let extension = this.profilePhoto.name.substr(extensionStart);

      if (CONFIG.accepted_file_extension.indexOf(extension.toUpperCase()) == -1) {
        this.validUpload = false;
      } else {
        this.validUpload = true;
        this.uploadProfilePicture();
      }
    }
  }

  onChangePasswordModalClose() {
    this.changePasswordErrorMessage = "";
    this.changePasswordForm.resetForm();
  }

  onSendEmailModalClose() {
    this.errorMessage = "";
    this.sendEmailForm.resetForm();
  }

  sendEmailToUser() {
    this.emailModel.Receiver = this.userModel.Email;
    this.emailModel.ReceiverFirstName = this.userModel.FullName;

    this._horseAdService.sendEmail(this.emailModel)
      .subscribe(res => {
        this.sendEmailModal.hide();
        this.notificationRefresh++;
        this._notificationService.setRefreshAndText(this.notificationRefresh, this._notificationService.sendEmailSuccesText())
      },
      error => this.errorMessage = error);
  }

  showSendEmailModal() {
    if (this._accountService.isLoggedIn()) {
      let currentUserId = this._accountService.getUserId();

      this._accountService.getUserDetails(currentUserId)
        .subscribe(res => {
          this.emailModel.Sender = res.Email;
          this.emailModel.SenderName = res.FirstName
        },
        error => this.errorMessage = error);
    }

    this.sendEmailModal.show();
  }

  scrollInto(anchor: string) {
    if (isPlatformBrowser(this.platformId)) {
      setTimeout(() => {
        (<HTMLScriptElement>this.document.querySelector('#' + anchor)).scrollIntoView({ behavior: 'smooth' });
      }, 0);
    }
  }

  goToUserHorsesForSale() {
    this._router.navigate(['/account/horses-for-sale/', this.userId, 1]);
  }

  goToUserHorseReferences() {
    this._router.navigate(['/account/sold-horses/', this.userId, 1]);
  }

  goToUserWishList() {
    this._router.navigate(['account/wishlist/', this.userId, 1]);
  }
}
