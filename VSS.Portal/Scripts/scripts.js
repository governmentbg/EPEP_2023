document.addEventListener('DOMContentLoaded', function() {
	
	//check if user agent is mobile device
	var isMobile = {
	    Android: function() {
	        return navigator.userAgent.match(/Android/i);
	    },
	    BlackBerry: function() {
	        return navigator.userAgent.match(/BlackBerry/i);
	    },
	    iOS: function() {
	        return navigator.userAgent.match(/iPhone|iPad|iPod/i);
	    },
	    Opera: function() {
	        return navigator.userAgent.match(/Opera Mini/i);
	    },
	    Windows: function() {
	        return navigator.userAgent.match(/IEMobile/i);
	    },
	    any: function() {
	        return (isMobile.Android() || isMobile.BlackBerry() || isMobile.iOS() || isMobile.Opera() || isMobile.Windows());
	    }
	};
	
	
	//GO TO TOP::
	var viewport = $('html, body');
	//animate to top:
	$('.gototop').click(function(){
    	 viewport.animate({scrollTop : 0}, 1400, 'easeOutQuint');
    });
    //stop animation on scroll
    viewport.bind("scroll mousedown DOMMouseScroll mousewheel keyup", function(e){
	     viewport.stop();
	});   
	
	
	//Select2
	if($(".map-search .select2").length > 0){
		$(".map-search .select2").select2({
			dropdownCssClass: "home-map"
		});
	}
	else if($(".select2").length > 0)
		$(".select2").select2();

	$("h3.section-heading, h4.section-heading").click(function() {
        section = $(this).data("section");
        $(this).hasClass("opened") ? $("section[data-section=" + section + "]").slideUp(300) : $("section[data-section=" + section + "]").slideDown(300);
        $(this).toggleClass("opened")
    });
	
	//HOME COURTS functions
	if($(".home-courts").length > 0){
		$(document).ready(function(){
			var wrapper = $(window).height() - 13 - 13;
			var offset = $('.home-courts').offset().top - 13;
			var map_container = wrapper - 40 - 40;
			$(".map-container").css("height", map_container+"px");
			$(".home-courts").css("height", wrapper/3+"px").attr("init-height", wrapper/3+"px");
			
			//SHOW MAP:
			$('.map-search-init button').click(function(){
				$(".map-search-init button").fadeOut(300, function(){
					$('html, body').animate({scrollTop : offset}, 1400, 'easeOutQuint');
					$(".home-courts").addClass('transition').css("height", wrapper+"px");
					if( isMobile.any() ) $(".map-search-init").hide(); else $(".map-search-init").fadeOut(900, function(){ 
						//$(".home-courts .select2").select2("open");
					});
					//if( !isMobile.any() ) $(".home-courts .select2").select2("open");
				});
			});
			
			//HIDE MAP:
			$(".map-container .close_map").click(function(){
				if( isMobile.any() ){
					$('html, body').animate({scrollTop : 0}, 1400, 'easeOutQuint');
					$(".map-search-init").show();
					$(".home-courts").css("height", $(".home-courts").attr('init-height'));
		  			$(".map-search-init button").show();
				}
				else{
					$('html, body').animate({scrollTop : 0}, 1400, 'easeOutQuint');
					$(".map-search-init").fadeIn(600, function(){
						$(".home-courts").css("height", $(".home-courts").attr('init-height'));
		  				$(".map-search-init button").fadeIn(150);
					});
				}
			});
			
			$("select[name=marker]").change(function(){
		    	$("select[name=marker] option[value=0]").remove();
			});
		});
	}


});