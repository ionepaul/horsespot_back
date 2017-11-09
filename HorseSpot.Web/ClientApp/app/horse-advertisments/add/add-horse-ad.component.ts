import { Component, OnInit, ElementRef, ViewChild, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { FileUploader } from 'ng2-file-upload/ng2-file-upload';
import { Meta, Title } from '@angular/platform-browser';
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

import { HorseAdsService } from '../horse-ads.service';
import { NotificationService } from '../../shared/notifications/notification.service';

import { PriceRangeModel } from '../models/priceRangeModel';
import { CountryModel } from '../models/countryModel';
import { GenderModel } from '../models/genderModel';
import { HorseAbilityModel } from '../models/horseAbilityModel';
import { RecommendedRiderModel } from '../models/recommendedRiderModel';
import { HorseAdModel } from '../models/horseAdModel';
import { AddressModel } from '../models/addressModel';
import { PedigreeModel } from '../models/pedigree';
import { ImageModel } from '../models/imageModel';
import { DOCUMENT } from '@angular/platform-browser';

import { CONFIG } from '../../config';

@Component({
    templateUrl: './add-horse-ad.component.html',   
})

export class AddHorseAdComponent implements OnInit, OnDestroy {
    @ViewChild('horseVideoFrame') public videoFrame: ElementRef;
    uploader: FileUploader = new FileUploader({});
    hasBaseDropZoneOver: boolean = false;
    priceRanges: PriceRangeModel[];
    genders: Array<string> = CONFIG.genders;    
    horseAbilities: HorseAbilityModel[];
    recommendedRiders: RecommendedRiderModel[];
    countries: string[];
    horseAdModel: HorseAdModel = <HorseAdModel> { };
    errorMessage: string;
    typeaheadNoResults:boolean = false;
    horseAbilityNotSelected: boolean = false;
    recommendedRiderNotSelected: boolean = false;
    notUploadedPhoto: boolean = false;
    videoLinkError: boolean = false;
    notificationRefresh: number;
    imagesSizeExceed: boolean = false;
    imageDisplayUrl: string = CONFIG.imagesUrl + '/Images/HorseAdsImg/'

    constructor(private _horseAdService: HorseAdsService, 
                private _router: Router,
                private _notificationService: NotificationService,
                @Inject(DOCUMENT) private document,
                @Inject(PLATFORM_ID) private platformId: Object,
                private _metaData: Meta, pageTitle: Title) { 
        pageTitle.setTitle('Add Horse | Horse Spot');
        _metaData.addTags([ 
            { name: 'robots', content: 'NOINDEX, NOFOLLOW'}
        ]);
    }

    ngOnInit() {
        this.getPriceRanges();
        this.getCountries();
        this.getHorseAbilities();
        this.getRecommendedRiders();
        
        this.horseAdModel.Gender = this.genders[0];
        this.horseAdModel.AbilityIds = new Array<number>();
        this.horseAdModel.RecomendedRidersIds = new Array<number>();
        this.horseAdModel.Address = <AddressModel> {};
        this.horseAdModel.Pedigree = <PedigreeModel> {};
        this.horseAdModel.Images = new Array<ImageModel>();
        this.notificationRefresh = this._notificationService.getRefresh();
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
                                       error =>this.errorMessage = error);
    }

    getPriceRanges() {
        this._horseAdService.getAllPriceRanges()
                            .subscribe(res => this.priceRanges = res,
                                       error => this.errorMessage = error,
                                       () => this.horseAdModel.PriceRangeId = this.priceRanges[0].Id);
    }

    addHorseAd() {
        if (this.checkForRequired()) {
            this.horseAdModel.Images[0].IsProfilePic = true;
            this._horseAdService.postHorseAd(this.horseAdModel)
                                .subscribe(res => this.redirectToHome(),
                                           error => this.errorMessage = error);
        } else {
            if (isPlatformBrowser(this.platformId)) {
                window.scrollTo(0, 0);                
            }
        }  
    }

    redirectToHome() {
        this.notificationRefresh++;
        this._notificationService.setRefreshAndText(this.notificationRefresh, this._notificationService.horseAddSuccesText());
        this._router.navigate(['/home']);
    }
    
    isValidExtenstion(filename: string): boolean { 
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
        this.imagesSizeExceed = false;
        if (this.uploader.queue.length > 0) {
            this.uploader.queue.forEach(element => {
                if (!this.isValidExtenstion(element._file.name) || !this.isValidImageSize(element._file.size)) {
                    this.imagesSizeExceed = true;
                    let pos = this.uploader.queue.indexOf(element);
                    this.uploader.queue.splice(pos, 1);
                }
            });
        }

        this.uploader.queue.forEach(el => {
            this._horseAdService.uploadHorseAdPhotos(el._file)
                            .subscribe(res => {
                                let img = new ImageModel();
                                img.ImageName = res;
                                this.horseAdModel.Images.push(img);
                            },
                            error => this.errorMessage = error);
        });
    }

    isValidImageSize(fileSize: number) {
        return fileSize < CONFIG.allImagesSizeLimit;
    }

    addHorseAbility(value: HorseAbilityModel) {        
        if (isPlatformBrowser(this.platformId)) {
            if ((<HTMLInputElement>this.document.getElementById(value.Ability)).checked === true) {
                this.horseAdModel.AbilityIds.push(value.Id);
            } else if ((<HTMLInputElement>this.document.getElementById(value.Ability)).checked === false) {
                let position = this.horseAdModel.AbilityIds.indexOf(value.Id);
                this.horseAdModel.AbilityIds.splice(position, 1);
            }
        }
    }

    addHorseRecommendedRider(value: RecommendedRiderModel) {
        if (isPlatformBrowser(this.platformId)) {
            if ((<HTMLInputElement>this.document.getElementById(value.Rider)).checked === true) {
                this.horseAdModel.RecomendedRidersIds.push(value.Id);
            } else if ((<HTMLInputElement>this.document.getElementById(value.Rider)).checked === false) {
                let position = this.horseAdModel.RecomendedRidersIds.indexOf(value.Id);
                this.horseAdModel.RecomendedRidersIds.splice(position, 1);
            }
        }
    }

    changeTypeaheadNoResults(e:boolean):void {
        this.typeaheadNoResults = e;
    }

    deleteUnsavedImage(fileName: string) {
       this._horseAdService.deleteUnsavedImage(fileName)
                           .subscribe(res => {
                                let index = this.horseAdModel.Images.findIndex(x => x.ImageName == fileName);
                                this.horseAdModel.Images.splice(index, 1);
                           }, 
                           error => this.errorMessage);
    }

    setAdvertismentFirstPicture(fileName: string) {
        var index = this.horseAdModel.Images.findIndex(x => x.ImageName == fileName);
        var aux = this.horseAdModel.Images[0];
        this.horseAdModel.Images[0] = this.horseAdModel.Images[index];
        this.horseAdModel.Images[index] = aux;
    }

    setVideoFrame(event?: any) {
        if (event) {
            this.videoLinkError = event.indexOf('www.youtube.com/watch?v') == -1 ? true : false;
            if (this.videoLinkError) {
                this.videoLinkError = event.indexOf('youtu.be') == -1 ? true : false;
            }
        } else {
            this.videoLinkError = true;
            this.horseAdModel.VideoLink = null;
            this.videoFrame.nativeElement.src = "";
            this.videoFrame.nativeElement.style.display = "none";
        }

        if (!this.videoLinkError) {
            let videoLink = event.indexOf("&") != -1 ? event.substr(0, event.indexOf("&")) : event;
            if (videoLink.indexOf('watch?v=') != -1) {
                this.horseAdModel.VideoLink = videoLink.replace("watch?v=", "embed/");
            }
            else {
                this.horseAdModel.VideoLink = videoLink.replace("youtu.be", "www.youtube.com/embed");             
            }
            this.videoFrame.nativeElement.src = this.horseAdModel.VideoLink;
            this.videoFrame.nativeElement.style.display = "inline-block";
        } else {
            this.horseAdModel.VideoLink = null;
            this.videoFrame.nativeElement.src = "";
            this.videoFrame.nativeElement.style.display = "none";
        }
    }

    checkForRequired() {
        var ok = true;

        if (this.horseAdModel.Images.length == 0) {
            this.notUploadedPhoto = true;
            ok = false;
        } else { 
            this.notUploadedPhoto = false;
        }

        if (this.horseAdModel.AbilityIds.length == 0) {
            this.horseAbilityNotSelected = true;
            ok = false;
        } else {
            this.horseAbilityNotSelected = false;
        }
      
        if (this.horseAdModel.RecomendedRidersIds.length == 0) {
            this.recommendedRiderNotSelected = true;
            ok = false;
        } else {
            this.recommendedRiderNotSelected = false;
        }

        return ok;
    }

    ngOnDestroy() {
        this._metaData.removeTag("name='robots'"); 
    }
}
