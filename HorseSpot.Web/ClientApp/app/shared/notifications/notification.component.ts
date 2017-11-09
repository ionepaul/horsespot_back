import { Component, Input, OnChanges, trigger, state, style, transition, animate } from '@angular/core';

@Component({
  selector: 'app-notification',
  template: `<div *ngIf="refresh != undefined" class="notification-bar" [@notificationShow]="state">
                    <div class="logo"><img src="../../../assets/images/logo_notif.png" alt="Logo"></div>
                    <div class="message"><p>{{text}}</p></div>
               </div>`,
  animations: [
    trigger('notificationShow', [
      state('out', style({ opacity: 0, transform: 'translate(0%, -200%)' })),
      transition('in => out', [
        style({ transform: 'translate(0%, 15%)' }),
        animate(800)
      ]),
      state('in', style({ opacity: 1, transform: 'translate(0%, 15%)' })),
      transition('out => in', [
        style({ transform: 'translate(0%, -200%)' }),
        animate(800)
      ])
    ])
  ]
})

export class NotificationComponent implements OnChanges {
  state: string = "out";
  @Input() text: string;
  @Input() refresh: number;

  ngOnChanges() {
    if (this.refresh != 0 && this.refresh != undefined) {
      this.state = (this.state == 'in' ? 'out' : 'in');
      setTimeout(() => {
        this.state = (this.state == 'in' ? 'out' : 'in');;
      }, 3500);
    }
  }
}
