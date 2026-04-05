import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Access } from '@app/_guards';
import { CognitoService, LoadingBusService } from '@app/_services';
import { BehaviorSubject } from 'rxjs';
import { ExamRunListViewComponent } from '../exam-run-list-view/exam-run-list-view.component';

@Component({
  selector: 'app-exam-run-query',
  templateUrl: './exam-run-query.component.html',
  styleUrls: ['./exam-run-query.component.css'],
  imports: [ExamRunListViewComponent]
})
export class ExamRunQueryComponent {
  startTime: Date;
  endTime: Date;
  queryFamily$ = new BehaviorSubject<boolean>(undefined);
  latest = true;
  private loading = inject(LoadingBusService);

  constructor(
    private route: ActivatedRoute,
    private cognitoService: CognitoService) {
  }

  ngOnInit() {
    this.route.data.subscribe(d => {
      this.latest = d.latest;
      if (d.latest) {
        // show latest exam runs
        this.endTime = new Date();
        this.startTime = new Date();
        this.startTime.setDate(this.endTime.getDate() - 14);
      } else {
        // show exam runs in the past one year
        this.endTime = new Date();
        this.startTime = new Date();
        this.startTime.setDate(this.endTime.getDate() - 365);
      }
    });

    this.loading.start();
    this.cognitoService.getUserAccess()
      .then(a => {
        this.queryFamily$.next(((a | Access.parent) === a));
      })
      .catch(err => { console.log(err); })
      .finally(() => { this.loading.stop(); });
  }

}
