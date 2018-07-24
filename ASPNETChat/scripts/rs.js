//
//  rs.js - Remote Scripting JavaScript Include File
//  
//  Rewritten from jsrsClient.js, taken from 
//    http://www.ashleyit.com/rs/jsrs/test.htm (version 2.3)
//    Copyright (C) 2001 by Brent Ashley
//  Rewrite Copyright (C) 2004 by Alvaro Mendez
//
//  Include this file inside your page like this:
//  <script language='JavaScript' src='/scripts/rs.js'></script>
//

// This is the object to use, along with the Execute function (below)

var RS = new RemoteScripting();
var calltouse=0;
// Remote Scripting class.  
// This class should NOT be instanciated -- use the global RS object. 
function RemoteScripting()
{
	this.pool = new Array();
	this.poolSize = 0;
	this.maxPoolSize = 200;
	this.usePOST = true;
	this.debug = false;

	// Sniff the browser
	if (document.layers)
		this.browser = "NS";
	else if (document.all)
	{
		var agent = navigator.userAgent.toLowerCase();
		if (agent.indexOf("opera") != -1)
			this.browser = "OPR";
		else if (agent.indexOf("konqueror") != -1)
			this.browser = "KONQ";
		else
			this.browser = "IE";
	}
	else if (document.getElementById)
		this.browser = "MOZ";
	else 
		this.browser = "OTHER";
}

// Executes a remote method found on a given URL.
// Usage: Execute(url,method,p1 ... pn,callback,error_callback)
//		  url				: url of file containing method to invoke
//		  method			: name of the Server-side method to be invoked
//		  p1...pn			: any parameters to be passed to the Server-side method
//		  callback		    : optional JavaScript function to call on successful return.
//        error_callback	: optional JavaScript function to call on failed return.  
//                            If not passed and an error occurs, an alert box is shown.
RemoteScripting.prototype.Execute = function(url, method)
{
	var call = this.getAvailableCall();	
	var args = RemoteScripting.prototype.Execute.arguments;
	var len = RemoteScripting.prototype.Execute.arguments.length;

	var methodArgs = new Array();

	for (var i = 2; i < len; i++)
	{
		if (typeof(args[i]) == 'function')
		{
			call.callback = args[i];		  
			if (i + 1 < len)
				call.error_callback = args[i + 1];
			break;
		}
		
		methodArgs[i - 2] = args[i];
	}
	
	call.showIfDebugging();

	if (this.usePOST && ((this.browser == 'IE') || (this.browser == 'MOZ')))
		call.POST(url, method, methodArgs);
	else 
		call.GET(url, method, methodArgs);
	
	return this.id;
}

// Pops up a separate window containing Debug information.
// You can attach this to the F1 key for IE with onHelp = "return RS.showDebugInfo() in the body tag.
RemoteScripting.prototype.PopupDebugInfo = function()
{
	var doc = window.open().document;
	doc.open();
	doc.write('<html><body>Pool Size: ' + this.poolSize + '<br><font face = "arial" size = "2"><b>');
	for (var i in this.pool)
	{
		var call = this.pool[i];
		doc.write('<hr>' + call.id + ' : ' + (call.busy ? 'busy' : 'available') + '<br>');
		doc.write(call.container.document.location.pathname + '<br>');
		doc.write(call.container.document.location.search + '<br>');
		doc.write('<table border = "1"><tr><td>' + call.container.document.body.innerHTML + '</td></tr></table>');
	}
	doc.write('</table></body></html>');
	doc.close();
	return false;
}

// Retrieves an available Call object from the pool.
// This function is used internally and should be treated as private.
RemoteScripting.prototype.getAvailableCall = function()
{
	for (var i in this.poolSize)
	{
		var call = this.pool['C' + (i + 1)];
		if (!call.busy)
		{
			call.busy = true;      
			return this.pool[call.id];
		}
	}
	
	// If we got here, there are no existing free calls
	if (this.poolSize <= this.maxPoolSize)
	{
		var callID = "C" + (this.poolSize + 1);
		this.pool[callID] = new RemoteScriptingCall(callID);
		this.poolSize++;
		return this.pool[callID];
	}
	else
	{
		var call = this.pool['C' + (calltouse + 1)];
		call.busy = true;      
		return this.pool[call.id];
		calltouse=(calltouse+1)%this.maxPoolSize;
	}
	
	//alert("RemoteScripting Error: Call pool is full!");
	//return null;
}


// Remote Scripting Call class.  
// This class should NOT be instanciated -- this is used by the RS object's pool.
function RemoteScriptingCall(callID)
{
	this.id = callID;
	this.busy = true;
	this.callback = null;
	this.error_callback = null;

	switch (RS.browser)
	{
		case 'IE':
			document.body.insertAdjacentHTML("afterBegin", '<span id = "SPAN' + callID + '"></span>');
			var span = document.all("SPAN" + callID);
			var html = '<iframe style = "width:800px" name = "' + callID + '" src = ""></iframe>';
			span.innerHTML = html;
			span.style.display = 'none';
			this.container = window.frames[callID];
			break;
			
		case 'NS':
			this.container = new Layer(100);
			this.container.name = callID;
			this.container.visibility = 'hidden';
			this.container.clip.width = 100;
			this.container.clip.height = 100;
			break;
			
		case 'MOZ':
		case 'OPR':        
			var span = document.createElement('SPAN');
			span.id = "SPAN" + callID;
			document.body.appendChild(span);
			var iframe = document.createElement('IFRAME');
			iframe.id = callID;
			iframe.name = callID;
			iframe.style.width = 800;
			iframe.style.height = 200;
			span.appendChild(iframe);
			this.container = iframe;
			break;
			
		case 'KONQ':  
		default:
			var span = document.createElement('SPAN');
			span.id = "SPAN" + callID;
			document.body.appendChild(span);
			var iframe = document.createElement('IFRAME');
			iframe.id = callID;
			iframe.name = callID;
			iframe.style.width = 800;
			iframe.style.height = 200;
			span.appendChild(iframe);
			this.container = iframe;
			
			// Needs to be hidden for Konqueror, otherwise it'll appear on the page
			span.style.display = none;
			iframe.style.display = none;
			iframe.style.visibility = hidden;
			iframe.height = 0;
			iframe.width = 0;
	}	
}

// Posts to the given url to have it invoke the given method.
// This function is used internally and should be treated as private.
RemoteScriptingCall.prototype.POST = function(url, method, args)
{
	var d = new Date();
	var unique = d.getTime() + '' + Math.floor(1000 * Math.random());
	var doc = (RS.browser == "IE") ? this.container.document : this.container.contentDocument;
	var paramSep = (url.lastIndexOf('?') < 0 ? '?' : '&');
	doc.open();
	doc.write('<html><body>');
	doc.write('<form name="rsForm" method="post" target=""');
	doc.write('action="' + url + paramSep + 'U=' + unique + '">');
	doc.write('<input type="hidden" name="RC" value="' + this.id + '">');
	
	// func and args are optional
	if (method != null)
	{
		doc.write('<input type = "hidden" name = "M" value = "' + method + '">');
		
		if (args != null)
		{
			if (typeof(args) == "string")
			{
				// single parameter
				doc.write('<input type = "hidden" name = "P0" '
					+ 'value = "[' + this.escapeParam(args) + ']">');
			}
			else 
			{
				// assume args is array of strings
				for (var i = 0; i < args.length; i++)
				{
					doc.write('<input type = "hidden" name = "P' + i + '" '
						+ 'value = "[' + this.escapeParam(args[i]) + ']">');
				}
			} // parm type
		} // args
	} // method
	
	doc.write('</form></body></html>');
	doc.close();
	doc.forms['rsForm'].submit();
}

// Navigates to the given url to have it invoke the given method.
// This function is used internally and should be treated as private.
RemoteScriptingCall.prototype.GET = function(url, method, args)
{
	// build URL to call
	var URL = url;
	var paramSep = (url.lastIndexOf('?') < 0 ? '?' : '&');
	
	// always send call
	URL += paramSep + "RC=" + this.id;
	
	// method and args are optional
	if (method != null)
	{
		URL += "&M=" + escape(method);
		
		if (args != null)
		{
			if (typeof(args) == "string")
			{
				// single parameter
				URL += "&P0=[" + escape(args + '') + "]";
			}
			else 
			{
				// assume args is array of strings
				for (var i = 0; i < args.length; i++)
				{
					URL += "&P" + i + "=[" + escape(args[i] + '') + "]";
				}
			} // parm type
		} // args
	} // method
	
	// unique string to defeat cache
	var d = new Date();
	URL += "&U=" + d.getTime();
	
	// make the call
	switch (RS.browser)
	{
		case 'IE':
			this.container.document.location.replace(URL);
			break;
		case 'NS':
			this.container.src = URL;
			break;
		case 'MOZ':
		case 'OPR':
		case 'KONQ':
		default:
			this.container.src = '';
			this.container.src = URL; 
			break;
	}  
}

// Sets the result of the call of the remote method.
// This function is designed to be called only when the response is received.
RemoteScriptingCall.prototype.setResult = function(result)
{
	this.busy = false;
	

	if (result == true)
	{
		if (this.callback != null)
			this.callback(this.unescape(this.getPayload()), this.id);
	}
	else
	{
		if (this.error_callback == null)
			alert(this.unescape(this.getPayload()));
		else
			this.error_callback(this.unescape(this.getPayload()), this.id);
	}
		
	this.callback = null;
	this.error_callback = null;
}

// Retrieves the payload's message sent by the response.
// This function is used internally and should be treated as private.
RemoteScriptingCall.prototype.getPayload = function()
{
	switch (RS.browser)
	{
		case 'IE':
			return this.container.document.forms['rsForm']['rsPayload'].value;
		case 'NS':
			return this.container.document.forms['rsForm'].elements['rsPayload'].value;
		case 'MOZ':
			return window.frames[this.container.name].document.forms['rsForm']['rsPayload'].value; 
		case 'OPR':
		case 'KONQ':
		default:
			return window.frames[this.container.name].document.getElementById("rsPayload").value;
	}  
}

// Shows (or hides) elements on the page that assist in debugging, based on RS.debug.
// This function is used internally and should be treated as private.
RemoteScriptingCall.prototype.showIfDebugging = function()
{
	var vis = (RS.debug == true);
	switch (RS.browser)
	{
		case 'IE':
			document.all("SPAN" + this.id).style.display = (vis) ? '' : 'none';
			break;
		case 'NS':
			this.container.visibility = (vis) ? 'show' : 'hidden';
			break;
		case 'MOZ':
		case 'OPR':
		case 'KONQ':
		default:
			document.getElementById("SPAN" + this.id).style.visibility = (vis) ? '' : 'hidden';
			this.container.width = (vis) ? 250 : 0;
			this.container.height = (vis) ? 100 : 0;
			break; 
	}  
}

// Converts a string to allow it to be properly passed down as a parameter to the page.
// This function is used internally and should be treated as private.
RemoteScriptingCall.prototype.escapeParam = function(str)
{
	return str.replace(/'"'/g, '\\"');
}

// Converts a string to allow it to be properly read back from the server.
// This function is used internally and should be treated as private.
RemoteScriptingCall.prototype.unescape = function(str)
{
	return str.replace(/\\\//g, "/");
}
