import { Component, OnInit, OnDestroy, ViewChild, HostListener, ElementRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common'
import { Subscription } from 'rxjs/Subscription';
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { DOCUMENT } from '@angular/platform-browser';
import { CONFIG } from '../../config';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { Meta, Title } from '@angular/platform-browser';

//SERVICES
import { AccountService } from '../../account/account.service';
import { HorseAdsService } from '../horse-ads.service';
//import { AppointmentsService } from '../../account/appointments/appointments.service';
import { NotificationService } from '../../shared/notifications/notification.service';

//MODELS
import { HorseAdModel } from '../models/horseAdModel';
import { AddressModel } from '../models/addressModel';
import { HorseAbilityModel } from '../models/horseAbilityModel';
import { RecommendedRiderModel } from '../models/recommendedRiderModel';
import { PriceRangeModel } from '../models/priceRAngeModel';
import { EmailModel } from '../models/emailModel';
import { UserModel } from '../../account/models/user.model';
import { AppointmentModel } from '../../account/models/appointmentModel';
import { PedigreeModel } from '../models/pedigree';
import { AppointmentStatus } from '../../shared/constants/appointment.status';

declare var google: any;
declare var window: any;

@Component({
    templateUrl: './horse-ad-detail.component.html'
})

export class HorseAdDetailComponent implements OnInit, OnDestroy {
    @ViewChild('sendEmailModal') public sendEmailModal: ModalDirective;
    @ViewChild('deleteModal') public deleteModal: ModalDirective;
    @ViewChild('video') public video: ElementRef;
    @ViewChild('horseVideoFrame') public horseVideoFrame: ElementRef;
    @ViewChild('map') public map: ElementRef;
    @ViewChild('userImage') public userImage: ElementRef;

    googleMapError: boolean = false;
    imagesUrl: string = CONFIG.horseAdsImagesUrl;
    horseAdModel: HorseAdModel = <HorseAdModel>{};
    horseAddress: AddressModel = <AddressModel>{};
    pedigree: PedigreeModel = <PedigreeModel>{};
    priceRange: PriceRangeModel = <PriceRangeModel>{};
    userModel: UserModel = <UserModel>{};
    emailModel: EmailModel = <EmailModel>{};
    errorMessage: string;
    isAdmin: boolean;
    currentUserId: string;
    videoFrame: HTMLIFrameElement;
    isFavorite: boolean = false;
    showPedigree: boolean = false;
    notificationRefresh: number;
    firstImage: string = '';
    horseImages: string[] = [];

    private _routeSub$: Subscription;

    constructor(public location: Location,
        private _route: ActivatedRoute,
        private _router: Router,
        private _horseAdService: HorseAdsService,
        private _accountService: AccountService,
        //private _appointmentsService: AppointmentsService,
        private _notificationService: NotificationService,
        @Inject(DOCUMENT) private document,
        @Inject(PLATFORM_ID) private platformId: Object,
        private _metaData: Meta,
        private _title: Title)
    { }

    ngOnInit() {
        this.horseAdModel.PriceRange = <PriceRangeModel>{};
        this.horseAdModel.Address = <AddressModel>{};
        this.notificationRefresh = this._notificationService.getRefresh();
        this.currentUserId = this._accountService.getUserId();

        if (this.currentUserId) {
            this._accountService.isAdmin(this.currentUserId).subscribe(res => this.isAdmin = res);
        }

        this._routeSub$ = this._route.params.subscribe(
            params => {
                let id = params['id'];
                this.getHorseAd(id);
            });

        window.FB.XFBML.parse();
    }

    getHorseAd(id: number) {
        this._horseAdService.getHorseAdDetails(id)
            .subscribe(res => {
                this.horseAdModel = res;
                this.initImages();
                this.initializeMetadata();
                this.horseAddress = res.Address;
                this.pedigree = res.Pedigree;
                this.priceRange = res.PriceRange;
                this.checkPermissions();
                this.setVideoFrame();
                this.initMap();
                this.checkPedigree();
                this.setHorseAdOwnerDetails();
            },
            error => this.errorMessage = error);
    }

    initializeMetadata() {
        this._title.setTitle(this.horseAdModel.Title + ' | Horse Spot');

        this._metaData.addTags([
            { name: 'description', content: this.horseAdModel.Description },
            { property: 'og:title', content: this.horseAdModel.Title + ' | Horse Spot' },
            { property: 'og:description', content: this.horseAdModel.Description },
            { property: 'og:image', content: CONFIG.imagesUrl + this.horseAdModel.Images[0] },
            { name: 'twitter:card', content: "summary_large_image" },
            { name: 'twitter:title', content: this.horseAdModel.Title + ' | Horse Spot' },
            { name: 'twitter:description', content: this.horseAdModel.Description },
            { name: 'twitter:image', content: CONFIG.imagesUrl + this.horseAdModel.Images[0] }
        ]);
    }

    initImages() {
      this.horseAdModel.Images.forEach(img => {
        console.log(img);
        if (!img.IsProfilePic) {
          this.horseImages.push(img.ImageName);
        }
        else {
          this.firstImage = img.ImageName
        }
      });
    }

    validateHorseAd(id: number) {
        this._horseAdService.validateHorseAd(id)
            .subscribe(res => this._router.navigate(['/horses/unvalidated/1']),
            error => this.errorMessage = error);
    }

    addHorseAdToFavorites(id: number) {
        this._horseAdService.addHorseAdToFavorites(id)
            .subscribe(res => this.isFavorite = !this.isFavorite,
            error => this.errorMessage = error);
    }

    checkPermissions() {
        if (this.currentUserId != this.horseAdModel.UserId) {
            this._horseAdService.increaseViews(this.horseAdModel.Id).subscribe();
        }

        if (!this.horseAdModel.IsValidated && !this.isAdmin && this.horseAdModel.UserId != this.currentUserId) {
            this._router.navigate(['/error/403']);
        }
    }

    checkPedigree() {
        if (JSON.stringify(this.pedigree) !== JSON.stringify(new PedigreeModel())) {
            this.showPedigree = true;
        } else {
            this.showPedigree = false;
        }
    }

    setVideoFrame() {
        if (this.horseAdModel.VideoLink) {
            setTimeout(() => {
                this.horseVideoFrame.nativeElement.src = this.horseAdModel.VideoLink;
            }, 0);
        } else {
            setTimeout(() => {
                this.video.nativeElement.style.display = 'none';
            }, 0);
        }
    }

    initMap() {
        var map = new google.maps.Map(this.map.nativeElement, {
            zoom: 12,
            center: { lat: 46.770439, lng: 23.591423 }
        });

        var horseAdAddress = this.horseAddress.Country + ", " + this.horseAddress.City + ", " + this.horseAddress.Street;
        var geocoder = new google.maps.Geocoder();
        geocoder.geocode({ 'address': horseAdAddress }, function (results: any, status: any) {
            if (status === 'OK') {
                map.setCenter(results[0].geometry.location);
                var marker = new google.maps.Marker({
                    map: map,
                    position: results[0].geometry.location
                });
            } else {
                this.googleMapError = true;
            }
        })
    }

    setHorseAdOwnerDetails() {
        this._accountService.getUserDetails(this.horseAdModel.UserId.toString())
            .subscribe(res => {
                this.userModel = res;
                this.setUserImage();
                this.setIfFavorite();
            },
            error => this.errorMessage = error);
    }

    setUserImage() {
        setTimeout(() => {
            this.userImage.nativeElement.src = this._accountService._profilePhotoGetUrl + this.userModel.ImagePath;
        }, 0);
    }

    setIfFavorite() {
        if (this.horseAdModel.FavoritesFor.indexOf(this.currentUserId) > -1) {
            this.isFavorite = true;
        }
    }

    goBack(): void {
        this.location.back();
    }

    sendEmailToUser() {
        this.emailModel.Receiver = this.userModel.Email;
        this.emailModel.ReceiverFirstName = this.userModel.FirstName
        this.emailModel.HorseAdTitle = this.horseAdModel.Title;

        this._horseAdService.sendEmail(this.emailModel)
            .subscribe(res => {
                this.sendEmailModal.hide();
                this.showNotification(this._notificationService.sendEmailSuccesText());
            },
            error => this.errorMessage = error);
    }

    showSendEmailModal(): void {
        if (this.currentUserId) {
            this._accountService.getUserDetails(this.currentUserId)
                .subscribe(res => {
                    this.emailModel.Sender = res.Email;
                    this.emailModel.SenderName = res.FirstName + " " + res.LastName;
                },
                error => this.errorMessage = error);
        }

        this.sendEmailModal.show();
    }

    redirectToEdit() {
        this._router.navigate(['/horses/edit/', this.horseAdModel.Id]);
    }

    viewProfile() {
        this._router.navigate(['/account/profile/', this.horseAdModel.UserId]);
    }

    deleteHorseAd(isSold: boolean) {
        this._horseAdService.deleteHorseAd(this.horseAdModel.Id, isSold)
            .subscribe(res => {
                this.showNotification(this._notificationService.deleteHorseSuccessText());
                this.deleteModal.hide();
                this._router.navigate(['/account/profile/', this.currentUserId]);
            },
            error => this.errorMessage = error);
    }

    showNotification(text: string) {
        this.notificationRefresh++;
        this._notificationService.setRefreshAndText(this.notificationRefresh, text);
    }

    scrollInto(anchor: string) {
        setTimeout(() => {
            if (isPlatformBrowser(this.platformId)) {
                (<HTMLScriptElement>this.document.querySelector('#' + anchor)).scrollIntoView({ behavior: 'smooth' });
            }
        }, 0);
    }

    ngOnDestroy() {
        this._routeSub$.unsubscribe();
        this._metaData.removeTag("name='description'");
        this._metaData.removeTag("property='og:title'");
        this._metaData.removeTag("property='og:description'");
        this._metaData.removeTag("property='og:image'");
        this._metaData.removeTag("name='twitter:card'");
        this._metaData.removeTag("name='twitter:description'");
        this._metaData.removeTag("name='twitter:card'");
        this._metaData.removeTag("name='twitter:image'");
    }

    //dateChanged() {
    //    this.dateChosedIsPast = this.appointmentDate.getTime() < new Date().getTime();
    //}

    //hideAppointmentModal() {
    //    this.makeAppointmentModal.close();
    //}

    //hideNotLoggedInAppointmentModal() {
    //    this.makeAppointmentNotLoggedInModal.close();
    //}

    //showAppointmentModal() {
    //    if (this.currentUserId == undefined) {
    //        this.makeAppointmentNotLoggedInModal.open();
    //    } else {
    //        this.makeAppointmentModal.open();
    //    }
    //}

    //makeAppointment() {
    //    this.appoinmentModel.Id = 0;
    //    //this.appoinmentModel.AdvertismentId = this.horseAdModel.Id;
    //    this.appoinmentModel.AdvertismentTitle = this.horseAdModel.Title;
    //    this.appointmentDate.setTime(this.appointmentDate.getTime() - this.appointmentDate.getTimezoneOffset() * 60 * 1000);
    //    this.appoinmentModel.AppointmentDateTime = this.appointmentDate;
    //    this.appoinmentModel.AdvertismentOwnerId = this.horseAdModel.UserId;
    //    this.appoinmentModel.InitiatorId = this._accountService.getUserId();
    //    this.appoinmentModel.IsAccepted = false;
    //    this.appoinmentModel.AdvertismentTitle = this.horseAdModel.Title;
    //    this.appoinmentModel.STATUS = AppointmentStatus.CREATED;
    //    this._appointmentsService.appointments.next(this.appoinmentModel);
    //    this.makeAppointmentModal.close();
    //    this.showNotification(this._notificationService.makeAppSuccessText());
    //}
}
