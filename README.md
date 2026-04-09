### KidProblem (Version 2.0)

This repository serves as a public showcase for KidProblem, a web application designed to help my children practice math skills. 

This version represents a complete migration to a Serverless AWS architecture, focusing on scalability, security, and modern cloud-native patterns.

#### KidProblem | Full-Stack Serverless Web App

- **Primary DevOps Hub:** GitHub and Jira

- **Backend:** C# / .NET 8 (AWS Lambda)

- **Frontend:** Angular 21+ (hosted in AWS S3) with MathJax

- **Database:** AWS DynamoDB

- **Security:** AWS Cognito

- **UI Test:** Selenium WebDriver + xUnit

#### Build and Test
To enable development mode in Lambda, change the value of environment variable *ASPNETCORE_ENVIRONMENT* to **Development**

This can be done in *aws-lambda-tools-defaults.json*, or update the environment variable directly in the console of Amazon Lambda.

#### Demo
[Click me to see the application](http://kidproblem.nowsuncorp.com)

|For|Username|Password|
|---|---|---|
|Parent|testuser|Demo123456!|
|Child|testkid|Demo123456!|

#### Story
This project was inspired by my son. In 2019, he participated in the AMC 8 math competition and began doing a large number of math practice problems. At first, I printed many worksheets for him, and after he completed them, I manually reviewed his answers to calculate his score. Very quickly, this process became time-consuming and inefficient.

I started to wonder whether I could build a web application where my son could view math problems, enter his answers, and receive an automatic score. This idea became KidProblem.

The first version of KidProblem was built using a MySQL database, Entity Framework, .NET Core, and the Angular framework. It was hosted on an AWS EC2 virtual machine running Linux. The application ran smoothly until 2023, when AWS announced that they would stop providing free public IP addresses for EC2 instances. As a result, the operating cost increased significantly. To address this, I completely rewrote the application using AWS DynamoDB, AWS Lambda, and AWS Cognito, which greatly reduced hosting costs. 

Previously, the source code was hosted in a private Azure DevOps repository. I set up a CI/CD pipeline to automatically build and deploy the application, and I used DevOps tools to track bugs, progress, and feature development. In 2026, I migrated the repository to this public GitHub repository to showcase my skills in web development and SDET.