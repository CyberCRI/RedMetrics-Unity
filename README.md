# RedMetrics-Unity
Unity 3D client for the RedMetrics open analytics server

## Redmetrics
Redmetrics is a free, open-source analytics tool. Refer to https://github.com/CyberCRI/RedMetrics to learn more.
## Deployment
### Standalone Deployment
The standalone version will send events to Redmetrics natively using the Unity3d WWW class. Just check your internet connection.
### Web Player Deployment
The following steps are necessary to deploy your project on the internet:
- get bower if you don't already have: https://github.com/bower/bower.
- install Redmetrics.js using bower in any directory, for instance by typing
```bower install Redmetrics.js``` in the Mac OS Terminal. It will create a folder ```bower_components```.
- copy the bower folder onto your server, so that your html on which your game will be displayed can access it.
- reference the bower folders to your html. In this example, the bower folder is in the same folder as the html file.
```html
<script type="text/javascript" src="bower_components/q/q.js"></script>
<script type="text/javascript" src="bower_components/q-xhr/q-xhr.js"></script>
<script type="text/javascript" src="bower_components/underscore/underscore.js"></script>
<script type="text/javascript" src="bower_components/RedMetrics.js/redmetrics.js"></script>
<script type="text/javascript" src="bower_components/RedMetrics.js/redmetrics-unity.js"></script>
```
- add this to allow debugging from the game into the browser
```html
<script type="text/javascript">
function DebugFromWebPlayerToBrowser(msg) {
  console.log(msg);
}
</script>
```