import { Component, inject } from '@angular/core';
import { AdminService, CognitoService, LoadingBusService } from '@app/_services';
import { AppInfo, IUser } from '@app/_models';
import { environment } from '@environments/environment';
import { Router, RouterOutlet } from '@angular/router';
import { MessagesComponent } from './_components/messages/messages.component';

import { KpMenuComponent } from './_components';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  imports: [KpMenuComponent, RouterOutlet, MessagesComponent]
})
export class AppComponent {

  appInfo: AppInfo;
  user: IUser;
  isAuthenticated = false;
  loading = inject(LoadingBusService);

  constructor(private router: Router, private cognitoService: CognitoService, private adminService: AdminService) {
    this.appInfo = { AppName: environment.applicationName, Version: environment.applicationVersion };
    this.user = {} as IUser;
    this.cognitoService.onCurrentUserChanged.subscribe(
      user => {
        if (user) {
          this.user = user;
          const isAuthenticated = user.isAuthenticated || false;
          this.isAuthenticated = isAuthenticated;
        }
      }
    );
  }

  ngOnInit(): void {
    this.loading.start();
    this.cognitoService.getUser()
      .then((user: any) => {
        if (user) {
          this.user = user.attributes;
        }
      })
      .catch(err => { console.log(err); })
      .finally(() => { this.loading.stop(); });

    this.loading.start();
    this.cognitoService.isAuthenticated()
      .then((success: boolean) => {
        this.isAuthenticated = success;
      })
      .catch(err => { console.log(err); })
      .finally(() => { this.loading.stop(); });

    this.loading.start();
    this.adminService.ping()
      .then(
        r => {
          console.log(r);
          this.appInfo.Version = `${r.DynamoDbTableNamePrefix}${this.appInfo.Version}`;
        })
      .catch(err => { console.log(err); })
      .finally(() => { this.loading.stop(); });

    // activate AWS Lambda every 900 seconds
    // setInterval(() => {
    //   this.adminService.ping().then(
    //     r => {
    //       console.log(r);
    //     });
    // }, 900000);

  }

  onSignOut(_: any): void {
    this.user = {} as IUser;
    this.cognitoService.signOut()
      .then(() => {
        this.isAuthenticated = false;
        this.router.navigate(['/login']);
      });
  }

}
