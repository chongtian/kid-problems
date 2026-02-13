import { Injectable } from '@angular/core';
import { Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

import { CognitoService } from '@app/_services';

@Injectable({ providedIn: 'root' })
export class AuthGuard {
    constructor(
        private router: Router,
        private cognitoService: CognitoService
    ) { }

    async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        const isAuthorized = await this.cognitoService.isAuthenticated();
        if (isAuthorized) {
            const requiredAccess: number = +route.data.requiredAccess;
            const access = await this.cognitoService.getUserAccess();
            if ((access | requiredAccess) === access) {
                return true;
            }
        }
        
        // not logged in so redirect to login page with the return url
        this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
        return false;
    }

}
