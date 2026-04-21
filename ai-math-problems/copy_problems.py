"""
This script copies KidProblem problems from staging environment to production environment.
Usage: python copy_problems.py <Exam_Category> <Exam_Year> <Start_Problem_number> <End_Problem_Number>
For example, python copy_problems.py HOME C0410 1 10
will copy problems from HOME-C0410-001 to HOME-C0410-010 to production.
python copy_problems.py HOME C0410 1 1
will copy only one problem HOME-C0410-001 to production.
Note:
If the problem already exists in production, the problem in production will be replaced by the one from staging.
"""
import os 
from kidproblem_apis import save_problems, get_problem, get_access_token_from_cognito
import logging
import sys

logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',
    handlers=[
        logging.StreamHandler(stream=sys.stdout)
    ]
)

logger = logging.getLogger(__name__)

def main(category:str, year:str, start_num:int, end_num:int):
     # Read sensitive values from environment variables
    USERNAME = os.getenv("COGNITO_USERNAME")
    PASSWORD = os.getenv("COGNITO_PASSWORD")
    
    if not all([USERNAME, PASSWORD]):
        raise ValueError("One or more required environment variables are missing.")
    
    access_token = get_access_token_from_cognito(USERNAME, PASSWORD)
    problems = []
    for i in range(start_num,end_num+1):
        problem_title = f'{category}-{year}-{str(i).rjust(3, "0")}'
        problem = get_problem(problem_title, access_token, False)
        if problem:
            problem['Action'] = 1
            problems.append(problem)
    
    logger.info(f"Save {len(problems)} problems to PROD.")
    results = save_problems(problems, access_token, True)
    logger.info(results)


if __name__ == "__main__":
    args = sys.argv
    if len(args) < 5:
        print("Usage: python script.py <Category> <Year> <startNum> <endNum>")
        exit(1)

    category = args[1]
    year = args[2]

    if args[3].isdigit():
        start_num = int(args[3])
    else:
        print("<startNum> must be a valid integer.")
        exit(1)

    if args[4].isdigit():
        end_num = int(args[4])
    else:
        print("<endNum> must be a valid integer.")
        exit(1)

    main(category, year, start_num, end_num)