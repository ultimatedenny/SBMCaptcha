(function($){
    $.fn.extend({
        sbmCaptcha: function(options) {
            var defaults = {
                captchaTheme: [''],
                captchaFontFamily: '',
                captchaClickDelay: 1000,
                captchaHoverDetection: true,
                showCredits: 'show',
                enableLoadingAnimation: false,
                loadingAnimationDelay: 2000,
                requestIconsDelay: 1500,
                captchaAjaxFile: '../php/captcha-request.php',
                captchaMessages: {
                    header: 'Select the image that does not belong in the row',
                    correct: {
                        top: 'Great!',
                        bottom: 'You do not appear to be a robot.'
                    },
                    incorrect: {
                        top: 'Oops!',
                        bottom: 'You\'ve selected the wrong image.'
                    }
                }
            };
            var $options =  $.extend(defaults, options);
            return this.each(function(id) {
                var $holder = $(this);
                var $captcha_id = id;
                var build_time = 0;
                var hovering = false;
                var generated = false;
                var images_ready = 0;
                buildCaptcha(false);
                function buildCaptcha(loaderActive) {
                    var captchaTheme = 'light';
                    if($options.captchaTheme[$captcha_id] !== undefined && ($options.captchaTheme[$captcha_id] === 'dark' || $options.captchaTheme[$captcha_id] === 'light')) {
                        captchaTheme = $options.captchaTheme[$captcha_id].toLowerCase();
                    }
                    images_ready = 0;
                    $holder.addClass('captcha-theme-' + captchaTheme);
                    if(!generated)
                        _buildCaptchaHolder();
                    var $icon_holder = $holder.find('.captcha-modal__icons');
                    if(($options.requestIconsDelay && $options.requestIconsDelay > 0) && !generated) {
                        if(!loaderActive)
                            addLoader($icon_holder);
                        setTimeout(function() {
                            loadCaptcha(captchaTheme, $icon_holder, true);
                        }, $options.requestIconsDelay)
                    } else {
                        loadCaptcha(captchaTheme, $icon_holder, loaderActive);
                    }
                }

                function loadCaptcha(captchaTheme, iconHolder, loadDelay) {
                    $.ajax({
                        url: $options.captchaAjaxFile,
                        type: 'post',
						dataType: 'json',
                        data: {cID: $captcha_id, rT : 1, tM: captchaTheme},
                        success: function (data) {
                            if(data && typeof data === "object") {
                                if(!loadDelay)
                                    addLoader(iconHolder);
                                build_time = new Date();
                                $holder.find('.captcha-image').each(function(i) {
                                    $(this).css('background-image', 'url(' + $options.captchaAjaxFile + '?cid=' + $captcha_id + '&hash=' + data[i] + ')');
                                    $(this).attr('icon-hash', data[i]);

                                    loadImage($(this), iconHolder);
                                });
                                if(!generated)
                                    $holder.trigger('init', [{captcha_id: id}]);

                                generated = true;
                            }
                        },
                        error: function() {
                            showError();
                        }
                    });
                }
                function _buildCaptchaHolder() {
                    if($options.captchaFontFamily) {
                        $holder.css('font-family', $options.captchaFontFamily);
                    } else {
                        $('body').append('<!-- SBM Captcha default font --><link href="https://fonts.googleapis.com/css?family=Roboto:400,500" rel="stylesheet">');
                    }
                    var captchaHTML = [];
                    captchaHTML.push(
                        "<div class='captcha-modal'>",
                        "<div class='captcha-modal__header'>",
                        "<span>" + (($options.captchaMessages.header && $options.captchaMessages.header) ? $options.captchaMessages.header : "Select the image that does not belong in the row") + "</span>",
                        "</div>",
                        "<div class='captcha-modal__icons'>",
                        "<div class='captcha-image'></div>",
                        "<div class='captcha-image'></div>",
                        "<div class='captcha-image'></div>",
                        "<div class='captcha-image'></div>",
                        "<div class='captcha-image'></div>",
                        "</div>"
                    );
                    if($options.showCredits === 'show' || $options.showCredits === 'hide') {
                        var className = 'captcha-modal__credits' + (($options.showCredits === 'hide') ? ' captcha-modal__credits--hide' : '');

                        captchaHTML.push(
                            "<div class='" + className + "' title='SBMCaptcha by PT SHIMANO BATAM'>",
                            "<a href='#' target='_blank' rel='follow'>SBMCaptcha</a> &copy;",
                            "</div>"
                        );
                    }
                    captchaHTML.push(
                        "<input type='hidden' name='captcha-hf' required />",
                        "<input type='hidden' name='captcha-idhf' value='" + $captcha_id + "' required />",
                        "</div>"
                    );

                    $holder.html(captchaHTML.join('')).attr('data-captcha-id', $captcha_id);
                }
                function submitCaptcha(captcha) {
                    var clicked_class = captcha.attr('icon-hash');

                    if(clicked_class) {
                        $holder.find('input[name="captcha-hf"]').attr('value', clicked_class);
                        $holder.find('input[name="captcha-idhf"]').attr('value', $captcha_id);
                        $.ajax({
                            url: $options.captchaAjaxFile,
                            type: 'post',
                            data: {cID: $captcha_id, pC: clicked_class, rT : 2},
                            success: function () {
                                showSuccess();
                            },
                            error: function() {
                                showError();
                            }
                        });
                    }
                }
                function showSuccess() {
                    $holder.find('.captcha-modal__icons').empty();
                    $holder.addClass('captcha-success');
                    $holder.find('.captcha-modal__icons').html('<div class="captcha-modal__icons-title">' + (($options.captchaMessages.correct && $options.captchaMessages.correct.top) ? $options.captchaMessages.correct.top : 'Great!')
                        + '</div><div class="captcha-modal__icons-subtitle">' + (($options.captchaMessages.correct && $options.captchaMessages.correct.bottom) ? $options.captchaMessages.correct.bottom : 'You do not appear to be a robot.') + '</div>');
                    $holder.trigger("success", [{captcha_id: $captcha_id}]);
                }
                function showError() {
                    $holder.find('.captcha-modal__icons').empty();
                    $holder.addClass('captcha-error');
                    $holder.find('.captcha-modal__icons').html('<div class="captcha-modal__icons-title">' + (($options.captchaMessages.incorrect && $options.captchaMessages.incorrect.top) ? $options.captchaMessages.incorrect.top : 'Oops!')
                        + '</div><div class="captcha-modal__icons-subtitle">' + (($options.captchaMessages.incorrect && $options.captchaMessages.incorrect.bottom) ? $options.captchaMessages.incorrect.bottom : 'You\'ve selected the wrong image.') + '</div>');
                    $holder.trigger('error', [{captcha_id: $captcha_id}]);
                    setTimeout(resetCaptcha, 3000);
                }
                function resetCaptcha() {
                    $holder.removeClass('captcha-error');
                    $holder.find("input[name='captcha-hf']").attr('value', null);

                    $holder.find('.captcha-modal__icons').html([
                        "<div class='captcha-loader'></div>",
                        "<div class='captcha-image'></div>",
                        "<div class='captcha-image'></div>",
                        "<div class='captcha-image'></div>",
                        "<div class='captcha-image'></div>",
                        "<div class='captcha-image'></div>"
                    ].join('\n'));
                    $holder.find('.captcha-modal__icons > .captcha-image').attr('icon-hash', null);
                    buildCaptcha(true);
                    $holder.trigger('refreshed', [{captcha_id: $captcha_id}]);
                }
                function loadImage(image, iconHolder) {
                    var url = image.css('background-image').match(/\((.*?)\)/)[1].replace(/('|")/g,'');
                    var img = new Image();
                    img.onload = function() {
                        images_ready += 1;
                        if(images_ready === 5) {
                            if(iconHolder)
                                removeLoader(iconHolder);
                        }
                    };
                    img.src = url;
                    if (img.complete) img.onload();
                }
                function addLoader(iconHolder) {
                    iconHolder.addClass('captcha-opacity');
                    iconHolder.prepend('<div class="captcha-loader"></div>');
                }
                function removeLoader(iconHolder) {
                    iconHolder.removeClass('captcha-opacity');
                    iconHolder.find('.captcha-loader').remove();
                }
                $holder.on('click', '.captcha-modal__icons > .captcha-image', function(e) {
                    if((new Date() - build_time) <= $options.captchaClickDelay)
                        return;
                    if($options.captchaHoverDetection && !hovering)
                        return;
                    var _x = (e.pageX - $(e.target).offset().left),
                        _y = (e.pageY - $(e.target).offset().top);
                    if(!_x || !_y) return;

                    var $form = $(this);
                    var $icon_holder = $holder.find('.captcha-modal__icons');
                    if($icon_holder.hasClass('captcha-opacity')) return;
                    $holder.trigger('selected', [{captcha_id: $captcha_id}]);
                    if($options.enableLoadingAnimation === true) {
                        addLoader($icon_holder);
                        setTimeout(function() {
                            submitCaptcha($form);
                        }, $options.loadingAnimationDelay);
                    } else {
                        submitCaptcha($form);
                    }
                }).on({
                        mouseenter: function() {
                            if(!hovering)
                                hovering = true
                        },
                        mouseleave: function() {
                            if(hovering)
                                hovering = false
                        }
                    }, $holder
                );
            });
        }
    });
})(jQuery);