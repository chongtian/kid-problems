import { Component, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { CognitoService } from '@app/_services';
import { Access } from '@app/_guards';
import { Router } from '@angular/router';
import { ExamRunListViewComponent } from '../exam-run/exam-run-list-view/exam-run-list-view.component';
import { AssignmentListViewComponent } from '../assignment/assignment-list-view/assignment-list-view.component';
import { MatDividerModule } from '@angular/material/divider';
import { ExamSummaryListViewComponent } from '../summary/exam-summary-list-view/exam-summary-list-view.component';
import { ExamSummaryViewComponent } from '../summary/exam-summary-view/exam-summary-view.component';
import { NgIf } from '@angular/common';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css'],
    standalone: true,
    imports: [NgIf, ExamSummaryViewComponent, ExamSummaryListViewComponent, MatDividerModule, AssignmentListViewComponent, ExamRunListViewComponent]
})
export class HomeComponent implements OnInit {

  startTime: Date;
  endTime: Date;
  isLoading = true;
  parentUser = false;
  queryFamily$ = new BehaviorSubject<boolean>(undefined);

  constructor(
    private cognitoService: CognitoService,
    private router: Router
  ) {
    this.endTime = new Date();
    this.startTime = new Date();
    this.startTime.setDate(this.endTime.getDate() - 14);
  }

  ngOnInit(): void {
    this.isLoading = true;
    this.cognitoService.getUserAccess().then(a => {
      this.parentUser = ((a | Access.parent) === a);
      this.queryFamily$.next(this.parentUser);
    });

  }

  onRun(event: string) {
    this.router.navigate([`examrun/run/${event}`]);
  }

}
