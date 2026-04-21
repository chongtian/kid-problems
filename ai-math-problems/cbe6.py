"""
This script generates math problems for CBE6 math practice, using a language model and saves them to a database.
"""

import os 
from ai_problems import generate_math_problems, initialize_openai_llm, initialize_local_llm, create_math_prompt_template_2, get_json_schema
from kidproblem_apis import prepare_problems, save_problems, get_access_token_from_cognito
import config
import logging
import sys
import re
from pathlib import Path

logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',
    handlers=[
        logging.StreamHandler(stream=sys.stdout)
    ]
)

logger = logging.getLogger(__name__)

USE_OPEN_AI = False


def get_objective(topic_num : int| None = None) -> list:
    """
    Read all objectives from a local text file, and return the objective

    Args:
        topic_num is the index of the objective in the collection of all objectives. It is zero-based.

    Returns:
        Objective or a collection of objectives
        Objective has the schema: 
            {
            "objective":"the name of the objective", 
            "count":"the count of the preoblems belong to the objective", 
            "text":"objective details"
            }
        if the topic_num is within the range of all objectives, returns the exact objective.
        otherwise, returns all objectives

    """
    with open(config.TOPCIS_FILE, "r", encoding="utf-8") as f:
        s = f.read()
    objectives = []
    raw = s.split("########")
    regex = re.compile(r"Objective:.+?(\d)\nCount:.+?(\d{1,2})(.*)", flags=re.DOTALL)
    for t in raw:
        objective = {}
        matches = regex.finditer(t)
        match = next(matches, None)
        if match:
            objective["objective"]=match.group(1)
            objective["count"]=(int) (match.group(2))
            objective["text"]=(match.group(3)).strip()
            objectives.append(objective)
    
    if not (topic_num is None) and topic_num >=0 and topic_num < len(objectives):
        return [objectives[topic_num]]      
    else:
        return objectives


def generate_prompt_text_files():
    logger.info('Generating prompt text files ... ')
    prompt_template = create_math_prompt_template_2()
    objectives = get_objective(-1)
    schema = get_json_schema(False)

    prompts_dir = Path(config.PROMPTS_FOLDER)
    prompts_dir.mkdir(exist_ok=True)

    for objective in objectives:
        objective_num = objective["objective"]
        count = objective["count"] 
        text = objective["text"]
        formatted_text = prompt_template.format(count=count, objective=text, schema=schema)
        prompt_filename = f"prompt-{objective_num}.txt"
        prompt_file_path = prompts_dir / prompt_filename
        prompt_file_path.write_text(formatted_text, encoding="utf-8")
        logger.info(f'{prompt_filename} is saved.')


def main(objective_num: int, start_num: int, production: bool):
     # Read sensitive values from environment variables
    USERNAME = os.getenv("COGNITO_USERNAME")
    PASSWORD = os.getenv("COGNITO_PASSWORD")
    OPENAI_API_KEY = os.getenv('OPENAI_API_KEY')
    
    if not all([USERNAME, PASSWORD]):
        raise ValueError("One or more required environment variables are missing.")
    
    if OPENAI_API_KEY and USE_OPEN_AI:
        llm = initialize_openai_llm()
        logger.info("Use OpenAI Model") 
    else:
        llm = initialize_local_llm()
        logger.info("Use Local Model") 
    
    logger.info(f'The generated math problems will be saved to {"Production" if production else "Staging"} environment.')
  
    logger.info(f'Reading CBE math learning objective ... ')
    objectives = get_objective(objective_num)
    logger.info(f'There are {len(objectives)} CBE math learning objective(s)')

    logger.info("Call llm to generate math problems ... ")
    raw_problems =  generate_math_problems(llm, objectives, USE_OPEN_AI)
    logger.info("Math problems are generated.")
    logger.debug(raw_problems)  

    problems = prepare_problems(raw_problems, start_num)
    if not problems or len(problems) == 0:
        logger.info("No valid math problem is generated.")
        return
    logger.info("Math problems are processed.")
    logger.debug(problems)

    access_token = get_access_token_from_cognito(USERNAME, PASSWORD)
    results = save_problems(problems, access_token, production)
    logger.info("Math problems are saved.")
    logger.debug(results)
    

if __name__ == "__main__":
    args = sys.argv
    if len(args) > 1 and args[1].lower().strip() == 'p':
        generate_prompt_text_files()
        exit(0)

    if len(args) < 3:
        print("Usage: python cbe6.py <1-based topic number> <start problem number> [prod]")
        exit(1)

    if args[1].isdigit():
        objective_num = int(args[1])-1
    else:
        # this will pull in all objectives
        objective_num = -1
    
    if len(args) > 2 and args[2].isdigit():
        start_num = int(args[2])
    else:
        start_num = 1

    if len(args) > 3 and args[3].strip().lower() == 'prod':
        production = True
    else:
        production = False        

    main(objective_num, start_num, production)