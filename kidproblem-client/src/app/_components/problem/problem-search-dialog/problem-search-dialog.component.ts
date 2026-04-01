/**
 * Query and select Problem.
 * Here is an example how to use this component.
 * 
 *   constructor(private query: MatDialog) { }

  openQuery(): void {
    const dialogRef = this.query.open(
      ProblemSearchDialogComponent, 
      { data: {keyword:'AMC10-2022', isStaging:false} }
    );

    dialogRef.afterClosed().subscribe(result => {
      console.log(result);
    });
  }
 */
import { Component, Inject, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ProblemListViewComponent } from '../problem-list-view/problem-list-view.component';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';

@Component({
    selector: 'app-problem-search-dialog',
    templateUrl: './problem-search-dialog.component.html',
    styleUrls: ['./problem-search-dialog.component.css'],
    imports: [MatCardModule, ProblemListViewComponent, MatButtonModule, MatTooltipModule]
})
export class ProblemSearchDialogComponent {

  @ViewChild(ProblemListViewComponent)
  private problemList!: ProblemListViewComponent;

  keyword = '';
  isStaging = false;

  constructor(
    private dialogRef: MatDialogRef<ProblemSearchDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private injectFilter: any
  ) {
    this.keyword = this.injectFilter.keyword || '';
    this.isStaging = this.injectFilter.isStaging || false;
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
