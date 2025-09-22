# tait
Ticket AI Team

## Concept

LLMs output quality depends on context, therefor the project aims to give the LLMs the best possible context for the work tasks

## Roles

The system expects to be handled as a Software Development Team, therefor the typical roles should be defined

### Product Owner

The role of the user. Define expected Features/Functionality

### Team Members

- Project Manager: The first LLM step to plan the steps/tasks
- Software Architect: Based on defined policies find the best solution
- Frontend Software Engineer: Writes the code/tests
- Backend Software Engineer: Writes the code/tests
- DevOps Engineer: Deploys the output
- QA Engineer: Checks the output

## Infrastructure

- Ticket system for convenient and business typical handling of tasks
- LM Studio as AI API provider since you can easily handle and switch models
- Middleware that handles ticket updates and queries AI API
- GIT repository for code

tbd:
- Docker/Podman for containers
- Jenkins for build pipeline
- UI/UX tests?


## Steps
- [x] Youtrack Webhook
- [x] Pipeline Service
- [x] MCP: Youtrack (Project Manger)
- [ ] MCP: GIT (Coder)?
- [ ] MCP: Docker (DevOps)
- [ ] MCP: Browser (QA)
- [ ] Knowledge Base Roles

## First test run

```
Create a new website with a 'hello world' text in red.
```

- Create a new ticket in the product owner project
- The project manager will parse the response and create tickets according to the requirements
- New ticket in software architect project: define the structure for the project (hello world from backend, shown in frontend)
- New ticket in backend developer project: create a hello world backend webserver
- New ticket in frontend developer project: create a css style for the fronted and show the hello world from the backend
- New ticket in DevOps project: deploy the frontend and backend to the server
- New ticket in QA project: test the frontend and backend if there is a website like it's described in the requirements
