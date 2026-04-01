import { Component, Input, OnInit, ViewChild, booleanAttribute, inject } from '@angular/core';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { PaginationIndicator } from '@app/_constants';
import { ExamDefinition, InfoCentralCodeDetail } from '@app/_models';
import { ExamDefinitionService, AdminService, MessageService, LoadingBusService } from '@app/_services';
import { BooleanLikeToTextPipe } from '@app/_pipes';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatOptionModule } from '@angular/material/core';
import { NgClass } from '@angular/common';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

const PageSize = 25;

@Component({
  selector: 'app-exam-def-list-view',
  templateUrl: './exam-def-list-view.component.html',
  styleUrls: ['./exam-def-list-view.component.css'],
  imports: [ReactiveFormsModule, FormsModule, MatFormFieldModule, MatSelectModule, MatOptionModule, MatInputModule, MatButtonModule, MatTableModule, MatCheckboxModule, RouterLink, MatPaginatorModule, NgClass, MatTooltipModule, MatSlideToggleModule, BooleanLikeToTextPipe]
})
export class ExamDefListViewComponent implements OnInit {

  @Input('category') category = '';
  @Input('keyword') keyword = '';
  @Input({ alias: 'active-only', transform: booleanAttribute }) activeOnly = false;
  @Input({ alias: 'is-selectable', transform: booleanAttribute }) isSelectable = false;
  @Input({ alias: 'pagination', transform: booleanAttribute }) pagination = true;
  @ViewChild(MatPaginator) paginator: MatPaginator;

  categories: InfoCentralCodeDetail[] = [];
  dataSource = new MatTableDataSource<ExamDefinition>();
  displayedColumns = ['select', 'category', 'year', 'title', 'type', 'count', 'active', 'memo'];
  selected: ExamDefinition[] = [];
  loadMoreData = false;
  private loading = inject(LoadingBusService);
  private paginationToken = PaginationIndicator;
  private currentKeyword = '';
  private currentCategory = '';
  private selectedAll = false;

  constructor(
    private service: ExamDefinitionService,
    private adminService: AdminService,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {
    this.loading.start();
    this.adminService.getCategoryCodes()
      .then(codes => {
        this.categories = codes.filter(c => { return c.Active; })
      })
      .catch(err => { console.log(err); })
      .finally(() => { this.loading.stop(); });

    if (!this.isSelectable) {
      // remove the first element which is 'select'
      this.displayedColumns.splice(0, 1);
    }

    // if the parent component pass a keyword, trigger search immediately
    if (this.category && this.keyword) {
      this.search();
    }
  }

  onCategoryChange(_: any) {
    this.keyword = this.category + ' ';
  }

  search() {
    if (!this.category) {
      this.messageService.openSnackBar("Category is required.");
      return;
    } else {
      this.currentKeyword = this.keyword;
      this.currentCategory = this.category;
    }

    this.paginationToken = this.pagination === true ? PaginationIndicator : '';
    this.loading.start();
    this.service.queryExamDefinitions(this.category, this.activeOnly, this.paginationToken, PageSize, this.keyword)
      .then(d => {
        this.dataSource.data = d.data;
        this.dataSource.paginator = this.paginator;
        if (d.data.length > 0) {
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
    if (this.keyword != this.currentKeyword || this.category != this.currentCategory) {
      // the search filter criteria have changed, reset search
      this.search();
    }
    this.loading.start();
    this.service.queryExamDefinitions(this.category, this.activeOnly, this.paginationToken, PageSize, this.keyword)
      .then(d => {
        this.dataSource.data.push(...d.data);
        // this.dataSource.paginator = this.paginator;
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

  toggle(element: ExamDefinition): void {
    const i = this.getIndex(this.selected, element);
    if (i >= 0) {
      this.selected.splice(i, 1);
    } else {
      this.selected.push(element);
    }
  }

  toggleAll(): void {
    this.selected = [];
    if (this.selectedAll) {
      this.selectedAll = false;
    } else {
      this.dataSource.data.forEach(d => {
        this.selected.push(d);
      });
      this.selectedAll = true;
    }
  }

  /**
   * This function set the checked attribute of checkbox.
   * It gets the Problem from html and compares it with selectedIds.
   */
  isChecked(element: ExamDefinition) {
    return this.getIndex(this.selected, element) >= 0;
  }

  /**
   * Get the index of the element in the array
   * @param array 
   * @param element 
   * @returns if the element does not exist in the array, return -1
   */
  private getIndex(array: ExamDefinition[], element: ExamDefinition): number {
    let i = -1;
    array.forEach(e => {
      if (e.ExamCategory == element.ExamCategory && e.ExamTitle == element.ExamTitle) {
        i++;
        return;
      }
    });
    return i;
  }

}
