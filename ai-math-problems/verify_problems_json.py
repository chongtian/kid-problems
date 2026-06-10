import json
import logging
import sys
from ai_problems import fix_problem, initialize_local_llm, initialize_openai_llm, verify_and_fix_problem

logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',
    handlers=[
        logging.StreamHandler(stream=sys.stdout)
    ]
)

logger = logging.getLogger(__name__)

def main(json_problem_file:str, output_file:str):
    open_ai = False
    if open_ai:
        llm = initialize_openai_llm()
    else:
        llm = initialize_local_llm()

    with open(json_problem_file, 'r', encoding='utf-8') as file:
        data = json.load(file)
        cleaned_problems = verify_and_fix_problem(llm, data)               

    with open(output_file, 'w', encoding='utf-8' ) as f:
        json.dump(cleaned_problems, f)
   

if __name__ == "__main__":
    args = sys.argv
    if len(args) < 3:
        print("Usage: python script.py <json file> <output file>")
        print("example: python script.py test.json clean.json")
        exit(1)
    main(args[1], args[2])    