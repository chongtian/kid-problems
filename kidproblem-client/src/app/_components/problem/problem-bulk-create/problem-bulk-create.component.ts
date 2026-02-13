import { Component, OnInit } from '@angular/core';
import { ProblemService } from '@app/_services';
import { Problem } from '@app/_models';
import { MatDividerModule } from '@angular/material/divider';
import { MathDirective } from '../../../math/math.directive';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { NgIf, NgFor } from '@angular/common';

@Component({
    selector: 'app-problem-bulk-create',
    templateUrl: './problem-bulk-create.component.html',
    styleUrls: ['./problem-bulk-create.component.css'],
    standalone: true,
    imports: [NgIf, MatFormFieldModule, MatInputModule, ReactiveFormsModule, FormsModule, MatButtonModule, MatProgressSpinnerModule, NgFor, MatCardModule, RouterLink, MathDirective, MatDividerModule]
})
export class ProblemBulkCreateComponent implements OnInit {
  startUrl: string;
  problemCategory: string;
  problemYear: string;
  answerOptions = 'A,B,C,D,E';
  rawText: string;
  message: string;
  isSubmitted: boolean;
  problems: Problem[];
  isLoading: boolean;

  constructor(
    private service: ProblemService) { }

  ngOnInit() {
    this.isSubmitted = false;
    this.isLoading = false;
  }

  parse(): void {
    if (this.rawText == null || this.problemCategory == null || this.problemYear == null) {
      return;
    }
    if (!window.confirm('Are you sure to continue?')) {
      return;
    }
    this.isLoading = true;

    const problems = this.parseProblems(this.rawText);
    if (!problems) {
      this.message = "Failed to parse the json text.";
    } else {
      this.service.bulkCreate(problems).then(data => {
        if (data != null) {
            this.problems = data;
        } else {
          this.problems = [];
        }
        this.isLoading = false;
        this.isSubmitted = true;
      });
    }
  }

  private parseProblems(text: string): Problem[] {
    try {
      let obj = JSON.parse(text);
      const items = obj.response.category.items;
      const problems = [];
      let count = 1;
      items.forEach(item => {
        const categoryId = +item.post_data.category_id;
        const problemNumber: string = item.item_text;
        if (problemNumber && categoryId !== 75) {
          // category id 75 is "Global Announcements" of AoPS. 
          const problemText: string = item.post_data.post_rendered.replace('\"', '"');
          const problemYear = problemNumber == count.toString() ? this.problemYear : (this.problemYear + 'B');
          const problem: Problem = {
            ProblemCategory: this.problemCategory,
            ProblemYear: problemYear,
            ProblemTitle: `${this.problemCategory}-${problemYear}-${problemNumber.padStart(3, '0')}`,
            ProblemNumber: problemNumber,
            ProblemText: problemText,
            ProblemAnswer: null,
            ProblemTags: null,
            IsStaging: true,
            SolutionText: null,
            AnswerOptions: this.answerOptions
          };
          count++;
          problems.push(problem);
        }
      });
      return problems;
    } catch (err) {
      console.log(err);
      return null;
    }
  }

  reset(): void {
    this.isSubmitted = false;
    this.isLoading = false;
    this.problems = [];
    this.rawText = null;
    this.message = null;
  }

}
