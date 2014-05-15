var StreakVisualization;
(function (StreakVisualization) {
    var maxWidth = 480;
    var barpadding = 1;

    var players;
    var selectionEvent;
    var height;
    var width;
    var center;
    var streaksvg;
    var lastPlayer;

    var scaleLayer;
    function CreateStreakDiagram(playas, event) {
        players = playas;
        selectionEvent = event;
        var currentsize = 1000;
        var div = document.getElementById("streak");
        var chartWidth = Math.min(div.clientWidth, maxWidth);
        CreateDiagram(1000, 250);
        resize();
    }
    StreakVisualization.CreateStreakDiagram = CreateStreakDiagram;
    function resize() {
        var div = document.getElementById("streak");
        var chartWidth = Math.min(div.clientWidth, maxWidth);
        scaleLayer.transition().delay(0).duration(0).attr("transform", "scale(" + chartWidth / 1000 + ")");
        streaksvg.transition().delay(0).duration(0).attr("height", chartWidth / 4).attr("width", chartWidth);
    }
    StreakVisualization.resize = resize;
    function CreateDiagram(diagramWidth, diagramHeight) {
        height = diagramHeight;
        width = diagramWidth;
        center = height / 2;
        var _20Bars = (d3).range(20);
        var colorScale = d3.scale.linear().domain([
            d3.min(_20Bars), 
            0, 
            d3.max(_20Bars)
        ]).range([
            "red", 
            "#CCCCCC", 
            "green"
        ]);
        var xScale = d3.scale.linear().domain([
            0, 
            _20Bars.length
        ]).range([
            0, 
            width
        ]);
        streaksvg = d3.select("#streak").append("svg").attr("width", width).attr("height", height).attr("name", "streakvisualization");
        scaleLayer = streaksvg.append("g").attr("transform", "translate(0,0)");
        scaleLayer.selectAll("rect").data(_20Bars, function (d, i) {
            return i;
        }).enter().append("rect").attr("x", 0).attr("y", 0).attr("width", 0).attr("height", 0).attr("fill", "black");
        scaleLayer.append("line").attr("x1", 0).attr("y1", center).attr("x2", width).attr("y2", center).attr("stroke", "black");
        selectionEvent.AddHandler(changeStreak, this);
    }
    function changeStreak(person) {
        if(person == undefined || person.RecentWinsLosses == undefined) {
            return;
        }
        lastPlayer = person;
        document.getElementById("winLossTitle").innerHTML = person.Name + "'s Win/Loss Streak";
        var colorScale = d3.scale.linear().domain([
            d3.min(person.RecentWinsLosses), 
            0, 
            d3.max(person.RecentWinsLosses)
        ]).range([
            "red", 
            "#CCCCCC", 
            "green"
        ]);
        var xScale = d3.scale.linear().domain([
            0, 
            person.RecentWinsLosses.length
        ]).range([
            0, 
            width
        ]);
        var badRectangles = streaksvg.selectAll("rect").data(person.RecentWinsLosses, function (d, i) {
            return i;
        }).exit();
        badRectangles.transition().attr("width", 0).attr("height", 0);
        scaleLayer.selectAll("rect").data(person.RecentWinsLosses, function (d, i) {
            return i;
        }).transition().attr("x", function (d, i) {
            return xScale(i);
        }).attr("height", height / 2).attr("width", width / person.RecentWinsLosses.length - barpadding).attr("y", function (d) {
            return d < 0 ? center : 0;
        }).attr("fill", function (d) {
            return colorScale(d);
        });
    }
})(StreakVisualization || (StreakVisualization = {}));

