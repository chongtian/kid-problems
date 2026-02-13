import { EventEmitter, Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Amplify, Auth } from 'aws-amplify';
import { environment } from '../../environments/environment';
import { IUser } from '@app/_models';
import { Access } from '@app/_guards';

@Injectable({
  providedIn: 'root'
})
export class CognitoService {

  private authenticationSubject: BehaviorSubject<any>;

  onCurrentUserChanged: EventEmitter<IUser> = new EventEmitter();

  constructor() {
    Amplify.configure({
      Auth: environment.cognito,
    });

    this.authenticationSubject = new BehaviorSubject<boolean>(false);
  }

  public signIn(user: IUser): Promise<any> {
    return Auth.signIn(user.email, user.password);
  }

  public submitNewPassword(user: IUser, password: string): Promise<any> {
    return Auth.completeNewPassword(user, password);
  }

  public forgetPasswordRequest(userName: string) {
    return Auth.forgotPassword(userName);
  }

  public submitForgetPassword(username: string, code: string, password: string) {
    return Auth.forgotPasswordSubmit(username, code, password);
  }

  public changePassword(user: any, oldPassword: string, newPassword: string) {
    return Auth.changePassword(user, oldPassword, newPassword);
  }

  public signOut(): Promise<any> {
    return Auth.signOut()
      .then(() => {
        this.authenticationSubject.next(false);
        this.onCurrentUserChanged.emit({} as IUser);
      });
  }

  public isAuthenticated(): Promise<boolean> {
    if (this.authenticationSubject.value) {
      return Promise.resolve(true);
    } else {
      return Auth.currentAuthenticatedUser()
        .then((user: any) => {
          if (user) {
            return true;
          } else {
            return false;
          }
        }).catch(() => {
          return false;
        });
    }
  }

  public getUser(): Promise<any> {
    return Auth.currentUserInfo();
  }

  public getCurrentAuthenticatedUser(): Promise<any> {
    return Auth.currentAuthenticatedUser();
  }

  /**
   * Returns the Cognito User Groups of the current user.
   * This can be used to authorize a user and grant accesses
   * @returns Promise of an array of user group names
   */
  public getUserGroups(): Promise<any> {
    return Auth.currentSession().then((auth) => {
      return auth.getAccessToken().decodePayload()['cognito:groups'];
    });
  }

  /**
   * Parse cognito:groups to get user access
   * @returns User access
   */
  public getUserAccess(): Promise<number> {
    return this.getUserGroups().then(
      groups => {
        const userGroups = groups as string[];
        let access = Access.everyone;
        if (userGroups.includes('AdminUserGroup')) {
          access = Access.AdminUserGroup;
        } else if (userGroups.includes('ParentUserGroup')) {
          access = Access.ParentUserGroup;
        } else if (userGroups.includes('ChildUserGroup')) {
          access = Access.ChildUserGroup;
        }
        return access;
      }
    ).catch(
      () => {
        return Access.everyone;
      }
    );
  }

  public getToken(): Promise<any> {
    return Auth.currentSession().then((auth) => { return auth.getAccessToken() });
  }

}
