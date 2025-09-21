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
