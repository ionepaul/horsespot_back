import { PipeTransform, Pipe } from '@angular/core';
import * as moment from 'moment';

@Pipe({
    name: 'dateFormatHourPipe'
})
export class DateFormatHourPipe implements PipeTransform {

    transform(value: Date): String {
        return moment.parseZone(value).format('HH:mm');
    }
}