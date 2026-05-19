import { Component, EventEmitter, Input, OnInit, Output, ViewChild, booleanAttribute, inject } from '@angular/core';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { DisplayMessages, PaginationIndicator } from '@app/_constants';
import { Assignment } from '@app/_models';
import { AssignmentService, CognitoService, ExamRunService, LoadingBusService, MessageService } from '@app/_services';
import { BooleanLikeToTextPipe } from '@app/_pipes';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';
import { NgClass, DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Access } from '@app/_guards';

const PageSize = 25;

@Component({
  selector: 'app-assignment-list-view',
  templateUrl: './assignment-list-view.component.html',
  styleUrls: ['./assignment-list-view.component.css'],
  imports: [ReactiveFormsModule, FormsModule, MatFormFieldModule, MatDatepickerModule, MatButtonModule, MatPaginatorModule, MatTableModule, RouterLink, NgClass, MatTooltipModule, DatePipe, BooleanLikeToTextPipe]
})
export class AssignmentListViewComponent implements OnInit {
  @Input('start-time') startTime: Date | undefined;
  @Input('end-time') endTime: Date | undefined;
  @Input({ alias: 'pagination', transform: booleanAttribute }) pagination = true;
  @ViewChild(MatPaginator) paginator: MatPaginator | undefined;
  @Output() run = new EventEmitter<string>();

  dataSource = new MatTableDataSource<Assignment>();
  displayedColumns = ['action', 'createTime', 'category', 'title', 'complete', 'memo'];
  loadMoreData = false;
  private loading = inject(LoadingBusService);
  private paginationToken = PaginationIndicator;
  private currentStartTime: Date | undefined;
  private currentEndTime: Date | undefined;
  private parentUser = false;
  messageTexts = DisplayMessages;

  constructor(
    private service: AssignmentService,
    private messageService: MessageService,
    private examRunService: ExamRunService,
    private cognitoService: CognitoService
  ) { }

  ngOnInit(): void {

    // if the parent component pass a keyword, trigger search immediately
    if (this.startTime && this.endTime) {
      this.search();
    }

    this.loading.start();
    this.cognitoService.getUserAccess().then(a => {
      this.parentUser = ((a | Access.parent) === a);
    }).catch(err => { console.log(err); })
      .finally(() => { this.loading.stop(); });
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
    this.service.queryAssignments(this.startTime, this.endTime, this.paginationToken, PageSize)
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
    this.service.queryAssignments(this.startTime!, this.endTime!, this.paginationToken, PageSize)
      .then(d => {
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

  createRunFromAssignment(e: Assignment) {
    if (this.parentUser === true) {
      this.messageService.openSnackBar(`${this.messageTexts.onlyForChild}`);
      this.messageService.add(`${this.messageTexts.onlyForChild}`);
    } else {
      this.loading.start();
      this.examRunService.createExamRunFromAssignment(e.Id)
        .then(
          data => {
            if (data && data.IsSuccessful) {
              this.run.emit(data.Id);
            } else {
              this.messageService.openSnackBar(`${this.messageTexts.saveFailed}`);
              this.messageService.add(`${this.messageTexts.saveFailed}: ${data.ReturnResult}`);
            }
          }
        )
        .catch(err => { console.log(err); })
        .finally(() => { this.loading.stop(); });
    }

  }

}


