import json
from langchain_core.prompts import PromptTemplate  
from langchain_ollama import OllamaLLM
from langchain_openai import ChatOpenAI
from langchain_core.output_parsers import JsonOutputParser, StrOutputParser
from langchain_core.runnables import RunnableParallel, RunnableLambda
import re
import config
import logging
import datetime
from pathlib import Path

from kidproblem_apis import prepare_problems, save_problems
from input_context import InputContext

logger = logging.getLogger(__name__)

def initialize_local_llm():
    """
    Initialize and return a llm model
    """
    return OllamaLLM(
        model=config.OLLAMA_MODEL_ID
        )

def initialize_openai_llm():
    """
    Initialize and return a OpenAI llm model
    """
    return ChatOpenAI(model=config.OPENAI_MODEL_ID, temperature=config.OPENAI_TEMPERATURE)


def create_math_prompt_template_1():
    """
    Create a PromptTemplate for generating math problems based on user example.

    Args:
        None

    Returns:
        PromptTemplate: A PromptTemplate object configured for generating math problems.
    """
       
    math_template = """
    You are a Senior Math Teacher specializing in elementary and middle school education. 
    You excel at creating engaging, grade-appropriate practice problems that reinforce core mathematical concepts.
    Your task is to generate exactly {count} math practice problems based on the objective: 

    {objective}

    ### Guidelines:
    - Only one math problem can be a simple expression or 1-sentence problem. The remaining math problems shall be word problems with 3 to 5 sentences. 
    - The answer to the math problem is multi-choice, with 4 options: A, B, C, D. Only one of them is correct. 
    - Each generated math problem shall have 3 properties: ProblemText, ProblemAnswer, AnswerOption
    - ProblemText: the text of the problem including answer choices
    - ProblemText: use standard LaTeX for all mathematical expressions, equations, and variables (e.g., $x + 5 = 12$). LaTex should be wrapped in $...$ for inline math. For line break, use "<br/>". Do not use "item" or "itemize" in Latex code. 
    - ProblemText: if the problem requires chart, draw the chart with asymptote code without any comment and line break. Then add the chart to the problem text in an img tag: <img src="PlaceHolder_<sequence>.png" alt="[asy] ... asymptote code ... [/asy]" />
    - ProblemText: do not use "table", "grid", "graphpaper", "crimson" in asymptote code. These are not supported by the asymptote compiler. You shall only write asymptote code which can compiles everywhere.
    - ProblemText: put each answer choice and the text of answer choice in a new line with a line break "<br/>. You must not use Latex code "item" or "itemize". 
    - ProblemAnswer: the Answer of the problem shall be a single selection from multi-choice options
    - AnswerOption: this is like "A,B,C,D". The actual text of answer choices shall be put in the ProblemText.
    - Output all problems as an json array using this json schema

      {schema}  

    - You must check if the latex code in ProblemText needing proper escaping to ensure the Json is valid
    """
    
    prompt_template = PromptTemplate.from_template(math_template)
    return prompt_template


def create_math_prompt_template_2():
    """
    Create a PromptTemplate for generating math problems based on user example.

    Args:
        None

    Returns:
        PromptTemplate: A PromptTemplate object configured for generating math problems.
    """
       
    math_template = """
    You are a Senior Math Teacher specializing in elementary and middle school education. 
    You excel at creating engaging, grade-appropriate practice problems that reinforce core mathematical concepts.
    Your task is to generate exactly {count} math practice problems based on the objective: 

    {objective}

    When generating math problems, follow all rules strictly:
    1. Problem Structure
    - Each problem must include exactly these fields:
       * "ProblemText": Full problem including answer choices
       * "ProblemAnswer": The correct option (A, B, C, or D)      
       * "AnswerOptions": always "A,B,C,D"   
    
    2. Problem Content
    - Most problems must be word problems of over 4 sentences, excluding the sentences of the Answer Choices.
    - At most one problem may be a short (1 sentence) or simple expression-based problem.
    - Ensure problems are clear, realistic, and mathematically sound.

    3. Answer Choices
    - Each problem must have exactly 4 options labeled A, B, C, D.
    - Only one option is correct.
    - All answer choices must be included inside "ProblemText" (not separately).
    - Format answer choices like:
        <br/>A. ...
        <br/>B. ...
        <br/>C. ...
        <br/>D. ...
        
    4. LaTeX Formatting
    - Use standard LaTeX for all math expressions.
    - Inline math must be wrapped in $...$ (e.g., $x + 5 = 12$).
    - Do NOT use LaTeX environments such as item, itemize, or similar.
    - Ensure all LaTeX is valid and properly escaped for JSON.
    - If the $ is used for US Dollar, escape it as \$.
    
    5. Line Breaks
    - Use "<br/>" for all line breaks inside "ProblemText" except for LaTex and Asymptote code.
    
    6. Charts / Diagrams (if needed)
    - If a chart or diagram is required:
      * Generate valid Asymptote code only (no comments, no extra text, no line break).
      * The code must compile in standard environments.
      * Embed using:
             <img src="PlaceHolder_<sequence>.png" alt="[asy] ...code... [/asy]" />
    
    7. Correctness & Validation
    Ensure:
    - The correct answer matches "ProblemAnswer".
    - All distractors are plausible but incorrect.
    - JSON output is valid and properly escaped.
    
    8. Output Format
    Output all problems as an json array using this json schema:
       {schema} 

    """
    
    prompt_template = PromptTemplate.from_template(math_template)
    return prompt_template


def create_prompt_template_to_clean_json():
    template = """
    You are a senior software engineer. The below json contains errors due to un-escape characters, etc. Fix the json text so that it can be successfully parsed by a json deserailizer. 
    You must only output json. Do not output any other comments.

    {text}
    """
    
    prompt_template = PromptTemplate.from_template(template)
    return prompt_template


def get_json_schema(simple_schema = False) -> str:
    if simple_schema:
        return """
            [
                {
                "ProblemText": "the text of the problem including answer choices, with proper escaping for latex code and line breaks",
                "ProblemAnswer": "the correct answer, which is a single selection from multi-choice options",
                "AnswerOptions": "the multi-choice options, like A,B,C,D"
                }
            ]
        """
    else:
        with open(config.SCHEMA_FILE, "r") as f:
            s = f.read()
        return s


def save_text_and_pass(x:str):
    """
    Save the text to a log file and pass the text to the next step.
    Args:        x: the text to be saved and passed
    Returns:        the same text x, which is passed to the next step"""
    if config.ALWAYS_LOG_RESPONSE_FROM_LLM:
        log_dir = Path(config.LOGS_FOLDER)
        log_dir.mkdir(exist_ok=True)
        timestamp = datetime.datetime.now().strftime("%Y-%m-%d_%H-%M-%S")
        log_filename = f"res-{timestamp}.log"
        log_file_path = log_dir / log_filename
        log_file_path.write_text(x, encoding="utf-8")
        return x  # pass-through to next parser/step
    else:
        return x


def clean_up_json(x:str):
    """
    Clean up the text to make it a valid json string. This is a workaround for the issue that the generated json string may contain unescaped backslashes in the LaTeX code, which makes the json invalid.
    Args:        x: the text to be cleaned up
    Returns:        the cleaned up text, which is a valid json string
    """
    try:
        json.loads(x, strict=False)
        return x
    except ValueError:
        # fixed = re.sub(r'\\(?!["\\/bdfnrtu])', r'\\\\', x)
        problems = []
        regex = re.compile(r'"ProblemText":\s+?"(.*)",', flags=re.UNICODE | re.MULTILINE)
        matches = regex.finditer(x)
        for match in matches:
            problem = {'ProblemText':match.group(1)}
            problems.append(problem)
        
        regex = re.compile(r'"ProblemAnswer":\s+?"(.*)",', flags=re.UNICODE | re.MULTILINE)
        matches = regex.finditer(x)
        for idx, match in enumerate(matches):
            problem = problems[idx]
            problem['ProblemAnswer'] = match.group(1)
        
        regex = re.compile(r'"AnswerOptions":\s+?"(.*)"', flags=re.UNICODE | re.MULTILINE)
        matches = regex.finditer(x)
        for idx, match in enumerate(matches):
            problem = problems[idx]
            problem['AnswerOptions'] = match.group(1)

        fixed = json.dumps(problems)       
        return fixed

def is_json_valid(j:str)->bool:
    try:
        json.loads(j, strict=False)
        return True
    except json.JSONDecodeError as e:
        logger.error("json file has errors: %s", e)
        return False 

def generate_math_problems(llm, objectives: list, simple_schema = False):
    """
    Generate math problems based on the objective and count specified in the input.
    Args:
        llm: the language model to be used for generating math problems
        objectives: a list of learning objectives.
        Returns:
        a list of generated math problems, each problem has the schema:
        {
            "ProblemText": "the text of the problem including answer choices, with proper escaping for latex code and line breaks",
            "ProblemAnswer": "the correct answer, which is a single selection from multi-choice options",
            "AnswerOptions": "the multi-choice options, like A,B,C,D"   
        }
    """

    prompt = create_math_prompt_template_2()
    json_parser = JsonOutputParser()
    text_parser = StrOutputParser()
    save_response = RunnableLambda(lambda x: save_text_and_pass(x))
    clean_up_response = RunnableLambda(lambda x: clean_up_json(x))
    input = RunnableParallel(
        count=RunnableLambda(lambda x: x["count"]),
        objective=RunnableLambda(lambda x: x["text"]),
        schema=RunnableLambda(lambda _: get_json_schema(simple_schema))
    )

    chain = (
        input 
        | prompt 
        | llm 
        | text_parser
        | save_response 
        | clean_up_response
        | json_parser 
        )

    problems = []
    for idx, objective in enumerate(objectives):    
        inputs = {"count": objective.get("count", 1), "text": objective.get("text", None) }
        logger.info(f"Processing #{idx + 1} objective which is Objective {objective.get("objective", "Unknown")} ... ")
        logger.debug(inputs)
        response = chain.invoke(inputs)
        logger.info(f"#{idx + 1} objective has been processed. ")
        problems.extend(response)
    
    return problems


def generate_and_save_math_problems(llm, input_context: InputContext):
    # validate and clean up input_context   
    if not input_context.ObjectiveText:
        raise ValueError("ObjectiveText is not found in input_context.")
    if not input_context.AccessToken:
        raise ValueError("AccessToken is not found in input_context.")    
    
    prompt = create_math_prompt_template_2()
    json_parser = JsonOutputParser()
    text_parser = StrOutputParser()
    save_response = RunnableLambda(lambda x: save_text_and_pass(x))
    clean_up_response = RunnableLambda(lambda x: clean_up_json(x))
    prepare = RunnableLambda(lambda x: prepare_problems(x, input_context.StartNum, input_context.ProblemYear))
    save = RunnableLambda(lambda x: save_problems(x, input_context.AccessToken, input_context.Production))
    
    input = RunnableParallel(
        count=RunnableLambda(lambda x: x["count"]),
        objective=RunnableLambda(lambda x: x["text"]),
        schema=RunnableLambda(lambda _: get_json_schema(input_context.SimpleSchema))
    )

    chain = (
        input 
        | prompt 
        | llm 
        | text_parser
        | save_response 
        | clean_up_response
        | json_parser 
        | prepare
        | save
        )

    inputs = {"count": input_context.Count, "text": input_context.ObjectiveText }
    response = chain.invoke(inputs)
    return response


def generate_math_problems_json(llm, json_file:str)->list:
    """
    Generate math problems based on the objective and count specified in the input.
    Args:
        llm: the language model to be used for generating math problems
        json_file: path to the json output file.
        Returns:
        True if the generated json is valid; False when the generated json contains errors and need human review.
    """

    text_parser = StrOutputParser()
    save_response = RunnableLambda(lambda x: save_text_and_pass(x))
    prompt_clean_json = create_prompt_template_to_clean_json()
    input = RunnableParallel(
        text=RunnableLambda(lambda x: x["text"])
    )

    chain = (
        llm 
        | text_parser
        | save_response 
        )
    
    clean_json_chain = (
        input
        | prompt_clean_json
        | llm
        | text_parser
        | save_response         
    )

    folder = Path(config.PROMPTS_FOLDER)
    prmopt_files = [f for f in folder.glob(config.PROMPTS_FILE) if f.is_file()]

    pattern1 = r"```json\s*(.*?)\s*```"
    pattern2 = r"^\s*\[\s*(.*?)\s*\]\s*$"

    with open(json_file, "w", encoding="utf-8") as res:
        res.write("[\n")

        for i, prmopt_file in enumerate(prmopt_files):
            logger.info(f"{prmopt_file}: processing prompt ... ")
            is_last = i == len(prmopt_files) - 1
            prompt = prmopt_file.read_text(encoding="utf-8")
            result = chain.invoke(prompt)
            raw_text = str(result)

            # extract json text from the raw response
            match1 = re.search(pattern1, raw_text, re.MULTILINE | re.DOTALL)
            if match1:
                logger.info(f"{prmopt_file}: extracted json from response. ")
                extracted = match1.group(1)
            else:
                logger.info(f"{prmopt_file}: use raw response as the extracted json. ")
                extracted = raw_text

            # clean up json to ensure it is valid
            is_valid = is_json_valid(extracted)
            if is_valid:
                logger.info(f"{prmopt_file}: json from response is valid. ")
                clean_json_text = extracted
            else:
                logger.info(f"{prmopt_file}: cleaning up json.")
                inputs = {"text": extracted }
                clean_json = clean_json_chain.invoke(inputs)
                clean_json_text = str(clean_json)
                if is_json_valid(clean_json_text):
                    logger.info(f"{prmopt_file}: json is cleaned and valid.")
                else:
                    logger.info(f"{prmopt_file}: json from response contains errors like un-escaped characters.")

            match2 = re.search(pattern2, clean_json_text, re.MULTILINE | re.DOTALL)
            if match2:
                logger.info(f"{prmopt_file}: extracted elements from json array. ")
                clean_json_text = match2.group(1)
                res.write(clean_json_text)
            else:
                logger.info(f"{prmopt_file}: save response as is. ")
                res.write(clean_json_text) 
            
            if is_last:
                res.write("\n")
            else:
                res.write(",\n")

            logger.info(f"{prmopt_file}: prompt has been processed. ")
            # dev
            # break   

        res.write("\n]")

    # validate the final result
    try:
        with open(json_file, 'r', encoding='utf-8') as f:
            problems = json.load(f, strict=False)   
        return problems
    except json.JSONDecodeError as e:
        logger.error("json file has errors: %s", e)
        return []
    except OSError as e:
        logger.error("File error: %s", e)
        return []

