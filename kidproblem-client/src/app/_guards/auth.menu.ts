import { Injectable } from '@angular/core';
import { CognitoService } from '@app/_services';
import { Access } from './auth.access';

@Injectable({ providedIn: 'root' })
export class AuthMenuGuard {

    enableProblem: boolean;
    enableUser: boolean;
    enableLogout: boolean;
    enableExam: boolean;
    enableBrowseExam: boolean;
    enableAdm: boolean;

    constructor(
        private cognitoService: CognitoService,
    ) { }

    async setEnableFlags(): Promise<void> {
        const isAuthenticated = await this.cognitoService.isAuthenticated();

        if (isAuthenticated) {
            const access = await this.cognitoService.getUserAccess();
            this.enableProblem = (access | Access.child) === access;
            this.enableExam = (access | Access.child) === access;
            this.enableBrowseExam = (access | Access.child) === access;
            this.enableLogout = (access | Access.everyone) === access;
            this.enableAdm = (access | Access.adm) === access;

        } else {
            this.enableProblem = false;
            this.enableExam = false;
            this.enableUser = false;
            this.enableLogout = false;
            this.enableAdm = false;
        }

        this.enableUser = this.enableLogout;
    }
}
