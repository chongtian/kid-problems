import { Injectable } from '@angular/core';
import { BaseHttpService } from '@app/_services/base-http-service';
import { HttpParams } from '@angular/common/http';
import { ExamRun, ExamRunDetail, Pagination } from '@app/_models';

const baseUrl = '/examrun';


@Injectable({
  providedIn: 'root'
})
export class ExamRunService {

  constructor(private httpService: BaseHttpService) { }

  /**
 * Get single Exam Run Detail
 * @param id Id of an existing Exam Run Detail
 * @returns An Exam Run Detail or 404 Not Found error
 */
  getExamRunDetail(id: string): Promise<ExamRunDetail> {
    return this.httpService.get(`${baseUrl}/detail/${id}`).toPromise<ExamRunDetail>();
  }

  /**
* Get single Exam Run
* @param id Id of an existing Exam Run
* @returns An Exam Run or 404 Not Found error
*/
  getExamRun(id: string): Promise<ExamRun> {
    return this.httpService.get(`${baseUrl}/${id}`).toPromise<ExamRun>();
  }

  /**
* Query Exam Run
* @param startDate 
* @param endDate 
* @param paginationToken set this to "_first_page_" if pagination is required. Otherwise set it to null or empty string
* @param size The number of records which will be returned in each page
* @param queryFamily if true, it returns all exam in under the current family
* @returns A Pagination object, which has 2 properties: data and pagination. 
* data is an array of Exam Run; pagination is the token which can be send back
*/
  queryExamRuns(startDate: Date, endDate: Date, paginationToken: string, size: number, queryFamily: boolean): Promise<Pagination<ExamRun>> {
    const httpParams = new HttpParams()
      .set('pagination', paginationToken)
      .set('size', size.toString())
      .set('startTimeValue', startDate.toISOString())
      .set('endTimeValue', endDate.toISOString());
    if (queryFamily) {
      return this.httpService.get(`${baseUrl}/query/family`, httpParams).toPromise<Pagination<ExamRun>>();
    } else {
      return this.httpService.get(`${baseUrl}/query/child`, httpParams).toPromise<Pagination<ExamRun>>();
    }
  }

  /**
* Create an Exam Run from an Assignment
* @param assignmentId This must be the Id of an existing Assignment
* @returns Check ReturnResult to see if there is any error.
*/
  createExamRunFromAssignment(assignmentId: string): Promise<ExamRun> {
    return this.httpService.post(`${baseUrl}/${assignmentId}`, null).toPromise<ExamRun>();
  }

  /**
 * Delete an Exam Run. 
 * @param id Id of an existing Exam Run
 * @returns Check ReturnResult to see if there is any error.
 */
  deleteExamRun(id: string): Promise<ExamRun> {
    return this.httpService.delete(`${baseUrl}/${id}`).toPromise<ExamRun>();
  }

  /**
 * Update an Exam Run Detail
 * @param payload 
 * @returns Check ReturnResult to see if there is any error.
 */
  updateExamRunDetail(payload: ExamRunDetail): Promise<ExamRunDetail> {
    return this.httpService.put(`${baseUrl}/detail/${payload.Id}`, payload).toPromise<ExamRunDetail>();
  }

  /**
   * Complete an Exam Run. All the user answers shall have been submitted by updateExamRunDetail() already.
   * This method does not submit the user answers again.
   * @param id The Id of an existing Exam Run
   * @returns Check ReturnResult to see if there is any error.
   */
  completeExamRun(id: string): Promise<ExamRun> {
    return this.httpService.put(`${baseUrl}/complete/${id}`, null).toPromise<ExamRun>();
  }

}
