var MediaQueries;
(function (MediaQueries) {
    MediaQueries.Queries;
    MediaQueries.Sizes = [
        300, 
        500, 
        700, 
        1000
    ];
    function GetMaxQuery(size) {
        return "(max-width:" + size + "px)";
    }
    function GetMinQuery(size) {
        return "(min-width:" + size + "px)";
    }
    function CreateQueries() {
        MediaQueries.Queries = [];
        for(var x = 0; x < MediaQueries.Sizes.length * 2; x++) {
            var current;
            current = x < MediaQueries.Sizes.length / 2 ? GetMaxQuery(MediaQueries.Sizes[x]) : GetMinQuery(MediaQueries.Sizes[x - MediaQueries.Sizes.length]);
            MediaQueries.Queries.push(window.matchMedia("screen and " + current));
        }
    }
    function GetQueries() {
        if(MediaQueries.Queries == undefined) {
            CreateQueries();
        }
        return MediaQueries.Queries;
    }
    MediaQueries.GetQueries = GetQueries;
})(MediaQueries || (MediaQueries = {}));

