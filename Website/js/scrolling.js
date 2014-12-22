function parallax(){
    var scrolled = $(window).scrollTop();
    $('.background').css('top', -(scrolled * 0.1) + 'px');
}

function fadeNav() {
	var scrolled = $(window).scrollTop();
	if (scrolled == 0)
		$('.navigation').fadeIn('slow');
	else
		$('.navigation').fadeOut('slow');
}

function backToTop(){
	$("html, body").animate({ scrollTop: 0 }, 1600, "easeOutQuart");
	return false;
}

function scrollTo(element){
	var pos = $(element).position().top + $(element).height()/2;
	$("html, body").animate({ scrollTop: pos }, 1600, "easeOutQuart");
	return false;
}

$(window).scroll(function(e){
    parallax();
	fadeNav();
});