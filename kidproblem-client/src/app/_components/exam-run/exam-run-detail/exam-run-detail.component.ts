import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DisplayMessages } from '@app/_constants';
import { BehaviorSubject } from 'rxjs';
import { ExamRunDetailViewComponent } from '../exam-run-detail-view/exam-run-detail-view.component';

@Component({
    selector: 'app-exam-run-detail',
    templateUrl: './exam-run-detail.component.html',
    styleUrls: ['./exam-run-detail.component.css'],
    imports: [ExamRunDetailViewComponent]
})
export class ExamRunDetailComponent {
  examRunId$ = new BehaviorSubject<string>(null);
  messageTexts = DisplayMessages;

  constructor(
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit() {
    this.route.url.subscribe(urls => {
      const action = urls[1].path.toLowerCase();
      this.route.paramMap.subscribe(params => {
        const id = params.get('id');
        if (id) {
          this.examRunId$.next(id)
        }
      });
    });
  }

  onDeleted(event: boolean): void {
    if (event) {
      this.router.navigate([`examruns`]);
    }
  }

}
