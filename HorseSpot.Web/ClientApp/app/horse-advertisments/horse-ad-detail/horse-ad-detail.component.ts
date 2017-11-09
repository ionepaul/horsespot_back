import { Component, OnInit, OnDestroy, ViewChild, HostListener, ElementRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common'
import { Subscription } from 'rxjs/Subscription';
import { Modal } from 'ngx-modal';
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

import { AccountService } from '../../account/account.service';
import { HorseAdsService } from '../horse-ads.service';
import { AppointmentsService } from '../../account/appointments/appointments.service';
import { NotificationService } from '../../shared/notifications/notification.service';

import { HorseAdModel } from '../models/horseAdModel';
import { AddressModel } from '../models/addressModel';
import { HorseAbilityModel } from '../models/horseAbilityModel';
import { RecommendedRiderModel } from '../models/recommendedRiderModel';
import { GenderModel } from '../models/genderModel';
import { PriceRangeModel } from '../models/priceRAngeModel';
import { EmailModel } from '../models/emailModel';
import { UserModel } from '../../account/models/user.model';
import { AppointmentModel } from '../../account/models/appointmentModel';
import { PedigreeModel } from '../models/pedigree';
import { AppointmentStatus } from '../../shared/constants/appointment.status';

import { CONFIG } from '../../config';
import { DOCUMENT } from '@angular/platform-browser';
declare var google: any;

@Component({ 
    templateUrl: './horse-ad-detail.component.html'
})

export class HorseAdDetailComponent implements OnInit, OnDestroy { 
    @ViewChild('sendEmailModal') public sendEmailModal: Modal;
    @ViewChild('deleteModal') public deleteModal: Modal;
    @ViewChild('makeAppointmentModal') public makeAppointmentModal: Modal;
    @ViewChild('makeAppointmentNotLoggedInModal') public makeAppointmentNotLoggedInModal: Modal;
    @ViewChild('navbarElement') public navbarElement: ElementRef;
    @ViewChild('video') public video: ElementRef;
    @ViewChild('horseVideoFrame') public horseVideoFrame: ElementRef;
    @ViewChild('map') public map: ElementRef;
    @ViewChild('userImage') public userImage: ElementRef;
    
    googleMapError: boolean = false;
    imagesUrl: string = CONFIG.imagesUrl + '/Images/HorseAdsImg/';
    horseAdModel: HorseAdModel = <HorseAdModel> {};
    horseAddress: AddressModel = <AddressModel> {};
    pedigree: PedigreeModel = <PedigreeModel> {};
    abilities: Array<HorseAbilityModel> = new Array<HorseAbilityModel>();
    recommendedRiders: Array<RecommendedRiderModel> = new Array<RecommendedRiderModel>();
    gender: string;
    priceRange: PriceRangeModel = <PriceRangeModel> {};
    userModel: UserModel = <UserModel> {};
    emailModel: EmailModel = <EmailModel> {};
    errorMessage: string;
    isAdmin: boolean;
    currentUserId: string;
    videoFrame: HTMLIFrameElement;
    appoinmentModel: AppointmentModel = <AppointmentModel> { };
    appointmentDate: Date = new Date();
    dateChosedIsPast: boolean = false;
    isFavorite: boolean = false;
    showPedigree: boolean = false;
    notificationRefresh: number;
    galleriImages: any[] = [];
    private _sub: Subscription;

    constructor(private _route: ActivatedRoute,
                private _router: Router,
                private _horseAdService: HorseAdsService,
                private _accountService: AccountService,
                private _appointmentsService: AppointmentsService,
                private _notificationService: NotificationService,
                @Inject(DOCUMENT) private document,
                @Inject(PLATFORM_ID) private platformId: Object, 
                private _location: Location) { }

    ngOnInit(): void {
        this.horseAdModel.PriceRange = <PriceRangeModel> { };
        this.horseAdModel.Address = <AddressModel> { };        
        this.currentUserId = this._accountService.getUserId();
        
        if (this.currentUserId) {
            this._accountService.isAdmin(this.currentUserId).subscribe(res => this.isAdmin = res)
        }
     
        this.notificationRefresh = this._notificationService.getRefresh();
        this._sub = this._route.params.subscribe(
            params => {
                let id = params['id'];
                this.getHorseAd(id);
        });
    }

    ngOnDestroy() {
        this._sub.unsubscribe();
    }

    makeAppointment() {
        this.appoinmentModel.Id = 0;
        //this.appoinmentModel.AdvertismentId = this.horseAdModel.Id;
        this.appoinmentModel.AdvertismentTitle = this.horseAdModel.Title;
        this.appointmentDate.setTime(this.appointmentDate.getTime() - this.appointmentDate.getTimezoneOffset()*60*1000);
        this.appoinmentModel.AppointmentDateTime = this.appointmentDate;
        this.appoinmentModel.AdvertismentOwnerId = this.horseAdModel.UserId;
        this.appoinmentModel.InitiatorId = this._accountService.getUserId();
        this.appoinmentModel.IsAccepted = false;
        this.appoinmentModel.AdvertismentTitle = this.horseAdModel.Title;
        this.appoinmentModel.STATUS = AppointmentStatus.CREATED;
        this._appointmentsService.appointments.next(this.appoinmentModel);
        this.makeAppointmentModal.close();
        this.showNotification(this._notificationService.makeAppSuccessText()); 
    }

    getHorseAd(id: number) {
        this._horseAdService.getHorseAdDetails(id)
                            .subscribe(res => { this.horseAdModel = res;
                                                this.horseAddress = res.Address;
                                                this.pedigree = res.Pedigree;
                                                this.gender = res.Gender;
                                                this.priceRange = res.PriceRange;
                                                this.recommendedRiders = res.RecomendedRiders;
                                                this.abilities = res.Abilities;
                                                this.setGalleriaImages(res.Images);
                                                this.checkPermissions(); 
                                                this.setVideoFrame(); 
                                                this.initMap(); 
                                                this.checkPedigree();
                                                this.setHorseAdOwnerDetails(); },
                                       error => this.errorMessage = error);        
    }

    setGalleriaImages(images: any) {
        let imagesArray: any[] = []
        images.forEach(element => {
            let url = this.imagesUrl + element.ImageName;
            imagesArray.push({source: url});
        });

        this.galleriImages = imagesArray;
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
        if (this.currentUserId) {
            this._accountService.isAdmin(this.currentUserId).subscribe(res => {
                this.isAdmin = res
                if (this.currentUserId != this.horseAdModel.UserId) {
                    this._horseAdService.increaseViews(this.horseAdModel.Id).subscribe();
                }
        
                if (!this.horseAdModel.IsValidated && !this.isAdmin) {
                    var loggedUserId = localStorage.getItem('id');
            
                    if (this.horseAdModel.UserId != loggedUserId) {
                        this._router.navigate(['/error/403']);
                    }
                }
            })
        } else { 
            this._horseAdService.increaseViews(this.horseAdModel.Id).subscribe();
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

    setIfFavorite() {
        if (this.horseAdModel.FavoritesFor.indexOf(this.currentUserId) > -1) {
            this.isFavorite = true;
        }
    }

    initMap() {
        var map = new google.maps.Map(this.map.nativeElement, {
          zoom: 12,
          center: {lat: 46.770439, lng: 23.591423}
        });

        var horseAdAddress = this.horseAddress.Country + ", " + this.horseAddress.City + ", "  + this.horseAddress.Street;
        var geocoder = new google.maps.Geocoder();
        geocoder.geocode({'address': horseAdAddress }, function(results: any, status: any) {
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
                            .subscribe(res => { this.userModel = res; 
                                                this.setUserImage();
                                                this.setIfFavorite(); },
                                       error => this.errorMessage = error);
    }

    setUserImage() {
        setTimeout(() => {
              this.userImage.nativeElement.src = this._accountService._profilePhotoGetUrl + this.userModel.ImagePath; 
        }, 0);
    }

    goBack(): void {
        this._location.back();
    }

    sendEmailToUser() { 
        this.emailModel.Receiver = this.userModel.Email;
        this.emailModel.ReceiverFirstName = this.userModel.FirstName
        this.emailModel.HorseAdTitle = this.horseAdModel.Title;

        this._horseAdService.sendEmail(this.emailModel)
                            .subscribe(res => { this.sendEmailModal.close(); 
                                                this.showNotification(this._notificationService.sendEmailSuccesText()); }, 
                                       error => this.errorMessage = error);
    }

    showSendEmailModal():void {
        if (this._accountService.isLoggedIn()) {
            let currentUserId = localStorage.getItem('id');
            this._accountService.getUserDetails(currentUserId)
                            .subscribe(res => { this.emailModel.Sender = res.Email; 
                                                this.emailModel.SenderName = res.FirstName + " " + res.LastName; },
                                       error => this.errorMessage = error);
        }

        this.sendEmailModal.open();
    }
 
    hideSendEmailModal():void {
        this.sendEmailModal.close();
    }

    showDeleteModal() {
        this.deleteModal.open();
    }

    hideDeleteModal() {
        this.deleteModal.close();
    }

    hideAppointmentModal() {
        this.makeAppointmentModal.close();
    }

    hideNotLoggedInAppointmentModal() {
        this.makeAppointmentNotLoggedInModal.close();
    }

    showAppointmentModal() {
        if (this.currentUserId == undefined) {
            this.makeAppointmentNotLoggedInModal.open();
        } else {
            this.makeAppointmentModal.open();            
        }
    }

    redirectToEdit() {
        this._router.navigate(['/horses/edit/', this.horseAdModel.Id]);
    }

    viewProfile() {
        this._router.navigate(['/account/profile/', this.horseAdModel.UserId]);
    }

    deleteHorseAd(isSold: boolean) { 
        this._horseAdService.deleteHorseAd(this.horseAdModel.Id, isSold)
                            .subscribe(res => { this.showNotification(this._notificationService.deleteHorseSuccessText()); 
                                                this._router.navigate(['/account/profile/', this.currentUserId]); },
                                       error => this.errorMessage = error);
    }

    dateChanged() {
        this.dateChosedIsPast = this.appointmentDate.getTime() < new Date().getTime();
    }

    showNotification(text: string) {
        this.notificationRefresh++;
        this._notificationService.setRefreshAndText(this.notificationRefresh, text);
    }

    scrollInto(anchor: string) {
        setTimeout(() => {
            if (isPlatformBrowser(this.platformId)) {
                (<HTMLScriptElement>this.document.querySelector('#'+ anchor)).scrollIntoView({ behavior: 'smooth' });  
            }
        }, 0);
    }
}
