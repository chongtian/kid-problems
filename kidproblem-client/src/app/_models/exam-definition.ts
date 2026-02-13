import { BaseModel } from '@app/_models/adm/base-model';

export class ExamDefinition extends BaseModel {
    ExamCategory: string;
    ExamTitle: string;
    ExamYear?: string;
    ExamType?: string; // H - Home Practice; O - Official
    Active: boolean;
    Memo?: string;
    ExamDetails?: ExamDetail[];
}

export class ExamDetail {
    ProblemTitle: string;
    ProblemAnswer?: string;
    AnswerOptions?: string;
}

export interface ExamDefinitionId {
    ExamCategory: string;
    ExamTitle: string;
}