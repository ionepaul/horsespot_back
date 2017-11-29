//import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
//import { Modal } from 'ngx-modal';

//import { AppointmentsService } from '../appointments.service';
//import { AccountService } from '../../account.service';
//import { NotificationService } from '../../../shared/notifications/notification.service';

//import { AppointmentModel } from '../../models/appointmentModel';
//import { CancelAppointmentModel } from '../../models/cancelAppointmentModel';
//import { AppointmentStatus } from '../../../shared/constants/appointment.status';

//import { Meta, Title } from '@angular/platform-browser';

//@Component({
//    templateUrl: './appointments-list.component.html'
//})

//export class AppointmentsListComponent implements OnInit, OnDestroy {
//    @ViewChild('changeAppointmentDateModal') public changeDateModal: Modal;
//    @ViewChild('cancelAppointmentModal') public cancelModal: Modal;
//    @ViewChild('acceptAppointmentModal') public acceptModal: Modal;

//    userId: string;    
//    pendingAppointmentsOwner: AppointmentModel[] = new Array<AppointmentModel>();
//    pendingAppointmentsInitiator: AppointmentModel[] = new Array<AppointmentModel>();
//    upcomingAppointmentsOwner: AppointmentModel[] = new Array<AppointmentModel>();
//    upcomingAppointmentsInitiator: AppointmentModel[] = new Array<AppointmentModel>();
//    seenAppointmentsModel: AppointmentModel = <AppointmentModel> {};
//    dateChangedAppointmentModel: AppointmentModel = <AppointmentModel> {};
//    acceptAppointmentModel: AppointmentModel = <AppointmentModel> {};
//    cancelAppointmentModel: CancelAppointmentModel = <CancelAppointmentModel> {};
//    appointmentStatus: any = AppointmentStatus;
        
//    newAppointmentDate: Date = new Date();
//    errorMessage: string;
//    dateChosedIsPast: boolean;
//    count: number;
//    notificationRefresh: number;
//    selectedAppointmentIndex: number;
    
//     constructor(private _appointmentsService: AppointmentsService, 
//                 private _accountService: AccountService,
//                 private _notificationService: NotificationService,
//                 private _metaData: Meta, pageTitle: Title) { 
//        pageTitle.setTitle('Appointments | Horse Spot');
//        _metaData.addTags([
//            { name: 'robots', content: 'NOINDEX, NOFOLLOW'}
//        ]);
//    }

//    ngOnInit() {
//        this.userId = this._accountService.getUserId();        
//        this.getAppointments();
//    }

//    getAppointments() {
//        this._appointmentsService.getUserAppointments(this.userId)
//                                 .subscribe(res => { this.pendingAppointmentsOwner = res.UserPendingWhenOwner;
//                                                     this.pendingAppointmentsInitiator = res.UserPendingWhenInitiator;
//                                                     this.upcomingAppointmentsOwner = res.UserUpcomingWhenOwner;
//                                                     this.upcomingAppointmentsInitiator = res.UserUpcomingWhenInitiator; 
//                                                     this.seenAppointmentsModel.SeenAppointmentsIds = res.UnseenAppointmentIds;
//                                                     this.setSeenAppointments(); },
//                                            error => this.errorMessage = error);
//    }

//    setSeenAppointments() {
//            this.seenAppointmentsModel.UserWhoSeenId = this.userId;
//            this.seenAppointmentsModel.STATUS = AppointmentStatus.SET_SEEN_APPOINTMETS;
//            this._appointmentsService.appointments.next(this.seenAppointmentsModel);
//    }

//    acceptAppointment() {
//        this.acceptAppointmentModel.IsAccepted = true;
//        this.acceptAppointmentModel.STATUS = this.acceptAppointmentModel.IsOwner ? AppointmentStatus.ACCEPTED_BY_OWNER : AppointmentStatus.ACCEPTED_BY_INITIATOR;
//        this._appointmentsService.appointments.next(this.acceptAppointmentModel);

//        if (this.acceptAppointmentModel.IsOwner) {
//            this.upcomingAppointmentsOwner.push(this.acceptAppointmentModel);
//            this.pendingAppointmentsOwner.splice(this.pendingAppointmentsOwner.indexOf(this.acceptAppointmentModel),1);
//        }
//        else {
//            this.upcomingAppointmentsInitiator.push(this.acceptAppointmentModel);
//            this.pendingAppointmentsInitiator.splice(this.pendingAppointmentsInitiator.indexOf(this.acceptAppointmentModel),1);
//        }

//        this.notificationRefresh++;
//        this._notificationService.setRefreshAndText(this.notificationRefresh, this._notificationService.appointmentAcceptedSuccessText());

//        this.hideAcceptAppointmentModal();
//    }

//    openAcceptAppointmentModal(appointment: AppointmentModel) {
//        this.acceptAppointmentModel = appointment;
//        this.acceptAppointmentModel.IsOwner = this.acceptAppointmentModel.AdvertismentOwnerId == this.userId;
//        this.acceptModal.open();        
//    }

//    hideAcceptAppointmentModal() {
//        this.acceptAppointmentModel = <AppointmentModel> { }
//        this.acceptModal.close();
//    }

//    openChangeDateModal(appointment: AppointmentModel, index: number) {
//        this.selectedAppointmentIndex = index;
//        this.dateChangedAppointmentModel = appointment;
//        this.dateChangedAppointmentModel.IsOwner = this.dateChangedAppointmentModel.AdvertismentOwnerId == this.userId;
//        this.changeDateModal.open();
//    }

//    hideChangeDateModal() {
//        this.selectedAppointmentIndex = -1;
//        this.dateChangedAppointmentModel = <AppointmentModel> {};
//        this.changeDateModal.close();
//    }

//    openCancelModal(appointment: AppointmentModel) {
//        this.cancelAppointmentModel.AdOwnerId = appointment.AdvertismentOwnerId;
//        this.cancelAppointmentModel.AdvertismentTitle = appointment.AdvertismentTitle;
//        this.cancelAppointmentModel.AppointmentId = appointment.Id;
//        this.cancelAppointmentModel.InitiatorId = appointment.InitiatorId;
//        this.cancelAppointmentModel.OwnerCanceled = appointment.AdvertismentOwnerId == this.userId;
//        this.cancelModal.open();
//    }

//    hideCancelModal() {
//        this.cancelAppointmentModel = <CancelAppointmentModel> { };
//        this.cancelModal.close();
//    }

//    changeAppointmentDate() {
//        if (this.dateChangedAppointmentModel.STATUS == AppointmentStatus.DATE_CHANGED_BY_OWNER) {
//            this.dateChangedAppointmentModel.STATUS = AppointmentStatus.DATE_CHANGED_BY_INITIATOR;
//        } else {
//            this.dateChangedAppointmentModel.STATUS = AppointmentStatus.DATE_CHANGED_BY_OWNER;
//        }
        
//        this.dateChangedAppointmentModel.AppointmentDateTime = new Date();
//        this.dateChangedAppointmentModel.AppointmentDateTime.setTime(this.newAppointmentDate.getTime() - this.newAppointmentDate.getTimezoneOffset()*60*1000);
//        this._appointmentsService.appointments.next(this.dateChangedAppointmentModel);

//        if (this.dateChangedAppointmentModel.IsOwner) {
//            this.pendingAppointmentsOwner[this.selectedAppointmentIndex].AppointmentDateTime = this.newAppointmentDate;
//            this.pendingAppointmentsOwner[this.selectedAppointmentIndex].STATUS = this.dateChangedAppointmentModel.STATUS;
//        }
//        else {
//            this.pendingAppointmentsInitiator[this.selectedAppointmentIndex].AppointmentDateTime = this.newAppointmentDate;
//            this.pendingAppointmentsInitiator[this.selectedAppointmentIndex].STATUS = this.dateChangedAppointmentModel.STATUS;
//        }

//        this.notificationRefresh++;
//        this._notificationService.setRefreshAndText(this.notificationRefresh, this._notificationService.appointmentDateChangedSuccessText());
        
//        this.hideChangeDateModal();          
//    }

//    cancelAppointment() {
//        this._appointmentsService.cancelAppointment(this.cancelAppointmentModel)
//                                 .subscribe(res => {  this.hideCancelModal();                                            
//                                                      this.getAppointments();
//                                                      this.notificationRefresh++;
//                                                      this._notificationService.setRefreshAndText(this.notificationRefresh, this._notificationService.appointmentCanceledSuccessText());    
//                                                   },
//                                            error => this.errorMessage = error);
       
//    }

//    dateChanged() {
//        this.dateChosedIsPast = this.newAppointmentDate.getTime() < new Date().getTime() - 1000;
//    }

//    ngOnDestroy() {
//        this._metaData.removeTag("name='robots'");
//    }
//}
