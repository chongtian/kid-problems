import { Component, inject, ViewChild } from '@angular/core';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { ExamSummary } from '@app/_models';
import { AdminService, LoadingBusService, MessageService, SummaryService } from '@app/_services';
import { NgClass, DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-exam-summary-list-view',
  templateUrl: './exam-summary-list-view.component.html',
  styleUrls: ['./exam-summary-list-view.component.css'],
  imports: [MatTableModule, MatPaginatorModule, NgClass, DecimalPipe]
})
export class ExamSummaryListViewComponent {
  @ViewChild(MatPaginator) paginator: MatPaginator | undefined;

  dataSource = new MatTableDataSource<ExamSummary>();
  displayedColumns = ['examCategory', 'answerBy', 'cntAll', 'cntCorrect', 'cntGuess', 'cntGuessCorrect', 'averageDuration'];
  private loading = inject(LoadingBusService);
  messageTexts: any;

  constructor(
    private service: SummaryService,
    private adminService: AdminService,
    private messageService: MessageService
  ) {
    this.dataSource.data = [];
  }

  async ngOnInit(): Promise<void> {
    this.loading.start();

    try {
      const children = await this.adminService.getChildren();

      if (!children) {
        throw new Error("No kid account is found.");
      }

      const results = await Promise.all(
        children.map(async (child) => {
          const summary = await this.service.queryExamSummaries(child, '', 25);
          return { child, summary };
        })
      );

      results.forEach(({ child, summary }) => {
        if (summary) {
          this.dataSource.data = this.dataSource.data.concat(summary.data);
        } else {
          this.messageService.openSnackBar(
            `Failed to load exam summary for ${child}.`
          );
        }
      });

      this.dataSource.paginator = this.paginator;

    } catch (err: any) {
      console.log(err);
      this.messageService.openSnackBar(err?.message || "Unexpected error");

    } finally {
      this.loading.stop();
    }
  }

}
