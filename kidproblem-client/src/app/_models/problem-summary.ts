import { BaseModel } from '@app/_models/adm/base-model';

export class ProblemSummary extends BaseModel {
    ProblemTitle: string;
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
