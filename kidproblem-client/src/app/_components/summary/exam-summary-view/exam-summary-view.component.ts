import { Component, OnInit } from '@angular/core';
import { ExamSummary } from '@app/_models';
import { CognitoService, SummaryService } from '@app/_services';
import { ExamSummaryDetailViewComponent } from '../exam-summary-detail-view/exam-summary-detail-view.component';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { NgIf, NgFor } from '@angular/common';

@Component({
    selector: 'app-exam-summary-view',
    templateUrl: './exam-summary-view.component.html',
    styleUrls: ['./exam-summary-view.component.css'],
    standalone: true,
    imports: [NgIf, MatProgressBarModule, NgFor, ExamSummaryDetailViewComponent]
})
export class ExamSummaryViewComponent implements OnInit {

  username = '';
  summaries: ExamSummary[] = [];
  isLoading = false;

  constructor(
    private cognitoService: CognitoService,
    private service: SummaryService
  ) {

  }

  ngOnInit() {
    this.isLoading = true;
    this.cognitoService.getCurrentAuthenticatedUser().then(
      user => {
        this.username = user.username;
        // this.username = 'yinkaigao'; // test
        this.service.queryExamSummaries(this.username, '', 25).then(
          data => {
            if (data) {
              this.summaries = data.data;
            }
          }
        );

        this.isLoading = false;
      }
    );
  }

}
