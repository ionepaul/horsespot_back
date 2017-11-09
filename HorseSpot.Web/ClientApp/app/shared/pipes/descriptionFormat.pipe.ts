import { PipeTransform, Pipe } from '@angular/core';

@Pipe({
    name: 'descriptionFormat'
})
export class DescriptionFormatPipe implements PipeTransform {

    transform(value: string): string {
        if (value.length > 130) {
            value = value.substring(0, 125).trim() + "...";  
        }

        return value;
    }
}