from datetime import datetime
import json
import logging
import os
import sys
from kidproblem_apis import create_assignent, create_exam_definition, prepare_problems, save_problems, get_access_token_from_cognito
import random
import string

PRODUCTION = False

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

def generate_problem_year() -> str:
    today = datetime.today()
    problem_year = f"C{today.strftime('%m%d')}{generate_random_string(2).upper()}"
    return problem_year

def generate_exam_title(exam_title_prefix:str=None) -> str:
    if exam_title_prefix is None:
        today = datetime.today()
        exam_title = f"Practice {today.strftime('%m%d')} - {generate_random_string(4)}"
    else:
        exam_title = f"{exam_title_prefix} - {generate_random_string(2)}"
    return exam_title    

def main(json_problem_file:str, exam_title:str=None):

    USERNAME = os.getenv("COGNITO_USERNAME")
    PASSWORD = os.getenv("COGNITO_PASSWORD")
    
    if not all([USERNAME, PASSWORD]):
        raise ValueError("One or more required environment variables are missing.")
    
    data = {}

    problem_year = generate_problem_year() 

    try:
        with open(json_problem_file, 'r', encoding='utf-8') as file:
            data = json.load(file)
            
            problems = prepare_problems(data, 1, problem_year)
            if not problems or len(problems) == 0:
                logger.info("No valid math problem is generated.")
                return
            for problem in problems:
                problem["IsStaging"] = False
            logger.info("Math problems are processed.")
            logger.debug(problems)

    except FileNotFoundError:
        print("The file doesn't exist.")
    except json.JSONDecodeError:
        print("The file is not valid JSON.")    

    access_token = get_access_token_from_cognito(USERNAME, PASSWORD)
    saved_problems = save_problems(problems, access_token, PRODUCTION)
    logger.info("Math problems are saved.")
    logger.debug(saved_problems) 

    exam_def = create_exam_definition(generate_exam_title(exam_title), saved_problems, access_token, PRODUCTION ) 
    if not all([exam_def["ExamCategory"], exam_def["ExamTitle"]]):
        logger.error("Failed to create exam definition.")
        return
    
    asn = create_assignent(exam_def["ExamCategory"], exam_def["ExamTitle"], access_token, PRODUCTION)
    if asn["Id"]:
        logger.info('Assignment has been created.')
    else:
        logger.error('Failed to create Assignment.')


if __name__ == "__main__":
    args = sys.argv
    if len(args) < 2:
        print("Usage: python create_assignment.py <json file> [exam_title]")
        print("example: python create_assignment.py test.json pratice")
        exit(1)
    json_file_path = args[1]
    exam_title_prefix = args[2] if len(args)>2 else None

    main(args[1], exam_title_prefix)
    