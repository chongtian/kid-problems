import { Component, inject, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { DisplayMessages } from '@app/_constants';
import { ExamDefinition, ExamDefinitionId } from '@app/_models';
import { AssignmentService, LoadingBusService, MessageService } from '@app/_services';
import { BehaviorSubject } from 'rxjs';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { ExamPrintComponent } from '../exam-print/exam-print.component';
import { ExamDefDetailViewComponent } from '../exam-def-detail-view/exam-def-detail-view.component';


@Component({
  selector: 'app-exam-def-detail',
  templateUrl: './exam-def-detail.component.html',
  styleUrls: ['./exam-def-detail.component.css'],
  imports: [ExamDefDetailViewComponent, ExamPrintComponent, MatDividerModule, MatCardModule, MatButtonModule, MatTooltipModule]
})
export class ExamDefDetailComponent implements OnInit {

  examDefId$ = new BehaviorSubject<ExamDefinitionId>(null);
  isEdit$ = new BehaviorSubject<boolean>(false);
  messageTexts = DisplayMessages;
  isNew = false;
  private isChildChanged = false;
  printExam = false;
  private loading = inject(LoadingBusService);

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private assignmentService: AssignmentService,
    private messageService: MessageService,
    private query: MatDialog
  ) { }

  ngOnInit() {
    this.route.url.subscribe(urls => {
      const action = urls[1].path.toLowerCase();
      if (action === 'create') {
        this.isEdit$.next(true);
        this.isNew = true;
      } else {
        this.isEdit$.next(action === 'edit');
        this.isNew = false;
        this.route.paramMap.subscribe(params => {
          const category = params.get('category');
          const title = params.get('title');
          if (category && title) {
            const id: ExamDefinitionId = { ExamCategory: category, ExamTitle: title };
            this.examDefId$.next(id)
          }
        });
      }
    });
  }

  /**
   * This is only available when isEdit = true. 
   * This parent component subscribes to child component and get the state of its Form.
   * If the Form is dirty, it asks user to confirm the navigate back action,
   * as any un-saved changes would be lost.
   * @returns 
   */
  switchView(): void {
    if (this.isEdit$.value) {
      if (this.isChildChanged && !window.confirm(this.messageTexts.warningUnsavedChanges)) return;
      this.isEdit$.next(false);
    } else {
      this.isEdit$.next(true);
    }

  }

  /**
 * This is only available when isEdit = true. 
 * This parent component subscribes to child component and get the id of the new entity.
 * Then it navigates to the newly-created entity.
 * @returns 
 */
  onCreated(event: ExamDefinitionId): void {
    if (this.isNew && event) {
      this.router.navigate([`examdef/edit/${event.ExamCategory}/${event.ExamTitle}`]);
    }
  }

  /**
* This is only available when isEdit = true. 
* This parent component subscribes to child component and get notified when the entity is deleted.
* @returns 
*/
  onDeleted(event: boolean): void {
    if (event) {
      this.router.navigate([`examdefs`]);
    }
  }

  /**
* This is only available when isEdit = true. 
* This parent component subscribes to child component and get notified when the Form is changed.
* @returns 
*/
  onChanged(event: boolean): void {
    this.isChildChanged = event;
  }

  createAssignment() {
    if (!window.confirm(this.messageTexts.confirmSubmit)) {
      return;
    }
    const examDef: ExamDefinition = { ExamCategory: this.examDefId$.value.ExamCategory, ExamTitle: this.examDefId$.value.ExamTitle, Active: true };

    this.loading.start();
    this.assignmentService.createAssignmentFromDefinition(examDef)
      .then(data => {
        if (data && data.IsSuccessful) {
          this.router.navigate([`assignment/view/${data.Id}`]);
        } else {
          this.messageService.openSnackBar(`${this.messageTexts.saveFailed}`);
          this.messageService.add(`${this.messageTexts.saveFailed}: ${data.ReturnResult}`);
        }
      })
      .catch(err => { console.log(err); })
      .finally(() => { this.loading.stop(); });
  }

  togglePrint(): void {
    this.printExam = !this.printExam;
  }

}
