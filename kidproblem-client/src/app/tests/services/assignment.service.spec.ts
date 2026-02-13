import { HttpParams } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { AssignmentService, BaseHttpService } from '@app/_services';
import { BaseHttpServiceStub } from '../stubs/base-http-service-stub';
import { Assignment, ExamDefinition } from '@app/_models';

describe('Test AssignmentService', () => {
    // this is the service to be tested
    let service: AssignmentService;

    // prepare spy objects for all dependencies
    let baseHttpServiceStub: BaseHttpServiceStub;

    // create a stub
    const stubValue = {};

    const payload: Assignment = {
        Id: 'fake-id', ExamCategory: 'AMC10', ExamTitle: 'Test', FamilyId: 'TestFamily', CreateTime: new Date(2023, 9, 26)
    };

    beforeEach(() => {
        baseHttpServiceStub = new BaseHttpServiceStub();
        TestBed.configureTestingModule({ providers: [{ provide: BaseHttpService, useValue: baseHttpServiceStub }] });
        service = TestBed.inject(AssignmentService);
    });

    it('getAssignment() should work', () => {
        const id = 'fake-id';
        service.getAssignment(id).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/assignment/${id}`);
        expect(baseHttpServiceStub.httpMethod).toBe('GET');
    });

    it('queryAssignments() should work', () => {
        const startDate = new Date(2023, 0, 1);
        const endDate = new Date(2023, 1, 15);
        const paginationToken = '_first_page_';
        const size = 25;

        service.queryAssignments(startDate, endDate, paginationToken, size).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe('/assignment');
        expect(baseHttpServiceStub.httpMethod).toBe('GET');
        const httpParams = baseHttpServiceStub.stubValue.Data as HttpParams;
        expect(httpParams.get('startTimeValue')).toBe(startDate.toISOString());
        expect(httpParams.get('endTimeValue')).toBe(endDate.toISOString());
        expect(httpParams.get('pagination')).toBe(paginationToken);
        expect(httpParams.get('size')).toBe(size.toString());
    });

    it('createAssignmentFromDefinition() should work', () => {
        const exam: ExamDefinition = {
            ExamCategory: 'AMC10', ExamTitle: 'Test', Active:true
        };
        service.createAssignmentFromDefinition(exam).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe('/assignment');
        expect(baseHttpServiceStub.httpMethod).toBe('POST');
        expect(baseHttpServiceStub.stubValue.Data.ExamCategory).toBe(exam.ExamCategory);
    });

    it('updateAssignment() should work', () => {
        service.updateAssignment(payload).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/assignment/${payload.Id}`);
        expect(baseHttpServiceStub.httpMethod).toBe('PUT');
        expect(baseHttpServiceStub.stubValue.Data.ExamCategory).toBe(payload.ExamCategory);
    });

    it('deleteAssignment() should work', () => {
        service.deleteAssignment(payload.Id).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/assignment/${payload.Id}`);
        expect(baseHttpServiceStub.httpMethod).toBe('DELETE');
    });

});
