import { Component, OnInit } from '@angular/core';
import { ProblemService } from '@app/_services';
import { Problem, CrawlProblemDefinition } from '@app/_models';
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
    selector: 'app-problem-scrap',
    templateUrl: './problem-scrap.component.html',
    styleUrls: ['./problem-scrap.component.css'],
    standalone: true,
    imports: [NgIf, MatFormFieldModule, MatInputModule, ReactiveFormsModule, FormsModule, MatButtonModule, MatProgressSpinnerModule, NgFor, MatCardModule, RouterLink, MathDirective, MatDividerModule]
})
export class ProblemScrapComponent implements OnInit {
  startUrl: string;
  problemCategory: string;
  problemYear: string;
  // regexPattern: string;
  startPattern = '<h2>.+?Problem\\s\\d{1,}.+?<\\/h2>';
  endPattern = '<a.+?>Solution<\\/a>';
  isSubmitted: boolean;
  problems: Problem[];
  isLoading: boolean;

  constructor(
    private service: ProblemService) { }

  ngOnInit() {
    this.isSubmitted = false;
    this.isLoading = false;
  }

  crawl(): void {
    if (this.startUrl == null || this.problemCategory == null || this.problemYear == null) {
      return;
    }
    if (!window.confirm('Are you sure to continue?')) {
      return;
    }
    const def: CrawlProblemDefinition = {
      StartUrl: this.startUrl,
      ProblemCategory: this.problemCategory,
      ProblemYear: this.problemYear,
      RegexPattern: null,
      StartPattern: this.startPattern,
      EndPattern: this.endPattern,
      UseSinglePattern: false
    };
    this.isLoading = true;
    this.isSubmitted = true;
    this.service.crawlProblems(def).then(data => {
      if (data != null) {
        this.problems = data;
      } else {
        this.problems = [];
      }
      this.isLoading = false;
    });
  }

  reset(): void {
    this.isSubmitted = false;
    this.isLoading = false;
    this.problems = [];
  }

}
