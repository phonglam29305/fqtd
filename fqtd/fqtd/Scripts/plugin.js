﻿
; (function ($, window, undefined) {
    var pluginName = 'smSlideChart';
    var privateVar = null;
    var privateMethod = function () {

    };

    function Plugin(element, options) {
        this.element = $(element);
        this.options = $.extend({}, $.fn[pluginName].defaults, options);
        this.init();
    };

    Plugin.prototype = {
        init: function () {
            var that = this;
            var options = this.options;
            var currIndex = 0;
            var flag = true;
            var spanContainerLength = that.element.find('div.inner').length;
            that.containerWidth = that.element.find('div.inner').outerWidth(true);
            if (spanContainerLength <= 1) {
                options.btnNext.css('display', 'none');
            }
            options.btnPrev.css('display', 'none');

            options.btnNext.unbind('click.' + pluginName).bind('click.' + pluginName, function () {
                if (flag) {
                    flag = false
                    currIndex++;
                    that.gotoSlide(currIndex, function () {
                        checkBtn();
                        flag = true;
                        //options.textChange.text('Step ' + (currIndex + 1) + ':');
                    }, currIndex - 1);
                }
            });

            options.btnPrev.unbind('click.' + pluginName).bind('click.' + pluginName, function () {
                if (flag) {
                    flag = false
                    currIndex--;
                    that.gotoSlide(currIndex, function () {
                        checkBtn();
                        flag = true;
                        //options.textChange.text('Step ' + (currIndex + 1) + ':');
                    }, currIndex + 1);
                }
            });
            options.thumbs.unbind('click.' + pluginName).bind('click.' + pluginName, function () {
                if (flag) {
                    flag = false
                    index = options.thumbs.index($(this))
                    that.gotoSlide(index, function () {
                        currIndex = index;
                        flag = true;
                        checkBtn();
                        //options.textChange.text('Step ' + (currIndex + 1) + ':');
                    }, currIndex);
                }
            });
            function checkBtn() {
                if (currIndex <= 0) {
                    options.btnPrev.css('display', 'none');
                    options.btnNext.css('display', 'block');
                } else {
                    if (currIndex == spanContainerLength - 1) {
                        options.btnNext.css('display', 'none');
                        options.btnPrev.css('display', 'block');
                    } else {
                        options.btnNext.css('display', 'block');
                        options.btnPrev.css('display', 'block');
                    }
                }
            }

        },
        gotoSlide: function (index, callback, last) {
            var that = this;
            var options = this.options;
            options.callback(index, last);
            that.element.animate({
                'margin-left': -that.containerWidth * index
            }, options.duration, 'easeInOutBack', function () {
                $.isFunction(callback) && callback();
                //options.callback(index, last);
            });
            options.thumbs.each(function () {
                if (options.thumbs.index($(this)) == index) {
                    $(this).addClass('active');
                } else {
                    $(this).removeClass('active');
                }
            });
        },
        destroy: function () {
            $.removeData(this.element, pluginName);
        }
    };

    $.fn[pluginName] = function (options, params) {
        return this.each(function () {
            var instance = $.data(this, pluginName);
            if (!instance) {
                $.data(this, pluginName, new Plugin(this, options));
            } else if (instance[options]) {
                instance[options](params);
            } else {
                alert(options ? options + ' method is not exists in ' + pluginName : pluginName + ' plugin has been initialized');
            }
        });
    };

    $.fn[pluginName].defaults = {
        btnNext: '',
        btnPrev: '',
        thumbs: '',
        duration: 1000,
        textChange: '',
        callback: function () { }
    };
}(jQuery, window));

;(function ($, window, undefined) {

    var
        // String constants for data names
        dataFlag = "watermark",
        dataClass = "watermarkClass",
        dataFocus = "watermarkFocus",
        dataFormSubmit = "watermarkSubmit",
        dataMaxLen = "watermarkMaxLength",
        dataPassword = "watermarkPassword",
        dataText = "watermarkText",

        // Copy of native jQuery regex use to strip return characters from element value
        rreturn = /\r/g,

        // Used to determine if type attribute of input element is a non-text type (invalid)
        rInvalidType = /^(button|checkbox|hidden|image|radio|range|reset|submit)$/i,

        // Includes only elements with watermark defined
        selWatermarkDefined = "input:data(" + dataFlag + "),textarea:data(" + dataFlag + ")",

        // Includes only elements capable of having watermark
        selWatermarkAble = ":watermarkable",

        // triggerFns:
        // Array of function names to look for in the global namespace.
        // Any such functions found will be hijacked to trigger a call to
        // hideAll() any time they are called.  The default value is the
        // ASP.NET function that validates the controls on the page
        // prior to a postback.
        // 
        // Am I missing other important trigger function(s) to look for?
        // Please leave me feedback:
        // http://code.google.com/p/jquery-watermark/issues/list
        triggerFns = [
            "Page_ClientValidate"
        ],

        // Holds a value of true if a watermark was displayed since the last
        // hideAll() was executed. Avoids repeatedly calling hideAll().
        pageDirty = false,

        // Detects if the browser can handle native placeholders
        hasNativePlaceholder = ("placeholder" in document.createElement("input"));

    // Best practice: this plugin adds only one method to the jQuery object.
    // Also ensures that the watermark code is only added once.
    $.watermark = $.watermark || {

        // Current version number of the plugin
        version: "3.1.4",

        runOnce: true,

        // Default options used when watermarks are instantiated.
        // Can be changed to affect the default behavior for all
        // new or updated watermarks.
        options: {

            // Default class name for all watermarks
            className: "watermark",

            // If true, plugin will detect and use native browser support for
            // watermarks, if available. (e.g., WebKit's placeholder attribute.)
            useNative: true,

            // If true, all watermarks will be hidden during the window's
            // beforeunload event. This is done mainly because WebKit
            // browsers remember the watermark text during navigation
            // and try to restore the watermark text after the user clicks
            // the Back button. We can avoid this by hiding the text before
            // the browser has a chance to save it. The regular unload event
            // was tried, but it seems the browser saves the text before
            // that event kicks off, because it didn't work.
            hideBeforeUnload: true
        },

        // Hide one or more watermarks by specifying any selector type
        // i.e., DOM element, string selector, jQuery matched set, etc.
        hide: function (selector) {
            $(selector).filter(selWatermarkDefined).each(
                function () {
                    $.watermark._hide($(this));
                }
            );
        },

        // Internal use only.
        _hide: function ($input, focus) {
            var elem = $input[0],
                inputVal = (elem.value || "").replace(rreturn, ""),
                inputWm = $input.data(dataText) || "",
                maxLen = $input.data(dataMaxLen) || 0,
                className = $input.data(dataClass);

            if ((inputWm.length) && (inputVal == inputWm)) {
                elem.value = "";

                // Password type?
                if ($input.data(dataPassword)) {

                    if (($input.attr("type") || "") === "text") {
                        var $pwd = $input.data(dataPassword) || [],
                            $wrap = $input.parent() || [];

                        if (($pwd.length) && ($wrap.length)) {
                            $wrap[0].removeChild($input[0]); // Can't use jQuery methods, because they destroy data
                            $wrap[0].appendChild($pwd[0]);
                            $input = $pwd;
                        }
                    }
                }

                if (maxLen) {
                    $input.attr("maxLength", maxLen);
                    $input.removeData(dataMaxLen);
                }

                if (focus) {
                    $input.attr("autocomplete", "off");  // Avoid NS_ERROR_XPC_JS_THREW_STRING error in Firefox

                    window.setTimeout(
                        function () {
                            $input.select();  // Fix missing cursor in IE
                        }
                    , 1);
                }
            }

            className && $input.removeClass(className);
        },

        // Display one or more watermarks by specifying any selector type
        // i.e., DOM element, string selector, jQuery matched set, etc.
        // If conditions are not right for displaying a watermark, ensures that watermark is not shown.
        show: function (selector) {
            $(selector).filter(selWatermarkDefined).each(
                function () {
                    $.watermark._show($(this));
                }
            );
        },

        // Internal use only.
        _show: function ($input) {
            var elem = $input[0],
                val = (elem.value || "").replace(rreturn, ""),
                text = $input.data(dataText) || "",
                type = $input.attr("type") || "",
                className = $input.data(dataClass);

            if (((val.length == 0) || (val == text)) && (!$input.data(dataFocus))) {
                pageDirty = true;

                // Password type?
                if ($input.data(dataPassword)) {

                    if (type === "password") {
                        var $pwd = $input.data(dataPassword) || [],
                            $wrap = $input.parent() || [];

                        if (($pwd.length) && ($wrap.length)) {
                            $wrap[0].removeChild($input[0]); // Can't use jQuery methods, because they destroy data
                            $wrap[0].appendChild($pwd[0]);
                            $input = $pwd;
                            $input.attr("maxLength", text.length);
                            elem = $input[0];
                        }
                    }
                }

                // Ensure maxLength big enough to hold watermark (input of type="text" or type="search" only)
                if ((type === "text") || (type === "search")) {
                    var maxLen = $input.attr("maxLength") || 0;

                    if ((maxLen > 0) && (text.length > maxLen)) {
                        $input.data(dataMaxLen, maxLen);
                        $input.attr("maxLength", text.length);
                    }
                }

                className && $input.addClass(className);
                elem.value = text;
            }
            else {
                $.watermark._hide($input);
            }
        },

        // Hides all watermarks on the current page.
        hideAll: function () {
            if (pageDirty) {
                $.watermark.hide(selWatermarkAble);
                pageDirty = false;
            }
        },

        // Displays all watermarks on the current page.
        showAll: function () {
            $.watermark.show(selWatermarkAble);
        }
    };

    $.fn.watermark = $.fn.watermark || function (text, options) {
        ///	<summary>
        ///		Set watermark text and class name on all input elements of type="text/password/search" and
        /// 	textareas within the matched set. If className is not specified in options, the default is
        /// 	"watermark". Within the matched set, only input elements with type="text/password/search"
        /// 	and textareas are affected; all other elements are ignored.
        ///	</summary>
        ///	<returns type="jQuery">
        ///		Returns the original jQuery matched set (not just the input and texarea elements).
        /// </returns>
        ///	<param name="text" type="String">
        ///		Text to display as a watermark when the input or textarea element has an empty value and does not
        /// 	have focus. The first time watermark() is called on an element, if this argument is empty (or not
        /// 	a String type), then the watermark will have the net effect of only changing the class name when
        /// 	the input or textarea element's value is empty and it does not have focus.
        ///	</param>
        ///	<param name="options" type="Object" optional="true">
        ///		Provides the ability to override the default watermark options ($.watermark.options). For backward
        /// 	compatibility, if a string value is supplied, it is used as the class name that overrides the class
        /// 	name in $.watermark.options.className. Properties include:
        /// 		className: When the watermark is visible, the element will be styled using this class name.
        /// 		useNative (Boolean or Function): Specifies if native browser support for watermarks will supersede
        /// 			plugin functionality. If useNative is a function, the return value from the function will
        /// 			determine if native support is used. The function is passed one argument -- a jQuery object
        /// 			containing the element being tested as the only element in its matched set -- and the DOM
        /// 			element being tested is the object on which the function is invoked (the value of "this").
        ///	</param>
        /// <remarks>
        ///		The effect of changing the text and class name on an input element is called a watermark because
        ///		typically light gray text is used to provide a hint as to what type of input is required. However,
        ///		the appearance of the watermark can be something completely different: simply change the CSS style
        ///		pertaining to the supplied class name.
        ///		
        ///		The first time watermark() is called on an element, the watermark text and class name are initialized,
        ///		and the focus and blur events are hooked in order to control the display of the watermark.  Also, as
        /// 	of version 3.0, drag and drop events are hooked to guard against dropped text being appended to the
        /// 	watermark.  If native watermark support is provided by the browser, it is detected and used, unless
        /// 	the useNative option is set to false.
        ///		
        ///		Subsequently, watermark() can be called again on an element in order to change the watermark text
        ///		and/or class name, and it can also be called without any arguments in order to refresh the display.
        ///		
        ///		For example, after changing the value of the input or textarea element programmatically, watermark()
        /// 	should be called without any arguments to refresh the display, because the change event is only
        /// 	triggered by user actions, not by programmatic changes to an input or textarea element's value.
        /// 	
        /// 	The one exception to programmatic updates is for password input elements:  you are strongly cautioned
        /// 	against changing the value of a password input element programmatically (after the page loads).
        /// 	The reason is that some fairly hairy code is required behind the scenes to make the watermarks bypass
        /// 	IE security and switch back and forth between clear text (for watermarks) and obscured text (for
        /// 	passwords).  It is *possible* to make programmatic changes, but it must be done in a certain way, and
        /// 	overall it is not recommended.
        /// </remarks>

        if (!this.length) {
            return this;
        }

        var hasClass = false,
            hasText = (typeof (text) === "string");

        if (hasText) {
            text = text.replace(rreturn, "");
        }

        if (typeof (options) === "object") {
            hasClass = (typeof (options.className) === "string");
            options = $.extend({}, $.watermark.options, options);
        }
        else if (typeof (options) === "string") {
            hasClass = true;
            options = $.extend({}, $.watermark.options, { className: options });
        }
        else {
            options = $.watermark.options;
        }

        if (typeof (options.useNative) !== "function") {
            options.useNative = options.useNative ? function () { return true; } : function () { return false; };
        }

        return this.each(
            function () {
                var $input = $(this);

                if (!$input.is(selWatermarkAble)) {
                    return;
                }

                // Watermark already initialized?
                if ($input.data(dataFlag)) {

                    // If re-defining text or class, first remove existing watermark, then make changes
                    if (hasText || hasClass) {
                        $.watermark._hide($input);

                        if (hasText) {
                            $input.data(dataText, text);
                        }

                        if (hasClass) {
                            $input.data(dataClass, options.className);
                        }
                    }
                }
                else {

                    // Detect and use native browser support, if enabled in options
                    if (
                        (hasNativePlaceholder)
                        && (options.useNative.call(this, $input))
                        && (($input.attr("tagName") || "") !== "TEXTAREA")
                    ) {
                        // className is not set because current placeholder standard doesn't
                        // have a separate class name property for placeholders (watermarks).
                        if (hasText) {
                            $input.attr("placeholder", text);
                        }

                        // Only set data flag for non-native watermarks
                        // [purposely commented-out] -> $input.data(dataFlag, 1);
                        return;
                    }

                    $input.data(dataText, hasText ? text : "");
                    $input.data(dataClass, options.className);
                    $input.data(dataFlag, 1); // Flag indicates watermark was initialized

                    // Special processing for password type
                    if (($input.attr("type") || "") === "password") {
                        var $wrap = $input.wrap("<span>").parent(),
                            $wm = $($wrap.html().replace(/type=["']?password["']?/i, 'type="text"'));

                        $wm.data(dataText, $input.data(dataText));
                        $wm.data(dataClass, $input.data(dataClass));
                        $wm.data(dataFlag, 1);
                        $wm.attr("maxLength", text.length);

                        $wm.focus(
                            function () {
                                $.watermark._hide($wm, true);
                            }
                        ).bind("dragenter",
                            function () {
                                $.watermark._hide($wm);
                            }
                        ).bind("dragend",
                            function () {
                                window.setTimeout(function () { $wm.blur(); }, 1);
                            }
                        );

                        $input.blur(
                            function () {
                                $.watermark._show($input);
                            }
                        ).bind("dragleave",
                            function () {
                                $.watermark._show($input);
                            }
                        );

                        $wm.data(dataPassword, $input);
                        $input.data(dataPassword, $wm);
                    }
                    else {

                        $input.focus(
                            function () {
                                $input.data(dataFocus, 1);
                                $.watermark._hide($input, true);
                            }
                        ).blur(
                            function () {
                                $input.data(dataFocus, 0);
                                $.watermark._show($input);
                            }
                        ).bind("dragenter",
                            function () {
                                $.watermark._hide($input);
                            }
                        ).bind("dragleave",
                            function () {
                                $.watermark._show($input);
                            }
                        ).bind("dragend",
                            function () {
                                window.setTimeout(function () { $.watermark._show($input); }, 1);
                            }
                        ).bind("drop",
                            // Firefox makes this lovely function necessary because the dropped text
                            // is merged with the watermark before the drop event is called.
                            function (evt) {
                                var elem = $input[0],
                                    dropText = evt.originalEvent.dataTransfer.getData("Text");

                                if ((elem.value || "").replace(rreturn, "").replace(dropText, "") === $input.data(dataText)) {
                                    elem.value = dropText;
                                }

                                $input.focus();
                            }
                        );
                    }

                    // In order to reliably clear all watermarks before form submission,
                    // we need to replace the form's submit function with our own
                    // function.  Otherwise watermarks won't be cleared when the form
                    // is submitted programmatically.
                    if (this.form) {
                        var form = this.form,
                            $form = $(form);

                        if (!$form.data(dataFormSubmit)) {
                            $form.submit($.watermark.hideAll);

                            // form.submit exists for all browsers except Google Chrome
                            // (see "else" below for explanation)
                            if (form.submit) {
                                $form.data(dataFormSubmit, form.submit);

                                form.submit = (function (f, $f) {
                                    return function () {
                                        var nativeSubmit = $f.data(dataFormSubmit);

                                        $.watermark.hideAll();

                                        if (nativeSubmit.apply) {
                                            nativeSubmit.apply(f, Array.prototype.slice.call(arguments));
                                        }
                                        else {
                                            nativeSubmit();
                                        }
                                    };
                                })(form, $form);
                            }
                            else {
                                $form.data(dataFormSubmit, 1);

                                // This strangeness is due to the fact that Google Chrome's
                                // form.submit function is not visible to JavaScript (identifies
                                // as "undefined").  I had to invent a solution here because hours
                                // of Googling (ironically) for an answer did not turn up anything
                                // useful.  Within my own form.submit function I delete the form's
                                // submit function, and then call the non-existent function --
                                // which, in the world of Google Chrome, still exists.
                                form.submit = (function (f) {
                                    return function () {
                                        $.watermark.hideAll();
                                        delete f.submit;
                                        f.submit();
                                    };
                                })(form);
                            }
                        }
                    }
                }

                $.watermark._show($input);
            }
        );
    };

    // The code included within the following if structure is guaranteed to only run once,
    // even if the watermark script file is included multiple times in the page.
    if ($.watermark.runOnce) {
        $.watermark.runOnce = false;

        $.extend($.expr[":"], {

            // Extends jQuery with a custom selector - ":data(...)"
            // :data(<name>)  Includes elements that have a specific name defined in the jQuery data
            // collection. (Only the existence of the name is checked; the value is ignored.)
            // A more sophisticated version of the :data() custom selector originally part of this plugin
            // was removed for compatibility with jQuery UI. The original code can be found in the SVN
            // source listing in the file, "jquery.data.js".
            data: $.expr.createPseudo ?
                $.expr.createPseudo(function (dataName) {
                    return function (elem) {
                        return !!$.data(elem, dataName);
                    };
                }) :
                // support: jQuery <1.8
                function (elem, i, match) {
                    return !!$.data(elem, match[3]);
                },

            // Extends jQuery with a custom selector - ":watermarkable"
            // Includes elements that can be watermarked, including textareas and most input elements
            // that accept text input.  It uses a "negative" test (i.e., testing for input types that DON'T
            // work) because the HTML spec states that you can basically use any type, and if it doesn't
            // recognize the type it will default to type=text.  So if we only looked for certain type attributes
            // we would fail to recognize non-standard types, which are still valid and watermarkable.
            watermarkable: function (elem) {
                var type,
                    name = elem.nodeName;

                if (name === "TEXTAREA") {
                    return true;
                }

                if (name !== "INPUT") {
                    return false;
                }

                type = elem.getAttribute("type");

                return ((!type) || (!rInvalidType.test(type)));
            }
        });

        // Overloads the jQuery .val() function to return the underlying input value on
        // watermarked input elements.  When .val() is being used to set values, this
        // function ensures watermarks are properly set/removed after the values are set.
        // Uses self-executing function to override the default jQuery function.
        (function (valOld) {

            $.fn.val = function () {
                var args = Array.prototype.slice.call(arguments);

                // Best practice: return immediately if empty matched set
                if (!this.length) {
                    return args.length ? this : undefined;
                }

                // If no args, then we're getting the value of the first element;
                // else we're setting values for all elements in matched set
                if (!args.length) {

                    // If element is watermarked, get the underlying value;
                    // else use native jQuery .val()
                    if (this.data(dataFlag)) {
                        var v = (this[0].value || "").replace(rreturn, "");
                        return (v === (this.data(dataText) || "")) ? "" : v;
                    }
                    else {
                        return valOld.apply(this);
                    }
                }
                else {
                    valOld.apply(this, args);
                    $.watermark.show(this);
                    return this;
                }
            };

        })($.fn.val);

        // Hijack any functions found in the triggerFns list
        if (triggerFns.length) {

            // Wait until DOM is ready before searching
            $(function () {
                var i, name, fn;

                for (i = triggerFns.length - 1; i >= 0; i--) {
                    name = triggerFns[i];
                    fn = window[name];

                    if (typeof (fn) === "function") {
                        window[name] = (function (origFn) {
                            return function () {
                                $.watermark.hideAll();
                                return origFn.apply(null, Array.prototype.slice.call(arguments));
                            };
                        })(fn);
                    }
                }
            });
        }

        $(window).bind("beforeunload", function () {
            if ($.watermark.options.hideBeforeUnload) {
                $.watermark.hideAll();
            }
        });
    }

})(jQuery, window);

; (function (defaults, $, window, undefined) {

    var

		type = ['input:not([type]),input[type="color"],input[type="date"],input[type="datetime"],input[type="datetime-local"],input[type="email"],input[type="file"],input[type="hidden"],input[type="month"],input[type="number"],input[type="password"],input[type="range"],input[type="search"],input[type="tel"],input[type="text"],input[type="time"],input[type="url"],input[type="week"],textarea', 'select', 'input[type="checkbox"],input[type="radio"]'],

		// All field types
		allTypes = type.join(','),

		extend = {},

		// Method to validate each fields
		validateField = function (event, options) {

		    var

				// Field status
				status = {
				    pattern: true,
				    conditional: true,
				    required: true
				},

				// Current field
				field = $(this),

				// Current field value
				fieldValue = field.val() || '',

				// An index of extend
				fieldValidate = field.data('validate'),

				// A validation object (jQuery.fn.validateExtend)
				validation = fieldValidate !== undefined ? extend[fieldValidate] : {},

				// One index or more separated for spaces to prepare the field value
				fieldPrepare = field.data('prepare') || validation.prepare,

				// A regular expression to validate field value
				fieldPattern = (field.data('pattern') || ($.type(validation.pattern) == 'regexp' ? validation.pattern : /(?:)/)),

				// Is case sensitive? (Boolean)
				fieldIgnoreCase = field.attr('data-ignore-case') || field.data('ignoreCase') || validation.ignoreCase,

				// A field mask
				fieldMask = field.data('mask') || validation.mask,

				// A index in the conditional object containing a function to validate the field value
				fieldConditional = field.data('conditional') || validation.conditional,

				// Is required?
				fieldRequired = field.data('required'),

				// The description element id
				fieldDescribedby = field.data('describedby') || validation.describedby,

				// An index of description object
				fieldDescription = field.data('description') || validation.description,

                // An index of description object
				fieldDisplayinside = field.data('displayinside') || validation.displayinside,

				// Trim spaces?
				fieldTrim = field.data('trim'),

				reTrue = /^(true|)$/i,

				reFalse = /^false$/i,

				// The description object
				fieldDescription = $.isPlainObject(fieldDescription) ? fieldDescription : (options.description[fieldDescription] || {}),

				name = 'validate';

		    fieldRequired = fieldRequired != '' ? (fieldRequired || !!validation.required) : true;

		    fieldTrim = fieldTrim != '' ? (fieldTrim || !!validation.trim) : true;

		    // Trim spaces?
		    if (reTrue.test(fieldTrim)) {

		        fieldValue = $.trim(fieldValue);
		    }

		    // The fieldPrepare is a function?
		    if ($.isFunction(fieldPrepare)) {

		        // Updates the fieldValue variable
		        fieldValue = String(fieldPrepare.call(field, fieldValue));
		    } else {

		        // Is a function?
		        if ($.isFunction(options.prepare[fieldPrepare])) {

		            // Updates the fieldValue variable
		            fieldValue = String(options.prepare[fieldPrepare].call(field, fieldValue));
		        }
		    }

		    // Is not RegExp?
		    if ($.type(fieldPattern) != 'regexp') {

		        fieldIgnoreCase = !reFalse.test(fieldIgnoreCase);

		        // Converts to RegExp
		        fieldPattern = fieldIgnoreCase ? RegExp(fieldPattern, 'i') : RegExp(fieldPattern);
		    }

		    // The conditional exists?
		    if (fieldConditional != undefined) {

		        // The fieldConditional is a function?
		        if ($.isFunction(fieldConditional)) {

		            status.conditional = !!fieldConditional.call(field, fieldValue, options);
		        } else {

		            var

						// Splits the conditionals in an array
						conditionals = fieldConditional.split(/[\s\t]+/);

		            // Each conditional
		            for (var counter = 0, len = conditionals.length; counter < len; counter++) {

		                if (options.conditional.hasOwnProperty(conditionals[counter]) && !options.conditional[conditionals[counter]].call(field, fieldValue, options)) {

		                    status.conditional = false;
		                }
		            }
		        }
		    }

		    fieldRequired = reTrue.test(fieldRequired);

		    // Is required?
		    if (fieldRequired) {

		        // Verifies the field type
		        if (field.is(type[0] + ',' + type[1])) {

		            // Is empty?
		            if (!fieldValue.length > 0) {

		                status.required = false;
		            }
		        } else if (field.is(type[2])) {

		            if (field.is('[name]')) {

		                // Is checked?
		                if ($('[name="' + field.prop('name') + '"]:checked').length == 0) {

		                    status.required = false;
		                }
		            } else {

		                status.required = field.is(':checked');
		            }
		        }
		    }

		    // Verifies the field type
		    if (field.is(type[0])) {

		        // Test the field value pattern
		        if (fieldPattern.test(fieldValue)) {

		            // If the event type is not equals to keyup
		            if (event.type != 'keyup' && fieldMask !== undefined) {

		                var matches = fieldValue.match(fieldPattern);

		                // Each characters group
		                for (var i = 0, len = matches.length; i < len; i++) {

		                    // Replace the groups
		                    fieldMask = fieldMask.replace(RegExp('\\$\\{' + i + '(?::`([^`]*)`)?\\}', 'g'), (matches[i] !== undefined ? matches[i] : '$1'));
		                }

		                fieldMask = fieldMask.replace(/\$\{\d+(?::`([^`]*)`)?\}/g, '$1');

		                // Test the field value pattern
		                if (fieldPattern.test(fieldMask)) {

		                    // Update the field value
		                    field.val(fieldMask);
		                }
		            }
		        } else {

		            // If the field is required
		            if (fieldRequired) {

		                status.pattern = false;
		            } else {

		                if (fieldValue.length > 0) {

		                    status.pattern = false;
		                }
		            }
		        }
		    }

		    var

				describedby = $('[id="' + fieldDescribedby + '"]'),

				log = fieldDescription.valid;

		    if (describedby.length > 0 && event.type != 'keyup') {

		        if (!status.required) {

		            log = fieldDescription.required;
		        } else if (!status.pattern) {

		            log = fieldDescription.pattern;
		        } else if (!status.conditional) {

		            log = fieldDescription.conditional;
		        }

		        describedby.html(log || '');		       
		    }

		    // Check if display message in the textbox
		    if (fieldDisplayinside) {
		        if (!status.required) {
		            field.val(fieldDescription.required);
		        }
		    }
		    

		    if (typeof (validation.each) == 'function') {

		        validation.each.call(field, event, status, options);
		    }

		    // Call the eachField callback
		    options.eachField.call(field, event, status, options);

		    // If the field is valid
		    if (status.required && status.pattern && status.conditional) {

		        // If WAI-ARIA is enabled
		        if (!!options.waiAria) {

		            field.prop('aria-invalid', false);
		        }

		        if (typeof (validation.valid) == 'function') {

		            validation.valid.call(field, event, status, options);
		        }

		        // Call the eachValidField callback
		        options.eachValidField.call(field, event, status, options);
		    } else {

		        // If WAI-ARIA is enabled
		        if (!!options.waiAria) {

		            field.prop('aria-invalid', true);
		        }

		        if (typeof (validation.invalid) == 'function') {

		            validation.invalid.call(field, event, status, options);
		        }

		        // Call the eachInvalidField callback
		        options.eachInvalidField.call(field, event, status, options);
		    }

		    // Returns the field status
		    return status;
		};

    $.extend({

        // Method to extends validations
        validateExtend: function (options) {

            return $.extend(extend, options);
        },

        // Method to change the default properties of jQuery.fn.validate method
        validateSetup: function (options) {

            return $.extend(defaults, options);
        }
    }).fn.extend({

        // Method to validate forms
        validate: function (options) {

            options = $.extend({}, defaults, options);

            return $(this).validateDestroy().each(function () {

                var form = $(this);

                // This is a form?
                if (form.is('form')) {

                    form.data(name, {
                        options: options
                    });

                    var

						fields = form.find(allTypes),

						// Events namespace
						namespace = options.namespace;

                    if (form.is('[id]')) {

                        fields = fields.add('[form="' + form.prop('id') + '"]').filter(allTypes);
                    }

                    fields = fields.filter(options.filter);

                    // If onKeyup is enabled
                    if (!!options.onKeyup) {

                        fields.filter(type[0]).on('keyup.' + namespace, function (event) {

                            validateField.call(this, event, options);
                        });
                    }

                    // If onBlur is enabled
                    if (!!options.onBlur) {

                        fields.on('blur.' + namespace, function (event) {

                            validateField.call(this, event, options);
                        });
                    }

                    // If onChange is enabled
                    if (!!options.onChange) {

                        fields.on('change.' + namespace, function (event) {

                            validateField.call(this, event, options);
                        });
                    }

                    // If onSubmit is enabled
                    if (!!options.onSubmit) {

                        form.on('submit.' + namespace, function (event) {

                            var formValid = true;

                            fields.each(function () {

                                var status = validateField.call(this, event, options);

                                if (!status.pattern || !status.conditional || !status.required) {

                                    formValid = false;

                                }
                            });

                            // If form is valid
                            if (formValid) {

                                // Send form?
                                if (!options.sendForm) {

                                    event.preventDefault();
                                }

                                // Is a function?
                                if ($.isFunction(options.valid)) {

                                    options.valid.call(form, event, options);
                                }
                            } else {

                                event.preventDefault();

                                // Is a function?
                                if ($.isFunction(options.invalid)) {

                                    options.invalid.call(form, event, options);
                                }
                            }
                        });
                    }
                }
            });
        },

        // Method to destroy validations
        validateDestroy: function () {

            var

				form = $(this),

				dataValidate = form.data(name);

            // If this is a form
            if (form.is('form') && $.isPlainObject(dataValidate) && typeof (dataValidate.options.nameSpace) == 'string') {

                var fields = form.removeData(name).find(allTypes).add(form);

                if (form.is('[id]')) {

                    fields = fields.add($('[form="' + form.prop('id') + '"]').filter(allTypes));
                }

                fields.off('.' + dataValidate.options.nameSpace);
            }

            return form;
        }
    });
})({

    // Send form if is valid?
    sendForm: true,

    // Use WAI-ARIA properties
    waiAria: true,

    // Validate on submit?
    onSubmit: true,

    // Validate on onKeyup?
    onKeyup: false,

    // Validate on onBlur?
    onBlur: false,

    // Validate on onChange?
    onChange: false,

    // Default namespace
    nameSpace: 'validate',

    // Conditional functions
    conditional: {},

    // Prepare functions
    prepare: {},

    // Fields descriptions
    description: {},

    // Callback
    eachField: $.noop,

    // Callback
    eachInvalidField: $.noop,

    // Callback
    eachValidField: $.noop,

    // Callback
    invalid: $.noop,

    // Callback
    valid: $.noop,

    // A fielter to the fields
    filter: '*'
}, jQuery, window);