function frxOpenPrint(url) {
  window.open(url, '_blank','resizable,scrollbars,width="400",height="300"'); 
};

function frRequestObject() {
  if (typeof XMLHttpRequest === 'undefined') {
    XMLHttpRequest = function() {
      try { return new ActiveXObject("Msxml2.XMLHTTP.6.0"); }
        catch(e) {}
      try { return new ActiveXObject("Msxml2.XMLHTTP.3.0"); }
        catch(e) {}
      try { return new ActiveXObject("Msxml2.XMLHTTP"); }
        catch(e) {}
      try { return new ActiveXObject("Microsoft.XMLHTTP"); }
        catch(e) {}
      throw new Error("This browser does not support XMLHttpRequest.");
    };
  }
  return new XMLHttpRequest();
}

function frRequestServer(url, form) {
    if (form) {
        var multiLineHtmlEncode = function (value) {
            var lines = value.replace(/\r\n|\n\r|\n|\r/g, "\n").split("\n");
            for (var i = 0; i < lines.length; i++) {
                lines[i] = $('<div/>').text(lines[i]).html();
            }
            return lines.join('\r\n');
        };

        // make post data because old IE doesn't have FormData
        var urlEncodedDataPairs = [];
        for (var name in form) {
            var htmlEncodedName = multiLineHtmlEncode(name);
            var htmlEncodedValue = multiLineHtmlEncode(form[name]);
            urlEncodedDataPairs.push(encodeURIComponent(htmlEncodedName) + '=' + encodeURIComponent(htmlEncodedValue));
        }
        var urlEncodedData = urlEncodedDataPairs.join('&').replace(/%20/g, '+');

        $.post(
            encodeURI(url),
            urlEncodedData,
            onFrAjaxSuccess
            );
        return;
    }

    $.get(
        encodeURI(url),        
        {
            "_": $.now()
        },
        onFrAjaxSuccess
        );
}

function frClick(url, id, kind, value) {
    if (kind == 'text_edit') {
        var that = this;
        if (that.win && that.win.close) {
            that.win.close();
        }
        that.win = frPopup(url + '?form=' + id + '&formName=text_edit&formClick=' + value, 'Text_edit', 400, 200);
        that.win.onmessage = function (e) {
            //if (e.data == 'submit') {
            var area = that.win.document.getElementById('js-textarea');
            frRequestServer(url + '?previewobject=' + id + '&' + kind + '=' + value, { text: area.value });
            that.win.close();
            //}
        };
        return;
    }

    frRequestServer(url + '?previewobject=' + id + '&' + kind + '=' + value);
}

function onFrAjaxSuccess(data, textStatus, request) {
    var obj = request.getResponseHeader("FastReport-container");
    var div = document.getElementById(obj);
    div = frReplaceInnerHTML(div, data);
    if (div != null) {
        var scripts = div.getElementsByTagName('script');
        for (var i = 0; i < scripts.length; i++) {
            eval(scripts[i].text);
        }
    }
}

function frReplaceInnerHTML(repobj, html) {
    var obj = repobj;
    if (obj != null) {
        var newObj = document.createElement(obj.nodeName);
        newObj.id = obj.id;
        newObj.className = obj.className;
        $(newObj).html(html);
        newObj.style.display = "inline-block";
        newObj.style.width = "100%";
        newObj.style.height = "100%";
        if (obj.parentNode)
            obj.parentNode.replaceChild(newObj, obj);
        else
            obj.innerHTML = html;
        return newObj;
    }
    else
        return null;
}

function frProcessReqChange() {
  try 
  { 
    if (req.readyState == 4) 
    {
        if (req.status == 200) 
        {
            obj = req.getResponseHeader("FastReport-container");            
            div = document.getElementById(obj);
            div = frReplaceInnerHTML(div, req.responseText);	        
            var scripts = div.getElementsByTagName('script');
            for (var i = 0; i < scripts.length; i++) 
                eval(scripts[i].text);
        } 
        else 
        {
            throw new Error("Error: " + req.statusText);
        }
    }
  }
  catch( e ) {
      throw new Error("Error: " + e);      
  }
}

function frOutline(data, id, cb) {
    var sizes = null;

    if (localStorage) {
        sizes = localStorage.getItem('fr-split-sizes');
    }

    if (sizes) {
        sizes = JSON.parse(sizes);
    } else {
        sizes = [25, 75];
    }

    var split = frOutline.Split(['.froutline', '.frbody'], {
        sizes: sizes,
        minSize: 50,
        snapOffset: 20,
        onDragEnd: function () {
            if (localStorage) {
                localStorage.setItem('fr-split-sizes', JSON.stringify(split.getSizes()));
            }
        }
    });

    $('.froutlinecontainer').jstree({
        'core': {
            'data': data,
            'multiple': false,
            'themes': {
                'dots': false
            }
        },
        'state': {
            'key': id
        },
        'types': {
            'default': {
                'icon': 'jstree-file'
            }
        },
        'plugins': ['state', 'types']
    });
    
    $('.froutlinecontainer').on('ready.jstree', function (e, data) {
        $('.froutlinecontainer').on('changed.jstree', function (e, data) {
            if (data.action == 'select_node' && data.node && data.node.data) {
                cb(+(data.node.data.page) + 1);
            }
        });
    });
}

function frPopup(url, title, w, h) {
    // Fixes dual-screen position                         Most browsers       Firefox
    var dualScreenLeft = window.screenLeft != undefined ? window.screenLeft : window.screenX;
    var dualScreenTop = window.screenTop != undefined ? window.screenTop : window.screenY;

    var width = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
    var height = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

    var left = ((width / 2) - (w / 2)) + dualScreenLeft;
    var top = ((height / 2) - (h / 2)) + dualScreenTop;

    var params = 'menubar=0, toolbar=0, location=0, status=0, resizable=1, scrollbars=1';
    var newWindow = window.open(url, title, params + ', width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);

    if (newWindow.focus) {
        newWindow.focus();
    }

    return newWindow;
}