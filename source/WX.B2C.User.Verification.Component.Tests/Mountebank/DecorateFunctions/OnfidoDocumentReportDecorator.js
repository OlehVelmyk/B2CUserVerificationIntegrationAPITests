(config) => {
    // https://www.w3schools.com/js/js_random.asp
    var Random = () => Math.floor(Math.random() * 10);

    var idDocNumber = "";
    for (var i = 0; i < 9; i++)
        idDocNumber += Random();

    config.response.body = config.response.body.replace(/{documentNumber}/g, idDocNumber);
}