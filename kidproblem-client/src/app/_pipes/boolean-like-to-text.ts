import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'booleanLikeToText',
    standalone: true
})
export class BooleanLikeToTextPipe implements PipeTransform {
  transform(value: boolean): string {
    if (value === true) {
      return 'Yes';
    } else {
      return 'No';
    }
  }
}
