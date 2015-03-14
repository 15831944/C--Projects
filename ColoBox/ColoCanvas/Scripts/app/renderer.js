define(['jquery', 'app/manualSelection'], function ($, manualSelection) {
    $ = jQuery;
    var render,
        renderBox,
        drawUniverse,
        drawObstacles,
        currentIndex,
        shadowIndex,
        //handler,        
        refresh,
        drawBox,
        drawShadow,
        model,
        maxIndex = -1,        
        transientLine,
        svg,
        evt = {},
        selectedPoints = [],
        drawNumbers,
        forceStop = false;

    render = function (context) {
        model = context;
        refresh();
        svg = d3.select("svg");
        manualSelection.init(svg,model);             
    }    

    refresh = function (drawNumbers) {
        forceStop = false;
        maxIndex = -1;
        $('#svgid').empty();        
        drawUniverse();
        drawFloors();
        drawObstacles(drawNumbers);//context.obstaclePointArr);

        currentIndex = shadowIndex = 0;
        //handler = setInterval(drawBox, parseInt($('#Delay').val()));
        setTimeout(drawShadow, parseInt($('#Delay').val()));
    }

    drawUniverse = function () {
        var points = model.universePoints,
            x = d3.select("svg").append("polyline")
         .attr("fill", "#005500")
         .attr("points", points)
         .attr("style", "fill:white;stroke:black;stroke-width:5;z-index:10");
       
                
        
    };

    drawFloors = function () {
        if (!model.floor)
            return;

        for (var i = 0; i < model.floor.length; ++i) {
            d3.select("svg").append("polyline")
         //.attr("fill", "#005500")
         .attr("points", model.floor[i])
         .attr("style", "fill:white;stroke:#cccccc;stroke-width:1;z-index:11");
        }
    }

    drawObstacles = function () {
        var pointsArr = model.obstaclePointArr;
        for (var i = 0; i < pointsArr.length - 1; ++i) {
            var pl = d3.select("svg").append("polyline")
             .attr("points", pointsArr[i])             
             .attr("style", "fill:yellow;stroke:black;stroke-width:2;z-index:10");
            manualSelection.registerEvents(pl);             
        }
    };    

    drawBox = function (drawNumbers) {//points, boxIndex, isValid) {
        var points = model.boxArr[currentIndex].points,
            boxIndex = currentIndex++,            
            style ="fill:white;stroke:green;stroke-width:2;z-index:50",
            $boxElement,
            boxId = 'box' + boxIndex;

        if (currentIndex > model.boxArr.length - 1) {
            //clearInterval(handler);
            currentIndex = 0;
            return;
        }

        var text = getTextObject(points);
        renderBox(boxId,points,style,text);        
    };

    getTextObject = function(points){
        var text = { };
        var pArr = points.split(' ');
        var minX, minY;
        var pointsArr = points.split(' ');
        for(var i = 0;i< pointsArr.length; ++i){
            var spArr = pointsArr[i].split(',');
            if (spArr[0] !== '') {
                if (!minX || parseFloat(spArr[0]) < minX) {
                    minX = spArr[0];
                }
            }
            if (spArr[1] !== '') {
                if (!minY || parseFloat(spArr[1]) > minY) {
                    minY = spArr[1];
                }
            }
        }
        
        text.x = minX;
        text.y = minY;
        return text;
    }
    drawShadow = function () {
        if (!model.boxArr)
            return;

        var points = model.boxArr[currentIndex].shadowArr[shadowIndex],
            style = "fill:white;stroke:brown;stroke-width:1;z-index:50",
            boxId = 'box' + currentIndex;
        shadowIndex++;
        renderBox(boxId, points, style);
        if (shadowIndex > model.boxArr[currentIndex].shadowArr.length - 1) {
            //when done showing the ShadowBoxes, show the real deal
            drawBox();
            shadowIndex = 0;            
        }

        if (maxIndex > currentIndex) {
            forceStop = true;
            
        }
        
        if (currentIndex > maxIndex) {
            maxIndex = currentIndex;
        }
        if (currentIndex < parseInt($('#Racks').val()) && !forceStop) {// model.boxArr.length - 1) {
            setTimeout(drawShadow, parseInt($('#Delay').val()));
        }
        
    }

    renderBox = function (boxId,points,style,text) {
        $boxElement = $('#' + boxId);
        if ($boxElement.size() > 0) {
            $boxElement.attr('points', points)
                       .attr('style', style);

            addText(text);
        } else {
            d3.select("svg").append("polyline")
             .attr("id", boxId)
             .attr("points", points)
             .attr("style", style);
            addText(text);
        }

        function addText(text) {
            if (!drawNumbers)
                return;

            if (text) {
                d3.select("svg").append("text")
                   .attr("x", text.x)
                   .attr("y", text.y)
                   .text(boxId.replace('box',''));
            }
        }
    }
    setDrawNumbers = function (ldrawNumbers) {
        drawNumbers = ldrawNumbers;
    }
    
    return {
        Render: render,
        Refresh: refresh,
        setDrawNumbers: setDrawNumbers
    };
});