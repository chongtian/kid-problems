import { Component, OnInit } from '@angular/core';
import { ProblemService, MessageService } from '@app/_services';
import { Problem } from '@app/_models';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { NgIf } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';

@Component({
    selector: 'app-update-answer',
    templateUrl: './update-answer.component.html',
    styleUrls: ['./update-answer.component.css'],
    standalone: true,
    imports: [MatCardModule, MatFormFieldModule, MatInputModule, ReactiveFormsModule, FormsModule, NgIf, MatProgressBarModule, MatButtonModule]
})
export class UpdateAnswerComponent implements OnInit {

  answerKeys = '';
  problemCategory = '';
  problemYear = '';
  problemAnswersText = '';
  problemAnswers: Problem[];
  isLoading = false;
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

    this.isLoading = true;
    this.problemService.updateAnswers(this.problemAnswers).then(
      data => {
        let isSuccessful = true;
        if (data != null) {
          this.problemAnswers = data as Problem[];
          let t = '';
          this.problemAnswers.forEach(p => {
            t += p.ProblemTitle + ' ' + p.ProblemAnswer + ' ' + p.ReturnResult + '\n';
            isSuccessful = isSuccessful && p.IsSuccessful;
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

        this.isLoading = false;

      }
    );
  }

}
