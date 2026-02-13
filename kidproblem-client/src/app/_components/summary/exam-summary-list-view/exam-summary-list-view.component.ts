import { Component, ViewChild } from '@angular/core';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { ExamSummary } from '@app/_models';
import { AdminService, MessageService, SummaryService } from '@app/_services';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { NgIf, NgClass, DecimalPipe } from '@angular/common';

@Component({
    selector: 'app-exam-summary-list-view',
    templateUrl: './exam-summary-list-view.component.html',
    styleUrls: ['./exam-summary-list-view.component.css'],
    standalone: true,
    imports: [NgIf, MatProgressBarModule, MatTableModule, MatPaginatorModule, NgClass, DecimalPipe]
})
export class ExamSummaryListViewComponent {
  @ViewChild(MatPaginator) paginator: MatPaginator;

  dataSource = new MatTableDataSource<ExamSummary>();
  displayedColumns = ['examCategory', 'answerBy', 'cntAll', 'cntCorrect', 'cntGuess', 'cntGuessCorrect', 'averageDuration'];
  isLoading = false;
  messageTexts: any;
  private children: string[] = [];

  constructor(
    private service: SummaryService,
    private adminService: AdminService,
    private messageService: MessageService
  ) {
    this.dataSource.data = [];
  }

  ngOnInit(): void {
    this.isLoading = true;
    this.adminService.getChildren().then(children => {
      if (children) {
        this.children = children;
        this.children.forEach(child => {
          this.service.queryExamSummaries(child, '', 25).then(
            summary => {
              if (summary) {
                this.dataSource.data = this.dataSource.data.concat([...summary.data]);
                this.dataSource.paginator = this.paginator;
              } else {
                this.messageService.openSnackBar(`Failed to load exam summary for ${child}.`);
              }
            }
          );
        });
      } else {
        this.messageService.openSnackBar("API Error. No kid account is found.");
      }
      this.isLoading = false;
    });

  }


}
