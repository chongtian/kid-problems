import { Component, Input } from '@angular/core';
import { ExamSummary } from '@app/_models';
import { MatCardModule } from '@angular/material/card';
import { DecimalPipe } from '@angular/common';

@Component({
    selector: 'app-exam-summary-detail-view',
    templateUrl: './exam-summary-detail-view.component.html',
    styleUrls: ['./exam-summary-detail-view.component.css'],
    imports: [MatCardModule, DecimalPipe]
})
export class ExamSummaryDetailViewComponent  {

  @Input({ alias: 'data' }) data: ExamSummary = null;

  constructor() { }

}
