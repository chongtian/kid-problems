# AI-Generated Math Problems Project
This project demonstrates how large language models (LLMs) can be used to generate, manage, and deploy math problems for educational use. It is designed to use with KidProblem by creating aligned practice problems that can be reviewed in a staging environment before being published for use.

# Scripts Overview
## main.py
This script starts a gradio interface, where user can enter any objective to generate math problems. The generated problems will be saved to KidProblem.

## cbe6.py
This script focuses on the Fort Bend ISD CBE Math test, which includes nine distinct learning objectives. Users can select a specific objective and use an AI LLM to generate aligned math problems. The generated problems are saved to KidProblem and made available in the Staging environment, where parents can review them.

## copy_problems.py
This script copies math problems from the KidProblem Staging environment to the Production environment. It allows parents to first validate and refine AI-generated problems before making them available for regular use.

## delete_problems.py
This script permanently deletes math problems from KidProblem, helping maintain content quality and manage outdated or incorrect problems.