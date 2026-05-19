import { Component, inject, Input, OnInit } from '@angular/core';
import { Access } from '@app/_guards';
import { ProblemSummary } from '@app/_models';
import { CognitoService, AdminService, SummaryService, LoadingBusService } from '@app/_services';
import { MatListModule } from '@angular/material/list';
import { MatExpansionModule } from '@angular/material/expansion';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-problem-summary-view',
  templateUrl: './problem-summary-view.component.html',
  styleUrls: ['./problem-summary-view.component.css'],
  imports: [MatExpansionModule, MatListModule, DecimalPipe]
})
export class ProblemSummaryViewComponent implements OnInit {

  @Input('problem-title') problemTitle = '';
  summaries: ProblemSummary[] = [];
  private loading = inject(LoadingBusService);

  constructor(
    private cognitoService: CognitoService,
    private adminService: AdminService,
    private service: SummaryService
  ) { }

  async ngOnInit() {
    await this.loadSummaries();
  }

  async loadSummaries() {
    try {
      this.loading.start();

      const access = await this.cognitoService.getUserAccess();

      let results;

      if ((access | Access.parent) === access) {
        const children = await this.adminService.getChildren();

        results = await Promise.all(
          children.map(child =>
            this.service.getProblemSummary(this.problemTitle, child)
              .then(summary => ({ child, summary }))
          )
        );
      } else {
        const user = await this.cognitoService.getCurrentAuthenticatedUser();

        const summary = await this.service.getProblemSummary(
          this.problemTitle,
          user.username
        );

        results = [{ child: user.username, summary }];
      }

      results.forEach(({ summary }) => {
        if (summary) {
          this.summaries.push(summary);
        }
      });

    } catch (err) {
      console.log(err);
    } finally {
      this.loading.stop();
    }
  }

}
