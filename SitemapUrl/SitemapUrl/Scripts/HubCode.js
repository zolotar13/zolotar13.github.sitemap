$(function () {


    var myHub = $.connection.myHub;
    myHub.client.addNewItem = function (name, message) {
        $('#response').append('<p><b>' + htmlEncode(name)
            + '</b>: ' + htmlEncode(message) + '</p>');
    };
    myHub.client.createDiagram = function (arr, total) {
        var max = 400;
        var fastTop = arr[0] * max / total;
        var midTop = arr[1] * max / total;
        var slowTop = arr[2] * max / total;
        $("#fast").animate({ height: fastTop }, 200);

        $("#mid").animate({ height: midTop }, 200);

        $("#slow").animate({ height: slowTop }, 200);

    }
    myHub.client.createGraphic = function (us, ts) {
        $("#table")[0].innerHTML = "";
        for (var i = 0; i < us.length; i++) {
            $("#table").append("<tr><td>" + us[i] + "</td><td>" + ts[i] + "</td></tr>");
        }
        $("table  tr").last().css({ backgroundColor: "forestgreen", color: "white" });
        $("table  tr").first().css({ backgroundColor: "darkred", color: "white" });
    }

    $.connection.hub.start().done(function () {

        $('#sendUrl').click(function () {
            myHub.server.getSitemap($('#url').val());

        });
        $('#loadHistory').click(function () {

            myHub.server.loadHistory($("select option:selected").val());

        });
        $('#removeHistory').click(function () {
            myHub.server.removeHistory();
            location.reload();
        });

    });
});
