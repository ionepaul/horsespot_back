import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FileUploader } from 'ng2-file-upload/ng2-file-upload';
import { Subscription } from 'rxjs/Subscription';
import { Meta, Title } from '@angular/platform-browser';
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

import { HorseAdsService } from '../horse-ads.service';
import { NotificationService } from '../../shared/notifications/notification.service';

import { PriceRangeModel } from '../models/priceRangeModel';
import { PedigreeModel } from '../models/pedigree';
import { CountryModel } from '../models/countryModel';
import { GenderModel } from '../models/genderModel';
import { HorseAbilityModel } from '../models/horseAbilityModel';
import { RecommendedRiderModel } from '../models/recommendedRiderModel';
import { HorseAdModel } from '../models/horseAdModel';
import { AddressModel } from '../models/addressModel';
import { DOCUMENT } from '@angular/platform-browser';
import { CONFIG } from '../../config';

@Component({
    templateUrl: './edit-horse-ad.component.html'
})

export class EditHorseAdComponent implements OnInit, OnDestroy {
    @ViewChild('edithorseVideoFrame') public videoFrame: ElementRef;
    uploader: FileUploader = new FileUploader({});
    hasBaseDropZoneOver: boolean = false;
    priceRanges: PriceRangeModel[];
    horseAbilities: HorseAbilityModel[];
    recommendedRiders: RecommendedRiderModel[];
    recommendedRiderNotSelected: boolean = false;
    horseAbilityNotSelected: boolean = false;
    countries: string[];
    horseAdModel: HorseAdModel = <HorseAdModel> { };
    firstRequestSucceded: boolean = false;
    advertismentId: string;
    errorMessage: string;
    filePhotoArray: File[] = [];
    genders: GenderModel[];
    validUpload: boolean = true;
    noMoreThanSixPhotos: boolean = true;
    typeaheadNoResults:boolean = false;
    horseGender: GenderModel;
    horsePriceRange: PriceRangeModel;
    cannotDeleteLastPhotoError: boolean = false;
    photoLimitError: boolean = false;
    videoLinkError: boolean = false;
    imagesSizeExceed: boolean = false;
    notificationRefresh: number;
    setVideoFirstTime: number = 0;
    picturesUrl: string = CONFIG.baseUrls.apiUrl + 'images/get/';
    private _sub: Subscription;

    constructor(private _horseAdService: HorseAdsService, 
                private _router: Router, 
                private _route: ActivatedRoute,
                private _notificationService: NotificationService,
                @Inject(DOCUMENT) private document,
                @Inject(PLATFORM_ID) private platformId: Object,
                private _metaData: Meta, pageTitle: Title) { 
            pageTitle.setTitle('Edit Your Horse | Horse Spot');
            _metaData.addTags([ 
                  { name: 'robots', content: 'NOINDEX, NOFOLLOW'}
            ]);
    }

    ngOnInit() {
        this._sub = this._route.params.subscribe(
            params => {
                let id = params['id'];
                this.getHorseAdDetails(id);
        });
        
        this.horseAdModel.Abilities = new Array<HorseAbilityModel>();
        this.horseAdModel.RecomendedRiders = new Array<RecommendedRiderModel>();
        this.horseAdModel.Address = <AddressModel> {};
        this.horseAdModel.PriceRange = <PriceRangeModel> {};
        this.horseAdModel.Pedigree = <PedigreeModel> {};
        this.notificationRefresh = this._notificationService.getRefresh();
    }

    ngOnDestroy() {
        this._metaData.removeTag("name='robots'");        
        this._sub.unsubscribe();
    }

    getHorseAdDetails(id: number) { 
        this._horseAdService.getHorseAdDetails(id)
                            .subscribe(res => { this.horseAdModel = res;
                                                this.horseAdModel.Address = res.Address;
                                                this.horseAdModel.Abilities = res.Abilities;
                                                this.horseAdModel.RecomendedRiders = res.RecomendedRiders;
                                                this.horseAdModel.Gender = res.Gender; 
                                                this.horseAdModel.PriceRange = res.PriceRange,
                                                this.getGenders();
                                                this.getPriceRanges();
                                                this.getCountries();
                                                this.getHorseAbilities();
                                                this.getRecommendedRiders();
                                                this.setVideoFrame(); 
                                                this.setVideoFirstTime = 0; },
                                       error => this.errorMessage = error,
                                       () => { this.cannotDeleteLastPhotoError = false; 
                                         this.uploader.clearQueue();
                                       });                                       
    }

    getGenders() {
        var horseGenderId = this.horseAdModel.Gender;
    }

    getPriceRanges() {
        var horsePriceRangeId = this.horseAdModel.PriceRange.Id;

        this._horseAdService.getAllPriceRanges()
                            .subscribe(res => { this.priceRanges = res; 
                                                this.horsePriceRange = res[horsePriceRangeId - 1]},
                                       error => this.errorMessage = error);
    }

    getHorseAbilities() {
        this._horseAdService.getAllAbilities()
                            .subscribe(res => this.horseAbilities = res,
                                       error =>this.errorMessage = error);
    }

    getRecommendedRiders() {
        this._horseAdService.getAllRecomendedRiders()
                            .subscribe(res => this.recommendedRiders = res,
                                       error =>this.errorMessage = error);
    }

    getCountries() {
        this._horseAdService.getAllCountries()
                            .subscribe(res => this.countries = res,
                                       error => this.errorMessage = error);
    }

    editHorseAd() {
        if (this.checkForRequired()) {
            this._horseAdService.editHorseAd(this.horseAdModel, this.horseAdModel.Id)
                                .subscribe(res => this.uploadPhotos(),
                                           error => this.errorMessage = error);
        } else {
            if (isPlatformBrowser(this.platformId)) {
                window.scrollTo(0, 0);                
            }
        }  
    }

    checkForRequired() {
        var ok = true;

        if (this.horseAdModel.Abilities.length == 0) {
            this.horseAbilityNotSelected = true;
            ok = false;
        } else {
            this.horseAbilityNotSelected = false;
        }

        if (this.horseAdModel.RecomendedRiders.length == 0) {
            this.recommendedRiderNotSelected = true;
            ok = false;
        } else {
            this.recommendedRiderNotSelected = false;
        }         
        
        if (this.exceedImagesSize()) {
            this.imagesSizeExceed = true;
            ok = false;
        }

        return ok;
    }

    uploadPhotos() {
        if (this.uploader.queue.length > 0) {
            this.uploader.queue.forEach(photo => {
                if (this.checkExtension(photo._file.name)) {
                    this.filePhotoArray.push(photo._file);               
                }
            });

        this._horseAdService.uploadHorseAdPhotos(this.filePhotoArray[0])
                            .subscribe(res => { this.showNotification(this._notificationService.horseEditSuccesText()); 
                                                this.getHorseAdDetails(this.horseAdModel.Id); },
                                       error => this.errorMessage = error);
        } else {
            this.showNotification(this._notificationService.horseEditSuccesText());
        }
    }

    deleteImage(imageId: string) {
        if (this.horseAdModel.Images.length == 1) {
            this.cannotDeleteLastPhotoError = true;
        }
        else {   
            this._horseAdService.deleteImage(this.horseAdModel.Id,imageId)
                            .subscribe(res => { this.getHorseAdDetails(this.horseAdModel.Id); 
                                                this.showNotification(this._notificationService.imageDeleteSuccesText()) },
                                       error => this.errorMessage = error);
        }
    }

    setHorseAdProfileImage(imageId: string) {
        this._horseAdService.setHorseAdProfilePicture(this.horseAdModel.Id, imageId)
                            .subscribe(res => this.getHorseAdDetails(this.horseAdModel.Id),
                                       error => this.errorMessage = error);
    }

    checkExtension(filename: string): boolean { 
        let extensionStart = filename.lastIndexOf(".");
        let extension = filename.substr(extensionStart);

        if (CONFIG.accepted_file_extension.indexOf(extension.toUpperCase()) == -1) {
            return false;
        }
        else {
            return true;
        }
    }

    onFileInputEventChange(event: any) {
        this.checkImages();
    }

    fileOverBase(e:any):void {
        this.hasBaseDropZoneOver = e;
        if (event.type == 'drop') {
            this.checkImages();
        }
    }

    checkImages() {
        if (this.uploader.queue.length > 0) {
            this.uploader.queue.forEach(element => {
                if (!this.checkExtension(element._file.name)) {
                    let pos = this.uploader.queue.indexOf(element);
                    this.uploader.queue.splice(pos, 1);
                }
            });
        }
    }

    exceedImagesSize() {
        let sum = 0;
        this.uploader.queue.forEach(element => {
            sum += element._file.size;
        });

        return sum >= CONFIG.allImagesSizeLimit;
    }

    updateGender(event: any) {
       this.horseAdModel.Gender = event;
    }

    updatePriceRange(event: any) {
        this.horseAdModel.PriceRange = event;
    }

    isCheckedHorseAbility(value: HorseAbilityModel) {
        return this.horseAdModel.Abilities.findIndex(y=> value.Id == y.Id) > -1;
    }

    isCheckedRecommendedRider(value: RecommendedRiderModel) {
        return this.horseAdModel.RecomendedRiders.findIndex(y => value.Id == y.Id) > -1;
    }

    addHorseAbility(value: HorseAbilityModel) {  
        if (isPlatformBrowser(this.platformId)) {
            if ((<HTMLInputElement>this.document.getElementById(value.Ability)).checked === true) {
                this.horseAdModel.Abilities.push(value);
            } else if ((<HTMLInputElement>this.document.getElementById(value.Ability)).checked === false) {
                let position = this.horseAdModel.Abilities.findIndex(y=> value.Id == y.Id);
                this.horseAdModel.Abilities.splice(position, 1);
            }
        }
    }

    addHorseRecommendedRider(value: RecommendedRiderModel) {
        if (isPlatformBrowser(this.platformId)) {
            if ((<HTMLInputElement>this.document.getElementById(value.Rider)).checked === true) {
                this.horseAdModel.RecomendedRiders.push(value);
            } else if ((<HTMLInputElement>this.document.getElementById(value.Rider)).checked === false) {
                let position = this.horseAdModel.RecomendedRiders.findIndex(y => value.Id == y.Id);
                this.horseAdModel.RecomendedRiders.splice(position, 1);
            }
        }
    }

    changeTypeaheadNoResults(e: boolean) {
        this.typeaheadNoResults = e;
    }

    removePhotoFromQueue(fileItem: any) {
       this.uploader.removeFromQueue(fileItem);
    }

    showNotification(text: string) {
        this.notificationRefresh++;
        this._notificationService.setRefreshAndText(this.notificationRefresh, text); 
    }

    setVideoFrame(event?: any) {        
        if (event) {
            let videoLink = event.indexOf("&") != -1 ? event.substr(0, event.indexOf("&")) : event;
            if (videoLink.indexOf('watch?v=') != -1) {
                this.horseAdModel.VideoLink = videoLink.replace("watch?v=", "embed/");
            }
            else {
                this.horseAdModel.VideoLink = videoLink.replace("youtu.be", "www.youtube.com/embed");             
            }
            this.videoLinkError = event.indexOf('www.youtube.com/watch?v') == -1 ? true : false;
            if (this.videoLinkError) {
                this.videoLinkError = event.indexOf('youtu.be') == -1 ? true : false;
            }       
        } else if (this.setVideoFirstTime != 0) {
            this.horseAdModel.VideoLink = null;            
            this.videoLinkError = false;
        }
        
        if (this.horseAdModel.VideoLink != null && !this.videoLinkError) {
            this.videoFrame.nativeElement.src = this.horseAdModel.VideoLink;
            this.videoFrame.nativeElement.style.display = "inline-block";
        } else {
            this.videoFrame.nativeElement.style.display = "none";
        }

        this.setVideoFirstTime++;        
    }
}
