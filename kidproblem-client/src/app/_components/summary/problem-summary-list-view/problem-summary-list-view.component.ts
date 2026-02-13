import { Component, Input, OnInit, ViewChild, booleanAttribute } from '@angular/core';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { PaginationIndicator } from '@app/_constants';
import { ExamSummary, InfoCentralCodeDetail, Problem, ProblemSummary } from '@app/_models';
import { AdminService, MessageService, SummaryService } from '@app/_services';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatOptionModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NgIf, NgFor, NgClass, DecimalPipe } from '@angular/common';

const PageSize = 50;

@Component({
    selector: 'app-problem-summary-list-view',
    templateUrl: './problem-summary-list-view.component.html',
    styleUrls: ['./problem-summary-list-view.component.css'],
    standalone: true,
    imports: [NgIf, ReactiveFormsModule, FormsModule, MatFormFieldModule, MatSelectModule, NgFor, MatOptionModule, MatInputModule, MatButtonModule, MatProgressBarModule, MatTableModule, MatCheckboxModule, RouterLink, MatPaginatorModule, NgClass, MatTooltipModule, MatSlideToggleModule, DecimalPipe]
})
export class ProblemSummaryListViewComponent implements OnInit {
  @Input('category') category = '';
  @Input('answer-by') answerBy = '';
  @Input('correct-rate') trueCorrectRateRng = '0.0-1.0';
  @Input('keyword') keyword = '';
  @Input({ alias: 'is-selectable', transform: booleanAttribute }) isSelectable = false;
  @Input({ alias: 'pagination', transform: booleanAttribute }) pagination = true;

  data: ProblemSummary[];
  selectedIds: string[];
  categories: InfoCentralCodeDetail[];
  children: string[];

  @ViewChild(MatPaginator) paginator: MatPaginator;
  dataSource = new MatTableDataSource<ExamSummary>();
  displayedColumns = ['select', 'problemTitle', 'answerBy', 'trueCorrectRate', 'cntAll', 'cntCorrect', 'cntGuess', 'cntGuessCorrect', 'averageDuration'];

  private paginationToken = PaginationIndicator;
  private currentCategory = '';
  private currentKeyword = '';
  private currentAnswerBy = '';
  private currentTrueCorrectRateRng = '0.0-1.0';
  loadMoreData = false;
  isLoading = false;
  private selectedAll = false;

  constructor(
    private service: SummaryService,
    private adminService: AdminService,
    private messageService: MessageService) {
    this.data = [];
    this.selectedIds = [];
  }

  ngOnInit(): void {
    // if all filter criteria are given, do search immediately. Otherwise, let user do more click.
    if (this.category && this.answerBy && this.currentTrueCorrectRateRng && this.keyword) {
      this.search();
    }

    this.adminService.getCategoryCodes().then(
      data => {
        this.categories = data.filter(d => { return d.Active; });
      }
    );

    this.adminService.getChildren().then(
      data => {
        this.children = data;
      }
    );

    if (!this.isSelectable) {
      this.displayedColumns.splice(0, 1);
    }
  }

  search() {
    if (!this.category) {
      this.messageService.openSnackBar("Category is required.");
      return;
    } else {
      this.currentCategory = this.category;
      this.currentAnswerBy = this.answerBy;
      this.currentTrueCorrectRateRng = this.trueCorrectRateRng;
      this.currentKeyword = this.keyword;
    }

    this.paginationToken = this.pagination === true ? PaginationIndicator : '';
    this.isLoading = true;
    this.service.queryProblemSummaries(this.category, this.answerBy, this.keyword, this.trueCorrectRateRng, this.paginationToken, PageSize).then(d => {
      this.data = d.data;
      if (!this.pagination) {
        this.data.sort((x, y) => { return y.TotalCount - x.TotalCount });
      }
      this.dataSource.data = this.data;
      this.dataSource.paginator = this.paginator;
      if (d.data.length > 0) {
        this.paginationToken = d.pagination;
        if (this.pagination === true && this.paginationToken && this.paginationToken !== '{}') {
          this.loadMoreData = true;
        } else {
          this.loadMoreData = false;
        }
      } else {
        this.messageService.openSnackBar("No record is found.");
      }
      this.isLoading = false;
    });
  }

  more(): void {
    if (this.currentCategory != this.category
      || this.keyword != this.currentKeyword
      || this.currentAnswerBy != this.answerBy
      || this.currentTrueCorrectRateRng != this.trueCorrectRateRng) {
      this.search();
    }
    this.isLoading = true;
    this.service.queryProblemSummaries(this.category, this.answerBy, this.keyword, this.trueCorrectRateRng, this.paginationToken, PageSize).then(d => {
      this.data.push(...d.data);
      this.dataSource.data = this.data;
      this.paginationToken = d.pagination;
      if (this.pagination === true && this.paginationToken && this.paginationToken !== '{}') {
        this.loadMoreData = true;
      } else {
        this.loadMoreData = false;
      }
      this.isLoading = false;
    });
  }

  toggle(element: any): void {
    const i = this.selectedIds.indexOf(element.ProblemTitle);
    if (i >= 0) {
      this.selectedIds.splice(i, 1);
    } else {
      this.selectedIds.push(element.ProblemTitle);
    }
  }

  toggleAll(): void {
    this.selectedIds = [];
    if (this.selectedAll) {
      this.selectedAll = false;
    } else {
      this.data.forEach(d => {
        this.selectedIds.push(d.ProblemTitle);
      });
      this.selectedAll = true;
    }
  }

  /**
   * This function set the checked attribute of checkbox.
   * It gets the Problem from html and compares it with selectedIds.
   */
  isChecked(element: Problem) {
    return this.selectedIds.includes(element.ProblemTitle);
  }
}
