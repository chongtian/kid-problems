import { Component, Input } from '@angular/core';
import { ExamSummary } from '@app/_models';
import { BehaviorSubject } from 'rxjs';
import { MatCardModule } from '@angular/material/card';
import { NgIf, DecimalPipe } from '@angular/common';

@Component({
    selector: 'app-exam-summary-detail-view',
    templateUrl: './exam-summary-detail-view.component.html',
    styleUrls: ['./exam-summary-detail-view.component.css'],
    standalone: true,
    imports: [NgIf, MatCardModule, DecimalPipe]
})
export class ExamSummaryDetailViewComponent  {

  @Input({ alias: 'data' }) data: ExamSummary = null;

  constructor() { }

}
