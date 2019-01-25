using Toybox.WatchUi;

class WatchMouseDelegate extends WatchUi.BehaviorDelegate {

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
    	if(x > 0 && x < 60 && y > 40 && y < 200)
    	{
    		Comms.send("le");
    	}
    	else if(x > 180 && x < 240 && y > 40 && y < 200)
    	{
    		Comms.send("ri");
    	}
    	else if(x > 60 && x < 180 && y > 0 && y < 60)
    	{
    		Comms.send("up");
    	}
    	else if(x > 60 && x < 180 && y > 180 && y < 240)
    	{
    		Comms.send("do");
    	}
    	else
    	{
    		Comms.send("cl");
    	}
    }

}