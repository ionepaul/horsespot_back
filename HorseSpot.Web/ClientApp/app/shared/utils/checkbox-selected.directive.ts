import { Directive, forwardRef, Attribute } from '@angular/core';
import { Validator, AbstractControl, NG_VALIDATORS } from '@angular/forms';

@Directive({
  selector: '[mustBeChecked][formControlName],[mustBeChecked][formControl],[mustBeChecked][ngModel]',
  providers: [
    { provide: NG_VALIDATORS, useExisting: forwardRef(() => CheckboxSelectedValidator), multi: true }
  ]
})
export class CheckboxSelectedValidator implements Validator {

  constructor() { }

  validate(c: AbstractControl): { [key: string]: any } {
    // self value
    let v = c.value;

    // value not true
    if (v !== true) {
      return {
        validateEqual: false
      }
    }

    return null;
  }
}
