import { HttpParams } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { BaseHttpService, SummaryService } from '@app/_services';
import { BaseHttpServiceStub } from '../stubs/base-http-service-stub';
import { Problem } from '@app/_models';

describe('Test SummaryService', () => {
    // this is the service to be tested
    let service: SummaryService;

    // prepare spy objects for all dependencies
    let baseHttpServiceStub: BaseHttpServiceStub;

    // create a stub
    const stubValue = {};

    const payload: Problem = {
        ProblemTitle: 'AMC10-2023A-020',
        ProblemCategory: 'AMC10',
        ProblemYear: '2023A',
        ProblemNumber: '20',
        ProblemText: 'test problem',
        AnswerOptions: 'A,B,C,D,E',
        ProblemAnswer: 'A',
        IsStaging: true,
        SolutionText: 'solution text'
    };

    beforeEach(() => {
        baseHttpServiceStub = new BaseHttpServiceStub();
        TestBed.configureTestingModule({ providers: [{ provide: BaseHttpService, useValue: baseHttpServiceStub }] });
        service = TestBed.inject(SummaryService);
    });

    it('getProblemSummary() should work', () => {
        const title = 'AMC10-2023A-025';
        const answerBy = 'yinkaigao';
        service.getProblemSummary(title, answerBy).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/problemsummary/${answerBy}/${title}`);
        expect(baseHttpServiceStub.httpMethod).toBe('GET');
    });

    it('queryExamSummaries() should work', () => {
        const answerBy = 'yinkaigao';
        const paginationToken = '_first_page_';
        const size = 25;

        service.queryExamSummaries(answerBy, paginationToken, size).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/examsummary/${answerBy}`);
        expect(baseHttpServiceStub.httpMethod).toBe('GET');
        const httpParams = baseHttpServiceStub.stubValue.Data as HttpParams;
        expect(httpParams.get('pagination')).toBe(paginationToken);
        expect(httpParams.get('size')).toBe(size.toString());
    });

    it('queryProblemSummaries() should work', () => {
        const answerBy = 'yinkaigao';
        const category = 'AMC10';
        const keyword = 'AMC10-2022';
        const trueCorrectRateRng = '0-0.2';
        const paginationToken = '_first_page_';
        const size = 25;

        service.queryProblemSummaries(category, answerBy, keyword, trueCorrectRateRng, paginationToken, size).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/problemsummary/category/${category}/query`);
        expect(baseHttpServiceStub.httpMethod).toBe('GET');
        const httpParams = baseHttpServiceStub.stubValue.Data as HttpParams;
        expect(httpParams.get('answerBy')).toBe(answerBy);
        expect(httpParams.get('keyword')).toBe(keyword);
        expect(httpParams.get('correct')).toBe(trueCorrectRateRng);
        expect(httpParams.get('pagination')).toBe(paginationToken);
        expect(httpParams.get('size')).toBe(size.toString());
    });

});
