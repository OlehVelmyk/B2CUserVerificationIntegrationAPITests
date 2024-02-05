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

    var Random = () => {
        var isNumber = Math.floor(Math.random() * 2);
        if (isNumber)
            return Math.floor(Math.random() * 10);
        else
            return String.fromCharCode(Math.floor(Math.random() * 6) + 97);
    } 

    const CheckIdTemplate = "{checkIdTemplate}";

    var uniquePart = "";
    for (var i = 0; i < 6; i++)
        uniquePart += Random();    

    var checkId = CheckIdTemplate.replace(/{uniquePart}/g, uniquePart);
    config.response.body = config.response.body.replace(/{checkId}/g, checkId);

    setTimeout(() => SendWebhook(checkId), 1000);
}