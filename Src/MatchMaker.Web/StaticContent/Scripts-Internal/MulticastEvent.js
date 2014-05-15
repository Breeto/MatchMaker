var MulticastEvent = (function () {
    function MulticastEvent() {
        this.Handlers = new Array();
        this.HandlerObjects = new Array();
    }
    MulticastEvent.prototype.FireEvent = function (parameter) {
        this.Handlers.forEach(function (funct) {
            funct(parameter);
        });
    };
    MulticastEvent.prototype.AddHandler = function (handler, object) {
        this.Handlers.push(handler);
        this.HandlerObjects.push(object);
    };
    MulticastEvent.prototype.RemoveHandler = function (object) {
        var position = $.inArray(object, this.HandlerObjects);
        if(~position) {
            this.HandlerObjects.splice(position, 1);
            this.Handlers.splice(position, 1);
        }
    };
    return MulticastEvent;
})();
