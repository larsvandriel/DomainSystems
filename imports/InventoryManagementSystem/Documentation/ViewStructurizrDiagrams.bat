docker run -it --rm -p 5092:8080 -v "%~dp0\Structurizr":/usr/local/structurizr -e STRUCTURIZR_WORKSPACE_FILENAME=ProjectManagementTool structurizr/lite
@pause