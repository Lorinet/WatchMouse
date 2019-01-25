using Toybox.WatchUi;
using Toybox.System;

class KeyShortcutMenuDelegate extends WatchUi.MenuInputDelegate {

    function initialize() {
        MenuInputDelegate.initialize();
    }

    function onMenuItem(item) {
        if(item == :item_enter)
        {
        	Comms.send("en");
        }
        else if(item == :item_esc)
        {
        	Comms.send("es");
        }
        else if(item == :item_spa)
        {
        	Comms.send("sp");
        }
        else if(item == :item_bsp)
        {
        	Comms.send("bs");
        }
        else if(item == :item_f5)
        {
        	Comms.send("f5");
        }
        else if(item == :item_f11)
        {
        	Comms.send("f11");
        }
        else if(item == :it_cust1)
        {
        	Comms.send("c1");
        }
        else if(item == :it_cust2)
        {
        	Comms.send("c2");
        }
        else if(item == :it_cust3)
        {
        	Comms.send("c3");
        }
        else if(item == :it_cust4)
        {
        	Comms.send("c4");
        }
    }

}