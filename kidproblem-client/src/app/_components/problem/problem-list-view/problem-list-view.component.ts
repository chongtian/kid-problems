import { Component, Input, OnInit, booleanAttribute, inject } from '@angular/core';
import { PaginationIndicator } from '@app/_constants';
import { Problem } from '@app/_models';
import { LoadingBusService, MessageService, ProblemService } from '@app/_services';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { NgClass } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

const PageSize = 25;

@Component({
  selector: 'app-problem-list-view',
  templateUrl: './problem-list-view.component.html',
  styleUrls: ['./problem-list-view.component.css'],
  imports: [ReactiveFormsModule, FormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, NgClass, MatCheckboxModule, RouterLink, MatTooltipModule, MatSlideToggleModule]
})
export class ProblemListViewComponent implements OnInit {

  @Input({ alias: 'is-staging', transform: booleanAttribute }) isStaging = false;
  @Input('keyword') keyword = '';
  @Input({ alias: 'is-selectable', transform: booleanAttribute }) isSelectable = false;
  @Input({ alias: 'pagination', transform: booleanAttribute }) pagination = true;

  data: Problem[];
  selectedIds: string[];

  private paginationToken = PaginationIndicator;
  private currentKeyword = '';
  loadMoreData = false;
  private loading = inject(LoadingBusService);
  private selectedAll = false;

  constructor(private service: ProblemService, private messageService: MessageService) {
    this.data = [];
    this.selectedIds = [];
  }

  ngOnInit(): void {
    // if the parent component pass a keyword, trigger search immediately
    if (this.keyword) {
      this.search();
    }
  }

  search() {
    if (!this.keyword) {
      this.messageService.openSnackBar("Keyword is required.");
      return;
    } else {
      this.currentKeyword = this.keyword;
    }

    this.paginationToken = this.pagination === true ? PaginationIndicator : '';
    this.loading.start();
    this.service.queryProblems(this.isStaging, this.paginationToken, PageSize, this.keyword.toUpperCase())
      .then(d => {
        this.data = d.data;
        if (d.data.length > 0) {
          this.data.sort();
          this.paginationToken = d.pagination;
          if (this.pagination === true && this.paginationToken && this.paginationToken !== '{}') {
            this.loadMoreData = true;
          } else {
            this.loadMoreData = false;
          }
        } else {
          this.messageService.openSnackBar("No record is found.");
        }
      })
      .catch(err => { console.log(err); })
      .finally(() => { this.loading.stop(); });
  }

  more(): void {
    if (this.keyword != this.currentKeyword) {
      // the search keyword has changed, reset search
      this.search();
    }
    this.loading.start();
    this.service.queryProblems(this.isStaging, this.paginationToken, PageSize, this.keyword.toUpperCase()).then(d => {
      this.data.push(...d.data);
      this.data.sort();
      this.paginationToken = d.pagination;
      if (this.pagination === true && this.paginationToken && this.paginationToken !== '{}') {
        this.loadMoreData = true;
      } else {
        this.loadMoreData = false;
      }
    })
      .catch(err => { console.log(err); })
      .finally(() => { this.loading.stop(); });
  }

  toggle(element: any): void {
    const i = this.selectedIds.indexOf(element.ProblemTitle);
    if (i >= 0) {
      this.selectedIds.splice(i, 1);
    } else {
      this.selectedIds.push(element.ProblemTitle);
    }
  }

  toggleAll(): void {
    this.selectedIds = [];
    if (this.selectedAll) {
      this.selectedAll = false;
    } else {
      this.data.forEach(d => {
        this.selectedIds.push(d.ProblemTitle);
      });
      this.selectedAll = true;
    }
  }

  /**
   * This function set the checked attribute of checkbox.
   * It gets the Problem from html and compares it with selectedIds.
   */
  isChecked(element: Problem) {
    return this.selectedIds.includes(element.ProblemTitle);
  }
}
