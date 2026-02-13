import { Injectable } from '@angular/core';
import { BaseHttpService } from '@app/_services/base-http-service';
import { CrawlProblemDefinition, Pagination } from '@app/_models';
import { HttpParams } from '@angular/common/http';
import { Problem } from '@app/_models';

const baseUrl = '/problem';

@Injectable({
  providedIn: 'root'
})
export class ProblemService {

  constructor(private httpService: BaseHttpService) { }

  /**
   * Get a single Problem
   * @param problemTitle Problem title is the unique id of a problem.
   * It is in the format of <Category>-<Year>-<Number>
   * @returns A Problem or 404 Not Found error
   */
  getProblem(problemTitle: string): Promise<Problem> {
    return this.httpService.get(`${baseUrl}/${problemTitle}`).toPromise<Problem>();
  }

  /**
   * Query Problems
   * @param isStaging 
   * @param paginationToken set this to "_first_page_" if pagination is required. Otherwise set it to null or empty string
   * @param size The number of records which will be returned in each page
   * @param keyword In the format of "category-year-number". category is required. year and number are optional.
   * @returns A Pagination object, which has 2 properties: data and pagination. 
   * data is an array of Problem; pagination is the token which can be send back
   */
  queryProblems(isStaging: boolean, paginationToken: string, size: number, keyword?: string): Promise<Pagination<Problem>> {
    const httpParams = new HttpParams()
      .set('pagination', paginationToken)
      .set('size', size.toString())
      .set('keyword', keyword)
      .set('staging', isStaging ? 'y' : 'n');
    return this.httpService.get(baseUrl, httpParams).toPromise<Pagination<Problem>>();
  }

  /**
   * Create a Problem
   * @param payload 
   * @returns Check ReturnResult to see if there is any error.
   */
  createProblem(payload: Problem): Promise<Problem> {
    return this.httpService.post(baseUrl, payload).toPromise<Problem>();
  }

  /**
   * Update a Problem
   * @param payload 
   * @returns Check ReturnResult to see if there is any error.
   */
  updateProblem(payload: Problem): Promise<Problem> {
    return this.httpService.put(`${baseUrl}/${payload.ProblemTitle}`, payload).toPromise<Problem>();
  }

  /**
   * Delete a Problem. 
   * @param ProblemTitle 
   * @returns Check ReturnResult to see if there is any error.
   */
  deleteProblem(problemTitle: string): Promise<Problem> {
    return this.httpService.delete(`${baseUrl}/${problemTitle}`).toPromise<Problem>();
  }

  /**
   * Bulk update problem answers
   * @param payload A Problem object which has ProblemTitle and ProblemAnswer populated
   * @returns Check ReturnResult to see if there is any error.
   */
  updateAnswers(payload: Problem[]): Promise<Problem[]> {
    return this.httpService.put(`${baseUrl}/bulk/answers`, payload).toPromise<Problem[]>();
  }

  /**
 * Bulk update the IsStage flags to false, which move Problems out of staging area
 * @param payload A Problem object which has ProblemTitle populated
 * @returns Check ReturnResult to see if there is any error.
 */
  updateStagingFlags(payload: Problem[]): Promise<Problem[]> {
    return this.httpService.put(`${baseUrl}/bulk/unstaging`, payload).toPromise<Problem[]>();
  }

  /**
   * Scrap problems based on the given definition
   * @param payload 
   * @returns Check ReturnResult to see if there is any error.
   */
  crawlProblems(payload: CrawlProblemDefinition): Promise<Problem[]> {
    return this.httpService.post(`${baseUrl}/scrap`, payload).toPromise<Problem[]>();
  }

  /**
 * Bulk create problems
 * @param payload 
 * @returns Check ReturnResult to see if there is any error.
 */
  bulkCreate(payload: Problem[]): Promise<Problem[]> {
    return this.httpService.post(`${baseUrl}/bulk/create`, payload).toPromise<Problem[]>();
  }

}
