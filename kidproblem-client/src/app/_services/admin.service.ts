import { Injectable } from '@angular/core';
import { BaseHttpService } from './base-http-service';
import { InfoCentralCodeDetail } from '@app/_models';

const baseUrl = '/admin';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  constructor(private httpService: BaseHttpService) { }

  /**
   * ping() is used to activate the service hosted as an AWS Lambda function
   */
  ping(): Promise<any> {
    return this.httpService.get(`${baseUrl}/ping`).toPromise<any>();
  }

  /**
   * Get the names of all children under the current family group. 
   * The caller shall be a Parent user. 
   * @returns An array of child user names
   */
  getChildren(): Promise<string[]> {
    return this.httpService.get(`${baseUrl}/children`).toPromise<string[]>();
  }

  getCategoryCodes(): Promise<InfoCentralCodeDetail[]> {
    return this.httpService.get(`${baseUrl}/codes/KIDPROBLEM_CATEGORIES`).toPromise<InfoCentralCodeDetail[]>();
  }

  updateCategoryCodes(codes: InfoCentralCodeDetail[]): Promise<InfoCentralCodeDetail[]> {
    return this.httpService.put(`${baseUrl}/codes/KIDPROBLEM_CATEGORIES`, codes).toPromise<InfoCentralCodeDetail[]>();
  }
}
