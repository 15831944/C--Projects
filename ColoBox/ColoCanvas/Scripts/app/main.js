requirejs.config({
    baseUrl: '/scripts',
    paths: {
        app: 'app',
        lib: 'lib'        
    }
});

define(['jquery', 'app/renderer','app/coloapp'], function ($, renderer) {
                     
    //$('#Refresh').bind('click', function () {
    //    renderer.Refresh();
    //});

    //$('#Start').bind('click', function () {
    //    $('#svgid').empty();
    //        var data = {
    //            ColdAisleWidth: $('#ColdWidth').val(),
    //            SpaceAfterObstacle: $('#SpaceAfterObstacle').val(),
    //            Racks: $('#Racks').val(),
    //            IndexToStop: $('#IndexToStop').val(),
    //            Direction: $('#direction input[type="radio"]:checked').val(),
    //            OriginX: $('#OriginX').val(),
    //            OriginY: $('#OriginY').val(),
    //            //
    //            FileName: $('#drawings input[type="radio"]:checked').val()
    //        };
        
    //        if (data.FileName === 'Ken-Crazy-Colo') {
    //            data.StaticLayers = [{ Name: "walls", IsUniverse: true }, { Name: "pillars", IsUniverse: false }];
    //        } else {
    //            data.StaticLayers = [{ Name: "Layer 4", IsUniverse: true }, { Name: "pillars", IsUniverse: false }];
    //        }

    //    //Regular Ajax
    //        $.ajax({
    //            url: 'Colo/GetResults',
    //            data: JSON.stringify(data),
    //            type: 'POST',
    //            contentType: 'application/json; charset=utf-8',
    //            success: function (context) {
    //                renderer.Render(context);                   
    //            }
    //        });

        

        

    return false;
       
});