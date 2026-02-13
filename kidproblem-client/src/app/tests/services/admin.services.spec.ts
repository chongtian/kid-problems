import { TestBed } from '@angular/core/testing';
import { AdminService, BaseHttpService } from '@app/_services';
import { BaseHttpServiceStub } from '../stubs/base-http-service-stub';
import { InfoCentralCodeDetail } from '@app/_models';

describe('Test AdminService', () => {
    // this is the service to be tested
    let adminService: AdminService;

    // prepare spy objects for all dependencies
    let baseHttpServiceStub: BaseHttpServiceStub;

    // create a stub
    const stubValue = {};

    beforeEach(() => {
        baseHttpServiceStub = new BaseHttpServiceStub();
        TestBed.configureTestingModule({ providers: [{ provide: BaseHttpService, useValue: baseHttpServiceStub }] });
        adminService = TestBed.inject(AdminService);
    });

    it('ping() should work', () => {
        adminService.ping().then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe('/admin/ping');
        expect(baseHttpServiceStub.httpMethod).toBe('GET');
    });

    it('getChildren() should work', () => {
        adminService.getChildren().then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe('/admin/children');
        expect(baseHttpServiceStub.httpMethod).toBe('GET');
    });

    it('getCategoryCodes() should work', () => {
        adminService.getCategoryCodes().then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe('/admin/codes/KIDPROBLEM_CATEGORIES');
        expect(baseHttpServiceStub.httpMethod).toBe('GET');
    });

    it('updateCategoryCodes() should work', () => {
        const payload: InfoCentralCodeDetail[] = [];
        adminService.updateCategoryCodes(payload).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe('/admin/codes/KIDPROBLEM_CATEGORIES');
        expect(baseHttpServiceStub.httpMethod).toBe('PUT');
    });

});
