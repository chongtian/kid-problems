import { BaseModel } from '@app/_models/adm/base-model';

export class ExamSummary extends BaseModel {
    ProblemCategory: string = '';
    AnswerBy: string = '';
    AnswerByFullname: string = '';
    FamilyId: string = '';
    TotalCount: number = 0;
    CorrectCount: number = 0;
    GuessCount: number = 0;
    GuessCorrectCount: number = 0;
    TotalDuration: number = 0;
}
