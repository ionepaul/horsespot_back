import { Component, Input, OnInit, OnChanges, Output, EventEmitter, HostListener, ViewChild, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { CONFIG } from '../../../config';

//SERVICES
import { AccountService } from '../../../account/account.service';
//import { AppointmentsService } from '../../../account/appointments/appointments.service';
import { NotificationService } from '../../../shared/notifications/notification.service';

//MODELS
import { AppointmentModel } from '../../../account/models/appointmentModel';

@Component({
    selector: 'navbar-user-part',
    templateUrl: './navbar.user.part.component.html'
})

export class NavbarUserPartComponent implements OnInit, OnChanges {
    //@ViewChild('notificationBell') notificationBell: ElementRef;
    @ViewChild('userPart') userPart: ElementRef;
    @ViewChild('userProfileBtn') userProfileBtn: ElementRef;
    @Input() userId: string;
    @Input() userName: string;
    @Output() menuItemClicked:EventEmitter<boolean> = new EventEmitter<boolean>();
    @Output() notifNumber: EventEmitter<number> = new EventEmitter<number>();

    profilePhotoUrl: string = CONFIG.profileImagesUrl;    
    isAdmin: boolean;
    isCollapsed: boolean = true;
    notificationRefresh: number;
    appointmentNotifs: Array<AppointmentModel> = new Array<AppointmentModel>();
    profilePicture: string;
    showUserMenu: boolean = false;
    currentUserId: string;
    isMobileDevice: boolean = false;
    loggedUserName: string;
    loggedUserId: string;
  
    constructor(private _accountService: AccountService, 
                //private _appointmentsService: AppointmentsService,
                private _notificationService: NotificationService,
                private _router: Router,
                @Inject(PLATFORM_ID) private _platformId: Object) { }

    ngOnInit() {
        this.currentUserId = this._accountService.getUserId();
        this.profilePicture = this._accountService.getProfilePicutreName();

        if (this.profilePicture.indexOf('http') >= 0) {
            this.profilePhotoUrl = this.profilePicture;
        }
        else {
            this.profilePhotoUrl += this.profilePicture;
        }

        if (this.currentUserId != undefined) {
            this._accountService.isAdmin(this.currentUserId).subscribe(res => this.isAdmin = res);
        }

        this.notificationRefresh = this._notificationService.getRefresh();
        this.isMobileDevice = isPlatformBrowser(this._platformId) ? window.screen.width <= CONFIG.mobile_width : false;

        // if (this._accountService.isLoggedIn && this._appointmentsService.appointments != undefined) {
        //     this._appointmentsService.appointments.subscribe(appointmentNotification => {
        //         if (appointmentNotification.AllSeen) {
        //             this.notificationBell.nativeElement.classList.remove('hasNotif');
        //             this.notifNumber.emit(0);
        //             this.appointmentNotifs = new Array<AppointmentModel>();
        //         } else {
        //             this.appointmentNotifs.push(appointmentNotification);
        //             this.notificationBell.nativeElement.classList.add('hasNotif');
        //             this.notifNumber.emit(this.appointmentNotifs.length);
        //             this.notificationRefresh++;
        //             this._notificationService.setRefreshAndText(this.notificationRefresh, appointmentNotification.NotificationBarMesseage);  
        //         }
        //     })
        // }

        // this._appointmentsService.getUnseenAppointmentsForUser(this.userId)
        //                          .subscribe(res => { this.appointmentNotifs = res; 
        //                                              this.initializeNotifList(res.length);
        //                                             });

    }

    //ngOnDestroy() {
    //    if (this._appointmentsService.appointments != undefined) {
    //        this._appointmentsService.appointments.unsubscribe();
    //    }
    //}

    itemClicked() {
        this.showUserMenu = false;
        this.emitMenuItemClick();
    }

    goToUserProfile() {
        this.showUserMenu = false;
        this.emitMenuItemClick();
        this._router.navigate(['/account/profile', this.currentUserId]);
    }

    logOut() {
        this.showUserMenu = false;
        this.emitMenuItemClick();
        this._accountService.logout();
    }

    emitMenuItemClick() {
      if (this.isMobileDevice) {
        this.menuItemClicked.emit(true);
      }
    }

    ngOnChanges() {
      this.loggedUserName = this.userName;
      this.loggedUserId = this.userId;
    }

    //initializeNotifList(notifListLength: number) {
    //    if (notifListLength != 0) { 
    //        this.notificationBell.nativeElement.classList.add('hasNotif');
    //        this.notifNumber.emit(notifListLength); 
    //    } 
    //}

    // navigateToAppointments() {
    //     if (this.isMobileDevice) {
	  // 	    this.menuItemClicked.emit(true);
    //     }
    //     this.notificationBell.nativeElement.click();
    //     this._router.navigate(["/account/appointments"]);
    // }
}
