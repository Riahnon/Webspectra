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
        },
        parametersubscriptions : []
    }

    function getSupportedModesView (aSupportedModes)
    {
        var lSupportedModes = [];
        for (var i = 0; i < aSupportedModes.length; ++i) {
            lSupportedModes.push(getModeView(aSupportedModes[i]));
        }
        return lSupportedModes;
    }
    function getModeView(aMode)
    {
        var lMode = {
            name: aMode.name,
            parameters: ko.observableArray(getModeParametersView(aMode.parameters))
        };
        return lMode;
    }

    function getModeParametersView(aParameters)
    {
        var lParameters = [];
        for(var i=0; i<aParameters.length; ++i)
        {
            lParameters.push(getModeParameterView(aParameters[i]));
        }
        return lParameters;
    }

    function getModeParameterView(aParameter)
    {
        var lParam = {
            name : aParameter.name,
            type : aParameter.type,
            value: ko.observable(aParameter.value)
        };

        if(aParameter.validvalues)
            lParam.validvalues = aParameter.validvalues;

        if(aParameter.validnamedvalues)
            lParam.validnamedvalues = aParameter.validnamedvalues;

        if(aParameter.min)
            lParam.min = aParameter.min;

        if(aParameter.max)
            lParam.max = aParameter.max;

        return lParam;
    }

    var lHub = $.connection.webSpecrtaHub;
    var lCanvas = document.getElementById('canvasFFT');
    var lCtx2D = lCanvas.getContext('2d');
    var lTextOutput = document.getElementById('textareaOutput');
    var lIgnoreChange = false;
    var lGetSupportedModesDeferred = $.Deferred();
    var lGetCurrentModeNameDeferred = $.Deferred();
    lHub.client.updateText = updateText;
    lHub.client.updateFFT = updateFFT;
    lHub.client.updateConfidence = updateConfidence;
    lHub.client.setCurrentMode = setCurrentMode;
    lHub.client.setSupportedModes = setSupportedModes;
    lViewModel.supportedModeParams = {};
    $.connection.hub.start().done(function () {
        
        lHub.server.getSupportedModes();
        lGetSupportedModesDeferred.promise().done(function () {
            //by default the first mode will be considered to be set
            if (lViewModel.supportedModes().length)
            {
                var lMode = lViewModel.supportedModes()[0];
                lViewModel.currentMode(lMode);
                subscribeParameterValueChange(lMode);
            }
            
            var lMainContainer = document.getElementById("div-maincontainer");
            
            lHub.server.getCurrentModeName();
            lGetCurrentModeNameDeferred.promise().done(function () {
                ko.applyBindings(lViewModel, lMainContainer);
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
        subscribeParameterValueChange(aNewMode);
    }

    function setCurrentMode(aModeName)
    {
        
        var lCurrentMode = lViewModel.currentMode();
        if (lCurrentMode && lCurrentMode.name == aModeName)
        {
            lGetCurrentModeNameDeferred.resolve();
            return;
        }

        for (var i = 0; i < lViewModel.supportedModes().length; ++i) {
            var lMode = lViewModel.supportedModes()[i];
            if (lMode.name == aModeName) {
                lViewModel.currentMode(lMode);
                subscribeParameterValueChange(lMode);
                break;
            }
        }
        lGetCurrentModeNameDeferred.resolve();
    }

    function subscribeParameterValueChange(aModeView)
    {
        for (var i = 0; i < lViewModel.parametersubscriptions.length; ++i)
            lViewModel.parametersubscriptions[i].dispose();

        lViewModel.parametersubscriptions = [];
        var lModeParametersView = aModeView.parameters();
        for (var i = 0; i < lModeParametersView.length; ++i)
        {
            var lParam = lModeParametersView[i];
            lViewModel.parametersubscriptions.push(lParam.value.subscribe(onParameterValueChanged, lParam));
        }
    }

    function onParameterValueChanged(aNewValue)
    {
        if(lViewModel.currentMode())
        {
            console.log("Parameter " + this.name + " in mode " + lViewModel.currentMode().name + " changed to " + aNewValue);
            //lHub.server.SetParameterValue(lViewModel.currentMode().Name)
        }
    }

    function setSupportedModes(aSupportedModes) {
        lViewModel.supportedModes(getSupportedModesView(aSupportedModes));
        lGetSupportedModesDeferred.resolve();
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