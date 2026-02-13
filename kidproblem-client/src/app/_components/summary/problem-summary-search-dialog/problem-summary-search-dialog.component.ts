import { Component, Inject, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ProblemSummaryListViewComponent } from '../problem-summary-list-view/problem-summary-list-view.component';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';

@Component({
    selector: 'app-problem-summary-search-dialog',
    templateUrl: './problem-summary-search-dialog.component.html',
    styleUrls: ['./problem-summary-search-dialog.component.css'],
    standalone: true,
    imports: [MatCardModule, ProblemSummaryListViewComponent, MatButtonModule, MatTooltipModule]
})
export class ProblemSummarySearchDialogComponent {
  
  @ViewChild(ProblemSummaryListViewComponent)
  private problemList!: ProblemSummaryListViewComponent;

  category = '';
  keyword = '';

  constructor(
    private dialogRef: MatDialogRef<ProblemSummarySearchDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private injectFilter: any
  ) {
    this.category = this.injectFilter.category || '';
    this.keyword = this.injectFilter.keyword || '';
  }

  /**
   * Send the selected items back to the caller
   */
  select() {
    const selected = this.problemList.data.filter(p => { return this.problemList.selectedIds.includes(p.ProblemTitle); });
    this.dialogRef.close(selected);
  }

  /**
   * Close the dialogue and the caller received an undefined object
   */
  close() {
    this.dialogRef.close();
  }

}
