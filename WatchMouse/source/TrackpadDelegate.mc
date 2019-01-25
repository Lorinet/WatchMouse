using Toybox.WatchUi;

class TrackpadDelegate extends WatchUi.BehaviorDelegate {

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
    	var y = coord[1];
    	Comms.send(x + " " + y);
    }
    function onSwipe(evt)
    {
    	var dir = evt.getDirection();
    	if(dir == WatchUi.SWIPE_LEFT)
    	{
    		Comms.send("cl");
    	}
    	else if(dir == WatchUi.SWIPE_UP)
    	{
    		Comms.send("sc");
    	}
    }
    function onHide()
    {
    	Comms.send("tr");
    }
}