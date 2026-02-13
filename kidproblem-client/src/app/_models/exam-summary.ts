import { BaseModel } from '@app/_models/adm/base-model';

export class ExamSummary extends BaseModel {
    ProblemCategory: string;
    AnswerBy: string;
    AnswerByFullname: string;
    FamilyId: string;
    TotalCount: number;
    CorrectCount: number;
    GuessCount: number;
    GuessCorrectCount: number;
    TotalDuration: number;
}
