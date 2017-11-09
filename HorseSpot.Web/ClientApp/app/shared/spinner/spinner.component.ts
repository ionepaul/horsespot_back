import { Component } from '@angular/core';

import { SpinnerService } from './spinner.service';

@Component({
    selector: 'spinner',
    template: `<div *ngIf="_spinnerService.isLoading" class="spinner-background">
                    <div class="spinner">
                        <div class="loader">{{'SPINNER_LOADING' | translate}}</div>
                        <span>{{'SPINNER_LOADING' | translate}}</span>
                    </div>
                </div>`
})

export class SpinnerComponent { 
    
    constructor(public _spinnerService: SpinnerService) { }
}