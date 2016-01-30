# RedMetrics-Unity
Unity 3D client for the RedMetrics open analytics server

## Redmetrics
Redmetrics is a free, open-source analytics tool. Go to https://github.com/CyberCRI/RedMetrics to learn more.

## Deployment

### Standalone Deployment

The standalone version will send events to Redmetrics natively using the Unity3d WWW class. Just check your internet connection.

### Web Player Deployment

The following steps are necessary to deploy your project on the internet:

1. Get bower if you don't already have it: https://github.com/bower/bower.
2. Install Redmetrics.js using bower in any directory, for instance by opening a terminal window, going to the directory where you installed RedMetrics-Unity, and typing```bower install Redmetrics.js```. Bower will create a folder ```bower_components``` where it installs the dependencies.
3. Copy the ```bower_components``` directory onto your server, so that your HTML which displays your game can access it.
4. Reference the ```bower_components``` directory in your HTML file. In this example, the ```bower_components``` directory is within the directory that contains the HTML file.

  ```html
  <script type="text/javascript" src="bower_components/q/q.js"></script>
  <script type="text/javascript" src="bower_components/q-xhr/q-xhr.js"></script>
  <script type="text/javascript" src="bower_components/underscore/underscore.js"></script>
  <script type="text/javascript" src="bower_components/RedMetrics.js/redmetrics.js"></script>
  <script type="text/javascript" src="bower_components/RedMetrics.js/redmetrics-unity.js"></script>
  ```

5. Add the following to the HTML file to allow debugging information to show in the browser JavaScript console.

  ```html
  <script type="text/javascript">
  function DebugFromWebPlayerToBrowser(msg) {
    console.log(msg);
  }
  </script>
  ```

## Testing the example scene

The example scene just contains a bouncing cube that you can push up by pressing the space bar. An event is sent to the Redmetrics server at the start of the game, when the cube bounces off, and when the cube is pushed by the user.

To see the logs, check the page redmetrics.io. On the left panel, choose ```Redmetrics-Unity``` as game, then ```test``` as the version.

If you want to use your own game version ID, statically edit the ```defaultGameVersion``` field in the ```RedmetricsManager.cs``` file, or dynamically use the ```setGameVersion``` method.

## Use in your own game

1. Download or clone the RedMetrics-Unity repository.
2. Import `Redmetrics.unitypackage` (Assets | Import Package | Custom Package).
3. Create an empty game object (GameObject | Create Empty) called `RedMetricsManager`. Add the `Red Metrics Manager` script as a component.
4. To create a trigger that sends an event, create a new script and put in the example code below. Attach this script to a game object that has a collider component that is a trigger.

```c#
	void OnTriggerEnter(Collider other) {
		RedMetricsManager.get ().sendEvent(TrackingEvent.START);
	}
```

6. To create new types of events, add or modify the list in `TrackingEvent.cs`
7. Profit!
