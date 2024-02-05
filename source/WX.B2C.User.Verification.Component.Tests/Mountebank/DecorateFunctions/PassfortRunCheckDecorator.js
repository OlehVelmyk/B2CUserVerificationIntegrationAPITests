(config) => {
    var SendWebhook = (checkId, timeout) => {
        var url = '{webhookUrl}';
        var body = JSON.stringify({webhookBody}).replace(/{checkId}/g, checkId);

        var http = require('http');
        var requestOptions = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        };

        var request = http
            .request(url, requestOptions)
            .on('error', (e) => {
                console.log("ERROR: " + JSON.stringify(e));
            });;

        request.write(body);
        request.end();
    }

    // https://gist.github.com/jed/982883
    var UUIDv4 = function b(a) { return a ? (a ^ Math.random() * 16 >> a / 4).toString(16) : ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, b); }

    var checkId = UUIDv4().toString();
    config.response.body = config.response.body.replace(/{checkId}/g, checkId);

    setTimeout(() => SendWebhook(checkId), 1000);
}