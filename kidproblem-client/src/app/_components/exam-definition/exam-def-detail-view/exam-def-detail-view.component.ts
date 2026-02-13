import { Component, EventEmitter, Input, OnInit, Output, booleanAttribute } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ProblemSummarySearchDialogComponent } from '@app/_components';
import { ProblemSearchDialogComponent } from '@app/_components/problem';
import { DisplayMessages } from '@app/_constants';
import { ExamDefinition, ExamDefinitionId, ExamDetail, InfoCentralCodeDetail, Problem, ProblemSummary } from '@app/_models';
import { AdminService, ExamDefinitionService, MessageService } from '@app/_services';
import { BehaviorSubject } from 'rxjs';
import { BooleanLikeToTextPipe } from '@app/_pipes';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { MatDividerModule } from '@angular/material/divider';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatInputModule } from '@angular/material/input';
import { MatOptionModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { NgIf, NgFor, NgClass } from '@angular/common';

@Component({
    selector: 'app-exam-def-detail-view',
    templateUrl: './exam-def-detail-view.component.html',
    styleUrls: ['./exam-def-detail-view.component.css'],
    standalone: true,
    imports: [NgIf, MatProgressBarModule, ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatSelectModule, NgFor, MatOptionModule, MatInputModule, MatCheckboxModule, MatDividerModule, NgClass, RouterLink, MatButtonModule, MatTooltipModule, BooleanLikeToTextPipe]
})
export class ExamDefDetailViewComponent implements OnInit {

  @Input({ alias: 'edit' }) isEdit$ = new BehaviorSubject<boolean>(false);
  @Input({ alias: 'entity-id' }) examDefId$ = new BehaviorSubject<ExamDefinitionId>(null);
  @Output() created = new EventEmitter<ExamDefinitionId>();
  @Output() deleted = new EventEmitter<boolean>();
  @Output() changed = new EventEmitter<boolean>();

  categories: InfoCentralCodeDetail[] = [];
  private examDef: ExamDefinition;
  messageTexts = DisplayMessages;
  isLoading: boolean;
  isEdit = false;
  examEditorForm: FormGroup;
  isNew = false;

  constructor(
    private adminService: AdminService,
    private service: ExamDefinitionService,
    private messageService: MessageService,
    private formBuilder: FormBuilder,
    private query: MatDialog
  ) { }

  ngOnInit() {
    this.adminService.getCategoryCodes().then(codes => {
      this.categories = codes.filter(c => { return c.Active; })
    });

    this.examDefId$.subscribe(
      id => {
        this.isLoading = true;
        this.getExamDefinition(id);
      }
    );
    this.isEdit$.subscribe(v => { this.isEdit = v; });
  }

  get details() {
    return this.examEditorForm.get('ExamDetails') as FormArray;
  }

  private getExamDefinition(id: ExamDefinitionId) {
    if (id) {
      this.isNew = false;
      this.service.getExamDefinition(id.ExamCategory, id.ExamTitle).then(
        data => {
          if (data != null) {
            this.examDef = data;
            this.createFormGroup();
          } else {
            this.messageService.add(`${this.messageTexts.cannotRetrieveRecord} ${id.ExamCategory}, ${id.ExamTitle}.`);
            this.examDef = null;
          }
          this.isLoading = false;
        }
      );

    } else {
      this.isNew = true;
      this.examDef = {
        ExamCategory: null,
        ExamYear: null,
        ExamTitle: null,
        ExamType: null,
        Active: true,
        Memo: null,
        ExamDetails: []
      };
      this.createFormGroup();
    }
  }

  private createFormGroup(): void {
    const f = this.formBuilder.group({
      ExamCategory: [this.examDef.ExamCategory, Validators.required],
      ExamTitle: [this.examDef.ExamTitle, Validators.required],
      ExamYear: this.examDef.ExamYear,
      ExamType: this.examDef.ExamType,
      Active: this.examDef.Active,
      Memo: this.examDef.Memo,
      ExamDetails: this.formBuilder.array([])
    });

    const detailForms = f.get('ExamDetails') as FormArray;
    if (this.examDef.ExamDetails) {
      let i = 0;
      this.examDef.ExamDetails.forEach(detail => {
        detailForms.push(this.formBuilder.group(
          {
            ProblemTitle: detail.ProblemTitle
          }
        ));
        i++;
      });
    }

    f.valueChanges.subscribe(() => {
      this.changed.emit(true);
    });

    this.examEditorForm = f;
  }

  add(action: number): void {
    let dialogRef;
    let keyword: string;
    switch (action) {
      case 0:
      default:
        keyword = `${this.examEditorForm.get('ExamCategory').value}-${this.examEditorForm.get('ExamYear').value || ''}`;
        dialogRef = this.query.open(ProblemSearchDialogComponent, {
          data: { keyword: keyword, isStaging: false }
        });
        break;

      case 1:
        keyword = `${this.examEditorForm.get('ExamCategory').value}-${this.examEditorForm.get('ExamYear').value || ''}`;
        dialogRef = this.query.open(ProblemSummarySearchDialogComponent, {
          data: { keyword: keyword, category: this.examEditorForm.get('ExamCategory').value }
        });
        break;
    }

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const problems = action === 0 ? (result as Problem[]) : (result as ProblemSummary[]);
        problems.forEach(problem => {
          const existing = (this.details.value as ExamDetail[]).find(d => d.ProblemTitle == problem.ProblemTitle);
          if (!existing) {
            this.details.push(this.formBuilder.group(
              {
                ProblemTitle: problem.ProblemTitle
              }));
            this.examEditorForm.markAsDirty();
          }
        });
      }
    });
  }

  deleteDetail(i: number) {
    this.details.removeAt(i);
    this.examEditorForm.markAsDirty();
  }

  save() {
    if (!this.examEditorForm.valid) {
      this.messageService.add(this.messageTexts.invalidFormData);
      return;
    }

    if (!window.confirm(this.messageTexts.confirmSubmit)) {
      return;
    }

    const entity = this.examEditorForm.value as ExamDefinition;
    if (this.isNew) {
      this.service.createExamDefinition(entity).then(data => this.handleSaveResponse(data));
    } else {
      this.service.updateExamDefinition(entity).then(data => this.handleSaveResponse(data));
    }
  }

  private handleSaveResponse(data: ExamDefinition) {
    let isSuccessful = false;
    const messages: string[] = [];

    if (data != null) {
      isSuccessful = data.IsSuccessful;
      messages.push(data.ReturnResult);
      this.examDef = data;
    }

    if (isSuccessful) {
      this.messageService.openSnackBar(`${this.messageTexts.saveSuccessful}`);
      this.examEditorForm.markAsPristine();
      this.changed.emit(false);
      if (this.isNew) {
        this.created.emit({ ExamCategory: data.ExamCategory, ExamTitle: data.ExamTitle });
      }
    } else {
      const message = messages.join(' ');
      this.messageService.openSnackBar(`${this.messageTexts.saveFailed}`);
      this.messageService.add(`${this.messageTexts.saveFailed}: ${message}`);
    }
  }

  delete() {
    if (this.isNew || !window.confirm(this.messageTexts.confirmDelete)) {
      return;
    }
    this.service.deleteExamDefinition(this.examDef.ExamCategory, this.examDef.ExamTitle).then(
      data => {
        if (data != null && data.IsSuccessful) {
          this.messageService.openSnackBar('Record is deleted');
          this.deleted.emit(true);
        } else {
          this.messageService.openSnackBar(`${this.messageTexts.deleteFailed}.`);
          this.messageService.add(`${this.messageTexts.deleteFailed}.`);
        }
      }
    );
  }


}
