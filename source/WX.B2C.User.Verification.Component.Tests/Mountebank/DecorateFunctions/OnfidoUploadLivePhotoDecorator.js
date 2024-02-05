(config) => {
    var UUIDv4 = function b(a) { return a ? (a ^ Math.random() * 16 >> a / 4).toString(16) : ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, b); }

    var photoId = UUIDv4().toString();
    config.response.body = config.response.body.replace(/{photoId}/g, photoId);
}