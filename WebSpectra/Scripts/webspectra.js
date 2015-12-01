/// <reference path="jquery.signalR-2.2.0.js" />

(function () {
    var lViewModel = 
    {
        confidence: ko.observable(100),
        currentMode: ko.observable(),
        supportedModes: ko.observableArray([]),
        getparametertype: function(aParameter)
        {
            if (aParameter.type == "namedselect")
                return "nameddropdown";
            else if (aParameter.type == "select")
                return "dropdown";

            return "raw";
        }
    }
    var lHub = $.connection.webSpecrtaHub;
    var lCanvas = document.getElementById('canvasFFT');
    var lCtx2D = lCanvas.getContext('2d');
    var lTextOutput = document.getElementById('textareaOutput');
    var lIgnoreChange = false;
    lHub.client.updateText = updateText;
    lHub.client.updateFFT = updateFFT;
    lHub.client.updateConfidence = updateConfidence;
    lHub.client.setCurrentMode = setCurrentMode;
    lHub.client.setSupportedModes = setSupportedModes;
    lViewModel.supportedModeParams = {};
    $.connection.hub.start().done(function () {
        lHub.server.getSupportedModes().done(function () {
            var lMainContainer = document.getElementById("div-maincontainer");
            ko.applyBindings(lViewModel, lMainContainer);
            lHub.server.getCurrentModeName().done(function () {
                lViewModel.currentMode.subscribe(onCurrentModeChanged);
            });
        });
    });

    function updateText(aText)
    {
        lTextOutput.value += aText;
    }

    function updateFFT(aData)
    {
        drawFFT(aData);
    }

    function updateConfidence (aConfidence )
    {
        lViewModel.confidence(aConfidence);
    }

    function onCurrentModeChanged(aNewMode) {
        lHub.server.setCurrentMode(aNewMode.name);
        //if(lViewModel.currentMode())
        //{
        //    var ldivParams = $("#divParameters");
        //    var lHtmlStr = "";
        //    for (var i = 0; i < aNewMode.parameters.length; ++i)
        //    {
        //        lHtmlStr += aNewMode.parameters[i].name + "</br>";
        //    }
        //    ldivParams.html(lHtmlStr);
        //}
    }

    function setCurrentMode(aModeName)
    {
        var lCurrentMode = lViewModel.currentMode();
        if (lCurrentMode && lCurrentMode.name == aModeName)
            return;

        for (var i = 0; i < lViewModel.supportedModes().length; ++i) {
            var lMode = lViewModel.supportedModes()[i];
            if (lMode.name == aModeName) {
                lViewModel.currentMode(lMode);
                break;
            }
        }
    }

    function setSupportedModes(aSupportedModes) {
        lViewModel.supportedModes(aSupportedModes);
    }

    function drawFFT(fftValues) {

        var lHeight = lCanvas.height;
        var lWidth = lCanvas.width;
        lCtx2D.clearRect(0, 0, lWidth, lHeight);
        lCtx2D.beginPath();
        lCtx2D.moveTo(0, lHeight);
        var lXStep = lWidth / fftValues.length;
        for (var i = 0; i < fftValues.length; i++) {
            var lY = (1 - fftValues[i]) * lHeight;
            lCtx2D.lineTo(i * lXStep, lY);
        }
        lCtx2D.stroke();
    }

}());