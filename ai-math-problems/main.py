import gradio as gr
import os 
from ai_problems import generate_and_save_math_problems, initialize_openai_llm, initialize_local_llm
from kidproblem_apis import get_access_token_from_cognito
import logging
import sys
from datetime import datetime

from input_context import InputContext

logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',
    handlers=[
        logging.StreamHandler(stream=sys.stdout)
    ]
)

logger = logging.getLogger(__name__)

USE_OPEN_AI = False
PRODUCTION = False

global access_token, llm

def generate_problems(count: str, year: str, objective):
    if count.isdigit():
        count_of_problems = int(count)
    else:
        return "Please provide a valid count of problem."
    
    global access_token, llm
 
    if objective:
        input_context = InputContext(
            Count = count_of_problems,
            ProblemYear=year,
            ObjectiveText= objective,
            AccessToken = access_token
        )
        logger.info("Call llm to generate and save math problems ... ")
        results = generate_and_save_math_problems(llm, input_context)
        logger.info("Math problems are saved.")
        return f"{'\n'.join(p['ProblemTitle'] for p in results)} \n The above problems are generated and saved."
    else:
        return "Please provide a valid question and ensure the transcript has been fetched."


def generate_interface():
    # create gradio user interface 
    with gr.Blocks() as interface:
    
        gr.Markdown(
            "<h2 style='text-align: center;'>KidProblem Problem Generator</h2>"
        )
        gr.Markdown(
            f"<h3 style='text-align: center;'>Environment: {'Production' if PRODUCTION else 'Staging'}. Use OpenAI: {USE_OPEN_AI} </h3>"
        )
    
        count = gr.Textbox(label="How many problems do you want to generate?", placeholder="1")
        year = gr.Textbox(label="Which year do you want to use?", placeholder=f"C{datetime.today().strftime('%m%d')}")
    
        objective_input = gr.Textbox(label="Provide your objective", placeholder="Your objective", lines=5)
        result_output = gr.Textbox(label="Result", lines=5)
    
        generate_btn = gr.Button("Generate")
    
        # Set up button actions
        generate_btn.click(generate_problems, inputs=[count, year, objective_input], outputs=result_output)
    
    # Launch the app with specified server name and port
    interface.launch(server_name="127.0.0.1", server_port=7860)

def main():
     # Read sensitive values from environment variables
    USERNAME = os.getenv("COGNITO_USERNAME")
    PASSWORD = os.getenv("COGNITO_PASSWORD")
    OPENAI_API_KEY = os.getenv('OPENAI_API_KEY')
    
    if not all([USERNAME, PASSWORD]):
        raise ValueError("One or more required environment variables are missing.")
    
    global access_token, llm

    if OPENAI_API_KEY and USE_OPEN_AI:
        llm = initialize_openai_llm()
        logger.info("Use OpenAI Model") 
    else:
        llm = initialize_local_llm()
        logger.info("Use Local Model") 
    
    access_token = get_access_token_from_cognito(USERNAME, PASSWORD)

    logger.info(f'The generated math problems will be saved to {"Production" if PRODUCTION else "Staging"} environment.')

    generate_interface();
  

if __name__ == "__main__":       
    main()