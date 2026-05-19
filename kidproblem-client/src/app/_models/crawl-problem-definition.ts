export interface CrawlProblemDefinition {
    StartUrl: string;
    ProblemCategory: string;
    ProblemYear: string;
    RegexPattern: string;
    UseSinglePattern?: boolean;
    StartPattern: string;
    EndPattern: string;
}
