// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

var dotnetsrv = {};
var tracks = {};
var sysendint = {};


export async function GetSysend() {
    return sysendint;
}


export function showPrompt(message) {
  return prompt(message, 'Type anything here');
}


export async function openService(devicehandler) {
    dotnetsrv = devicehandler;
    loadJS('./_content/Sysend.Blazor/sysend.js/sysend.js', OnSysendLoaded, document.body);
}

var loadJS = function (url, implementationCode, location) {
    //url is URL of external file, implementationCode is the code
    //to be called from the file, location is the location to 
    //insert the <script> element

    var scriptTag = document.createElement('script');
    scriptTag.src = url;

    scriptTag.onload = implementationCode;
    scriptTag.onreadystatechange = implementationCode;

    location.appendChild(scriptTag);
};

var OnSysendLoaded = async function () {
    console.log('loaded sysend!');
    console.dir(sysend);
    sysendint = sysend;
    await dotnetsrv.invokeMethodAsync('HandleOnInputReport');
    
}
