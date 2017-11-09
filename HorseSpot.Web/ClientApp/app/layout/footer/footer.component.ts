import { Component } from '@angular/core';

import { AccountService } from '../../account/account.service';
import { NotificationService } from '../../shared/notifications/notification.service';

@Component({
    selector: 'horse-spot-footer',
    templateUrl: './footer.component.html'
})

export class FooterComponent { 
    notificationRefresh:number;
    newsletterEmail: string = "";
    errorMessage: string = "";

    constructor(private _accountService: AccountService,
                private _notificationService: NotificationService) { 
                    this.notificationRefresh = this._notificationService.getRefresh();
                }

    subscribeToNewsletter() {
        this._accountService.registerToNewsletter(this.newsletterEmail)
                            .subscribe(res => this.showNotification(),
                                       error => this.errorMessage = error);
    }

    showNotification() {
        this.notificationRefresh++;
        this._notificationService.setRefreshAndText(this.notificationRefresh, this._notificationService.newsletterSubscriptionSuccessText());
        this.newsletterEmail = "";
        this.errorMessage = "";
    }
}