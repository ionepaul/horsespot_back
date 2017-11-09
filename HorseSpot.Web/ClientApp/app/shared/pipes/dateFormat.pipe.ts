import { PipeTransform, Pipe } from '@angular/core';
import * as moment from 'moment';

@Pipe({
    name: 'dateFormatPipe'
})
export class DateFormatPipe implements PipeTransform {

    transform(value: Date, appointment?: boolean): String {
        if (appointment) {
            return moment.parseZone(value).format('DD MMM');
        }
                
        var date = moment(value).format('DD{0} MMM YYYY');
        var suffix = this.getSuffix(new Date(value).getDate());
        
        return date.replace('{0}', suffix);
    }

    getSuffix(dayNumber: number) {
        if (dayNumber == 1 || (dayNumber % 10) == 1 && dayNumber != 11) {
            return 'st'
        }

        if (dayNumber == 2 || (dayNumber % 10) == 2 && dayNumber != 12) {
            return 'nd'
        }

        if (dayNumber == 3 || (dayNumber % 10) == 3) {
            return 'rd'
        }

        return 'th';
    }
}