/**
 * This component is not used by any other component.
 * Assigment and Exam Definiiton has a one-to-one relationship,
 * therefore, a search dialog is not required.
 */
import { Component, Inject, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ExamDefListViewComponent } from '../exam-def-list-view/exam-def-list-view.component';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';

@Component({
    selector: 'app-exam-def-search-dialog',
    templateUrl: './exam-def-search-dialog.component.html',
    styleUrls: ['./exam-def-search-dialog.component.css'],
    standalone: true,
    imports: [MatCardModule, ExamDefListViewComponent, MatButtonModule, MatTooltipModule]
})
export class ExamDefSearchDialogComponent {

  @ViewChild(ExamDefListViewComponent)
  private entityList!: ExamDefListViewComponent;

  category = '';
  keyword = '';
  activeOnly = true;

  constructor(
    private dialogRef: MatDialogRef<ExamDefSearchDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private injectFilter: any
  ) {
    this.category = this.injectFilter.category || '';
    this.keyword = this.injectFilter.keyword || '';
    this.activeOnly = this.injectFilter.activeOnly;
  }

  /**
   * Send the selected items back to the caller
   */
  select() {
    const selected = this.entityList.selected;
    this.dialogRef.close(selected);
  }

  /**
   * Close the dialogue and the caller received an undefined object
   */
  close() {
    this.dialogRef.close();
  }
}
