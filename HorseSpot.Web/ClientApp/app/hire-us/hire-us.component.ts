import { Component, OnInit, ViewChild } from '@angular/core';
import { HttpWrapper } from '../shared/http/http.wrapper';
import { CONFIG } from '../config';
import { NgForm } from '@angular/forms';

//SERVICE
import { NotificationService } from '../shared/notifications/notification.service';

//MODELS
import { EmailModel } from '../horse-advertisments/models/emailModel';

@Component({
  templateUrl: './hire-us.component.html'
})

export class HireUsComponent implements OnInit {
  @ViewChild('quoteForm') public quoteForm: NgForm;
  emailModel: EmailModel = <EmailModel>{};
  errorMessage: string;
  notificationRefresh: number;

  private _contactReceiveEmailUrl = CONFIG.baseUrls.apiUrl + "contactformemail";

  constructor(private _httpWrapper: HttpWrapper,
    private _notificationService: NotificationService) { }

  ngOnInit() {
    this.notificationRefresh = this._notificationService.getRefresh();
  }

  send() {
    this._httpWrapper.post(this._contactReceiveEmailUrl, JSON.stringify(this.emailModel))
      .subscribe(res => {
        this.notificationRefresh++;
        this._notificationService.setRefreshAndText(this.notificationRefresh, this._notificationService.contactEmailSentSuccessText());
        this.quoteForm.resetForm();
      },
      error => this.errorMessage = error);
  }
}
