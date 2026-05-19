import { Component, Input, ViewChild, booleanAttribute, inject } from '@angular/core';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { PaginationIndicator } from '@app/_constants';
import { ExamRun } from '@app/_models';
import { ExamRunService, LoadingBusService, MessageService } from '@app/_services';
import { BehaviorSubject } from 'rxjs';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { NgClass, DecimalPipe, DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

const PageSize = 25;

@Component({
  selector: 'app-exam-run-list-view',
  templateUrl: './exam-run-list-view.component.html',
  styleUrls: ['./exam-run-list-view.component.css'],
  imports: [ReactiveFormsModule, FormsModule, MatFormFieldModule, MatDatepickerModule, MatButtonModule, MatProgressBarModule, MatTableModule, RouterLink, MatPaginatorModule, NgClass, MatTooltipModule, DecimalPipe, DatePipe]
})
export class ExamRunListViewComponent {
  @Input('start-time') startTime: Date | undefined;
  @Input('end-time') endTime: Date | undefined;
  @Input({ alias: 'query-family' }) queryFamily$ = new BehaviorSubject<boolean>(false);
  @Input({ alias: 'pagination', transform: booleanAttribute }) pagination = true;
  @ViewChild(MatPaginator) paginator: MatPaginator | undefined;

  dataSource = new MatTableDataSource<ExamRun>();
  displayedColumns = ['examCategory', 'examTitle', 'cntAllProblem', 'cntCorrect',
    'startTime', 'completeTime', 'duration', 'answerBy'];
  loadMoreData = false;
  private loading = inject(LoadingBusService);
  private paginationToken = PaginationIndicator;
  private currentStartTime: Date | undefined;
  private currentEndTime: Date | undefined;
  private queryFamily = false;
  messageTexts: any;

  constructor(
    private service: ExamRunService,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {
    this.queryFamily$.subscribe(v => {
      if (v !== undefined) {
        this.queryFamily = v;
        if (this.startTime && this.endTime) {
          this.search();
        }
      }
    });
  }

  search() {
    if (!this.startTime || !this.endTime || this.startTime > this.endTime) {
      this.messageService.openSnackBar('Please input a valid range of date.');
      return;
    } else {
      this.currentStartTime = this.startTime;
      this.currentEndTime = this.endTime;
    }

    this.paginationToken = this.pagination === true ? PaginationIndicator : '';
    this.loading.start();
    this.service.queryExamRuns(this.startTime, this.endTime, this.paginationToken, PageSize, this.queryFamily)
      .then(d => {
        d.data.sort((a, b) => { return (b.CreateTime > a.CreateTime) ? 1 : -1; });
        this.dataSource.data = d.data;
        this.dataSource.paginator = this.paginator;
        if (d.data.length > 0) {
          this.paginationToken = d.pagination;
          if (this.pagination === true && this.paginationToken && this.paginationToken !== '{}') {
            this.loadMoreData = true;
          } else {
            this.loadMoreData = false;
          }
        } else {
          // this.messageService.openSnackBar("No record is found.", 250);
        }
      })
      .catch(err => { console.log(err); })
      .finally(() => { this.loading.stop(); });
  }

  more(): void {
    if (this.currentEndTime != this.endTime || this.currentStartTime != this.startTime) {
      this.search();
      return;
    }

    this.loading.start();
    this.service.queryExamRuns(this.startTime!, this.endTime!, this.paginationToken, PageSize, this.queryFamily).then(d => {
      d.data.sort((a, b) => { return (b.CreateTime > a.CreateTime) ? 1 : -1; });
      this.dataSource.data.push(...d.data);
      this.paginationToken = d.pagination;
      if (this.pagination === true && this.paginationToken && this.paginationToken !== '{}') {
        this.loadMoreData = true;
      } else {
        this.loadMoreData = false;
      }
    })
      .catch(err => { console.log(err); })
      .finally(() => { this.loading.stop(); });
  }
}
