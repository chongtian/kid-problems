import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'timezone',
    standalone: true
})
export class TimezoneOffsetPipe implements PipeTransform {
  transform(value: string): string {
    const d = new Date(value);
    const offset = d.getTimezoneOffset() * 60000;
    return new Date(d.getTime() - offset).toISOString();
  }
}
