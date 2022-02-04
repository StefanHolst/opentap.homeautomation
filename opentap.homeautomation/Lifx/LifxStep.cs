using System;
using System.Collections.Generic;
using OpenTap;

namespace opentap.homeautomation.Lifx
{
    public enum LifxAction
    {
        TurnOn,
        TurnOff,
        ChangeBrightness,
        SetColor,
        SetTemperature
    }
    
    public class LifxStep : TestStep
    {
        public LifxStep()
        {
            Name = "{Light} {Action}";
        }
        
        public List<LifxLight> AvailableLights => LifxSettings.Current.Lights;
        
        [AvailableValues(nameof(AvailableLights))]
        public LifxLight Light { get; set; }
        public LifxAction Action { get; set; }

        [EnabledIf(nameof(Action), LifxAction.ChangeBrightness, HideIfDisabled = true)]
        public double Brightness { get; set; }
        [EnabledIf(nameof(Action), LifxAction.SetColor, HideIfDisabled = true)]
        public string Color { get; set; }
        [EnabledIf(nameof(Action), LifxAction.SetTemperature, HideIfDisabled = true)]
        public int Temperature { get; set; }
        
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
                    case LifxAction.TurnOff:
                        LifxApi.TurnOff(Light);
                        break;
                    case LifxAction.ChangeBrightness:
                        LifxApi.SetBrightness(Light, Brightness);
                        break;
                    case LifxAction.SetColor:
                        LifxApi.SetColor(Light, Color);
                        break;
                    case LifxAction.SetTemperature:
                        LifxApi.SetTemperature(Light, Temperature);
                        break;
                }
                
                UpgradeVerdict(Verdict.Pass);
            }
            catch
            {
                UpgradeVerdict(Verdict.Fail);
            }
        }
    }
}