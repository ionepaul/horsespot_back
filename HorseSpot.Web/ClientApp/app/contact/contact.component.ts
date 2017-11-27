import { Component, OnInit, ViewChild, ElementRef, OnDestroy } from '@angular/core';
import { NgForm } from '@angular/forms';
import { HttpWrapper } from '../shared/http/http.wrapper';
import { CONFIG } from '../config';

//SERVICE
import { NotificationService } from '../shared/notifications/notification.service';

//MODELS
import { EmailModel } from '../horse-advertisments/models/emailModel';

@Component({
  templateUrl: "./contact.component.html"
})

export class ContactComponent implements OnInit {
  @ViewChild('contactForm') public contactForm: NgForm;
  emailModel: EmailModel = <EmailModel>{};
  errorMessage: string;
  notificationRefresh: number;
  private _contactReceiveEmailUrl = CONFIG.baseUrls.apiUrl + "contactformemail";

  constructor(private _httpWrapper: HttpWrapper,
    private _notificationService: NotificationService) { }

  ngOnInit() {
    this.notificationRefresh = this._notificationService.getRefresh();
  }

  send(form: any) {
    this._httpWrapper.post(this._contactReceiveEmailUrl, JSON.stringify(this.emailModel))
      .subscribe(res => {
      this.notificationRefresh++;
        this._notificationService.setRefreshAndText(this.notificationRefresh, this._notificationService.contactEmailSentSuccessText());
        this.contactForm.resetForm();
      },
      error => this.errorMessage = error);
  }
}
