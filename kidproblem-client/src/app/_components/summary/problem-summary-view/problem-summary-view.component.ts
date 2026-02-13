import { Component, Input, OnInit } from '@angular/core';
import { Access } from '@app/_guards';
import { ProblemSummary } from '@app/_models';
import { CognitoService, AdminService, SummaryService } from '@app/_services';
import { MatListModule } from '@angular/material/list';
import { MatExpansionModule } from '@angular/material/expansion';
import { NgIf, NgFor, DecimalPipe } from '@angular/common';

@Component({
    selector: 'app-problem-summary-view',
    templateUrl: './problem-summary-view.component.html',
    styleUrls: ['./problem-summary-view.component.css'],
    standalone: true,
    imports: [NgIf, MatExpansionModule, MatListModule, NgFor, DecimalPipe]
})
export class ProblemSummaryViewComponent implements OnInit {

  @Input('problem-title') problemTitle = '';
  summaries: ProblemSummary[] = [];

  constructor(
    private cognitoService: CognitoService,
    private adminService: AdminService,
    private service: SummaryService
  ) { }

  ngOnInit(): void {
    this.cognitoService.getUserAccess().then(a => {

      if ((a | Access.parent) === a) {
        this.adminService.getChildren().then(children => {
          children.forEach(child => {
            this.service.getProblemSummary(this.problemTitle, child).then(
              d => {
                if (d) {
                  this.summaries.push(d);
                }
              }
            );
          });
        });
      } else {
        this.cognitoService.getCurrentAuthenticatedUser().then(user => {
          const username = user.username;
          this.service.getProblemSummary(this.problemTitle, username).then(
            d => {
              if (d) {
                this.summaries.push(d);
              }
            }
          );
        });
      }
    });
  }
}
