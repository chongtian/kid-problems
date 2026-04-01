import { Component, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Problem } from '@app/_models';
import { ProblemService, MessageService, LoadingBusService } from '@app/_services';
import { ProblemSearchDialogComponent } from '@app/_components';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-problem-bulk-update',
  templateUrl: './problem-bulk-update.component.html',
  styleUrls: ['./problem-bulk-update.component.css'],
  imports: [MatFormFieldModule, MatInputModule, ReactiveFormsModule, FormsModule, MatButtonModule, RouterLink, NgClass]
})
export class ProblemBulkUpdateComponent {

  private loading = inject(LoadingBusService);
  queryKeyword = '';
  problems: Problem[];
  problemTitles: string[];

  constructor(
    private service: ProblemService,
    private messageService: MessageService,
    private query: MatDialog) {
    this.problemTitles = [];
    this.problems = [];
  }

  openQuery(): void {
    const dialogRef = this.query.open(ProblemSearchDialogComponent, {
      data: { keyword: this.queryKeyword, isStaging: true }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const problems = result as Problem[];
        problems.forEach(p => {
          if (!this.problemTitles.includes(p.ProblemTitle)) {
            this.problems.push(p);
            this.problemTitles.push(p.ProblemTitle);
          }
        });
      }
    });
  }

  delete(element: Problem) {
    const i = this.problemTitles.indexOf(element.ProblemTitle);
    this.problemTitles.splice(i, 1);
    this.problems.splice(i, 1);
  }

  save(): void {
    if (!window.confirm('Are you sure to update Staging flags?')) {
      return;
    }
    // if user updates the staging problems and keep the updated ones in the table
    // we shall remove these regular problems
    this.problems = this.problems.filter(p => { return p.IsStaging });

    this.loading.start();
    this.service.updateStagingFlags(this.problems)
      .then(
        data => {
          if (data != null) {
            this.problems = data;
            let isSuccessful = true;
            this.problems.forEach(p => {
              if (p.IsSuccessful) {
                p.ReturnResult = "Updated.";
              } else {
                isSuccessful = false;
                // this.messageService.add(`Failed to update ${p.ProblemTitle}: ${p.Message}`);
              }
            });
            if (isSuccessful) {
              this.messageService.openSnackBar(`${this.problems.length} problems are updated successfully`);
            } else {
              this.messageService.openSnackBar('There is error when updating the problem.');
            }

          }
        }
      )
      .catch(err => { console.log(err); })
      .finally(() => { this.loading.stop(); });
  }


}
