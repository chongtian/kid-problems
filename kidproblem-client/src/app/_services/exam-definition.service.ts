import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ExamDefinition, Pagination } from '@app/_models';
import { BaseHttpService } from '@app/_services/base-http-service';

const baseUrl = '/examdef';

@Injectable({
  providedIn: 'root'
})
export class ExamDefinitionService {

  constructor(private httpService: BaseHttpService) { }

  /**
   * Get single Exam Definition
   * @param category 
   * @param title 
   * @returns An Exam Definition or 404 Not Found error
   */
  getExamDefinition(category: string, title: string): Promise<ExamDefinition> {
    return this.httpService.get(`${baseUrl}/${category}/${title}`).toPromise<ExamDefinition>();
  }

  /**
 * Query Exam Definitions
 * @param category The Category of exam definition
 * @param active true if only querying active exam definitions
 * @param paginationToken set this to "_first_page_" if pagination is required. Otherwise set it to null or empty string
 * @param size The number of records which will be returned in each page
 * @param keyword keyword is used to query Exam Title. The service queries with an operator of begin with.
 * @returns A Pagination object, which has 2 properties: data and pagination. 
 * data is an array of ExamDefinition; pagination is the token which can be send back
 */
  queryExamDefinitions(category: string, active: boolean, paginationToken: string, size: number, keyword?: string): Promise<Pagination<ExamDefinition>> {
    const httpParams = new HttpParams()
      .set('pagination', paginationToken)
      .set('size', size.toString())
      .set('keyword', keyword)
      .set('active', active ? 'y' : 'n');
    return this.httpService.get(`${baseUrl}/${category}`, httpParams).toPromise<Pagination<ExamDefinition>>();
  }

  /**
 * Create a Exam Definition
 * @param payload 
 * @returns Check ReturnResult to see if there is any error.
 */
  createExamDefinition(payload: ExamDefinition): Promise<ExamDefinition> {
    return this.httpService.post(baseUrl, payload).toPromise<ExamDefinition>();
  }

  /**
   * Update a Exam Definition
   * @param payload 
   * @returns Check ReturnResult to see if there is any error.
   */
  updateExamDefinition(payload: ExamDefinition): Promise<ExamDefinition> {
    return this.httpService.put(`${baseUrl}/${payload.ExamCategory}/${payload.ExamTitle}`, payload).toPromise<ExamDefinition>();
  }

  /**
   * Delete a Exam Definition. 
   * @param category 
   * @param title
   * @returns Check ReturnResult to see if there is any error.
   */
  deleteExamDefinition(category: string, title: string): Promise<ExamDefinition> {
    return this.httpService.delete(`${baseUrl}/${category}/${title}`).toPromise<ExamDefinition>();
  }

}
