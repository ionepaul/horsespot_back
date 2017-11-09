export class AppointmentModel { 
        Id: number;
        AdvertismentId: string;
        AdvertismentTitle: string;
        AdvertismentLocation: string;
        TimezoneCapital: string;        
        AppointmentDateTime: Date;
        AdvertismentOwnerId: string;
        OwnerFullName: string;
        OwnerEmail: string;
        OwnerPhoneNumber: string;
        InitiatorId: string;
        InitiatorFullName: string;
        NotificationBarMesseage: string;
        Messeage: string;
        InitiatorEmail: string;
        InitiatorPhoneNumber: string;
        IsAccepted: boolean;
        IsOwner: boolean;
        SeenAppointmentsIds: Array<number>;
        STATUS: string;
        IsPendingResponse: boolean;
        AllSeen: boolean;
        UserWhoSeenId: string;
        IsPendingResponseFromInitiator: boolean;    
}