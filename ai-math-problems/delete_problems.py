"""
Delete problems.
Usage: python delete_problems.py <Category> <Year> <startNum> <endNum> [prod]
for example: python delete_problems.py HOME C0410 1 4 prod
will delete problems HOME-C0410-001 to HOME-C0410-004 from production environment.
"""

import os 
from kidproblem_apis import delete_problem, get_access_token_from_cognito
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

def main(category:str, year:str, start_num:int, end_num:int, production: bool):
     # Read sensitive values from environment variables
    USERNAME = os.getenv("COGNITO_USERNAME")
    PASSWORD = os.getenv("COGNITO_PASSWORD")
    
    if not all([USERNAME, PASSWORD]):
        raise ValueError("One or more required environment variables are missing.")    

    access_token = get_access_token_from_cognito(USERNAME, PASSWORD)
    for i in range(start_num,end_num+1):
        problem_title = f'{category}-{year}-{str(i).rjust(3, "0")}'
        logger.info(f"Deleting Problem {problem_title} if it already exists...")
        delete_problem(problem_title, access_token, production)
    

if __name__ == "__main__":
    args = sys.argv
    if len(args) < 5:
        print("Usage: python clean.py <Category> <Year> <startNum> <endNum> [prod]")
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

    if len(args) > 5 and args[5].lower().strip() == 'prod':
        prod = True
    else:
        prod = False

    main(category, year, start_num, end_num, prod)