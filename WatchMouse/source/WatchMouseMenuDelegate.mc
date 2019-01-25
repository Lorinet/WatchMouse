using Toybox.WatchUi;
using Toybox.System;

class WatchMouseMenuDelegate extends WatchUi.MenuInputDelegate {

    function initialize() {
        MenuInputDelegate.initialize();
    }

    function onMenuItem(item) {
        if (item == :item_1) 
        {
            Comms.send("sc");
        } 
        else if(item == :item_2)
        {
        	WatchUi.pushView(new Rez.Menus.KeyShortcutMenu(), new KeyShortcutMenuDelegate(), WatchUi.SLIDE_LEFT);
        }
        else if(item == :item_4)
        {
        	Comms.send("it");
        }
        else if(item == :item_5)
        {
        	Comms.send("am");
        }
        else if (item == :item_6) 
        {
            Comms.send("wm");
        }
        else if(item == :item_7)
        {
        	Comms.send("9s");
        }
        else if(item == :item_8)
        {
        	Comms.send("5s");
        }
        else if(item == :item_9)
        {
        	Comms.send("2s");
        }
        else if(item == :item_10)
        {
        	Comms.send("1s");
        }
        else if(item == :item_11)
        {
        	WatchUi.pushView(new PresentationView(), new PresentationDelegate(), WatchUi.SLIDE_LEFT);
        }
        else if(item == :item_12)
        {
        	WatchUi.pushView(new AboutScreenView(), null, WatchUi.SLIDE_LEFT);
        }
        else if(item == :item_tr)
        {
        	Comms.send("tr");
        	WatchUi.pushView(new TrackpadView(), new TrackpadDelegate(), WatchUi.SLIDE_LEFT);
        }
    }

}