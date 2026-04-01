import { Component, OnInit, Input } from '@angular/core';
import { ExamDefinitionId } from '@app/_models';
import { ExamDefinitionService } from '@app/_services';
import { BehaviorSubject } from 'rxjs';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatButtonModule } from '@angular/material/button';
import { ProblemDetailViewComponent } from '../../problem/problem-detail-view/problem-detail-view.component';


@Component({
    selector: 'app-exam-print',
    templateUrl: './exam-print.component.html',
    styleUrls: ['./exam-print.component.css'],
    imports: [ProblemDetailViewComponent, MatButtonModule, MatTooltipModule]
})
export class ExamPrintComponent implements OnInit {

  problemTitles$: BehaviorSubject<string>[] = [];
  @Input({ alias: 'entity-id' }) examDefId$ = new BehaviorSubject<ExamDefinitionId>(null);
  readyToRender = false;

  constructor(
    private examDefService: ExamDefinitionService
  ) {
  }

  ngOnInit(): void {
    this.examDefId$.subscribe(
      examDefId => {
        this.examDefService.getExamDefinition(examDefId.ExamCategory, examDefId.ExamTitle).then(data => {
          data.ExamDetails.forEach(
            detail => {
              const problemTitle$ = new BehaviorSubject<string>(detail.ProblemTitle);
              this.problemTitles$.push(problemTitle$);
            }
          );
          this.readyToRender = true;
        });
      }
    );
  }

  print() {
    window.print();
  }

}
