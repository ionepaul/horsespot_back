import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs/Rx';
import { Http, Response} from '@angular/http';
import { Router } from '@angular/router';
import { WebSocketService } from './websocket.service';
import { CONFIG } from '../../config';
import { UserAppointmentsModel } from '../models/userAppointmentsModel';
import { AppointmentModel } from '../models/appointmentModel';
import { CancelAppointmentModel } from '../models/cancelAppointmentModel';
import { AuthService } from '../../shared/auth/auth.service';

@Injectable()
export class AppointmentsService {
	public appointments: Subject<AppointmentModel>;

    private _userAppointmentsUrl: string = CONFIG.baseUrls.apiUrl + 'appointments/user/';
	private _cancelAppointmentUrl: string = CONFIG.baseUrls.apiUrl + 'appointments/cancel';
    private _unseenAppointmentsUrl: string = CONFIG.baseUrls.apiUrl + 'appointments/unseen/';

	constructor(private _wsService: WebSocketService, 
                private _http: Http, 
                private _router: Router, 
                private _authService: AuthService) { }

    webSocketConnect(userId: string) {
        this.appointments = <Subject<AppointmentModel>>this._wsService
			.connect(CONFIG.webSocketUrl + userId)
			.map((response: MessageEvent) => {
				let data = JSON.parse(response.data);
				return data;
			});
    }

    getUnseenAppointmentsForUser(userId: string) {
        return this._http.get(this._unseenAppointmentsUrl + userId)
                  .map((res: Response) => res.json() as AppointmentModel[])
                  .catch(error => { 
                            if (error && error.status === 401 && this._authService.isTokenExpired()) {
                                return this._authService.refreshToken().flatMap((data) => {
                                    this._authService.storeUserAccessInfo(data);
                                    return this._http.get(this._unseenAppointmentsUrl + userId)
                                                     .map((res: Response) => res.json() as AppointmentModel[])
                                                     .catch(this.handleError);
                                })
                            } else if (error && error.status === 401) {
                                this._router.navigate(['/error/401']);
                            }       
                            else {
                                return this.handleError(error);  
                            }
                         });
    }

	getUserAppointments(userId: string) {
		return this._http.get(this._userAppointmentsUrl + userId)
                  .map((res: Response) => res.json() as UserAppointmentsModel)
                  .catch(error => { 
                            if (error && error.status === 401 && this._authService.isTokenExpired()) {
                                return this._authService.refreshToken().flatMap((data) => {
                                    this._authService.storeUserAccessInfo(data);
                                    return this._http.get(this._userAppointmentsUrl + userId)
                                                     .map((res: Response) => res.json() as UserAppointmentsModel)
                                                     .catch(this.handleError);
                                })
                            } else if (error && error.status === 401) {
                                this._router.navigate(['/error/401']);
                            }       
                            else {
                                return this.handleError(error);  
                            }
                         });
	}

	cancelAppointment(cancelAppointmentModel: CancelAppointmentModel) {
		let body = JSON.stringify(cancelAppointmentModel);

        return this._http.post(this._cancelAppointmentUrl, body)
                         .map((res: Response) => res.json())
                         .catch(error => { 
                            if (error && error.status === 401 && this._authService.isTokenExpired()) {
                                return this._authService.refreshToken().flatMap((data) => {
                                    this._authService.storeUserAccessInfo(data);
                                    return this._http.post(this._cancelAppointmentUrl, body)
                                                     .map((res: Response) => res.json())
                                                     .catch(this.handleError);
                                })
                            } else if (error && error.status === 401) {
                                this._router.navigate(['/error/401']);
                            }       
                            else {
                                return this.handleError(error); 
                            }
                         });
	}

	private handleError(error: Response) {
        let errorMsg = error.json().Message || "Server Error";
        return Observable.throw(errorMsg);
    }
}