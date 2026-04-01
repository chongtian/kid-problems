import { Component, Input, booleanAttribute, inject } from '@angular/core';
import { DisplayMessages } from '@app/_constants';
import { Problem } from '@app/_models';
import { ProblemService, MessageService, LoadingBusService } from '@app/_services';
import { BehaviorSubject } from 'rxjs';
import { BooleanLikeToTextPipe } from '@app/_pipes';
import { ProblemSummaryViewComponent } from '../../summary/problem-summary-view/problem-summary-view.component';
import { MathDirective } from '../../../math/math.directive';


@Component({
  selector: 'app-problem-detail-view',
  templateUrl: './problem-detail-view.component.html',
  styleUrls: ['./problem-detail-view.component.css'],
  imports: [MathDirective, ProblemSummaryViewComponent, BooleanLikeToTextPipe]
})
export class ProblemDetailViewComponent {
  @Input({ alias: 'display-all', transform: booleanAttribute }) displayAll = true;
  @Input({ alias: 'problem-title' }) problemTitle$ = new BehaviorSubject<string>('');

  problem: Problem;
  messageTexts = DisplayMessages;
  private loading = inject(LoadingBusService);
  AnswerOptions: string[] = []; // this property is used by exam-runner component

  constructor(
    private problemService: ProblemService,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.problemTitle$.subscribe(
      problemTitle => {
        this.getProblem(problemTitle);
      }
    );
  }

  private getProblem(problemTitle: string) {
    this.loading.start();
    this.problemService.getProblem(problemTitle)
      .then(
        data => {
          if (data != null) {
            this.problem = data;
            if (this.problem.AnswerOptions) {
              this.AnswerOptions = this.problem.AnswerOptions.split(',');
            }
          } else {
            this.messageService.add(`${this.messageTexts.cannotRetrieveRecord} ${problemTitle}.`);
            this.problem = null;
          }
        }
      )
      .catch(err => { console.log(err); })
      .finally(() => { this.loading.stop(); });
  }

}
