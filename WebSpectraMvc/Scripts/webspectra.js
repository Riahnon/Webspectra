var Canvas;
var Ctx2D;
var TextOutput;
var ConfidenceLabel;
var SSEStream;
function onMessage(e) { }

function onWBFFTData(e) {
    var lData = JSON.parse(e.data);
    if (lData.length) {
        DrawWBFFT(lData);
    }
}
function onFFTData(e) {
    var lData = JSON.parse(e.data);
    if (lData.length) {
        drawFFT(lData);
    }
}

function onTextData(e) {
    var lData = JSON.parse(e.data);
    if (lData)
        appendText(lData);
}

function onConfidenceChanged(e) {
    var lData = JSON.parse(e.data);
    if (lData)
        setConfidence(lData);
}

function onModeChanged(e) {
    var lData = JSON.parse(e.data);
    var options = {
        type: "get",
        url: lData.modeurl
    };
    $.ajax(options).done(function (data, textStatus, jqXHR) {
        var $target = $("#ddlModes");
        $target.val(data);
    });
}

function onParamsChanged(e) {
    var lData = JSON.parse(e.data);
    var options = {
        type: "get",
        url: lData.paramsurl
    };
    $.ajax(options).done(function (data, textStatus, jqXHR) {
        var $target = $("#tableModeParameters");
        var $lNewHTML = $(data);
        $target.replaceWith($lNewHTML);
        $lNewHTML.effect("highlight");
        var $textOutput = $("#textAreaDecoderOutput");
        $textOutput.val("");
        resetChangeEvents($lNewHTML.html);
    });
}


function onParamValueChanged(e) {
   
    var lData = JSON.parse(e.data);
    var options = {
        type: "get",
        url: lData.paramurl,
        data: { paramname: lData.paramname }
    };
    $.ajax(options).done(function (data, textStatus, jqXHR) {
        var $target = $("#" + lData.paramname);
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
        //This write action may return a response if the sent parameter value is not valid. This response will replace the paramter value in the client and set it to the current one
        var $target = $("#" + $input.attr("name"));
        if (data) {
            var $lNewHTML = $(data);
            $target.replaceWith($lNewHTML);
            $lNewHTML.effect("highlight");
            resetChangeEvents($lNewHTML.html);
        }
    });
};

var setParameterValue = function () {
    var $input = $(this);
    var options = {
        url: $input.attr("data-action"),
        type: $input.attr("data-method"),
        cache: "false",
        data: { param: $input.attr("data-param"), value: $input.attr("value") }
    };
    $.ajax(options).done(function (data) {
        //This write action may return a response if the sent parameter value is not valid. This response will replace the paramter value in the client and set it to the current one
        var $target = $("#" + $input.attr("id"));
        if (data) {
            var $lNewHTML = $(data);
            $target.replaceWith($lNewHTML);
            $lNewHTML.effect("highlight");
            resetChangeEvents($lNewHTML.html);
        }
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
    $.ajax(options).done(function (data, textStatus, jqXHR) {

    });

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

$(function () {
    
    Canvas = document.getElementById('fftCanvas');
    Ctx2D = Canvas.getContext('2d');
    TextOutput = document.getElementById('textAreaDecoderOutput');
    ConfidenceLabel = document.getElementById('labelConfidence');
    SSEStream;

    startSSEConnection();
    window.addEventListener('resize', resizeCanvas, false);
    resetChangeEvents();
    $("select[id='ddlModes']").change(setMode);
    resizeCanvas();
});
