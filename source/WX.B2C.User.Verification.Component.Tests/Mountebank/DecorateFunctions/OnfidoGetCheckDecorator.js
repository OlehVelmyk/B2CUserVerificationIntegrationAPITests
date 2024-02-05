(config) => {
    var parts = config.request.path.split('/');
    var checkId = parts[parts.length - 1];
    config.response.body = config.response.body.replace(/{checkId}/g, checkId);
}