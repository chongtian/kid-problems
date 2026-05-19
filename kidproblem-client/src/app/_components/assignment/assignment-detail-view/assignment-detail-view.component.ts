import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray, ReactiveFormsModule } from '@angular/forms';
import { DisplayMessages } from '@app/_constants';
import { Assignment } from '@app/_models';
import { AssignmentService, LoadingBusService, MessageService } from '@app/_services';
import { BehaviorSubject } from 'rxjs';
import { BooleanLikeToTextPipe } from '@app/_pipes';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { MatDividerModule } from '@angular/material/divider';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatListModule } from '@angular/material/list';
import { MatCardModule } from '@angular/material/card';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-assignment-detail-view',
  templateUrl: './assignment-detail-view.component.html',
  styleUrls: ['./assignment-detail-view.component.css'],
  imports: [ReactiveFormsModule, MatCardModule, MatListModule, MatFormFieldModule, MatInputModule, MatCheckboxModule, MatDividerModule, RouterLink, MatButtonModule, MatTooltipModule, DatePipe, BooleanLikeToTextPipe]
})
export class AssignmentDetailViewComponent implements OnInit {

  @Input({ alias: 'edit' }) isEdit$ = new BehaviorSubject<boolean>(false);
  @Input({ alias: 'entity-id' }) assignmentId$ = new BehaviorSubject<string>('');
  @Output() deleted = new EventEmitter<boolean>();
  @Output() changed = new EventEmitter<boolean>();

  private assignment: Assignment | undefined;
  messageTexts = DisplayMessages;
  private loading = inject(LoadingBusService);
  isEdit = false;
  editorForm: FormGroup | undefined;

  constructor(
    private service: AssignmentService,
    private messageService: MessageService,
    private formBuilder: FormBuilder
  ) { }

  ngOnInit() {
    this.assignmentId$.subscribe(
      id => {
        this.getAssignment(id);
      }
    );
    this.isEdit$.subscribe(v => { this.isEdit = v; });
  }

  private getAssignment(id: string) {
    if (id) {
      this.loading.start();
      this.service.getAssignment(id).then(
        data => {
          if (data != null) {
            this.assignment = data;
            this.assignment.ExamRunIds = this.assignment.ExamRunIds ?? [];
            this.createFormGroup();
          } else {
            this.messageService.add(`${this.messageTexts.cannotRetrieveRecord} ${id}.`);
            this.assignment = undefined;
          }
        }
      ).catch(err => { console.log(err); })
        .finally(() => { this.loading.stop(); });
    }
  }

  private createFormGroup(): void {
    const f = this.formBuilder.group({
      Id: this.assignment!.Id,
      FamilyId: this.assignment!.FamilyId,
      ExamCategory: [this.assignment!.ExamCategory, Validators.required],
      ExamTitle: [this.assignment!.ExamTitle, Validators.required],
      CreateTime: this.assignment!.CreateTime,
      IsComplete: this.assignment!.IsComplete,
      Memo: this.assignment!.Memo,
      ExamRunIds: this.formBuilder.array(this.assignment!.ExamRunIds!)
    });

    f.valueChanges.subscribe(() => {
      this.changed.emit(true);
    });

    this.editorForm = f;
  }

  save() {
    if (!this.editorForm!.valid) {
      this.messageService.add(this.messageTexts.invalidFormData);
      return;
    }

    if (!window.confirm(this.messageTexts.confirmSubmit)) {
      return;
    }

    const entity = this.editorForm!.value as Assignment;
    this.loading.start();
    this.service.updateAssignment(entity)
      .then(data => this.handleSaveResponse(data))
      .catch(err => { console.log(err); })
      .finally(() => { this.loading.stop(); });
  }

  private handleSaveResponse(data: Assignment) {
    let isSuccessful = false;
    const messages: string[] = [];

    if (data != null) {
      isSuccessful = data.IsSuccessful ?? false;
      messages.push(data.ReturnResult ?? '');
      this.assignment = data;
    }

    if (isSuccessful) {
      this.messageService.openSnackBar(`${this.messageTexts.saveSuccessful}`);
      this.editorForm!.markAsPristine();
      this.changed.emit(false);
    } else {
      const message = messages.join(' ');
      this.messageService.openSnackBar(`${this.messageTexts.saveFailed}`);
      this.messageService.add(`${this.messageTexts.saveFailed}: ${message}`);
    }
  }

  delete() {
    if (!window.confirm(this.messageTexts.confirmDelete)) {
      return;
    }
    this.loading.start();
    this.service.deleteAssignment(this.assignment!.Id)
      .then(
        data => {
          if (data != null && data.IsSuccessful) {
            this.messageService.openSnackBar('Record is deleted');
            this.deleted.emit(true);
          } else {
            this.messageService.openSnackBar(`${this.messageTexts.deleteFailed}.`);
            this.messageService.add(`${this.messageTexts.deleteFailed}.`);
          }
        }
      )
      .catch(err => { console.log(err); })
      .finally(() => { this.loading.stop(); });
  }

}
