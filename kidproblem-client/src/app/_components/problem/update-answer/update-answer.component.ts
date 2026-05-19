import { Component, inject, OnInit } from '@angular/core';
import { ProblemService, MessageService, LoadingBusService } from '@app/_services';
import { Problem } from '@app/_models';
import { MatButtonModule } from '@angular/material/button';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-update-answer',
  templateUrl: './update-answer.component.html',
  styleUrls: ['./update-answer.component.css'],
  imports: [MatCardModule, MatFormFieldModule, MatInputModule, ReactiveFormsModule, FormsModule, MatButtonModule]
})
export class UpdateAnswerComponent implements OnInit {

  answerKeys = '';
  problemCategory = '';
  problemYear = '';
  problemAnswersText = '';
  problemAnswers: Problem[] = [];
  private loading = inject(LoadingBusService);
  cannotSave = true;

  constructor(
    private problemService: ProblemService,
    private messageService: MessageService) { }

  ngOnInit() {
  }

  generate(): void {
    if (this.problemCategory.trim() === '' || this.problemYear.trim() === '' || this.answerKeys.trim() === '') {
      return;
    }

    this.problemAnswersText = '';

    const prefix = this.problemCategory.trim() + '-' + this.problemYear.trim() + '-';

    this.problemAnswers = [];
    this.problemAnswersText = '';
    let t = '';

    this.answerKeys.split(/\r?\n/).forEach((a, index) => {
      if (a) {
        const i = a.indexOf('.');
        let num: string;
        let answerKey: string;

        if (i < 0) {
          num = (index + 1).toString();
          answerKey = a.trim();
        } else {
          num = a.substr(0, i).trim();
          answerKey = a.substr(i + 1).trim();
        }

        const p = new Problem();
        p.ProblemTitle = prefix + num.padStart(3, '0');
        p.ProblemAnswer = answerKey;
        this.problemAnswers.push(p);

        t += p.ProblemTitle + ' ' + p.ProblemAnswer + '\n';
      }
    });

    this.problemAnswersText = t.trim();
    this.cannotSave = false;
  }

  save(): void {

    if (!this.problemAnswers) {
      return;
    }

    if (!window.confirm('Are you sure to save answer keys?')) {
      return;
    }

    this.loading.start();
    this.problemService.updateAnswers(this.problemAnswers)
      .then(
        data => {
          let isSuccessful = true;
          if (data != null) {
            this.problemAnswers = data as Problem[];
            let t = '';
            this.problemAnswers.forEach(p => {
              t += p.ProblemTitle + ' ' + p.ProblemAnswer + ' ' + p.ReturnResult + '\n';
              isSuccessful = isSuccessful && (p.IsSuccessful ?? false);
            });
            this.problemAnswersText = t.trim();
            this.cannotSave = true;
          }

          if (isSuccessful) {
            this.messageService.openSnackBar('Update successfully');
          } else {
            this.messageService.openSnackBar('Error occurred during update Problem Answers');
            this.messageService.add('Error occurred during update Problem Answers');
          }
        }
      )
      .catch(err => { console.log(err); })
      .finally(() => { this.loading.stop(); });
  }

}
