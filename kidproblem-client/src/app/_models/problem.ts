import { BaseModel } from '@app/_models/adm/base-model';

export class Problem  extends BaseModel {
    ProblemTitle: string;
    ProblemCategory: string;
    ProblemYear: string;
    ProblemNumber: string;
    ProblemText: string;
    ProblemTextBase64?: string;
    ProblemAnswer: string;
    ProblemTags?: string[];
    IsStaging: boolean;
    SolutionText: string;
    AnswerOptions: string;
}
