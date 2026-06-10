import json
from pathlib import Path
import logging
import sys
from ai_problems import initialize_local_llm, initialize_openai_llm, generate_math_problems_json

logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',
    handlers=[
        logging.StreamHandler(stream=sys.stdout)
    ]
)

logger = logging.getLogger(__name__)

def main(output_file:str):
    open_ai = False
    if open_ai:
        llm = initialize_openai_llm()
    else:
        llm = initialize_local_llm()

    result = generate_math_problems_json(llm, output_file)
    print(len(result))
   

if __name__ == "__main__":
    args = sys.argv
    if len(args) < 2:
        print("Usage: python generate_problems_json.py <json file>")
        print("example: python generate_problems_json.py test.json")
        exit(1)
    main(args[1])    