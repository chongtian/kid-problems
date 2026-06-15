from datetime import datetime
import logging
import os
import sys
from kidproblem_apis import create_assignment, create_exam_definition, get_access_token_from_cognito
import random
import string

PRODUCTION = True
USERNAME = os.getenv("COGNITO_USERNAME")
PASSWORD = os.getenv("COGNITO_PASSWORD")

logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',
    handlers=[
        logging.StreamHandler(stream=sys.stdout)
    ]
)

logger = logging.getLogger(__name__)

def generate_random_string(k:int) -> str:
    character_pool = string.ascii_letters + string.digits
    random_string = "".join(random.choices(character_pool, k=k))
    return random_string

def generate_exam_title(exam_title_prefix:str=None) -> str:
    if exam_title_prefix is None:
        today = datetime.today()
        exam_title = f"Practice {today.strftime('%m%d')} - {generate_random_string(4)}"
    else:
        exam_title = f"{exam_title_prefix} - {generate_random_string(2)}"
    return exam_title      

def main(start_year:int, end_year:int, number:str, exam_title:str=None):
    if not all([USERNAME, PASSWORD]):
        raise ValueError("One or more required environment variables are missing.")

    problem_list = []
    common_suffixes = ['A', 'B']
    special_suffixes = ['FA', 'FB', 'SA', 'SB']
    for y in range(start_year, end_year + 1):
        suffixes = special_suffixes if y == 2021 else common_suffixes
        for suffix in suffixes:
            problem_title = f"AMC12-{y}{suffix}-{number.rjust(3, '0')}"
            problem_list.append({"ProblemCategory":"AMC12", "ProblemTitle":problem_title, "ProblemYear":"Mixed"})
    logger.debug(problem_list)
    random.shuffle(problem_list)
    
    access_token = get_access_token_from_cognito(USERNAME, PASSWORD)
    exam_def = create_exam_definition(generate_exam_title(exam_title), problem_list, access_token, PRODUCTION ) 
    if not all([exam_def["ExamCategory"], exam_def["ExamTitle"]]):
        logger.error("Failed to create exam definition.")
        return
    
    asn = create_assignment(exam_def["ExamCategory"], exam_def["ExamTitle"], access_token, PRODUCTION)
    if asn["Id"]:
        logger.info('Assignment has been created.')
    else:
        logger.error('Failed to create Assignment.')


if __name__ == "__main__":
    args = sys.argv
    if len(args) < 4:
        print("Usage: python create_assignment.py <start_year> <end_year> <number> [exam title]")
        print("example: python create_assignment.py 2015 2019 1 AMC12_Problem_01")
        exit(1)
    main(int(args[1]), int(args[2]), args[3], exam_title=args[4] if len(args) > 4 else None)
    