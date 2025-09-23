const WEBHOOK_URL = 'http://192.168.100.128:8000/youtrack/webhook';

const entities = require('@jetbrains/youtrack-scripting-api/entities');
const http = require('@jetbrains/youtrack-scripting-api/http');

exports.rule = entities.Issue.onChange({
  title: 'Notify TAIT about changes',
  action: (ctx) => {
    if (ctx.issue.id == 'Issue.Draft') {
      return;
    }

    const issue = ctx.issue;

    const payload = {
      'issueId': issue.id,
      'state': issue.fields.State.name,
      'oldState': issue.fields.oldValue(ctx.State)
    };

    const connection = new http.Connection(WEBHOOK_URL, null, 2000);
    connection.addHeader({name: 'Content-Type', value: 'application/json'});
    const response = connection.postSync('', [], JSON.stringify(payload));
    if (!response.isSuccess) {
      console.warn('Failed to post notification to TAIT. Details: ' + response.toString());
    }
  },
  requirements: {
    State: {
      type: entities.State.fieldType,
      name: 'State'
    }
  }
});
