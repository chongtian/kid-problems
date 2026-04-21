from pydantic import BaseModel

class InputContext(BaseModel):
    Count: int = 1
    ObjectiveText: str
    ProblemYear: str
    StartNum: int = 1    
    SimpleSchema: bool = False
    Production: bool = False
    AccessToken: str
