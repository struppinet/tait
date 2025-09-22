// always use a const for the webhook URL
const WEBHOOK_URL = 'http://192.168.100.129:5000/youtrack/webhook';

const entities = require('@jetbrains/youtrack-scripting-api/entities');
const http = require('@jetbrains/youtrack-scripting-api/http');

exports.rule = entities.Issue.onChange({
    title: 'Send all events to Webhook',
    guard: (ctx) => {
        return ctx.issue.becomesReported || ctx.issue.becomesResolved || ctx.issue.becomesUnresolved || ctx.issue.isChanged;
    },
    action: (ctx) => {
        if (ctx.issue.id == 'Issue.Draft') {
            return;
        }
        
        const issue = ctx.issue;

        const issueLink = '<' + issue.url + '|' + issue.id + '>';
        let eventType;
        let isNew;
        let message;

        if (issue.becomesReported) {
            eventType = 'Created';
            isNew = true;
        } else if (issue.becomesResolved) {
            eventType = 'Resolved';
            isNew = false;
        } else if (issue.becomesUnresolved) {
            eventType = 'Reopened';
            isNew = false;
        } else {
            eventType = 'Updated';
            isNew = false;
        }
        message += eventType + ": " + issue.summary;

        let changedByTitle = '';
        let changedByName = '';

        if (isNew) {
            changedByTitle = 'Created By';
            changedByName = issue.reporter.fullName;
        } else {
            changedByTitle = 'Updated By';
            changedByName = issue.updatedBy.fullName;
        }

        const payload = {
            'issueId': issue.id,
            'eventType': eventType,
            'fields': [
                {
                    'title': 'Summary',
                    'value': issue.summary,
                    'oldValue': issue.summary?.oldValue,
                    'changed': issue.summary?.isChanged,
                },
                {
                    'title': 'Description',
                    'value': issue.description || '',
                    'oldValue': issue.description?.oldValue,
                    'changed': issue.description?.isChanged,
                },
                {
                    'title': 'State',
                    'value': issue.fields.State.name,
                    'oldValue': issue.fields.State?.oldValue,
                    'changed': issue.fields.State?.isChanged,
                },
                {
                    'title': 'Priority',
                    'value': issue.fields.Priority.name,
                    'oldValue': issue.fields.Priority?.oldValue,
                    'changed': issue.fields.Priority?.isChanged,
                },
                {
                    'title': 'Assignee',
                    'value': issue.fields.Assignee ? issue.fields.Assignee.fullName : '',
                    'oldValue': issue.fields.Assignee?.oldValue?.fullName,
                    'changed': issue.fields.Assignee?.isChanged,
                },
                {
                    'title': changedByTitle,
                    'value': changedByName,
                }
            ],
            'attachments': [{
                'fallback': eventType + ' (' + issueLink + ')',
                'pretext': eventType + ' (' + issueLink + ')',
                'color': issue.fields.Priority.backgroundColor || '#edb431',
            }]
        };

        const connection = new http.Connection(WEBHOOK_URL, null, 2000);
        const response = connection.postSync('', null, JSON.stringify(payload));
        if (!response.isSuccess) {
            console.warn('Failed to post notification to Slack. Details: ' + response.toString());
        }
    },
    requirements: {
        Priority: {
            type: entities.EnumField.fieldType
        },
        State: {
            type: entities.State.fieldType
        },
        Assignee: {
            type: entities.User.fieldType
        }
    }
});