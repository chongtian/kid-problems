import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { DisplayMessages } from '@app/_constants';
import { ExamRun } from '@app/_models';
import { ExamRunService, LoadingBusService, MessageService } from '@app/_services';
import { BehaviorSubject } from 'rxjs';
import { BooleanLikeToTextPipe } from '@app/_pipes';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { NgClass, DecimalPipe, DatePipe } from '@angular/common';

@Component({
  selector: 'app-exam-run-detail-view',
  templateUrl: './exam-run-detail-view.component.html',
  styleUrls: ['./exam-run-detail-view.component.css'],
  imports: [RouterLink, MatButtonModule, MatTooltipModule, MatTableModule, NgClass, DecimalPipe, DatePipe, BooleanLikeToTextPipe]
})
export class ExamRunDetailViewComponent {
  @Input({ alias: 'entity-id' }) examRunId$ = new BehaviorSubject<string>(null);
  @Output() deleted = new EventEmitter<boolean>();

  data: ExamRun;
  messageTexts = DisplayMessages;
  private loading = inject(LoadingBusService);
  displayedColumns: string[] = ['problemTitle', 'answer', 'isCorrect', 'isGuess', 'duration'];
  canDelete = true;

  constructor(
    private service: ExamRunService,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.examRunId$.subscribe(
      id => {
        this.getExamRun(id);
      }
    );
  }

  private getExamRun(id: string) {
    if (id) {
      this.loading.start();
      this.service.getExamRun(id)
        .then(
          data => {
            if (data != null) {
              this.data = data;
              this.data.ExamRunDetails.forEach(d => {
                if (d.Duration > 0 || d.UserAnswer) {
                  this.canDelete = false;
                  return;
                }
              });

            } else {
              this.messageService.add(`${this.messageTexts.cannotRetrieveRecord} ${id}.`);
              this.data = null;
            }
          }
        )
        .catch(err => { console.log(err); })
        .finally(() => { this.loading.stop(); });
    }
  }

  delete() {
    if (!this.canDelete || !window.confirm(this.messageTexts.confirmDelete)) {
      return;
    }

    this.loading.start();
    this.service.deleteExamRun(this.data.Id)
      .then(
        data => {
          if (data != null && data.IsSuccessful) {
            this.messageService.openSnackBar('Record is deleted');
            this.deleted.emit(true);
          } else {
            this.messageService.openSnackBar(`${this.messageTexts.deleteFailed}.`);
            this.messageService.add(`${this.messageTexts.deleteFailed}. ${data.ReturnResult}`);
          }
        }
      )
      .catch(err => { console.log(err); })
      .finally(() => { this.loading.stop(); });
  }

}
