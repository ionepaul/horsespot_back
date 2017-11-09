import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Injectable()
export class NotificationService {
    refresh: number = 0;
    text: string;

    constructor(private _translateService: TranslateService) {}

    setRefreshAndText(notifRefresh: number, notifText: string) {
        this.refresh = notifRefresh;
        this.text = notifText;
    }

    getRefresh() {
        return this.refresh;
    }

    getText() {
        return this.text;
    }

    getTranslationMessage(key: string): string {
        var value = "";
        this._translateService.get(key).subscribe((res: string) => value = res);
        return value;
    }

    newsletterSubscriptionSuccessText() {
        return this.getTranslationMessage("FOOTER.NEWSLETTER_SUCCESS_NOTIF_MSG");
    }

    passwordRecoverySuccessText() {
        return this.getTranslationMessage("LOGIN_PAGE.FORGOT_NOTIF_MSG");
    }

    profileChangesSuccessText() {
        return this.getTranslationMessage("PROFILE.PROFILE_CHANGES_SUCCESS_NOTIF");
    }

    changePasswordSuccessText() {
        return this.getTranslationMessage("PROFILE.CHANGE_PASS.SUCCESS_NOTIF");
    }

    appointmentCanceledSuccessText() {
        return this.getTranslationMessage("APPOINTMENTS.APPOINTMENT_CANCELED_NOTIF");
    }

    appointmentAcceptedSuccessText() {
        return this.getTranslationMessage("APPOINTMENTS.APPOINTMENT_ACCEPTED_NOTIF");
    }

    appointmentDateChangedSuccessText() {
        return this.getTranslationMessage("APPOINTMENTS.APPOINTMENT_CHANGE_DATE_NOTIF");
    }

    horseAddSuccesText() {
        return this.getTranslationMessage("ADD_AND_EDIT_PAGE.ADD_SUCCESS_NOTIF");
    }

    horseEditSuccesText() {
        return this.getTranslationMessage("ADD_AND_EDIT_PAGE.EDIT_SUCCESS_NOTIF")
    }

    imageDeleteSuccesText() {
        return this.getTranslationMessage("ADD_AND_EDIT_PAGE.DELETE_IMAGE_SUCCESS_NOTIF");
    }

    sendEmailSuccesText() {
        return this.getTranslationMessage("HORSE_AD_DETAIL.MAIL_NOTIF_SUCCESS_TEXT");
    }

    makeAppSuccessText() {
        return this.getTranslationMessage("HORSE_AD_DETAIL.MAKE_APP_NOTIF_SUCCESS_TEXT");
    }

    deleteHorseSuccessText() {
        return this.getTranslationMessage("HORSE_AD_DETAIL.DELETE_NOTIF_SUCCESS_TEXT");
    }

    contactEmailSentSuccessText() {
        return this.getTranslationMessage("CONTACT_PAGE.MAIL_SEND_NOTIF_SUCCES");
    }
}