function test()
{
	var x = new XmlHttpRequest();
	
	// single-line comment
	
	var s = "most escapes just eaten\n";
	var t = "escaped quote works: \"; see, still in string\"";
	
	/* multi-line comment,
		/* embedded comment, does the right thing 
		**/
	
	var q = "/*this should not be a comment!*/";
	var r = "//neither should this"; //but this is.
	
	var y = function(x) { return x; }
	
	var elems = document.getElementById( "div" );
	
	var breaks = "foo" + "bar";
	
	var heresABug = "one\\\\"; /*comment comment */
}

