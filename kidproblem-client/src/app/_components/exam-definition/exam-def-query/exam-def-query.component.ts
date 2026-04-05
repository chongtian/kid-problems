import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ExamDefListViewComponent } from '../exam-def-list-view/exam-def-list-view.component';

@Component({
    selector: 'app-exam-def-query',
    templateUrl: './exam-def-query.component.html',
    styleUrls: ['./exam-def-query.component.css'],
    imports: [ExamDefListViewComponent]
})
export class ExamDefQueryComponent implements OnInit {
  
  activeOnly = false;

  constructor(private route: ActivatedRoute) {
  }

  ngOnInit() {
    this.route.data.subscribe(d => { this.activeOnly = d.active; });
  }
}
