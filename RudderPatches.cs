using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace exploration
{
    internal class RudderPatches
    {
        public static int index = -1;

        [HarmonyPatch(typeof(GPButtonSteeringWheel))]
        public static class GPButtonSteeringWheelPatches
        {
            [HarmonyPrefix]
            [HarmonyPatch("RudderToWheelAngle")]
            public static bool RudderToWheelAnglePatch(GPButtonSteeringWheel __instance, Rudder ___rudder, ref float __result)
            {
                if (__instance.name == "whipstaff" && __instance.attachedRudder.connectedBody.name == "BOAT ASTURIAS (227)(Clone)")
                {
                    float angle = Mathf.Abs(___rudder.currentAngle);
                    bool negative = ___rudder.currentAngle < 0f;

                    // weird glitches at the extremeties
                    if (angle > 30f)
                        angle = 30f;

                    // rotation
                    float rotation = 0f;
                    float height = 0f;

                    if (angle <= 10f)
                    {
                        rotation = Mathf.Lerp(0, 20f, angle / 10f) * (negative ? -1 : 1);
                        height = Mathf.Lerp(4.03f, 4.01f, angle / 10f);
                    }
                    else if (angle <= 20f)
                    {
                        rotation = Mathf.Lerp(20f, 40f, (angle - 10f) / 10f) * (negative ? -1 : 1);
                        height = Mathf.Lerp(4.01f, 3.56f, (angle - 10f) / 10f);
                    }
                    else if (angle <= 30f)
                    {
                        rotation = Mathf.Lerp(40f, 67.5f, (angle - 20f) / 10f) * (negative ? -1 : 1);
                        height = Mathf.Lerp(3.56f, 3f, (angle - 20f) / 10f);
                    }

                    __instance.transform.localPosition = new Vector3(-7.399f, 0f, height);
                    __result = rotation;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(ShipyardPartsInstaller))]
        public static class ShipyardPartsInstallerPatches
        {
            [HarmonyPrefix]
            [HarmonyPatch("ChangeOrderedOption")]
            public static void Prefix(int partIndex, BoatPartsOrder ___currentOrder, BoatCustomParts ___currentParts)
            {
                int selected = ___currentOrder.orderedOptions[partIndex];

                GameObject option = ___currentParts.availableParts[partIndex].partOptions[selected].gameObject;
                GPButtonSteeringWheel component = option.GetComponentInChildren<GPButtonSteeringWheel>(true);

                if (component)
                {
                    Transform visual = component.attachedRudder.gameObject.GetComponent<Rudder>().rudderGraphicsFix;

                    if (visual)
                    {
                        Debug.Log($"Disabing rudder connected to: {component.gameObject.name}");
                        visual.gameObject.SetActive(false);
                    }
                }

                index = partIndex;
            }

            [HarmonyPostfix]
            [HarmonyPatch("ChangeOrderedOption")]
            public static void Postfix(int partIndex, BoatPartsOrder ___currentOrder, BoatCustomParts ___currentParts)
            {
                int selected = ___currentOrder.orderedOptions[partIndex];

                GameObject option = ___currentParts.availableParts[partIndex].partOptions[selected].gameObject;
                GPButtonSteeringWheel component = option.GetComponentInChildren<GPButtonSteeringWheel>(true);

                if (component)
                {
                    Transform visual = component.attachedRudder.gameObject.GetComponent<Rudder>().rudderGraphicsFix;

                    if (visual)
                    {
                        Debug.Log($"Enabling rudder connected to: {component.gameObject.name}");
                        visual.gameObject.SetActive(true);
                    }
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("ResetOrder")]
            public static void ResetOrderPostfix(BoatPartsOrder ___currentOrder, BoatCustomParts ___currentParts)
            {
                if (index > -1)
                {
                    int currentlySelectedValue = ___currentOrder.orderedOptions[index];

                    List<BoatPartOption> options = ___currentParts.availableParts[index].partOptions;

                    // if option.index is currentlySelectedValue then enable rudder graphics fix
                    // if not, disable it
                    for (int i = 0; i < options.Count; i++)
                    {
                        GameObject ob = options[i].gameObject;
                        GPButtonSteeringWheel component = ob.GetComponentInChildren<GPButtonSteeringWheel>(true);

                        if (component)
                        {
                            Transform visual = component.attachedRudder.gameObject.GetComponent<Rudder>().rudderGraphicsFix;

                            if (visual)
                            {
                                visual.gameObject.SetActive(i == currentlySelectedValue);
                            }
                        }
                    }

                    index = -1;
                }
            }
        }

        //[HarmonyPatch(typeof(BoatPart))]
        //public static class BoatPartPatches
        //{
        //    [HarmonyPostfix]
        //    [HarmonyPatch("SetOptionEnabled")]
        //    public static void Postfix(BoatPart __instance, int i, bool state)
        //    {
        //        GameObject ob = __instance.partOptions[i].gameObject;
        //        GPButtonSteeringWheel component = ob.GetComponentInChildren<GPButtonSteeringWheel>(true);

        //        if (component)
        //        {
        //            Transform visual = component.attachedRudder.gameObject.GetComponent<Rudder>().rudderGraphicsFix;

        //            if (visual)
        //            {
        //                visual.gameObject.SetActive(state);
        //            }
        //        }
        //    }
        //}

        [HarmonyPatch(typeof(Shipyard))]
        public static class ShipyardPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("AdmitShip")]
            public static void Postfix(Shipyard __instance, GameObject ship)
            {

                GPButtonSteeringWheel[] components = ship.GetComponentsInChildren<GPButtonSteeringWheel>(true);

                foreach (GPButtonSteeringWheel component in components)
                {
                    component.currentInput = 0f;
                }
            }
        }
    }
}
