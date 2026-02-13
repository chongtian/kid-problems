/**
 * ProblemQueryComponent is an entry point to browse Problems.
 * It calls ProblemListViewComponent, 
 * which does the job of searching and displaying Problems.
 * This component interacts with Router.
 */

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProblemListViewComponent } from '../problem-list-view/problem-list-view.component';
import { NgIf } from '@angular/common';

@Component({
    selector: 'app-problem-query',
    templateUrl: './problem-query.component.html',
    styleUrls: ['./problem-query.component.css'],
    standalone: true,
    imports: [NgIf, ProblemListViewComponent]
})
export class ProblemQueryComponent implements OnInit {

  isStaging = false;

  constructor(private route: ActivatedRoute) {
  }

  ngOnInit() {
    this.route.data.subscribe(d => { this.isStaging = d.staging; });
  }

}
