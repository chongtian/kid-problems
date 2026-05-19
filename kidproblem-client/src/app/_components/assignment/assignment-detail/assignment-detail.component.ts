import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DisplayMessages } from '@app/_constants';
import { Access } from '@app/_guards';
import { CognitoService, LoadingBusService } from '@app/_services';
import { BehaviorSubject } from 'rxjs';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { AssignmentDetailViewComponent } from '../assignment-detail-view/assignment-detail-view.component';

@Component({
  selector: 'app-assignment-detail',
  templateUrl: './assignment-detail.component.html',
  styleUrls: ['./assignment-detail.component.css'],
  imports: [AssignmentDetailViewComponent, MatDividerModule, MatCardModule, MatButtonModule, MatTooltipModule]
})
export class AssignmentDetailComponent implements OnInit {
  assignmentId$ = new BehaviorSubject<string>('');
  isEdit$ = new BehaviorSubject<boolean>(false);
  messageTexts = DisplayMessages;
  private isChildChanged = false;
  canEdit = false;
  private loading = inject(LoadingBusService);

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private cognitoService: CognitoService
  ) { }

  ngOnInit() {
    this.route.url.subscribe(urls => {
      const action = urls[1].path.toLowerCase();
      this.isEdit$.next(action === 'edit');
      this.route.paramMap.subscribe(params => {
        const id = params.get('id');
        if (id) {
          this.assignmentId$.next(id)
        }
      });
    });

    this.loading.start();
    this.cognitoService.getUserAccess()
      .then(a => {
        this.canEdit = ((a | Access.parent) === a);
      })
      .catch(err => { console.log(err); })
      .finally(() => { this.loading.stop(); });
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
* This parent component subscribes to child component and get notified when the entity is deleted.
* @returns 
*/
  onDeleted(event: boolean): void {
    if (event) {
      this.router.navigate([`assignments`]);
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

}
