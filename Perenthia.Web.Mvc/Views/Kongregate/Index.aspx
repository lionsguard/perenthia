<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html lang="en" xml:lang="en" xmlns="http://www.w3.org/1999/xhtml">
  <title>Perenthia</title>
  <head>
    <!-- Load the Kongregate Javascript API -->
    <script language="javascript" type="text/javascript" src="http://www.kongregate.com/javascripts/kongregate_api.js"></script>
    
    <!-- Give the shell no border/scroll bars and match the Kongregate background color.
         If your game needs scrollbars, you might need to modify these styles -->
    <style>
      html{border: none; overflow: hidden; background-color: #333;height: 100%;}
      body{border: none; background-color: #333;margin:0; padding:0;}
    </style>
  </head>
  
  <body>
    <script language="javascript" type="text/javascript">
      // Called when the API is finished loading
      function onLoadCompleted(){
        // Get a global reference to the kongregate API
        kongregate = kongregateAPI.getAPI();
        
        // Embed the game into the "contentdiv" div, which is defined below. You can also
        // manually create your own iframe, this function is just for convenience.
        // kongregateAPI.embedFrame("game.html");
        alert("kongregate loaded");
      }
      
      // Begin the process of loading the Kongregate API:
      kongregateAPI.loadAPI(onLoadCompleted);
    </script>
    
    <!-- The div that the game will be placed into. Make sure to set the Width and height properly -->
    <div id="contentdiv" style="top:0px; left:0px; width:700px; height:500px; border:none;">
      <!-- You can manually put your game iframe in here instead of calling embedFrame above if you wish -->
      <iframe id="gameFrame" src="/kongregate/game" frameborder="0" scrolling="no" width="700" height="500" marginwidth="0" marginheight="0" title="Perenthia Game Frame" />
    </div>
  </body>
</html>