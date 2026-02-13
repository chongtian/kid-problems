import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ExamDefinition, Pagination, Assignment } from '@app/_models';
import { BaseHttpService } from '@app/_services/base-http-service';

const baseUrl = '/assignment';

@Injectable({
  providedIn: 'root'
})
export class AssignmentService {

  constructor(private httpService: BaseHttpService) { }

  /**
 * Get single Assignment
 * @param id Id of an existing Assignment 
 * @returns An Assignment or 404 Not Found error
 */
  getAssignment(id: string): Promise<Assignment> {
    return this.httpService.get(`${baseUrl}/${id}`).toPromise<Assignment>();
  }

  /**
* Query Assignment
* @param startDate 
* @param endDate 
* @param paginationToken set this to "_first_page_" if pagination is required. Otherwise set it to null or empty string
* @param size The number of records which will be returned in each page
* @returns A Pagination object, which has 2 properties: data and pagination. 
* data is an array of Assignment; pagination is the token which can be send back
*/
  queryAssignments(startDate: Date, endDate: Date, paginationToken: string, size: number): Promise<Pagination<Assignment>> {
    const httpParams = new HttpParams()
      .set('pagination', paginationToken)
      .set('size', size.toString())
      .set('startTimeValue', startDate.toISOString())
      .set('endTimeValue', endDate.toISOString());
    return this.httpService.get(baseUrl, httpParams).toPromise<Pagination<Assignment>>();
  }

  /**
* Create an Assignment from an Exam Definition
* @param payload This must be an existing Exam Definition
* @returns Check ReturnResult to see if there is any error.
*/
  createAssignmentFromDefinition(payload: ExamDefinition): Promise<Assignment> {
    return this.httpService.post(baseUrl, payload).toPromise<Assignment>();
  }

  /**
 * Update an Assignment
 * @param payload 
 * @returns Check ReturnResult to see if there is any error.
 */
  updateAssignment(payload: Assignment): Promise<Assignment> {
    return this.httpService.put(`${baseUrl}/${payload.Id}`, payload).toPromise<Assignment>();
  }

  /**
   * Delete an Assignment. 
   * @param id Id of an existing Assignment
   * @returns Check ReturnResult to see if there is any error.
   */
  deleteAssignment(id: string): Promise<Assignment> {
    return this.httpService.delete(`${baseUrl}/${id}`).toPromise<Assignment>();
  }

}
