import { AppointmentModel } from './appointmentModel';

export class UserAppointmentsModel {
    UserPendingWhenOwner: AppointmentModel[];
    UserPendingWhenInitiator: AppointmentModel[];
    UserUpcomingWhenOwner: AppointmentModel[];
    UserUpcomingWhenInitiator: AppointmentModel[];
    UnseenAppointmentIds: number[];
}