import { HttpParams } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { BaseHttpService, ProblemService } from '@app/_services';
import { BaseHttpServiceStub } from '../stubs/base-http-service-stub';
import { CrawlProblemDefinition, Problem } from '@app/_models';

describe('Test ProblemService', () => {
    // this is the service to be tested
    let service: ProblemService;

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
        service = TestBed.inject(ProblemService);
    });

    it('getProblem() should work', () => {
        const id = 'AMC10-2023A-025';
        service.getProblem(id).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/problem/${id}`);
        expect(baseHttpServiceStub.httpMethod).toBe('GET');
    });

    it('queryProblems() should work', () => {
        const keyword = 'AMC10-2023';
        const staging = false;
        const paginationToken = '_first_page_';
        const size = 25;

        service.queryProblems(staging, paginationToken, size, keyword).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe('/problem');
        expect(baseHttpServiceStub.httpMethod).toBe('GET');
        const httpParams = baseHttpServiceStub.stubValue.Data as HttpParams;
        expect(httpParams.get('keyword')).toBe(keyword);
        expect(httpParams.get('staging')).toBe('n');
        expect(httpParams.get('pagination')).toBe(paginationToken);
        expect(httpParams.get('size')).toBe(size.toString());
    });

    it('createProblem() should work', () => {
        service.createProblem(payload).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/problem`);
        expect(baseHttpServiceStub.httpMethod).toBe('POST');
        expect(baseHttpServiceStub.stubValue.Data.ProblemTitle).toBe(payload.ProblemTitle);
    });

    it('updateProblem() should work', () => {
        service.updateProblem(payload).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/problem/${payload.ProblemTitle}`);
        expect(baseHttpServiceStub.httpMethod).toBe('PUT');
        expect(baseHttpServiceStub.stubValue.Data.ProblemTitle).toBe(payload.ProblemTitle);
    });

    it('deleteProblem() should work', () => {
        const id = 'fake-problem-id';
        service.deleteProblem(id).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/problem/${id}`);
        expect(baseHttpServiceStub.httpMethod).toBe('DELETE');
    });

    it('updateAnswers() should work', () => {
        const problems: Problem[] = [payload];
        service.updateAnswers(problems).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/problem/bulk/answers`);
        expect(baseHttpServiceStub.httpMethod).toBe('PUT');
        expect(baseHttpServiceStub.stubValue.Data[0].ProblemTitle).toBe(payload.ProblemTitle);
    });

    it('updateStagingFlags() should work', () => {
        const problems: Problem[] = [payload];
        service.updateStagingFlags(problems).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/problem/bulk/unstaging`);
        expect(baseHttpServiceStub.httpMethod).toBe('PUT');
        expect(baseHttpServiceStub.stubValue.Data[0].ProblemTitle).toBe(payload.ProblemTitle);
    });

    it('bulkCreate() should work', () => {
        const problems: Problem[] = [payload];
        service.bulkCreate(problems).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/problem/bulk/create`);
        expect(baseHttpServiceStub.httpMethod).toBe('POST');
        expect(baseHttpServiceStub.stubValue.Data[0].ProblemTitle).toBe(payload.ProblemTitle);
    });

    it('crawlProblems() should work', () => {
        const definition: CrawlProblemDefinition = {
            StartUrl: 'http://someurl',
            ProblemCategory: 'AMC10',
            ProblemYear: '2023A',
            RegexPattern: 'pattern1',
            StartPattern: 'pattern2',
            EndPattern: 'pattern3'
        };
        service.crawlProblems(definition).then(_ => { });
        expect(baseHttpServiceStub.navigateUrl).toBe(`/problem/scrap`);
        expect(baseHttpServiceStub.httpMethod).toBe('POST');
        expect(baseHttpServiceStub.stubValue.Data.ProblemCategory).toBe(definition.ProblemCategory);
    });

});
