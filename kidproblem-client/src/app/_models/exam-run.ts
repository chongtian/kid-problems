import { BaseModel } from '@app/_models/adm/base-model';

export class ExamRun extends BaseModel {
    Id: string = '';
    AnswerBy: string = '';
    AnswerByFullname: string = '';
    CreateTime!: Date;
    AssignmentId: string = '';
    ExamTitle: string = '';
    ExamCategory: string = '';
    StartTime?: Date;
    CompleteTime?: Date;
    FamilyId?: string;
    TotalCount: number = 0;
    CorrectCount?: number;
    GuessCount?: number;
    GuessCorrectCount?: number;
    TotalDuration?: number;
    ExamRunDetails: ExamRunDetail[] = [];
    Amc10Score?: number;
}

export interface ExamRunDetail {
    Id: string;
    ProblemTitle: string;
    UserAnswer?: string;
    IsGuess?: boolean;
    Duration?: number;
    IsCorrect?: boolean;
}