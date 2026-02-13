import { HttpParams } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { BaseHttpService, ExamRunService } from '@app/_services';
import { BaseHttpServiceStub } from '../stubs/base-http-service-stub';
import { ExamRunDetail } from '@app/_models';

describe('Test ExamRunService', () => {
    // this is the service to be tested
    let service: ExamRunService;

    // prepare spy objects for all dependencies
    let baseHttpServiceStub: BaseHttpServiceStub;

    // create a stub
    const stubValue = {};

    beforeEach(() => {
        baseHttpServiceStub = new BaseHttpServiceStub();
        TestBed.configureTestingModule({ providers: [{ provide: BaseHttpService, useValue: baseHttpServiceStub }] });
        service = TestBed.inject(ExamRunService);
    });

    it('getExamRun() should work', () => {
        const id = 'fake-id';
        service.getExamRun(id).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/examrun/${id}`);
        expect(baseHttpServiceStub.httpMethod).toBe('GET');
    });

    it('getExamRunDetail() should work', () => {
        const id = 'fake-id';
        service.getExamRunDetail(id).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/examrun/detail/${id}`);
        expect(baseHttpServiceStub.httpMethod).toBe('GET');
    });

    it('queryExamRuns() for family should work', () => {
        const startDate = new Date(2023, 0, 1);
        const endDate = new Date(2023, 1, 15);
        const paginationToken = '_first_page_';
        const size = 25;

        service.queryExamRuns(startDate, endDate, paginationToken, size, true).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe('/examrun/query/family');
        expect(baseHttpServiceStub.httpMethod).toBe('GET');
        const httpParams = baseHttpServiceStub.stubValue.Data as HttpParams;
        expect(httpParams.get('startTimeValue')).toBe(startDate.toISOString());
        expect(httpParams.get('endTimeValue')).toBe(endDate.toISOString());
        expect(httpParams.get('pagination')).toBe(paginationToken);
        expect(httpParams.get('size')).toBe(size.toString());
    });

    it('queryExamRuns() for child should work', () => {
        const startDate = new Date(2023, 0, 1);
        const endDate = new Date(2023, 1, 15);
        const paginationToken = '_first_page_';
        const size = 25;

        service.queryExamRuns(startDate, endDate, paginationToken, size, false).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe('/examrun/query/child');
        expect(baseHttpServiceStub.httpMethod).toBe('GET');
        const httpParams = baseHttpServiceStub.stubValue.Data as HttpParams;
        expect(httpParams.get('startTimeValue')).toBe(startDate.toISOString());
        expect(httpParams.get('endTimeValue')).toBe(endDate.toISOString());
        expect(httpParams.get('pagination')).toBe(paginationToken);
        expect(httpParams.get('size')).toBe(size.toString());
    });

    it('createExamRunFromAssignment() should work', () => {
        const assignmentId = 'fake-id';
        service.createExamRunFromAssignment(assignmentId).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/examrun/${assignmentId}`);
        expect(baseHttpServiceStub.httpMethod).toBe('POST');
    });

    it('updateExamRunDetail() should work', () => {
        const detail: ExamRunDetail = {Id:'fake-detail-id', ProblemTitle:'problem title'}
        service.updateExamRunDetail(detail).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/examrun/detail/${detail.Id}`);
        expect(baseHttpServiceStub.httpMethod).toBe('PUT');
        expect(baseHttpServiceStub.stubValue.Data.Id).toBe(detail.Id);
    });

    it('completeExamRun() should work', () => {
        const id = 'fake-exam-id';
        service.completeExamRun(id).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/examrun/complete/${id}`);
        expect(baseHttpServiceStub.httpMethod).toBe('PUT');
    });

    it('deleteExamRun() should work', () => {
        const id = 'fake-exam-id';
        service.deleteExamRun(id).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/examrun/${id}`);
        expect(baseHttpServiceStub.httpMethod).toBe('DELETE');
    });

});
