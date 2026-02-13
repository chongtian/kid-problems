import { Injectable } from '@angular/core';
import { BaseHttpService } from '@app/_services/base-http-service';
import { HttpParams } from '@angular/common/http';
import { ExamSummary, Pagination, ProblemSummary } from '@app/_models';

const baseUrlExam = '/examsummary';
const baseUrlProblem = '/problemsummary';

@Injectable({
  providedIn: 'root'
})
export class SummaryService {

  constructor(private httpService: BaseHttpService) { }

  /**
 * Query Exam Summaries
 * @param answerBy 
 * @param paginationToken set this to "_first_page_" if pagination is required. Otherwise set it to null or empty string
 * @param size The number of records which will be returned in each page
 * @returns A Pagination object, which has 2 properties: data and pagination. 
 * data is an array of ExamSummary; pagination is the token which can be send back
 */
  queryExamSummaries(answerBy: string, paginationToken: string, size: number): Promise<Pagination<ExamSummary>> {
    const httpParams = new HttpParams()
      .set('pagination', paginationToken)
      .set('size', size.toString())
    return this.httpService.get(`${baseUrlExam}/${answerBy}`, httpParams).toPromise<Pagination<ExamSummary>>();
  }

  /**
   * Get single Problem Summary
   * @param title ProblemTitle of an existing Problem
   * @param answerBy 
   * @returns An Problem Summary or 404 Not Found error
   */
  getProblemSummary(title: string, answerBy: string): Promise<ProblemSummary> {
    return this.httpService.get(`${baseUrlProblem}/${answerBy}/${title}`).toPromise<ProblemSummary>();
  }

  /**
 * Query Problem Summaries
 * @param category an existing Problem Category 
 * @param answerBy an existing child username. The value can be empty, which will return records of all children.
 * @param keyword keyword is used to set filter on ProblemTitle using a contains operator. The value can be empty, which will return records of all problems.
 * @param trueCorrectRateRange it returns the problem summaries which has TrueCorrectRate within the given range, i.e., 0.05-0.5. The value can be empty, which will return records of all problems. 
 * @param paginationToken set this to "_first_page_" if pagination is required. Otherwise set it to null or empty string
 * @param size The number of records which will be returned in each page
 * @returns A Pagination object, which has 2 properties: data and pagination. 
 * data is an array of ProblemSummary; pagination is the token which can be send back
 */
  queryProblemSummaries(category: string, answerBy: string, keyword: string, trueCorrectRateRange: string, paginationToken: string, size: number): Promise<Pagination<ProblemSummary>> {
    const httpParams = new HttpParams()
      .set('answerBy', answerBy)
      .set('keyword', keyword)
      .set('correct', trueCorrectRateRange)
      .set('pagination', paginationToken)
      .set('size', size.toString())
    return this.httpService.get(`${baseUrlProblem}/category/${category}/query`, httpParams).toPromise<Pagination<ProblemSummary>>();
  }

}
