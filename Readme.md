# OpenTAP Home Automation
Today is side project friday... So let's create a home automation system using OpenTAP.

### Prerequisites
- LIFX Light Bulbs
- OpenTAP

## Create a LIFX API
LIFX provides an easy to use HTTP WEB API with great [documentation](https://api.developer.lifx.com/docs).


### Authentication and Setup
From this documentation we can see we need to get a authentication token and use it when calling the API.

With C# it's pretty easy to setup a `HttpClient` with that token attached.

```cs
private const string TOKEN = "xxxxxxxxxxxxx";
private static HttpClient client;
static LifxApi()
{
    client = new HttpClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TOKEN);
}
```

This HttpClient can now be used to send request to the LIFX HTTP WEB API. We will use this to create method to control our lights.

### Find our Available Lights
Before we can control our lights, we need to find them. LIFX has a simple endpoint to list all lights:

```cs
public static List<LifxLight> GetLights()
{
    var data = client.GetStringAsync("https://api.lifx.com/v1/lights/all").Result;
    return JsonConvert.DeserializeObject<List<LifxLight>>(data);
}
```

### Control the Lights
Now with a list of our lights, we can create methods that allow us to control them.

To turn on a light we need to provide the id of the light that need to change `/id:<the light id>/`. 

We also need to set which properties of the light need to change, in our case we want to change `power` to `off`. We can do this with form url encoded data (`x-www-form-urlencoded`).

```cs
public static void TurnOn(LifxLight light)
{
    var form = new List<KeyValuePair<string, string>>();
    form.Add(new KeyValuePair<string, string>("power", "on"));
    var content = new FormUrlEncodedContent(form);
    client.PutAsync($"https://api.lifx.com/v1/lights/id:{light.id}/state", content).Wait(1000);
}
```


### OpenTAP LIFX Settings
Since we now have a working API to control our LIFX light bulbs, we need a way to map the to OpenTAP.

We can create `ComponentSetting` that can store the details of the lights.

```cs
public class LifxSettings : ComponentSettings<LifxSettings>
{
    public List<LifxLight> Lights { get; set; } = new List<LifxLight>();


    [Display("Search for Devices")]
    [Browsable(true)]
    public void ScanForDevices()
    ...
}
```

We can also create method that can be called from a GUI that can search for light using our LIFX API and add them to this list.

Below we add a check if they already exists and ask if the user wants to add them:
```cs
[Display("Search for Devices")]
[Browsable(true)]
public void ScanForDevices()
{
    var lights = LifxApi.GetLights();
    
    foreach (var light in lights)
    {
        if (Lights.Any(d => d.uuid == light.uuid) == false)
        {
            var newDeviceRequest = new NewDeviceRequest()
            {
                Name = $"{light.label} {light.uuid}"
            };
            UserInput.Request(newDeviceRequest);
            
            if (newDeviceRequest.response == NewDeviceRequest.Response.Yes)
                Lights.Add(light);
        }
    }
}
```

Below is how it looks in the TUI:
![](doc/tui-settings.png)



### OpenTAP LIFX TestStep
In order for OpenTAP to control the lights, we can create a test step.

The step needs a way to select which light should be changed and how it should change. We can do this with a property and the `AvailableValues` attribute, this way the GUI will show a list available lights to control.

```cs
public class LifxStep : TestStep
{
    public List<LifxLight> AvailableLights => LifxSettings.Current.Lights;
    
    [AvailableValues(nameof(AvailableLights))]
    public LifxLight Light { get; set; }
}
```
![](doc/tui-available-lights.png)


Now to select which change we want to make to the lights, we can create a enum with the possible actions we can run. This will function similarly as the `AvailableValues` attribute by automatically providing the available options:
```cs
public enum LifxAction
{
    TurnOn,
    TurnOff,
    ChangeBrightness,
    SetColor,
    SetTemperature
}
public LifxAction Action { get; set; }
```

With these options we just need to run the right API call in the TestStep `Run` method:
```cs
public override void Run()
{
    if (Light == null)
        UpgradeVerdict(Verdict.Inconclusive);

    try
    {
        switch (Action)
        {
            case LifxAction.TurnOn:
                LifxApi.TurnOn(Light);
                break;
            ...
        }
        
        UpgradeVerdict(Verdict.Pass);
    }
    catch
    {
        UpgradeVerdict(Verdict.Fail);
    }
}
```

![](doc/tui-testplan.png)


## Create an OpenTAP TestPlan
With the TestStep ready we can start creating an OpenTAP TestPlan. Which allows us to do all kinds of cool stuff with the lights.

One example is we can sweep different colors going through all imaginable colors of the lights:
![](doc/tui-sweep.png)



## Create a small GUI

## Servy
Serves cli calls directly to rest endpoints