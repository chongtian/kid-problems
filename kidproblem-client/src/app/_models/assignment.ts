import { BaseModel } from '@app/_models/adm/base-model';

export class Assignment extends BaseModel {
    Id: string = '';
    FamilyId: string = '';
    CreateTime!: Date;
    ExamCategory: string = '';
    ExamTitle: string = '';
    Memo?: string;
    IsComplete?: boolean;
    ExamRunIds?: string[];
    ChildId?: string;
}