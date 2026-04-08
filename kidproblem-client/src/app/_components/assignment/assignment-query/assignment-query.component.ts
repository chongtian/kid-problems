import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AssignmentListViewComponent } from '../assignment-list-view/assignment-list-view.component';

@Component({
    selector: 'app-assignment-query',
    templateUrl: './assignment-query.component.html',
    styleUrls: ['./assignment-query.component.css'],
    imports: [AssignmentListViewComponent]
})
export class AssignmentQueryComponent implements OnInit {
  startTime: Date;
  endTime: Date;
  latest = true;

  constructor(private route: ActivatedRoute, private router: Router) {
  }

  ngOnInit() {
    this.route.data.subscribe(d => {
      if (d.latest) {
        this.latest = true;
        // show latest assignments
        this.endTime = new Date();
        this.startTime = new Date();
        this.startTime.setDate(this.endTime.getDate() - 14);
      } else {
        this.latest = false;
        // show assignment in the past one year
        this.endTime = new Date();
        this.startTime = new Date();
        this.startTime.setDate(this.endTime.getDate() - 365);
      }
    });
  }

  onRun(event: string) {
    this.router.navigate([`examrun/run/${event}`]);
  }

}
