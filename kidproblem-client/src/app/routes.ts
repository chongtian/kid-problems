import { Routes } from '@angular/router';
import { AssignmentDetailComponent, AssignmentQueryComponent, ChangePwdComponent, ExamDefDetailComponent, ExamDefQueryComponent, ExamRunDetailComponent, ExamRunQueryComponent, ExamRunnerComponent, ExamSummaryListViewComponent, HomeComponent, LoginComponent, ProblemBulkCreateComponent, ProblemBulkUpdateComponent, ProblemDetailComponent, ProblemQueryComponent, ProblemScrapComponent, ProblemSummaryListViewComponent, UpdateAnswerComponent } from '@app/_components';
import { Access, AuthGuard } from '@app/_guards';

export const routes: Routes = [
  { path: '', component: HomeComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.child } },
  { path: 'home', component: HomeComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.child } },
  { path: 'login', component: LoginComponent },
  { path: 'password', component: ChangePwdComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.child } },
  { path: 'problems/r', component: ProblemQueryComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.child, staging: false } },
  { path: 'problems/s', component: ProblemQueryComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.adm, staging: true } },
  { path: 'problem/view/:id', component: ProblemDetailComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.child } },
  { path: 'problem/edit/:id', component: ProblemDetailComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.adm } },
  { path: 'problem/create', component: ProblemDetailComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.adm } },
  { path: 'problem/scrap', component: ProblemScrapComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.adm } },
  { path: 'problem/bulkupdate', component: ProblemBulkUpdateComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.adm } },
  { path: 'problem/bulkcreate', component: ProblemBulkCreateComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.adm } },
  { path: 'problem/answers', component: UpdateAnswerComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.adm } },
  { path: 'examdefs', component: ExamDefQueryComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.child, active: true } },
  { path: 'examdefs/all', component: ExamDefQueryComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.child, active: false } },
  { path: 'examdef/view/:category/:title', component: ExamDefDetailComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.child } },
  { path: 'examdef/edit/:category/:title', component: ExamDefDetailComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.parent } },
  { path: 'examdef/create', component: ExamDefDetailComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.parent } },
  { path: 'assignments', component: AssignmentQueryComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.child, latest: true } },
  { path: 'assignments/all', component: AssignmentQueryComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.child, latest: false } },
  { path: 'assignment/view/:id', component: AssignmentDetailComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.child } },
  { path: 'assignment/edit/:id', component: AssignmentDetailComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.parent } },
  { path: 'examruns', component: ExamRunQueryComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.child, latest: true } },
  { path: 'examruns/all', component: ExamRunQueryComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.child, latest: false } },
  { path: 'examrun/view/:id', component: ExamRunDetailComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.child } },
  { path: 'examrun/run/:id', component: ExamRunnerComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.child } },
  { path: 'summary/exam', component: ExamSummaryListViewComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.parent } },
  { path: 'summary/problem', component: ProblemSummaryListViewComponent, canActivate: [AuthGuard], data: { requiredAccess: Access.child } },
];
