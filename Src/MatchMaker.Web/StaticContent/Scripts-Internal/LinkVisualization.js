var LinkVisualization;
(function (LinkVisualization) {
    var formatPercent = d3.format(".1%");
    var scale = d3.scale.linear().domain([
        0, 
        0.5, 
        1
    ]).range([
        "red", 
        "#CCCCCC", 
        "green"
    ]);
    var players;
    var selectionEvent;
    var maxWidth = 480;
    var translateLayer;
    var scaleLayer;
    var actualSVG;

    function getMatrix(playas) {
        var matrix = [];
        playas.forEach(function (player) {
            matrix.push(player.GameMatrix);
        });
        return matrix;
    }
    function CreateChordDiagram(playas, event) {
        players = playas;
        selectionEvent = event;
        var div = document.getElementById("chart");
        var chartWidth = 1000;
        CreateDiagram(chartWidth, chartWidth, chartWidth * 0.41, chartWidth * 0.41 * 1.1);
        resize();
    }
    LinkVisualization.CreateChordDiagram = CreateChordDiagram;
    function resize() {
        var div = document.getElementById("chart");
        var chartWidth = Math.min(div.clientWidth, maxWidth);
        translateLayer.transition().delay(0).duration(0).attr("transform", "translate(" + chartWidth / 2 + "," + chartWidth / 2 + ")");
        scaleLayer.transition().delay(0).duration(0).attr("transform", "scale(" + chartWidth / 1000 + ")");
        actualSVG.transition().delay(0).duration(0).attr("height", chartWidth).attr("width", chartWidth);
    }
    LinkVisualization.resize = resize;
    function CreateDiagram(width, height, innerRadius, outerRadius) {
        var matrix = getMatrix(players);
        var chord = d3.layout.chord().padding(0.05).sortSubgroups(d3.descending).matrix(matrix);
        actualSVG = translateLayer = d3.select("#chart").append("svg").attr("width", width).attr("height", height).attr("name", "linkvisualization");
        translateLayer = actualSVG.append("g").attr("transform", "translate(" + width / 2 + "," + height / 2 + ")");
        scaleLayer = translateLayer.append("g").attr("transform", "scale(1)");
        var svg = scaleLayer;
        var group = svg.selectAll(".group").data(chord.groups).enter().append("g").attr("class", "group").on("mouseover", function (g, i) {
            svg.selectAll("g.chord path").filter(function (d) {
                return d.source.index != i && d.target.index != i;
            }).transition().style("opacity", 0.1);
            selectionEvent.FireEvent(players[i]);
        }).on("mouseout", fade(1));
        group.append("title").text(function (d, i) {
            return players[i].Name;
        });
        var groupPath = group.append("path").attr("id", function (d, i) {
            return "group" + i;
        }).style("fill", "#888888").style("stroke", "#888888").attr("d", d3.svg.arc().innerRadius(innerRadius).outerRadius(outerRadius));
        var groupText = group.append("text").attr("x", 6).attr("dy", 30);
        groupText.append("textPath").attr("xlink:href", function (d, i) {
            return "#group" + i;
        }).text(function (d, i) {
            return players[i].Name;
        }).attr("font-size", "36");
        groupText.filter(function (d, i) {
            return groupPath[0][i].getTotalLength() / 3 - 5 < this.getComputedTextLength();
        }).remove();
        svg.append("g").attr("class", "chord").selectAll("path").data(chord.chords).enter().append("path").style("fill", function (d) {
            return scale(players[d.source.index].Matrix[d.target.index]);
        }).attr("d", d3.svg.chord().radius(innerRadius)).style("opacity", 1).append("title").text(function (d) {
            return players[d.source.index].Name + " and " + players[d.target.index].Name + ": " + formatPercent(players[d.source.index].Matrix[d.target.index]) + "\n" + d.source.value + " Games";
        });
        function fade(opacity) {
            return function (g, i) {
                svg.selectAll("g.chord path").filter(function (d) {
                    return d.source.index != i && d.target.index != i;
                }).transition().style("opacity", opacity);
            }
        }
    }
})(LinkVisualization || (LinkVisualization = {}));

