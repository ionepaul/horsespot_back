import { Component, OnInit, ViewChild, ElementRef, OnDestroy } from '@angular/core';
import { Http } from '@angular/http';
import { Meta, Title } from '@angular/platform-browser';
import { NotificationService } from '../shared/notifications/notification.service';

import { EmailModel } from '../horse-advertisments/models/emailModel';

import { CONFIG } from '../config';

declare var google: any;

@Component({
    templateUrl: "./contact.component.html"
})

export class ContactComponent implements OnInit, OnDestroy {
    @ViewChild('googleContactMap') public googleContactMap: ElementRef;
    @ViewChild('map') public map: ElementRef 
    emailModel: EmailModel = <EmailModel> { };
    errorMessage: string;
    notificationRefresh: number;
    private _contactReceiveEmailUrl = CONFIG.baseUrls.apiUrl + "contactformemail";

    constructor(private _http: Http,
                private _notificationService: NotificationService,
                private _metaData: Meta, pageTitle: Title) {
        pageTitle.setTitle('Contact | Horse Spot');
        _metaData.addTags([
            { name: 'robots', content: 'NOINDEX, NOFOLLOW'}
        ]); 
    }

    ngOnInit() {
        this.notificationRefresh = this._notificationService.getRefresh();

        var map = new google.maps.Map(this.map.nativeElement, {
          zoom: 14,
          center: {lat: 46.770439, lng: 23.591423},
        });

        var geocoder = new google.maps.Geocoder();
        geocoder.geocode({'address': "Romania, Cluj-Napoca, Calea Dorobantilor 73" }, function(results: any, status: any) {
             if (status === 'OK') {
                map.setCenter(results[0].geometry.location);
                var marker = new google.maps.Marker({
                    map: map,
                position: results[0].geometry.location
                });
          } else {
            this.googleContactMap.nativeElement.classList.add('map-error');
          }
        })
    }

    send(form: any) {
        this._http.post(this._contactReceiveEmailUrl, JSON.stringify(this.emailModel))
                   .subscribe(res => { this.notificationRefresh++;
                                       this._notificationService.setRefreshAndText(this.notificationRefresh, this._notificationService.contactEmailSentSuccessText());
                                       form.reset();
                                     },
                                     error => this.errorMessage = error);
    }

    ngOnDestroy() {
        this._metaData.removeTag("name='robots'");
    }
}