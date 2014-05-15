var Sparkline = (function () {
    function Sparkline(data, Parent) {
        this.maxWidth = 300;
        this.parent = Parent;
        var chartWidth = Math.min(this.parent.clientWidth, this.maxWidth);
        this.CreateDiagram(this.ParseData(data));
        this.resize();
    }
    Sparkline.prototype.ParseData = function (data) {
        var numbers = new Array();
        data = data.replace("<!--", "");
        data = data.replace("-->", "");
        var stringNumbers = data.split(",");
        stringNumbers.forEach(function (str) {
            numbers.push(parseFloat(str));
        });
        return numbers;
    };
    Sparkline.prototype.CreateDiagram = function (data) {
        var xscale = d3.scale.linear().domain([
            0, 
            data.length
        ]).range([
            0, 
            1000
        ]);
        var yscale = d3.scale.linear().domain([
            0, 
            10
        ]).range([
            80, 
            0
        ]);
        var parentSelection = (d3).select(this.parent);
        this.svg = parentSelection.append("svg").attr("width", 1000).attr("height", 80);
        this.scaleLayer = this.svg.append("g").attr("transform", "scale(1)");
        this.scaleLayer.selectAll("path").data(data).enter().append("path").attr("stroke", "blue").attr("stroke-width", "3px").attr("d", function (d, i) {
            if(i == 0) {
                return "M" + xscale(0) + "," + yscale(d) + " L " + xscale(0) + "," + yscale(d);
            }
            return "M" + xscale(i - 1) + "," + yscale(data[i - 1]) + " L " + xscale(i) + "," + yscale(d);
        });
        this.scaleLayer.append("circle").attr("fill", "orange").attr("cx", xscale(data.length - 1) - 5).attr("cy", yscale(data[data.length - 1])).attr("r", 5);
        this.resize();
    };
    Sparkline.prototype.resize = function () {
        var chartWidth = Math.min(this.parent.clientWidth, this.maxWidth);
        this.scaleLayer.transition().delay(0).duration(0).attr("transform", "scale(" + chartWidth / 1000 + ")");
        this.svg.transition().delay(0).duration(0).attr("height", chartWidth / 12.5).attr("width", chartWidth);
    };
    return Sparkline;
})();
