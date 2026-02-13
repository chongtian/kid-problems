import { Component } from '@angular/core';
import { AdminService, CognitoService } from '@app/_services';
import { AppInfo, IUser } from '@app/_models';
import { environment } from '@environments/environment';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { MessagesComponent } from './_components/messages/messages.component';
import { NgIf } from '@angular/common';
import { KpMenuComponent } from './_components';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  standalone: true,
  imports: [RouterLink, KpMenuComponent, NgIf, RouterOutlet, MessagesComponent]
})
export class AppComponent {

  appInfo: AppInfo;
  user: IUser;
  isAuthenticated = false;

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
    this.cognitoService.getUser()
      .then((user: any) => {
        if (user) {
          this.user = user.attributes;
        }
      });

    this.cognitoService.isAuthenticated()
      .then((success: boolean) => {
        this.isAuthenticated = success;
      });

    this.adminService.ping().then(
      r => {
        console.log(r);
      });

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
