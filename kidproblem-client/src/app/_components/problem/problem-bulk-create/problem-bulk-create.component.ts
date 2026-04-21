import { Component, inject, OnInit } from '@angular/core';
import { LoadingBusService, ProblemService } from '@app/_services';
import { Problem } from '@app/_models';
import { MatDividerModule } from '@angular/material/divider';
import { MathDirective } from '../../../math/math.directive';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';


@Component({
  selector: 'app-problem-bulk-create',
  templateUrl: './problem-bulk-create.component.html',
  styleUrls: ['./problem-bulk-create.component.css'],
  imports: [MatFormFieldModule, MatInputModule, ReactiveFormsModule, FormsModule, MatButtonModule, MatCardModule, RouterLink, MathDirective, MatDividerModule]
})
export class ProblemBulkCreateComponent implements OnInit {
  problemCategory = 'HOME';
  problemYear = `C${((new Date()).getMonth()+1).toString().padStart(2,'0')}${(new Date()).getDate().toString().padStart(2,'0')}`;
  startProblemNumber = 1;
  answerOptions = 'A,B,C,D';
  rawText = '';
  message = '';
  isSubmitted = false;
  problems: Problem[] = [];
  private loading = inject(LoadingBusService);

  constructor(
    private service: ProblemService) { }

  ngOnInit() {
    this.isSubmitted = false;
  }

  validate(): void {
    this.message = '';

    if (this.rawText == null || this.problemCategory == null || this.problemYear == null) {
      this.message = 'Problem Category, Problem Year, and Raw Json Text are required.';
    }

    this.problems = this.parseProblems(this.rawText);
  }

  save(): void {
    if (!this.problems) {
      return;
    }

    if (!window.confirm('Are you sure to continue?')) {
      return;
    }

    this.isSubmitted = false;
    this.loading.start();
    this.service.bulkCreate(this.problems)
      .then(data => {
        if (data != null) {
          this.problems = data;
          this.isSubmitted = true;
        } else {
          this.problems = [];
          this.message = 'The problems are not saved by API service.';
        }

      })
      .catch(err => {
        console.log(err);
        this.message = 'The problems are not saved by API service.';
      })
      .finally(() => { this.loading.stop(); });
  }

  private parseProblems(text: string): Problem[] {
    try {
      let rawProblems = JSON.parse(text);

      if (!Array.isArray(rawProblems)) {
        this.message = 'Raw Json Text must be a JSON array.';
        return [];
      }

      const problems: Problem[] = [];
      let count = 0;
      rawProblems.forEach(item => {
        if (item.ProblemText) {
          const problemNumber = (this.startProblemNumber + count).toString().padStart(3, '0');
          const problem: Problem = {
            ProblemCategory: this.problemCategory,
            ProblemYear: this.problemYear,
            ProblemTitle: `${this.problemCategory}-${this.problemYear}-${problemNumber}`,
            ProblemNumber: problemNumber,
            ProblemText: item.ProblemText,
            ProblemAnswer: item.ProblemAnswer,
            ProblemTags: [],
            IsStaging: true,
            SolutionText: '',
            AnswerOptions: item.AnswerOptions ?? this.answerOptions
          };
          count++;
          problems.push(problem);
        }
      });

      return problems;

    } catch (err) {
      console.log(err);
      this.message = 'Raw Json Text is invalid.';
      return [];
    }
  }

  reset(): void {
    this.isSubmitted = false;
    this.problems = [];
    this.rawText = '';
    this.message = '';
  }

}
