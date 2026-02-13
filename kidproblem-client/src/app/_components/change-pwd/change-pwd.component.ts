import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CognitoService, MessageService } from '@app/_services/';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatButtonModule } from '@angular/material/button';
import { NgClass, NgIf } from '@angular/common';
import { MatCardModule } from '@angular/material/card';

@Component({
    selector: 'app-change-pwd',
    templateUrl: './change-pwd.component.html',
    styleUrls: ['./change-pwd.component.css'],
    standalone: true,
    imports: [MatCardModule, ReactiveFormsModule, NgClass, NgIf, MatButtonModule, MatProgressBarModule]
})
export class ChangePwdComponent implements OnInit {

  passwordForm: UntypedFormGroup;
  isValid = true;
  isLoading = false;
  submitted = false;

  constructor(
    private formBuilder: UntypedFormBuilder,
    private cognitoService: CognitoService,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.passwordForm = this.getPasswordForm();
  }

  private getPasswordForm(): UntypedFormGroup {
    return this.formBuilder.group({
      oldPassword: ['', Validators.required],
      newPassword1: ['', Validators.required],
      newPassword2: ['', Validators.required]
    });
  }

  get f() { return this.passwordForm.controls; }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.passwordForm.invalid) {
      return;
    }

    if (this.f.newPassword1.value !== this.f.newPassword2.value) {
      this.isValid = false;
      this.messageService.openSnackBar('You entered different new password');
      this.messageService.add('You entered different new password');
      return;
    }

    this.isLoading = true;

    this.cognitoService.getCurrentAuthenticatedUser().then(
      user => {
        const cognitoUser = user;

        this.cognitoService.changePassword(cognitoUser, this.f.oldPassword.value, this.f.newPassword1.value)
          .then(
            result => {
              // console.log(result);
              if (result === 'SUCCESS') {
                this.messageService.openSnackBar('Successfully updated password');
              } else {
                this.messageService.openSnackBar('Failed to update password');
                this.messageService.add(`Failed to update password: ${result}`);
              }
              this.isLoading = false;
            }
          )
          .catch((err) => {
            // console.log(err);
            this.messageService.openSnackBar('Failed to update password');
            this.messageService.add(`Failed to update password: ${err.message}`);
            this.isLoading = false;
          });

      }
    );

  }

}
