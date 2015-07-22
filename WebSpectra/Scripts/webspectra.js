var Canvas;
var Ctx2D;
var TextOutput;
var ConfidenceLabel;
var SSEStream;
function onMessage(e) { }

function onFFTData(aData) {
    if (aData.length) {
        drawFFT(aData);
    }
}

function onTextData(aData) {
    if (aData)
        appendText(aData);
}

function onConfidenceChanged(aData) {
    if (aData)
        setConfidence(aData);
}

function onModeChanged(aData) {
    var options = {
        type: "get",
        url: aData.modeurl
    };
    $.ajax(options).done(function (data, textStatus, jqXHR) {
        var $target = $("#ddlModes");
        $target.val(data);
    });
}

function onParamsChanged(aData) {
    var options = {
        type: "get",
        url: aData.paramsurl
    };
    $.ajax(options).done(function (data, textStatus, jqXHR) {
        var $target = $("#table-modeParameters");
        var $lNewHTML = $(data);
        $target.replaceWith($lNewHTML);
        $lNewHTML.effect("highlight");
        var $textOutput = $("#textarea-decoderOutput");
        $textOutput.val("");
        resetChangeEvents($lNewHTML.html);
    });
}


function onParamValueChanged(aData) {
    var options = {
        type: "get",
        url: aData.paramurl,
        data: { paramname: aData.paramname }
    };
    $.ajax(options).done(function (data, textStatus, jqXHR) {
        var $target = $("#" + aData.paramname);
        var $lNewHTML = $(data);
        $target.replaceWith($lNewHTML);
        $lNewHTML.effect("highlight");
        resetChangeEvents($lNewHTML.html);
    });
}

function onError(e) {
    if (e.readyState != EventSource.CLOSED) {
        this.close();  //here, `this` refers to `SSEStream`
        alert("Error");
    }
    else {
        alert("Event source closed");
    }
}

function resetChangeEvents(context) {
    $("input[data-modeparminput='true'][type='text']", context).change(setParameterValue);
    $("input[data-modeparminput='true'][type='checkbox']", context).change(setFlagParameterValue);
    $("select[data-modeparamdropdown='true']", context).change(setParameterValue);
}

function drawFFT(fftValues) {

    var lHeight = Canvas.height;
    var lWidth = Canvas.width;
    Ctx2D.clearRect(0, 0, lWidth, lHeight);
    Ctx2D.beginPath();
    Ctx2D.moveTo(0, lHeight);
    var lXStep = lWidth / fftValues.length;
    for (var i = 0; i < fftValues.length; i++) {
        var lY = (1 - fftValues[i]) * lHeight;
        Ctx2D.lineTo(i * lXStep, lY);
    }
    Ctx2D.stroke();
}

function appendText(aText) {
    TextOutput.value += aText;
}

function setConfidence(aConfidence) {
    ConfidenceLabel.innerHTML = "Confidence: " + aConfidence + "%";
}

function resizeCanvas() {

}

function getFlagsValue(aName)
{
    var lValue = 0;
    var lCheckBoxes = $("input:checked[type='checkbox'][name='" + aName + "']").each(function () {
        var $input = $(this);
        lValue = lValue | $input.val();
    });
    return lValue;
};

var setFlagParameterValue = function () {
    var $input = $(this);
    var options = {
        url: $input.attr("data-action"),
        type: $input.attr("data-method"),
        cache: "false",
        data: { param: $input.attr("data-param"), value: getFlagsValue($input.attr("name")) }
    };
    $.ajax(options).done(function (data) {
        /*//This write action may return a response if the sent parameter value is not valid. This response will replace the paramter value in the client and set it to the current one
        var $target = $("#" + $input.attr("name"));
        if (data) {
            var $lNewHTML = $(data);
            $target.replaceWith($lNewHTML);
            $lNewHTML.effect("highlight");
            resetChangeEvents($lNewHTML.html);
        }*/
    });
};

var setParameterValue = function () {
    var $input = $(this);
    var options = {
        url: $input.attr("data-action"),
        type: $input.attr("data-method"),
        cache: "false",
        data: { param: $input.attr("data-param"), value: $input.val() }
    };
    $.ajax(options).done(function (data) {
        //This write action may return a response if the sent parameter value is not valid. This response will replace the paramter value in the client and set it to the current one
        /*var $target = $("#" + $input.attr("id"));
        if (data) {
            var $lNewHTML = $(data);
            $target.replaceWith($lNewHTML);
            $lNewHTML.effect("highlight");
            resetChangeEvents($lNewHTML.html);
        }*/
    });
};

var setMode = function () {
    var $select = $(this);
    var $lSelectedMode = $select.val();
    cache: "false"
    var options = {
        url: $select.attr("data-action"),
        type: $select.attr("data-method"),
        data: { mode: $lSelectedMode }
    };
    $.ajax(options).done( /*function (data, textStatus, jqXHR) {

    }*/);

    return false;
};

function startSSEConnection() {
    SSEStream = new EventSource("/api/EventStream");
    SSEStream.onmessage = onMessage;
    SSEStream.onerror = onError;
    SSEStream.addEventListener('fft', onFFTData);
    SSEStream.addEventListener('text', onTextData);
    SSEStream.addEventListener('confidence', onConfidenceChanged);
    SSEStream.addEventListener('modechanged', onModeChanged);
    SSEStream.addEventListener('paramschanged', onParamsChanged);
    SSEStream.addEventListener('paramvaluechanged', onParamValueChanged);
}

$(document).ready(function () {
    
    Canvas = document.getElementById('canvas-fft');
    Ctx2D = Canvas.getContext('2d');
    TextOutput = document.getElementById('textarea-decoderOutput');
    ConfidenceLabel = document.getElementById('labelConfidence');

    var decoderHub = $.connection.decoderHub;

    /*startSSEConnection();*/
    window.addEventListener('resize', resizeCanvas, false);
    resetChangeEvents();
    $("select[id='ddlModes']").change(setMode);
    resizeCanvas();
    
    decoderHub.client.onServerEvent = function (aEventName, aEventData) {
        switch (aEventName) {
            case "fft": onFFTData(aEventData);
                break;
            case "text": onTextData(aEventData);
                break;
            case "confidence": onConfidenceChanged(aEventData);
                break;
            case "modechanged": onModeChanged(aEventData);
                break;
            case "paramschanged": onParamsChanged(aEventData);
                break;
            case "paramvaluechanged": onParamValueChanged(aEventData);
                break;
        }
    }
    $.connection.hub.start();
});
