import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Location } from '@angular/common';
import { DisplayMessages } from '@app/_constants';
import { BehaviorSubject } from 'rxjs';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { ProblemEditorComponent } from '../problem-editor/problem-editor.component';
import { ProblemDetailViewComponent } from '../problem-detail-view/problem-detail-view.component';

@Component({
    selector: 'app-problem-detail',
    templateUrl: './problem-detail.component.html',
    styleUrls: ['./problem-detail.component.css'],
    imports: [ProblemDetailViewComponent, ProblemEditorComponent, MatDividerModule, MatCardModule, MatButtonModule, RouterLink, MatTooltipModule]
})
export class ProblemDetailComponent implements OnInit {

  problemTitle$ = new BehaviorSubject<string>('');
  messageTexts = DisplayMessages;
  isEdit = false;
  isNew = false;
  nextProblemTitle: string;
  prevProblemTitle: string;
  currProblemTitle: string;
  private isChildChanged = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private location: Location
  ) { }

  ngOnInit() {
    this.route.url.subscribe(urls => {
      const action = urls[1].path.toLowerCase();
      if (action === 'create') {
        this.isEdit = true;
        this.isNew = true;
      } else {
        this.isEdit = action === 'edit';
        this.isNew = false;
        this.route.paramMap.subscribe(params => {
          const id = params.get('id');
          if (id) {
            this.problemTitle$.next(id)
            this.currProblemTitle = id;
            this.nextProblemTitle = this.getPrevNextProblem(1);
            this.prevProblemTitle = this.getPrevNextProblem(-1);
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
  goBack(): void {
    if (this.isChildChanged && !window.confirm(this.messageTexts.warningUnsavedChanges)) return;
    this.location.back();
  }

  /**
 * This is only available when isEdit = true. 
 * This parent component subscribes to child component and get the id of the new entity.
 * Then it navigates to the newly-created entity.
 * @returns 
 */
  onCreated(event: string): void {
    if (this.isNew && event) {
      this.router.navigate([`problem/edit/${event}`]);
    }
  }

  /**
* This is only available when isEdit = true. 
* This parent component subscribes to child component and get notified when the entity is deleted.
* @returns 
*/
  onDeleted(event: boolean): void {
    if (event) {
      this.router.navigate([`problems/s`]);
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

  private getPrevNextProblem(offset: number): string {
    if (this.problemTitle$.getValue()) {
      const t = this.problemTitle$.getValue().split('-');
      if (t.length === 3) {
        const problemCategory = t[0];
        const problemYear = t[1];
        let number = +t[2];
        if (number) {
          number += offset;
          number = number < 1 ? 1 : number;
          let maxNumber = 1000;
          switch (problemCategory) {
            case 'AMC8':
            case 'AMC10':
            case 'AMC12':
              maxNumber = 25;
              break;
            case 'AIME':
              maxNumber = 15;
              break;
            case 'USAJMO':
            case 'USAMO':
              maxNumber = 7;
              break;
          }
          number = number > maxNumber ? maxNumber : number;
          const newProblemTitle = `${problemCategory}-${problemYear}-${number.toString().padStart(3, '0')}`;
          return newProblemTitle;
        }
      }
    }
    return this.problemTitle$.getValue();
  }

}