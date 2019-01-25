using Toybox.WatchUi;

class PresentationDelegate extends WatchUi.BehaviorDelegate {

    function initialize() {
        BehaviorDelegate.initialize();
    }

    function onMenu() {
        WatchUi.pushView(new Rez.Menus.MainMenu(), new WatchMouseMenuDelegate(), WatchUi.SLIDE_UP);
        return true;
    }
    function onTap(evt)
    {
    	var coord = evt.getCoordinates();
    	var x = coord[0];
    	if(x < 120) 
    	{
    		Comms.send("bs");
    	}
    	else if(x > 120)
    	{
    		Comms.send("cl");
    	}
    }
    function onSwipe(evt)
    {
    	var dir = evt.getDirection();
    	if(dir == WatchUi.SWIPE_UP)
    	{
    		Comms.send("f5");
    	}
    	else if(dir == WatchUi.SWIPE_DOWN)
    	{
    		Comms.send("es");
    	}
    }

}