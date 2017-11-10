+(function($) {
	'use strict';

	if (typeof window.$ === 'undefined' || !window.$)
		throw "This theme need jQuery";

 
	/**
		//////////////////////////////// * Doc ready //////////////////////////////
	*/
	

	$(function() {
		
		/* sidebar toggle */
		$('.sidebar-toggler').on('click', function(){
			$(this).toggleClass('open');
			$('aside').toggleClass('mini');
			$('main').toggleClass('slide');
			$('.logo-toggle').toggleClass('mini');
			$('.title-parent').toggleClass('slide');
		})
		
		// datepicker init
		$('.date').datetimepicker({
			format: 'DD/MM/YYYY'
		});
		$('.date input').click(function(event){
		   $('.date').data("DateTimePicker").show();
		});
	});
	
})(jQuery);
