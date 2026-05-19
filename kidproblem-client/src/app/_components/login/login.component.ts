import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { IUser } from '@app/_models';
import { CognitoService, LoadingBusService } from '@app/_services';
import { MatButtonModule } from '@angular/material/button';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  imports: [MatCardModule, MatFormFieldModule, MatInputModule, ReactiveFormsModule, FormsModule, MatButtonModule, RouterLink]
})
export class LoginComponent {

  user: IUser;
  result = '';
  showPassword = false;
  newPasswordRequired = false;
  password1 = '';
  password2 = '';
  resetPassword = false;
  code = '';
  private loading = inject(LoadingBusService);
  private cognitoUser: any;

  constructor(private router: Router,
    private cognitoService: CognitoService) {
    this.cognitoService.signOut().finally(
      () => { console.debug('Sign out the current user.'); }
    );
    this.user = {} as IUser;
  }

  signIn(): void {
    this.result = '';
    this.loading.start();
    this.cognitoService.signIn(this.user)
      .then((result) => {
        // console.log(result);
        this.cognitoUser = result;
        // this.cognitoService.getUserGroups().then(d=>{console.log(d);});
        const challengeName = result.challengeName;
        if (challengeName === 'NEW_PASSWORD_REQUIRED') {
          const userName = result.challengeParam.userAttributes.name || 'Unknown User';
          this.user.name = userName;
          console.log(`User ${userName} need to change the password.`);
          this.newPasswordRequired = true;
          this.resetPassword = false;
        } else {
          this.user = result.attributes;
          this.user.isAuthenticated = true;
          this.cognitoService.onCurrentUserChanged.emit(this.user);
          this.router.navigate(['/home']);
        }
      }).catch((err) => {
        this.result = err.message;
      })
      .finally(() => { this.loading.stop(); });
  }

  submit(): void {
    this.result = '';
    if (!this.password1 || !this.password2 || this.password1 !== this.password2) {
      this.result = 'Please enter password and ensure you have entered the same password twice';
      return;
    }

    if (this.resetPassword && !this.code) {
      this.result = 'Please enter confirmation code (check you email to get the code)';
      return;
    }

    this.loading.start();
    if (this.resetPassword) {
      this.cognitoService.submitForgetPassword(this.user.email, this.code, this.password1)
        .then(
          result => {
            console.log(result);
            if (result === 'SUCCESS') {
              this.newPasswordRequired = false;
              this.result = 'Please use your new password to sign in';
            } else {
              this.result = `Failed to update password: ${result}`;
            }
          }
        )
        .catch((err) => {
          // console.log(err);
          this.result = err.message;
        })
        .finally(() => { this.loading.stop(); });

    } else {
      this.cognitoService.submitNewPassword(this.cognitoUser, this.password1).then(
        result => {
          // console.log(result);
          this.user.password = this.password1;
          this.signIn();
        }
      ).catch((err) => {
        // console.log(err);
        this.result = err.message;
      })
        .finally(() => { this.loading.stop(); });
    }

  }

  forgetPassword() {
    this.result = '';
    if (!this.user.email) {
      this.result = 'Enter user name (email)';
    }

    this.loading.start();
    this.cognitoService.forgetPasswordRequest(this.user.email)
      .then(result => {
        // console.log(result);
        this.newPasswordRequired = true;
        this.resetPassword = true;
      }).catch(err => {
        this.result = err.message;
      })
      .finally(() => { this.loading.stop(); });
  }

}




