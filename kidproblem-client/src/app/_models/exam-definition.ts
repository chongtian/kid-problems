import { BaseModel } from '@app/_models/adm/base-model';

export class ExamDefinition extends BaseModel {
    ExamCategory: string = '';
    ExamTitle: string = '';
    ExamYear?: string;
    ExamType?: string; // H - Home Practice; O - Official
    Active: boolean = true;
    Memo?: string;
    ExamDetails?: ExamDetail[];
}

export interface ExamDetail {
    ProblemTitle: string;
    ProblemAnswer?: string;
    AnswerOptions?: string;
}

export interface ExamDefinitionId {
    ExamCategory: string;
    ExamTitle: string;
}