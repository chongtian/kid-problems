### KidProblem (Version 2.0)

This repository serves as a public showcase for KidProblem, a web application designed to help my children practice math skills. While the primary development, CI/CD pipelines, and project management are handled within Azure DevOps, this mirror provides a transparent look at the architecture and codebase.

This version represents a complete migration to a Serverless AWS architecture, focusing on scalability, security, and modern cloud-native patterns.

#### KidProblem | Full-Stack Serverless Web App

- **Primary DevOps Hub:** Azure DevOps (Repo, Boards, Pipelines).

- **Public Mirror:** GitHub (for portfolio visibility).

- **Backend:** C# / .NET 6 (AWS Lambda).

- **Frontend:** Angular 14+ (Amazon S3).

- **Database:** DynamoDB (NoSQL).

- **Security:** Amazon Cognito (JWT-based Auth).

#### Build and Test
To enable development mode in Lambda, change the value of environment variable *ASPNETCORE_ENVIRONMENT* to **Development**

This can be done in *aws-lambda-tools-defaults.json*, or update the environment variable directly in the console of Amazon Lambda.

#### Demo
[Click me to see the application](http://kidproblem.nowsuncorp.com)

|For|Username|Password|
|---|---|---|
|Parent|testuser|Demo123456!|
|Child|testkid|Demo123456!|
