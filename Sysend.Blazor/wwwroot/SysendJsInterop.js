// This is a JavaScript module that is loaded on demand. It can export any number of
var sysendservice = {};

export async function InitSysend(service) {
    sysendservice = service;
    console.log('initialized systendinterop!');
    loadJS("./_content/Sysend.Blazor/sysend.js/sysend.js", OnSysendLoaded, document.body);
}

var loadJS = function (url, implementationCode, location) {
    var scriptTag = document.createElement('script');
    scriptTag.src = url;

    scriptTag.onload = implementationCode;
    scriptTag.onreadystatechange = implementationCode;

    location.appendChild(scriptTag);
};

var OnSysendLoaded = async function () {
    console.log('loaded sysend!');
    console.dir(sysend);
    window.sysend = sysend;
    sysendservice.invokeMethodAsync('InteropOnSysendLoaded', sysend.id);

    sysend.track('open', function (data) {
        console.log('sysend opened!');
        console.dir(data);
    });
}

DotNet.attachReviver(function (key, value) {
    if (value &&
        typeof value === 'object' &&
        value.hasOwnProperty("__isCallBackWrapper")) {

        var netObjectRef = value.callbackRef;

        return function () {
            netObjectRef.invokeMethodAsync('Invoke', ...arguments);
        };
    } else {
        return value;
    }
});

