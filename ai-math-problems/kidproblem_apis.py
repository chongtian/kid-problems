import boto3
import requests
from datetime import datetime
import re
import logging

logger = logging.getLogger(__name__)

API_BASE_URL_STAGING = "https://qajvrb7w7waa5icxlfm6tqustm0wspyy.lambda-url.us-east-2.on.aws/api"
API_BASE_URL_PROD = "https://yxawg4gswpwbcsfjthhubcjnj40wjqyg.lambda-url.us-east-2.on.aws/api"
API_URL_PROBLEM_STAGING = API_BASE_URL_STAGING + "problem"
API_URL_PROBLEM_PROD = API_BASE_URL_PROD + "/problem"
API_URL_EXAM_STAGING = API_BASE_URL_STAGING + "/examdef"
API_URL_EXAM_PROD = API_BASE_URL_PROD + "/examdef"
API_URL_ASN_STAGING = API_BASE_URL_STAGING + "/assignment"
API_URL_ASN_PROD = API_BASE_URL_PROD + "/assignment"
COGNITO_REGION = "us-east-2"
COGNITO_CLIENT_ID = "6bgffqhdrg8a8d6mrqtc2e4plr"


def get_access_token_from_cognito(
        username:str, 
        password:str) -> str:
    """
    Get access token from AWS Cognito. Return the access token if successful, otherwise return an empty string.
    Args:
        cognito_region (str): The AWS region where Cognito is hosted.
        client_id (str): The client ID of the Cognito user pool.
        username (str): The username for authentication.
        password (str): The password for authentication.
        Returns:
            str: The access token if successful, otherwise an empty string.
    """
    
    if not all([username, password]):
        return "";

    client = boto3.client("cognito-idp", region_name=COGNITO_REGION)
    response = client.initiate_auth(
        ClientId=COGNITO_CLIENT_ID,
        AuthFlow="USER_PASSWORD_AUTH",
        AuthParameters={"USERNAME": username, "PASSWORD": password}
        )
    client = None
    
    logger.info("Successful get access token from AWS Cognito.")
    auth_result = response["AuthenticationResult"]
    id_token = auth_result["IdToken"]
    access_token = auth_result["AccessToken"]
    refresh_token = auth_result.get("RefreshToken")

    return access_token


def get_problem(problem_title:str, bearer_token: str, is_production = False) -> dict:
    """
    Get a problem from API by problem_title. Return the retrieved problem.
    
    Args:
        problem_title (str): the problem_title of the problem.
        bearer_token (str): The access token for authentication.
        is_production (bool, optional): Whether to call the production API or staging API. Default is False.
    
    Returns:
        dict: the retrieved problem.
    """
    
    if is_production:
        url = API_URL_PROBLEM_PROD
    else: 
        url = API_URL_PROBLEM_STAGING
        
    headers = {
        "Authorization": f"Bearer {bearer_token}",
        "Content-Type": "application/json"
    }

    logger.info(f"Retrieving Problem {problem_title} ...")
    response = requests.get(f'{url}/{problem_title}', headers=headers)
    if response.status_code == 200:
        logger.info(f"Successful retrieved Problem {problem_title}")    
        return response.json()
    else:
        logger.info(f"Failed to retrieve Problem {problem_title}")
        return {}    


def delete_problem(problem_title: str, bearer_token: str, is_production = False) -> bool:
    """
    Delete a problem. Return True if the problem is deleted successfully, otherwise return False.
    
    Args:
    problem_title: the title of the problem to be deleted
    bearer_token: the access token for authentication
    is_production: whether to call the production API or staging API. Default is False, which means calling staging API.
    
     Returns:
     True if the problem is deleted successfully, otherwise False.
     """
    
    if is_production:
        url = API_URL_PROBLEM_PROD
    else: 
        url = API_URL_PROBLEM_STAGING
        
    headers = {
        "Authorization": f"Bearer {bearer_token}",
        "Content-Type": "application/json"
    }
    
    response = requests.delete(url+'/'+problem_title, headers=headers)
    logger.debug(f"response status code {response.status_code}. {response.content}")
    if response.status_code == 200:
        logger.info(f"Problem {problem_title} deleted successfully.")
        return True
    else:
        logger.warning(f"Failed to delete Problem {problem_title}. It may not exist. Status code: {response.status_code}")
        return False


def save_problem(problem: dict, bearer_token: str, is_production = False) -> dict:
    """
    Insert a problem. Return the result of the insertion.
    
    Args:
        problem (dict): The problem dictionary to be inserted.
        bearer_token (str): The access token for authentication.
        is_production (bool, optional): Whether to call the production API or staging API. Default is False.
    
    Returns:
        dict: The result of the insertion.
    """
    
    if is_production:
        url = API_URL_PROBLEM_PROD
    else: 
        url = API_URL_PROBLEM_STAGING
        
    headers = {
        "Authorization": f"Bearer {bearer_token}",
        "Content-Type": "application/json"
    }  

    logger.info(f"Deleting Problem {problem['ProblemTitle']} if it already exists...")
    response = requests.delete(url+'/' + problem["ProblemTitle"], headers=headers)

    logger.info(f"Inserting Problem {problem['ProblemTitle']}...")
    problem['Action'] = 1
    response = requests.post(url, json=problem, headers=headers)
    
    logger.debug(f"response status code {response.status_code}. {response.content}")
    if response.status_code == 200:
        logger.info(f"Problem {problem['ProblemTitle']} inserted successfully.")   
        return response.json()
    
    return {}


def save_problems(payload:list, bearer_token: str, is_production = False) -> list:
    """
    Insert problems. Return a list of results for the inserted problems.
    
    Args:
        payload (list): A list of problem dictionaries to be inserted.
        bearer_token (str): The access token for authentication.
        is_production (bool, optional): Whether to call the production API or staging API. Default is False.
    
    Returns:
        list: A list of results for the inserted problems.
    """
    
    if is_production:
        url = API_URL_PROBLEM_PROD
    else: 
        url = API_URL_PROBLEM_STAGING
        
    headers = {
        "Authorization": f"Bearer {bearer_token}",
        "Content-Type": "application/json"
    }
    
    if not isinstance(payload, list):
        raise ValueError("payload is not a list.")

    results = []    
    for p in payload:
        logger.info(f"Deleting Problem {p['ProblemTitle']} if it already exists...")
        response = requests.delete(url+'/' + p["ProblemTitle"], headers=headers)

        logger.info(f"Inserting Problem {p['ProblemTitle']}...")
        p['Action'] = 1
        response = requests.post(url, json=p, headers=headers)
        
        logger.debug(f"response status code {response.status_code}. {response.content}")
        if response.status_code == 200:
            logger.info(f"Problem {p['ProblemTitle']} inserted successfully.")   
            results.append(response.json())
    
    return results


def prepare_problems(raw_json: list, start_num = 1, problem_year = '') -> list:
    """
    Clean up the raw problems generated by LLM, and convert them to the format required by the API. Return a list of cleaned problems.
    Args:
        raw_json (list): A list of raw problem dictionaries.
        start_num (int): The starting number for problem IDs.
        problem_year (str): The year for the problems.

    Returns:
        list: A list of cleaned problem dictionaries.
    """

    if not isinstance(raw_json, list):
        raise ValueError("raw_json is not a list.")
    
    if not problem_year:
        today = datetime.today()
        problem_year = f"C{today.strftime('%m%d')}"

    problems = []
    for raw in raw_json:
        if "ProblemText" not in raw:
            logger.debug("Wrong input ... ")
            logger.debug(raw)
            continue

        problem = {}
        problem["ProblemCategory"] = "HOME"
        problem["ProblemYear"] = problem_year
        problem["ProblemNumber"] = str(start_num).rjust(3, "0")
        problem["ProblemTitle"] = f"{problem["ProblemCategory"]}-{problem["ProblemYear"]}-{problem["ProblemNumber"]}"
        problem["ProblemText"] = process_problem_text(raw["ProblemText"], problem["ProblemTitle"])
        problem["AnswerOptions"] = raw["AnswerOptions"]
        problem["ProblemAnswer"] = raw.get("ProblemAnswer", '')
        problem["ProblemTags"] = [raw.get("ProblemTags", 'CBE6')]
        problem["SolutionText"] = ""
        problem["IsStaging"] = True
        problem["Action"] = 1
        start_num += 1
        problems.append(problem)
    
    return problems


def process_problem_text(raw_text:str, problem_title:str) -> str :
    cleaned = raw_text
    cleaned = cleaned.replace('\\n', '</br>\n')
    cleaned = cleaned.replace('PlaceHolder', problem_title)
    cleaned = cleaned.replace(r'\\\\', r'\\').replace(r'\\\"', r'\"') 
    cleaned = re.sub(r'\\\\(?![\s])', r'\\', cleaned)
    return cleaned


def create_exam_definition(examTitle: str, problems: list, bearer_token: str, is_production = False) -> dict:
    """
    Insert an exam definition. Return the result of the insertion.
    
    Args:
        examTitle (str): the exam title.
        problem (dict): The problem dictionary to be inserted.
        bearer_token (str): The access token for authentication.
        is_production (bool, optional): Whether to call the production API or staging API. Default is False.
    
    Returns:
        dict: The result of the insertion.
    """
    
    if is_production:
        url = API_URL_EXAM_PROD
    else: 
        url = API_URL_EXAM_STAGING
        
    headers = {
        "Authorization": f"Bearer {bearer_token}",
        "Content-Type": "application/json"
    }

    payload = {
        "ExamCategory": problems[0]["ProblemCategory"],
        "ExamTitle": examTitle,
        "ExamType": "H",
        "Active": True,
        "Memo": "",
        "ExamYear": problems[0]["ProblemYear"],
        "ExamDetails": []
    }

    for problem in problems:
        payload["ExamDetails"].append({"ProblemTitle": problem["ProblemTitle"]})

    logger.info(f"Inserting ExamDefinition {payload['ExamTitle']} ...")
    response = requests.post(url, json=payload, headers=headers)
    
    logger.debug(f"response status code {response.status_code}. {response.content}")
    if response.status_code == 200:
        logger.info(f"ExamDefinition {payload['ExamTitle']} inserted successfully.")   
        return response.json()
    
    return {}


def create_assignment(examCategory:str, examTitle: str, bearer_token: str, is_production = False) -> dict:
    """
    Insert an exam definition. Return the result of the insertion.
    
    Args:
        exmCategory (str): the exam category
        examTitle (str): the exam title.
        bearer_token (str): The access token for authentication.
        is_production (bool, optional): Whether to call the production API or staging API. Default is False.
    
    Returns:
        dict: The result of the insertion.
    """
    
    if is_production:
        url = API_URL_ASN_PROD
    else: 
        url = API_URL_ASN_STAGING
        
    headers = {
        "Authorization": f"Bearer {bearer_token}",
        "Content-Type": "application/json"
    }

    payload = {
        "ExamCategory": examCategory,
        "ExamTitle": examTitle,
        "Active": True,
    }

    logger.info(f"Inserting Assignment {payload['ExamTitle']} ...")
    response = requests.post(url, json=payload, headers=headers)
    
    logger.debug(f"response status code {response.status_code}. {response.content}")
    if response.status_code == 200:
        logger.info(f"ExamDefinition {payload['ExamTitle']} inserted successfully.")   
        return response.json()
    
    return {}