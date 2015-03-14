define(function () {
    var selectedNodes = [],        
        selectX = 0,
        selectY = 0,
        model,
        svg;
    function init(arg,mdl) {
        svg = arg;
        model = mdl;
        $('body').on("click", function (arg) {
            selectX = arg.offsetX;
            selectY = arg.offsetY;
        })
        .on("contextmenu", function () { return false;});
    }

    function registerEvents(ent) {
        ent.on("click", function () {
            var points = this.attributes['points'].nodeValue;                 
            drawSelectedPoint(points);                 
            return;
        });
    }

    function reset() {
        selectedNodes = [];
        selectX = 0;
        selectY = 0;
    }

    function rightClick(evt) {
        var lastPoint = selectedNodes[selectedNodes.length - 1];
        var thisPoints = '';

        if (this.localName === 'polyline') {
            thisPoints = this.attributes['endpoint'].value;
        } else {
            thisPoints = this.attributes["cx"].value + ',' + this.attributes["cy"].value
        }

        if ( thisPoints === lastPoint.render) {
            var x = d3.select("circle[points='" + lastPoint.render + "']");
            x.remove();
            var y = d3.select("polyline[endpoint='" + lastPoint.render + "']");
            y.remove();

            selectedNodes.pop();
        }

        return false;
    }
    
    drawSelectedPoint = function (points) {
        var pointsArr = points.split(' ');
        var minDist = 1000000000;
        var pointToUse;
        for (var i = 0; i < pointsArr.length; ++i) {
            var p = pointsArr[i];
            var dist = getDistance(p);
            if (dist < minDist) {
                pointToUse = p;
                minDist = dist;
            }
        }
        drawPoint(pointToUse);
        addSelectedPoint(pointToUse);
        //var point1 = pointsArr[0];
        //var point2 = pointsArr[1];
        //var dist1 = getDistance(point1);
        //var dist2 = getDistance(point2);

        //if (dist1 < dist2) {
        //    drawPoint(point1);
        //    addSelectedPoint(point1);
        //} else {
        //    drawPoint(point2);
        //    addSelectedPoint(point2);
        //}

        function drawPoint(p) {
            var sp = d3.select("circle[points='" + p + "']");
            if (sp[0][0] === null) {
                var pArr = p.split(',');                
                var gPoint = svg.append('circle')
                 .attr("cx", pArr[0])
                 .attr("cy", pArr[1])
                 .attr("r", 4)
                 .attr("fill", "green")
                 .attr("style", "z-index:100")
                 .attr("points", pArr)
                .on("contextmenu", rightClick)
                .on("click",pointClick);

                selectedNodes.push({render: p});                
            }
        }
    };

    pointClick = function (evt) {
        if (this.attributes["points"].value === selectedNodes[0].render) {
            var conf = confirm("You finish selected the universe. Do you want to proceed?");
            if (conf == true) {
                selectedNodes.push({ render: selectedNodes[0].render });
                addSelectedPoint(selectedNodes[0].render);
                //var selectedNodesArray = []
                //for (var i = 0; i < selectedNodes.length; ++i) {                    
                //        var realPoint = selectedNodes[i].real.split(',');
                //        selectedNodesArray.push({ x: realPoint[0], y: realPoint[1] });                    
                //}
                $('body').trigger('PolygonClosed', { selectedNodes: selectedNodes });
            }
        }
        
    }
    
    addSelectedPoint = function (p) {
        //drawing a line
        if (selectedNodes.length > 1) {
            var prevPoint = selectedNodes[selectedNodes.length - 2];
            var points = prevPoint.render + ' ' + p;
            var line = svg.append("polyline")             
             .attr("points", points)
             .attr("endpoint", p)
             .attr("style", "fill:white;stroke:red;stroke-width:5;z-index:10")
             .on('contextmenu',rightClick);

        }
    }
    getDistance = function (point) {
        var pArr = point.split(',');
        var pX = pArr[0];
        var pY = pArr[1];

        return Math.pow(selectX - pX, 2) + Math.pow(selectY - pY, 2);
    };
    return {
        init: init,
        registerEvents: registerEvents,
        reset: reset
    };
});