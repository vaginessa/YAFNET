/*
 * CeeBox 2.1.4 jQuery Plugin (minimized version)
 * Requires jQuery 1.3.2 and swfobject.jquery.js plugin to work
 * Code hosted on GitHub (http://github.com/catcubed/ceebox) Please visit there for version history information
 * By Colin Fahrion (http://www.catcubed.com)
 * Inspiration for ceebox comes from Thickbox (http://jquery.com/demo/thickbox/) and Videobox (http://videobox-lb.sourceforge.net/)
 * However, along the upgrade path ceebox has morphed a long way from those roots.
 * Copyright (c) 2009 Colin Fahrion
 * Licensed under the MIT License: http://www.opensource.org/licenses/mit-license.php
*/

(function (b) {
    function v(c, a, d) {
        l.vidRegex = function () {
            var f = "";
            b.each(b.fn.ceebox.videos, function (e, g) {
                if (g.siteRgx !== null && typeof g.siteRgx !== "string") {
                    e = String(g.siteRgx);
                    f = f + e.slice(1, e.length - 2) + "|";
                }
            });
            return new RegExp(f + "\\.swf$", "i");
        }();
        l.userAgent = navigator.userAgent;
		b(".cee_close").off()
		b("body").on("click",".cee_close", function () {
            b.fn.ceebox.closebox();
            return false;
        });
		
        d != false && b(c).each(function (f) {
            B(this, f, a, d);
        });
        b(c).on("click",null, function (f) {
			
            var e = b(f.target).closest("[href]"),
                g = e.data("ceebox");
            if (g) {
                var h = g.opts ? b.extend({}, a, g.opts) : a;
                b.fn.ceebox.overlay(h);
                if (g.type == "image") {
                    var i = new Image;
                    i.onload = function () {
                        var m = i.width,
                            j = i.height;
                        h.imageWidth = s(m, b.fn.ceebox.defaults.imageWidth);
                        h.imageHeight = s(j, b.fn.ceebox.defaults.imageHeight);
                        h.imageRatio = m / j;
                        b.fn.ceebox.popup(e, b.extend(h, {
                            type: g.type
                        }, {
                            gallery: g.gallery
                        }));
                    };
                    i.src = b(e).attr("href");
                } else b.fn.ceebox.popup(e, b.extend(h, {
                    type: g.type
                }, {
                    gallery: g.gallery
                }));
				
                return false;
            }
        });
    }
    function w(c) {
        var a = document.documentElement;
        c = c || 100;
        this.width = (window.innerWidth || self.innerWidth || a && a.clientWidth || document.body.clientWidth) - c;
        this.height = (window.innerHeight || self.innerHeight || a && a.clientHeight || document.body.clientHeight) - c;
        return this;
    }
    function y(c) {
        var a = "fixed",
            d = 0,
            f = z(c.borderWidth, /[0-9]+/g);
        if (!window.XMLHttpRequest) {
            b("#cee_HideSelect") === null && b("body").append("<iframe id='cee_HideSelect'></iframe>");
            a = "absolute";
            d = parseInt(document.documentElement && document.documentElement.scrollTop || document.body.scrollTop, 10);
        }
        this.mleft = parseInt(-1 * (c.width / 2 + Number(f[3])),
        10);
        this.mtop = parseInt(-1 * (c.height / 2 + Number(f[0])), 10) + d;
        this.position = a;
        return this;
    }
    function z(c, a) {
        c = c.match(a);
        a = [];
        var d = c.length;
        if (d > 1) {
            a[0] = c[0];
            a[1] = c[1];
            a[2] = d == 2 ? c[0] : c[2];
            a[3] = d == 4 ? c[3] : c[1];
        } else a = [c, c, c, c];
        return a;
    }
    function C() {
        document.onkeydown = function (c) {
            c = c || window.event;
            switch (c.keyCode || c.which) {
            case 13:
                return false;
            case 27:
                b.fn.ceebox.closebox();
                document.onkeydown = null;
                break;
            case 188:
            case 37:
                b("#cee_prev").trigger("click");
                break;
            case 190:
            case 39:
                b("#cee_next").trigger("click");
                break;
            default:
                break;
            }
            return true;
        };
    }
    function D(c, a, d) {
        function f(m, j) {
            var k, o = i[d.type].bgtop,
                p = o - 2E3;
            m == "prev" ? (k = [{
                left: 0
            }, "left"]) : (k = [{
                right: 0
            },
            x = "right"]);
            var n = function (q) {
                return b.extend({
                    zIndex: 105,
                    width: i[d.type].w + "px",
                    height: i[d.type].h + "px",
                    position: "absolute",
                    top: i[d.type].top,
                    backgroundPosition: k[1] + " " + q + "px"
                }, k[0]);
            };
            b("<a href='#'></a>").text(m).attr({
                id: "cee_" + m
            }).css(n(p)).hover(function () {
                b(this).css(n(o));
            }, function () {
                b(this).css(n(p));
            }).one("click", function (q) {
                q.preventDefault();
                (function (E, F, G) {
                    b("#cee_prev,#cee_next").unbind().click(function () {
                        return false;
                    });
                    document.onkeydown = null;
                    var u = b("#cee_box").children(),
                        H = u.length;
                    u.fadeOut(G, function () {
                        b(this).remove();
                        this == u[H - 1] && E.eq(F).trigger("click");
                    });
                })(a, j, d.fadeOut);
            }).appendTo("#cee_box");
        }
        var e = d.height,
            g = d.titleHeight,
            h = d.padding,
            i = {
                image: {
                    w: parseInt(d.width / 2, 10),
                    h: e - g - 2 * h,
                    top: h,
                    bgtop: (e - g - 2 * h) / 2
                },
                video: {
                    w: 60,
                    h: 80,
                    top: parseInt((e - g - 10 - 2 * h) / 2, 10),
                    bgtop: 24
                }
            };
        i.html = i.video;
        c.prevId >= 0 && f("prev", c.prevId);
        c.nextId && f("next",
        c.nextId);
        b("#cee_title").append("<div id='cee_count'>Item " + (c.gNum + 1) + " of " + c.gLen + "</div>");
    }
    function s(c, a) {
        return c && c < a || !a ? c : a;
    }
    function t(c) {
        return typeof c == "function";
    }
    function r(c) {
        var a = c.length;
        return a > 1 ? c[a - 1] : c;
    }
    b.ceebox = {
        version: "2.1.5"
    };
    b.fn.ceebox = function (c) {
        c = b.extend({
            selector: b(this).selector
        }, b.fn.ceebox.defaults, c);
        var a = this,
            d = b(this).selector;
        c.videoJSON ? b.getJSON(c.videoJSON, function (f) {
            b.extend(b.fn.ceebox.videos, f);
            v(a, c, d);
        }) : v(a, c, d);
        return this;
    };
    b.fn.ceebox.defaults = {
        html: true,
        image: true,
        video: true,
        modal: false,
        titles: true,
        htmlGallery: true,
        imageGallery: true,
        videoGallery: true,
        videoWidth: false,
        videoHeight: false,
        videoRatio: "16:9",
        htmlWidth: false,
        htmlHeight: false,
        htmlRatio: false,
        imageWidth: false,
        imageHeight: false,
        animSpeed: "normal",
        easing: "swing",
        fadeOut: 400,
        fadeIn: 400,
        overlayColor: "#000",
        overlayOpacity: 0.8,
        boxColor: "",
        textColor: "",
        borderColor: "",
        borderWidth: "3px",
        padding: 15,
        margin: 150,
        onload: null,
        unload: null,
        videoJSON: null,
        iPhoneRedirect: true
    };
    b.fn.ceebox.ratios = {
        "4:3": 1.333,
        "3:2": 1.5,
        "16:9": 1.778,
        "1:1": 1,
        square: 1
    };
    b.fn.ceebox.relMatch = {
        width: /(?:width:)([0-9]+)/i,
        height: /(?:height:)([0-9]+)/i,
        ratio: /(?:ratio:)([0-9\.:]+)/i,
        modal: /modal:true/i,
        nonmodal: /modal:false/i,
        videoSrc: /(?:videoSrc:)(http:[\/\-\._0-9a-zA-Z:]+)/i,
        videoId: /(?:videoId:)([\-\._0-9a-zA-Z:]+)/i
    };
    b.fn.ceebox.loader = "<div id='cee_load' style='z-index:99999;top:50%;left:50%;position:fixed'></div>";
    b.fn.ceebox.videos = {
        base: {
            param: {
                wmode: "transparent",
                allowFullScreen: "true",
                allowScriptAccess: "always"
            },
            flashvars: {
                autoplay: true
            }
        },
        facebook: {
            siteRgx: /facebook\.com\/video/i,
            idRgx: /(?:v=)([a-zA-Z0-9_]+)/i,
            src: "http://www.facebook.com/v/[id]"
        },
        youtube: {
            siteRgx: /youtube\.com\/watch/i,
            idRgx: /(?:v=)([a-zA-Z0-9_\-]+)/i,
            src: "http://www.youtube.com/v/[id]&hl=en&fs=1&autoplay=1"
        },
        metacafe: {
            siteRgx: /metacafe\.com\/watch/i,
            idRgx: /(?:watch\/)([a-zA-Z0-9_]+)/i,
            src: "http://www.metacafe.com/fplayer/[id]/.swf"
        },
        google: {
            siteRgx: /google\.com\/videoplay/i,
            idRgx: /(?:id=)([a-zA-Z0-9_\-]+)/i,
            src: "http://video.google.com/googleplayer.swf?docId=[id]&hl=en&fs=true",
            flashvars: {
                playerMode: "normal",
                fs: true
            }
        },
        spike: {
            siteRgx: /spike\.com\/video|ifilm\.com\/video/i,
            idRgx: /(?:\/)([0-9]+)/i,
            src: "http://www.spike.com/efp",
            flashvars: {
                flvbaseclip: "[id]"
            }
        },
        vimeo: {
            siteRgx: /vimeo\.com\/[0-9]+/i,
            idRgx: /(?:\.com\/)([a-zA-Z0-9_]+)/i,
            src: "http://www.vimeo.com/moogaloop.swf?clip_id=[id]&server=vimeo.com&show_title=1&show_byline=1&show_portrait=0&color=&fullscreen=1"
        },
        dailymotion: {
            siteRgx: /dailymotion\.com\/video/i,
            idRgx: /(?:video\/)([a-zA-Z0-9_]+)/i,
            src: "http://www.dailymotion.com/swf/[id]&related=0&autoplay=1"
        },
        cnn: {
            siteRgx: /cnn\.com\/video/i,
            idRgx: /(?:\?\/video\/)([a-zA-Z0-9_\/\.]+)/i,
            src: "http://i.cdn.turner.com/cnn/.element/apps/cvp/3.0/swf/cnn_416x234_embed.swf?context=embed&videoId=[id]",
            width: 416,
            height: 374
        }
    };
    b.fn.ceebox.overlay = function (c) {
        c = b.extend({
            width: 60,
            height: 30,
            type: "html"
        }, b.fn.ceebox.defaults, c);
        b("#cee_overlay").size() === 0 && b("<div id='cee_overlay'></div>").css({
            opacity: c.overlayOpacity,
            position: "absolute",
            top: 0,
            left: 0,
            backgroundColor: c.overlayColor,
            width: "100%",
            height: b(document).height(),
            zIndex: 9995
        }).appendTo(b("body"));
        if (b("#cee_box").size() === 0) {
            var a = y(c);
            a = {
                position: a.position,
                zIndex: 9999,
                top: "50%",
                left: "50%",
                height: c.height + "px",
                width: c.width + "px",
                marginLeft: a.mleft + "px",
                marginTop: a.mtop + "px",
                opacity: 0,
                borderWidth: c.borderWidth,
                borderColor: c.borderColor,
                backgroundColor: c.boxColor,
                color: c.textColor
            };
            b("<div id='cee_box'></div>").css(a).appendTo("body").animate({
                opacity: 1
            }, c.animSpeed, function () {
                b("#cee_overlay").addClass("cee_close");
            });
        }
        b("#cee_box").removeClass().addClass("cee_" + c.type);
        b("#cee_load").size() === 0 && b(b.fn.ceebox.loader).appendTo("body");
        b("#cee_load").show("fast").animate({
            opacity: 1
        }, "fast");
    };

    b.fn.ceebox.popup = function (c, a) {
        var d = w(a.margin);
        a = b.extend({
            width: d.width,
            height: d.height,
            modal: false,
            type: "html",
            onload: null
        }, b.fn.ceebox.defaults, a);
        var f;
        if (b(c).is("a,area,input") && (a.type == "html" || a.type == "image" || a.type == "video")) {
            if (a.gallery) f = b(a.selector).eq(a.gallery.parentId).find("a[href],area[href],input[href]");
            A[a.type].prototype = new I(c, a);
            d = new A[a.type];
            c = d.content;
            a.action = d.action;
            a.modal = d.modal;
            if (a.titles) {
                a.titleHeight = b(d.titlebox).contents().contents().wrap("<div></div>").parent().attr("id", "ceetitletest").css({
                    position: "absolute",
                    top: "-300px",
                    width: d.width + "px"
                }).appendTo("body").height();
                b("#ceetitletest").remove();
                a.titleHeight = a.titleHeight >= 10 ? a.titleHeight + 20 : 30;
            } else a.titleHeight = 0;
            a.width = d.width + 2 * a.padding;
            a.height = d.height + a.titleHeight + 2 * a.padding;
        }
        b.fn.ceebox.overlay(a);
        l.action = a.action;
        l.onload = a.onload;
        l.unload = a.unload;
        d = y(a);
        d = {
            marginLeft: d.mleft,
            marginTop: d.mtop,
            width: a.width + "px",
            height: a.height + "px",
            borderWidth: a.borderWidth
        };
        if (a.borderColor) {
            var e = z(a.borderColor, /#[1-90a-f]+/gi);
            d = b.extend(d, {
                borderTopColor: e[0],
                borderRightColor: e[1],
                borderBottomColor: e[2],
                borderLeftColor: e[3]
            });
        }
        d = a.textColor ? b.extend(d, {
            color: a.textColor
        }) : d;
        d = a.boxColor ? b.extend(d, {
            backgroundColor: a.boxColor
        }) : d;
        b("#cee_box").animate(d, a.animSpeed, a.easing, function () {
            var g = b(this).append(c).children().hide(),
                h = g.length,
                i = true;
            g.fadeIn(a.fadeIn,

            function () {
                if (b(this).is("#cee_iframeContent")) i = false;
                i && this == g[h - 1] && b.fn.ceebox.onload();
            });
            if (a.modal === true) b("#cee_overlay").removeClass("cee_close");
            else {
                b("<a href='#' id='cee_closeBtn' class='cee_close' title='Close'>close</a>").prependTo("#cee_box");
                a.gallery && D(a.gallery, f, a);
                C(void 0, f, a.fadeOut);
            }
        });
    };
    b.fn.ceebox.closebox = function (c, a) {
        c = c || 400;
        b("#cee_box").fadeOut(c);
        b("#cee_overlay").fadeOut(typeof c == "number" ? c * 2 : "slow", function () {
            b("#cee_box,#cee_overlay,#cee_HideSelect,#cee_load").unbind().trigger("unload").remove();
            if (t(a)) a();
            else t(l.unload) && l.unload();
            l.unload = null;
        });
        document.onkeydown = null;
    };
    b.fn.ceebox.onload = function () {
        b("#cee_load").hide(300).fadeOut(600, function () {
            b(this).remove();
        });
        if (t(l.action)) {
            l.action();
            l.action = null;
        }
        if (t(l.onload)) {
            l.onload();
            l.onload = null;
        }
    };
    var l = {}, B = function (c, a, d) {
        var f, e = [],
            g = [],
            h = 0;
        b(c).is("[href]") ? (f = b(c)) : (f = b(c).find("[href]"));
        var i = {
            image: function (j, k) {
                return k && k.match(/\bimage\b/i) ? true : j.match(/resource\.ashx|\.jpg$|\.jpeg$|\.png$|\.gif$|\.bmp$/i) || false;
            },
            video: function (j, k) {
                return k && k.match(/\bvideo\b/i) ? true : j.match(l.vidRegex) || false;
            },
            html: function () {
                return true;
            }
        };
        f.each(function (j) {
            var k = this,
                o = b.metadata ? b(k).metadata() : false,
                p = o ? b.extend({}, d, o) : d;
            b.each(i, function (n) {
                if (i[n](b(k).attr("href"), b(k).attr("rel")) && p[n]) {
                    var q = false;
                    if (p[n + "Gallery"] === true) {
                        g[g.length] = j;
                        q = true;
                    }
                    e[e.length] = {
                        linkObj: k,
                        type: n,
                        gallery: q,
                        linkOpts: p
                    };
                    return false;
                }
                return false;
            });
        });
        var m = g.length;
        b.each(e, function (j) {
            if (e[j].gallery) {
                var k = {
                    parentId: a,
                    gNum: h,
                    gLen: m
                };
                if (h > 0) k.prevId = g[h - 1];
                if (h < m - 1) k.nextId = g[h + 1];
                h++;
            }!b.support.opacity && b(c).is("map") && b(e[j].linkObj).click(function (o) {
                o.preventDefault();
            });
            b.data(e[j].linkObj, "ceebox", {
                type: e[j].type,
                opts: e[j].linkOpts,
                gallery: k
            });
        });
    }, I = function (c, a) {
        var d = a[a.type + "Width"],
            f = a[a.type + "Height"],
            e = a[a.type + "Ratio"] || d / f,
            g = b(c).attr("rel");
        if (g && g !== "") {
            var h = {};
            b.each(b.fn.ceebox.relMatch, function (m, j) {
                h[m] = j.exec(g);
            });
            if (h.modal) a.modal = true;
            if (h.nonmodal) a.modal = false;
            if (h.width) d = Number(r(h.width));
            if (h.height) f = Number(r(h.height));
            if (h.ratio) {
                e = r(h.ratio);
                e = Number(e) ? Number(e) : String(e);
            }
            if (h.videoSrc) this.videoSrc = String(r(h.videoSrc));
            if (h.videoId) this.videoId = String(r(h.videoId));
        }
        var i = w(a.margin);
        d = s(d, i.width);
        f = s(f, i.height);
        if (e) {
            Number(e) || (e = b.fn.ceebox.ratios[e] ? Number(b.fn.ceebox.ratios[e]) : 1);
            if (d / f > e) d = parseInt(f * e, 10);
            if (d / f < e) f = parseInt(d / e, 10);
        }
        this.modal = a.modal;
        this.href = b(c).attr("href");
        this.downloadhref = b(c).attr("date-img");
        this.title = b(c).attr("title") || c.t || "";
        this.downloadbox = this.downloadhref ? "<a style='padding-left:5px' href='" + this.downloadhref + "'>Download Image</a>" : "";
        this.titlebox = a.titles ? "<div id='cee_title'><h2>" + this.title + "</h2>" + this.downloadbox + "</div>" : "";
        this.width = d;
        this.height = f;
        this.rel = g;
        this.iPhoneRedirect = a.iPhoneRedirect;
    }, A = {
        image: function () {
            this.content = "<img id='cee_img' src='" + this.href + "' width='" + this.width + "' height='" + this.height + "' alt='" + this.title + "'/>" + this.titlebox;
        },
        video: function () {
            var c = "",
                a = this,
                d = function () {
                    var e = this,
                        g = a.videoId;
                    e.flashvars = e.param = {};
                    e.src = a.videoSrc || a.href;
                    e.width = a.width;
                    e.height = a.height;
                    b.each(b.fn.ceebox.videos, function (h, i) {
                        if (i.siteRgx && typeof i.siteRgx != "string" && i.siteRgx.test(a.href)) {
                            if (i.idRgx) {
                                i.idRgx = new RegExp(i.idRgx);
                                g = String(r(i.idRgx.exec(a.href)));
                            }
                            e.src = i.src ? i.src.replace("[id]", g) : e.src;
                            i.flashvars && b.each(i.flashvars, function (m, j) {
                                if (typeof j == "string") e.flashvars[m] = j.replace("[id]", g);
                            });
                            i.param && b.each(i.param, function (m, j) {
                                if (typeof j == "string") e.param[m] = j.replace("[id]", g);
                            });
                            e.width = i.width || e.width;
                            e.height = i.height || e.height;
                            e.site = h;
                        }
                    });
                    return e;
                }();
            if (b.flash.hasVersion(8)) {
                this.width = d.width;
                this.height = d.height;
                this.action = function () {
                    b("#cee_vid").flash({
                        swf: d.src,
                        params: b.extend(b.fn.ceebox.videos.base.param, d.param),
                        flashvars: b.extend(b.fn.ceebox.videos.base.flashvars,
                        d.flashvars),
                        width: d.width,
                        height: d.height
                    });
                };
            } else {
                this.width = 400;
                this.height = 200;
                if (l.userAgent.match(/iPhone/i) && this.iPhoneRedirect || l.userAgent.match(/iPod/i) && this.iPhoneRedirect) {
                    var f = this.href;
                    this.action = function () {
                        b.fn.ceebox.closebox(400, function () {
                            window.location = f;
                        });
                    };
                } else {
                    d.site = d.site || "SWF file";
                    c = "<p style='margin:20px'>Adobe Flash 8 or higher is required to view this movie. You can either:</p><ul><li>Follow link to <a href='" + this.href + "'>" + d.site + " </a></li><li>or <a href='http://www.adobe.com/products/flashplayer/'>Install Flash</a></li><li> or <a href='#' class='cee_close'>Close This Popup</a></li></ul>";
                }
            }
            this.content = "<div id='cee_vid' style='width:" + this.width + "px;height:" + this.height + "px;'>" + c + "</div>" + this.titlebox;
        },
        html: function () {
            var c = this.href,
                a = this.rel;
            a = [c.match(/[a-zA-Z0-9_\.]+\.[a-zA-Z]{2,4}/i), c.match(/^http:+/), a ? a.match(/^iframe/) : false];
            if (document.domain == a[0] && a[1] && !a[2] || !a[1] && !a[2]) {
                var d, f = (d = c.match(/#[a-zA-Z0-9_\-]+/)) ? String(c.split("#")[0] + " " + d) : c;
                this.action = function () {
                    b("#cee_ajax").load(f);
                };
                this.content = this.titlebox + "<div id='cee_ajax' style='width:" + (this.width - 30) + "px;height:" + (this.height - 20) + "px'></div>";
            } else {
                b("#cee_iframe").remove();
                this.content = this.titlebox + "<iframe frameborder='0' hspace='0' src='" + c + "' id='cee_iframeContent' name='cee_iframeContent" + Math.round(Math.random() * 1E3) + "' onload='jQuery.fn.ceebox.onload()' style='width:" + this.width + "px;height:" + this.height + "px;' > </iframe>";
            }
        }
    };
})(jQuery);


(function ($) {
    $.extend({
        metadata: {
            defaults: {
                type: 'class',
                name: 'metadata',
                cre: /({.*})/,
                single: 'metadata'
            },
            setType: function (type, name) {
                this.defaults.type = type;
                this.defaults.name = name;
            },
            get: function (elem, opts) {
                var settings = $.extend({}, this.defaults, opts);
                if (!settings.single.length) settings.single = 'metadata'; {
                    var data = $.data(elem, settings.single);
                }
                if (data) {
                    return data;
                }
                data = "{}";
                var getData = function (data) {
                    if (typeof data != "string") {
                        return data;
                    }
                    if (data.indexOf('{') < 0) {
                        data = eval("(" + data + ")");
                    }
                };
                var getObject = function (data) {
                    if (typeof data != "string") {
                        return data;
                    }
                    data = eval("(" + data + ")");
                    return data;
                };
                if (settings.type == "html5") {
                    var object = {};
                    $(elem.attributes).each(function () {
                        var name = this.nodeName;
                        if (name.match(/^data-/)) {
                            name = name.replace(/^data-/, '');
                        } else {
                            return true;
                        }
                        object[name] = getObject(this.nodeValue);
                    });
                } else {
                    if (settings.type == "class") {
                        var m = settings.cre.exec(elem.className);
                        if (m) {
                            data = m[1];
                        }
                    } else if (settings.type == "elem") {
                        if (!elem.getElementsByTagName) {
                            return;
                        }
                        var e = elem.getElementsByTagName(settings.name);
                        if (e.length) {
                            data = $.trim(e[0].innerHTML);
                        }
                    } else if (elem.getAttribute != undefined) {
                        var attr = elem.getAttribute(settings.name);
                        if (attr) {
                            data = attr;
                        }
                    }
                    object = getObject(data.indexOf("{") < 0 ? "{" + data + "}" : data);
                }
                $.data(elem, settings.single, object);
                return object;
            }
        }
    });
    $.fn.metadata = function (opts) {
        return $.metadata.get(this[0], opts);
    };
})(jQuery);