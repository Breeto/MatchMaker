var MapWinPercentageVisualization;
(function (MapWinPercentageVisualization) {
    var players;
    var selectionEvent;
    var maxRadius = 240;
    var groups;
    var tooltips;
    var losses;
    var wins;
    var lastPlayer;
    var svg;
    var scaleLayer;
    var translateLayer;

    var radius;
    var winScale = d3.scale.linear().domain([
        0, 
        1
    ]).range([
        "#CCCCCC", 
        "green"
    ]);
    var lossScale = d3.scale.linear().domain([
        0, 
        1
    ]).range([
        "#CCCCCC", 
        "red"
    ]);
    function resize() {
        var div = document.getElementById("mapPercentage");
        var chartWidth = Math.min(div.clientWidth / 2, maxRadius);
        translateLayer.transition().delay(0).duration(0).attr("transform", "translate(" + chartWidth + "," + chartWidth + ")");
        scaleLayer.transition().delay(0).duration(0).attr("transform", "scale(" + chartWidth / 1000 + ")");
        svg.transition().delay(0).duration(0).attr("height", chartWidth * 2).attr("width", chartWidth * 2);
    }
    MapWinPercentageVisualization.resize = resize;
    function GetTimesPlayed(mapInformation) {
        var timesPlayed = [];
        for(var a = 0; a < mapInformation.length; a++) {
            timesPlayed.push(mapInformation[a].TimesPlayed);
        }
        return timesPlayed;
    }
    function GetMaps(mapInformation) {
        var maps = [];
        for(var a = 0; a < mapInformation.length; a++) {
            maps.push(mapInformation[a].Map);
        }
        return maps;
    }
    function GetPercentages(mapInformation) {
        var actualPercentages = [];
        for(var a = 0; a < mapInformation.length; a++) {
            actualPercentages.push(mapInformation[a].Percentage);
        }
        return actualPercentages;
    }
    function CreateMapWinPercentageVisualization(playas, event) {
        players = playas;
        selectionEvent = event;
        CreateDiagram(1000);
        resize();
    }
    MapWinPercentageVisualization.CreateMapWinPercentageVisualization = CreateMapWinPercentageVisualization;
    function CreateDiagram(myRadius) {
        groups = undefined;
        radius = myRadius;
        var mapDictionary = new Array();
        var maps = GetMaps(mapDictionary);
        var actualPercentages = GetPercentages(mapDictionary);
        svg = d3.select("#mapPercentage").append("svg").attr("width", radius * 2).attr("height", radius * 2).attr("name", "mapwinpercentagevisualization");
        translateLayer = svg.append('g').attr('transform', 'translate(' + radius + ',' + radius + ')');
        scaleLayer = translateLayer.append('g').attr('transform', 'translate(' + radius + ',' + radius + ')');
        selectionEvent.AddHandler(updateChart, this);
    }
    function updateChart(person) {
        if(person == undefined || person.MapWinPercentages == undefined) {
            return;
        }
        lastPlayer = person;
        document.getElementById("mapTitle").innerHTML = person.Name + "'s Win/Loss Data by Map";
        if(groups == undefined) {
            var maps = GetMaps(person.MapWinPercentages);
            var actualPercentages = GetPercentages(person.MapWinPercentages);
            groups = scaleLayer.selectAll(".group").data((d3.layout).pie()(GetTimesPlayed(person.MapWinPercentages))).enter().append("g");
            tooltips = groups.append("title").text(function (d, i) {
                return maps[i].Name + " " + (actualPercentages[i] * 100) + "% of " + person.MapWinPercentages[i].TimesPlayed + " games.";
            });
            losses = groups.append("path").attr("class", "lossSlice").attr("d", function (d, i) {
                return (d3.svg).arc().innerRadius(0).outerRadius(radius)(d);
            }).attr("fill", function (d, i) {
                return lossScale(Math.abs(1 - actualPercentages[i]));
            });
            wins = groups.append("path").attr("class", "winSlice").attr("d", function (d, i) {
                return (d3.svg).arc().innerRadius(0).outerRadius(radius * Math.sqrt(actualPercentages[i]))(d);
            }).attr("fill", function (d, i) {
                return winScale(actualPercentages[i]);
            });
            return;
        }
        var mymaps = GetMaps(person.MapWinPercentages);
        var myactualPercentages = GetPercentages(person.MapWinPercentages);
        var pieParts = (d3.layout).pie()(GetTimesPlayed(person.MapWinPercentages));
        losses.transition().attr("d", function (d, i) {
            return (d3.svg).arc().innerRadius(0).outerRadius(radius)(pieParts[i]);
        }).attr("fill", function (d, i) {
            return lossScale(Math.abs(1 - myactualPercentages[i]));
        });
        ; ;
        wins.transition().attr("d", function (d, i) {
            return (d3.svg).arc().innerRadius(0).outerRadius(radius * Math.sqrt(myactualPercentages[i]))(pieParts[i]);
        }).attr("fill", function (d, i) {
            return winScale(myactualPercentages[i]);
        });
        ; ;
        tooltips.transition().text(function (d, i) {
            return mymaps[i].Name + " " + (myactualPercentages[i] * 100) + "% of " + person.MapWinPercentages[i].TimesPlayed + " games.";
        });
    }
})(MapWinPercentageVisualization || (MapWinPercentageVisualization = {}));

