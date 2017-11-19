import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FileUploader } from 'ng2-file-upload/ng2-file-upload';
import { Subscription } from 'rxjs/Subscription';
import { Meta, Title } from '@angular/platform-browser';
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser, Location } from '@angular/common';
import { DOCUMENT } from '@angular/platform-browser';
import { Observable } from 'rxjs/Observable';
import { CONFIG } from '../../config';
import 'rxjs/add/observable/of';

//SERVICES
import { HorseAdsService } from '../horse-ads.service';
import { NotificationService } from '../../shared/notifications/notification.service';

//MODELS
import { PriceRangeModel } from '../models/priceRangeModel';
import { PedigreeModel } from '../models/pedigree';
import { CountryModel } from '../models/countryModel';
import { GenderModel } from '../models/genderModel';
import { HorseAbilityModel } from '../models/horseAbilityModel';
import { RecommendedRiderModel } from '../models/recommendedRiderModel';
import { HorseAdModel } from '../models/horseAdModel';
import { AddressModel } from '../models/addressModel';
import { ImageModel } from '../models/imageModel';

@Component({
  templateUrl: './edit-horse-ad.component.html'
})

export class EditHorseAdComponent implements OnInit {
  @ViewChild('edithorseVideoFrame') public videoFrame: ElementRef;
  uploader: FileUploader = new FileUploader({});
  hasBaseDropZoneOver: boolean = false;
  priceRanges: PriceRangeModel[];
  horseAbilities: HorseAbilityModel[];
  genders: Array<string> = CONFIG.genders;
  recommendedRiders: RecommendedRiderModel[];
  horsePriceRange: PriceRangeModel;
  recommendedRiderNotSelected: boolean = false;
  horseAbilityNotSelected: boolean = false;
  errorMessage: string;
  selectedCountry: string;
  countryData: Observable<any[]>;
  horseAdModel: HorseAdModel = <HorseAdModel>{};
  typeaheadNoResults: boolean = false;
  cannotDeleteLastPhotoError: boolean = false;
  videoLinkError: boolean = false;
  imagesSizeExceed: boolean = false;
  notificationRefresh: number;
  setVideoFirstTime: number = 0;
  imageDisplayUrl: string = CONFIG.horseAdsImagesUrl;

  private _routerSub$: Subscription;

  constructor(private _horseAdService: HorseAdsService,
    private _router: Router,
    private _route: ActivatedRoute,
    private _notificationService: NotificationService,
    private _location: Location,
    @Inject(DOCUMENT) private document,
    @Inject(PLATFORM_ID) private platformId: Object) { }

  ngOnInit() {
    this._routerSub$ = this._route.params.subscribe(
      params => {
        let id = params['id'];
        this.getHorseAdDetails(id);
      });

    this.initHorseAdModel();

    this.countryData = Observable.create((observer: any) => {
      observer.next(this.selectedCountry);
    }).mergeMap((name: string) => this.getCountries(name));

    this.notificationRefresh = this._notificationService.getRefresh();
  }

  ngOnDestroy() {
    this._routerSub$.unsubscribe();
  }

  initHorseAdModel() {
    this.horseAdModel.Abilities = new Array<HorseAbilityModel>();
    this.horseAdModel.RecomendedRiders = new Array<RecommendedRiderModel>();
    this.horseAdModel.Address = <AddressModel>{};
    this.horseAdModel.PriceRange = <PriceRangeModel>{};
    this.horseAdModel.Pedigree = <PedigreeModel>{};
  }

  getHorseAdDetails(id: number) {
    this._horseAdService.getHorseAdDetails(id)
      .subscribe(res => {
        this.horseAdModel = res;
        this.horseAdModel.Address = res.Address;
        this.selectedCountry = res.Address.Country;
        this.horseAdModel.Abilities = res.Abilities;
        this.horseAdModel.RecomendedRiders = res.RecomendedRiders;
        this.horseAdModel.PriceRange = res.PriceRange,
          this.horseAdModel.PriceRangeId = res.PriceRange.Id,
          this.setAbilitiesAndRecommendedRidersIds();
        this.getPriceRanges();
        this.getHorseAbilities();
        this.getRecommendedRiders();
        this.setVideoFrame();
        this.setVideoFirstTime = 0;
      },
      error => this.errorMessage = error,
      () => {
        this.cannotDeleteLastPhotoError = false;
        this.uploader.clearQueue();
      });
  }

  getPriceRanges() {
    var horsePriceRangeId = this.horseAdModel.PriceRange.Id;

    this._horseAdService.getAllPriceRanges()
      .subscribe(res => {
        this.priceRanges = res;
        this.horsePriceRange = res[horsePriceRangeId - 1]
      },
      error => this.errorMessage = error);
  }

  getHorseAbilities() {
    this._horseAdService.getAllAbilities()
      .subscribe(res => this.horseAbilities = res,
      error => this.errorMessage = error);
  }

  getRecommendedRiders() {
    this._horseAdService.getAllRecomendedRiders()
      .subscribe(res => this.recommendedRiders = res,
      error => this.errorMessage = error);
  }

  getCountries(name: string) {
    return this._horseAdService.getAllCountries(name);
  }

  editHorseAd() {
    if (this.checkForRequired()) {
      this.horseAdModel.Address.Country = this.selectedCountry;

      this._horseAdService.editHorseAd(this.horseAdModel, this.horseAdModel.Id)
        .subscribe(res => {
          window.scrollTo(0, 0);
          this.showNotification(this._notificationService.horseEditSuccesText());
        },
        error => this.errorMessage = error);
    } else {
      if (isPlatformBrowser(this.platformId)) {
        window.scrollTo(0, 0);
      }
    }
  }

  deleteImage(imageId: number, imageName: string) {
    if (this.horseAdModel.Images.length == 1) {
      this.cannotDeleteLastPhotoError = true;
      return;
    }

    if (imageId != undefined) {
      this._horseAdService.deleteImage(imageId)
        .subscribe(res => this.removeDeleteImage(imageId),
        error => this.errorMessage);
    } else {
      this._horseAdService.deleteUnsavedImage(imageName)
        .subscribe(res => this.removeDeleteImage(imageId),
        error => this.errorMessage);
    }
  }

  removeDeleteImage(imageId: number) {
    let index = this.horseAdModel.Images.findIndex(x => x.ImageId == imageId);
    this.horseAdModel.Images.splice(index, 1);
  }

  setAdvertismentFirstPicture(fileName: string) {
    var index = this.horseAdModel.Images.findIndex(x => x.ImageName == fileName);
    var aux = this.horseAdModel.Images[0];
    this.horseAdModel.Images[0] = this.horseAdModel.Images[index];
    this.horseAdModel.Images[index] = aux;
  }

  setHorseAdProfileImage(imageId: number, imageName: string) {
    if (imageId != undefined) {
      this._horseAdService.setHorseAdProfilePicture(imageId)
        .subscribe(res => this.setAdvertismentFirstPicture(imageName),
        error => this.errorMessage = error);
    } else {
      this.setAdvertismentFirstPicture(imageName);
    }
  }

  onFileInputEventChange(event: any) {
    this.checkImages();
  }

  fileOverBase(e: any): void {
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

          this._horseAdService.saveHorseAdImage(this.horseAdModel.Id, res).subscribe(res => {
            img.ImageId = res;
            this.horseAdModel.Images.push(img);
          }, error => this.errorMessage = error);
        },
        error => this.errorMessage = error);
    });
  }

  isValidImageSize(fileSize: number) {
    return fileSize < CONFIG.allImagesSizeLimit;
  }

  isValidExtenstion(filename: string): boolean {
    let extensionStart = filename.lastIndexOf(".");
    let extension = filename.substr(extensionStart);

    var isValid = CONFIG.accepted_file_extension.indexOf(extension.toUpperCase()) == -1 ? false : true;

    return isValid;
  }

  isCheckedHorseAbility(value: HorseAbilityModel) {
    return this.horseAdModel.Abilities.findIndex(y => value.Id == y.Id) > -1;
  }

  isCheckedRecommendedRider(value: RecommendedRiderModel) {
    return this.horseAdModel.RecomendedRiders.findIndex(y => value.Id == y.Id) > -1;
  }

  setAbilitiesAndRecommendedRidersIds() {
    this.horseAdModel.AbilityIds = new Array<number>();
    this.horseAdModel.RecomendedRidersIds = new Array<number>();

    this.horseAdModel.Abilities.forEach(ability => {
      this.horseAdModel.AbilityIds.push(ability.Id);
    });

    this.horseAdModel.RecomendedRiders.forEach(rider => {
      this.horseAdModel.RecomendedRidersIds.push(rider.Id);
    });
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

  changeTypeaheadNoResults(e: boolean) {
    this.typeaheadNoResults = e;
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

  checkForRequired() {
    var ok = true;

    if (this.horseAdModel.AbilityIds.length == 0) {
      this.horseAbilityNotSelected = true;
      ok = false;
    } else {
      this.horseAbilityNotSelected = false;
    }

    if (this.horseAdModel.AbilityIds.length == 0) {
      this.recommendedRiderNotSelected = true;
      ok = false;
    } else {
      this.recommendedRiderNotSelected = false;
    }

    return ok;
  }

  goBack() {
    this._location.back();
  }
}
