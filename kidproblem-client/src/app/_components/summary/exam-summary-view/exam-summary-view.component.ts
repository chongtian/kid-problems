import { Component, inject, OnInit } from '@angular/core';
import { ExamSummary } from '@app/_models';
import { CognitoService, LoadingBusService, SummaryService } from '@app/_services';
import { ExamSummaryDetailViewComponent } from '../exam-summary-detail-view/exam-summary-detail-view.component';


@Component({
  selector: 'app-exam-summary-view',
  templateUrl: './exam-summary-view.component.html',
  styleUrls: ['./exam-summary-view.component.css'],
  imports: [ExamSummaryDetailViewComponent]
})
export class ExamSummaryViewComponent implements OnInit {

  username = '';
  summaries: ExamSummary[] = [];
  private loading = inject(LoadingBusService);

  constructor(
    private cognitoService: CognitoService,
    private service: SummaryService
  ) { }

  async ngOnInit(): Promise<void> {
    this.loading.start();

    try {
      const user = await this.cognitoService.getCurrentAuthenticatedUser();
      this.username = user.username;

      const data = await this.service.queryExamSummaries(this.username, '', 25);

      if (data) {
        this.summaries = data.data;
      }

    } catch (err) {
      console.log(err);
    } finally {
      this.loading.stop();
    }
  }

}
