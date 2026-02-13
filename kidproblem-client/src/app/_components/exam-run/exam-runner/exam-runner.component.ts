import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ExamRun, ExamRunDetail } from '@app/_models';
import { ExamRunService } from '@app/_services';
import { MessageService } from '@app/_services/message.service';
import { DisplayMessages } from '@app/_constants';
import { BehaviorSubject } from 'rxjs';
import { MatExpansionPanel, MatExpansionModule } from '@angular/material/expansion';
import { BooleanLikeToTextPipe } from '@app/_pipes';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { ProblemDetailViewComponent } from '../../problem/problem-detail-view/problem-detail-view.component';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { NgIf, NgFor, NgClass, DecimalPipe } from '@angular/common';

@Component({
    selector: 'app-exam-runner',
    templateUrl: './exam-runner.component.html',
    styleUrls: ['./exam-runner.component.css'],
    standalone: true,
    imports: [NgIf, MatProgressBarModule, MatDividerModule, MatCardModule, ProblemDetailViewComponent, NgFor, MatButtonModule, NgClass, MatFormFieldModule, MatInputModule, ReactiveFormsModule, FormsModule, MatSlideToggleModule, MatExpansionModule, DecimalPipe, BooleanLikeToTextPipe]
})
export class ExamRunnerComponent implements OnInit {

  @ViewChild(MatExpansionPanel) panel: MatExpansionPanel;
  readyForComplete = false;
  exam: ExamRun;
  isLoading: boolean;
  problemTitle$ = new BehaviorSubject<string>(null);
  currentDetailIndex = 0;
  currentExamRunDetail: ExamRunDetail;
  messageTexts = DisplayMessages;
  private startTime: Date;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private service: ExamRunService,
    private messageService: MessageService,
  ) { }

  ngOnInit() {
    this.isLoading = true;
    this.route.url.subscribe(urls => {
      const action = urls[1].path.toLowerCase();
      this.route.paramMap.subscribe(params => {
        const id = params.get('id');
        if (id) {
          this.getExamRun(id);
        }
      });
    });
  }

  private getExamRun(id: string) {
    this.service.getExamRun(id).then(
      data => {
        if (data != null) {
          this.exam = data;
          // set the current Problem to the first Problem of the exam
          this.problemTitle$.next(this.exam.ExamRunDetails[this.currentDetailIndex].ProblemTitle);
          // set the current ExamRunDetail to the first detail of the exam
          this.currentExamRunDetail = this.exam.ExamRunDetails[this.currentDetailIndex];
          this.startTime = new Date();
        } else {
          this.messageService.add(`${this.messageTexts.cannotRetrieveRecord} ${id}.`);
          this.exam = null;
        }
        this.isLoading = false;
      }
    );
  }

  viewDetail(index: number) {
    if (index !== this.currentDetailIndex) {
      this.updateCurrentExamDetail();

      this.isLoading = true;
      this.problemTitle$.next(this.exam.ExamRunDetails[index].ProblemTitle);
      this.service.getExamRunDetail(this.exam.ExamRunDetails[index].Id).then(d => {
        if (d) {
          this.currentExamRunDetail = d;
          this.exam.ExamRunDetails[index] = d;
        } else {
          this.currentExamRunDetail = this.exam.ExamRunDetails[index];
        }
        this.currentDetailIndex = index;
        this.startTime = new Date();
        this.isLoading = false;
      });

    }
  }

  private updateCurrentExamDetail(){
    this.isLoading = true;
    const diff = (new Date().getTime() - this.startTime.getTime()) / 1000
    this.currentExamRunDetail.Duration += diff;
    const prevIndex = this.currentDetailIndex;
    this.service.updateExamRunDetail(this.currentExamRunDetail).then(d => {
      if (d) {
        this.exam.ExamRunDetails[prevIndex] = d;
      } else{
        this.messageService.openSnackBar('Failed to update answer');
      }
      this.isLoading = false;
    });
  }

  answer(answer: string) {
    this.currentExamRunDetail.UserAnswer = answer;
  }

  reset() {
    if (!window.confirm('Warning: This will delete your current answer.')) {
      return;
    }
    this.currentExamRunDetail.UserAnswer = '';
  }

  // this seems to be redundant
  submit() {
    if (this.currentExamRunDetail.UserAnswer === '') {
      this.messageService.openSnackBar('You do not give any answer.');
    }
    let index = this.currentDetailIndex + 1;
    if (index === this.exam.ExamRunDetails.length) {
      index = this.exam.ExamRunDetails.length - 1;
    }
    this.viewDetail(index);
  }

  reviewBeforeComplete(): void {
    if (!window.confirm(this.messageTexts.confirmSubmit)) {
      return;
    }
    this.updateCurrentExamDetail();
    this.panel.open();
    this.readyForComplete = true;
  }

  complete(action: number) {
    if (action === 0) {
      this.readyForComplete = false;
    } else {
      this.isLoading = true;
        this.exam.CompleteTime = new Date(); // the service will ignore this value
        this.service.completeExamRun(this.exam.Id).then(
          data => {
            this.isLoading = false;
            if (data != null && data.IsSuccessful) {
              this.messageService.openSnackBar('You have completed exam');
              this.router.navigate([`examrun/view/${this.exam.Id}`]);
            } else {
              this.messageService.openSnackBar('Failed to complete exam');
              this.messageService.add(`Failed to complete exam:${this.exam.ReturnResult}.`);
            }    
          }
        );
    }
  }

}
