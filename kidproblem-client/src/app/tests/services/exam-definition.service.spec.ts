import { HttpParams } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { BaseHttpService, ExamDefinitionService } from '@app/_services';
import { BaseHttpServiceStub } from '../stubs/base-http-service-stub';
import { ExamDefinition } from '@app/_models';

describe('Test ExamDefinitionService', () => {
    // this is the service to be tested
    let service: ExamDefinitionService;

    // prepare spy objects for all dependencies
    let baseHttpServiceStub: BaseHttpServiceStub;

    // create a stub
    const stubValue = {};

    const payload: ExamDefinition = {
        ExamCategory: 'AMC10', ExamTitle: 'Test', Active:true
    };

    beforeEach(() => {
        baseHttpServiceStub = new BaseHttpServiceStub();
        TestBed.configureTestingModule({ providers: [{ provide: BaseHttpService, useValue: baseHttpServiceStub }] });
        service = TestBed.inject(ExamDefinitionService);
    });

    it('getExamDefinition() should work', () => {
        const category = 'AMC10';
        const title = 'Test Exam';
        service.getExamDefinition(category, title).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/examdef/${category}/${title}`);
        expect(baseHttpServiceStub.httpMethod).toBe('GET');
    });

    it('queryExamDefinitions() should work', () => {
        const category = 'AMC10';
        const active = true;
        const keyword = "AMC Test";
        const paginationToken = '_first_page_';
        const size = 25;

        service.queryExamDefinitions(category, active, paginationToken, size, keyword).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/examdef/${category}`);
        expect(baseHttpServiceStub.httpMethod).toBe('GET');
        const httpParams = baseHttpServiceStub.stubValue.Data as HttpParams;
        expect(httpParams.get('active')).toBe('y');
        expect(httpParams.get('keyword')).toBe(keyword);
        expect(httpParams.get('pagination')).toBe(paginationToken);
        expect(httpParams.get('size')).toBe(size.toString());
    });

    it('createExamDefinition() should work', () => {
        
        service.createExamDefinition(payload).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe('/examdef');
        expect(baseHttpServiceStub.httpMethod).toBe('POST');
        expect(baseHttpServiceStub.stubValue.Data.ExamCategory).toBe(payload.ExamCategory);
    });

    it('updateExamDefinition() should work', () => {
        service.updateExamDefinition(payload).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/examdef/${payload.ExamCategory}/${payload.ExamTitle}`);
        expect(baseHttpServiceStub.httpMethod).toBe('PUT');
        expect(baseHttpServiceStub.stubValue.Data.ExamCategory).toBe(payload.ExamCategory);
    });

    it('deleteExamDefinition() should work', () => {
        service.deleteExamDefinition(payload.ExamCategory, payload.ExamTitle).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/examdef/${payload.ExamCategory}/${payload.ExamTitle}`);
        expect(baseHttpServiceStub.httpMethod).toBe('DELETE');
    });

});
