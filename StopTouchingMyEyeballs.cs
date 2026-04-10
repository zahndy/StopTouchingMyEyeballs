using FrooxEngine;
using FrooxEngine.CommonAvatar;
using HarmonyLib;
using Elements.Core;
using ResoniteModLoader;

namespace StopTouchingMyEyeballs
{
    public class StopTouchingMyEyeballs : ResoniteMod
    {
        public static ModConfiguration Config;

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> ENABLED = new ModConfigurationKey<bool>("enabled", "Enabled", () => true);

        public override String Name => "StopTouchingMyEyeballs";
        public override String Author => "zahndy";
        public override String Link => "https://github.com/zahndy/StopTouchingMyEyeballs";
        public override String Version => "1.0.1";

        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("com.zahndy.StopTouchingMyEyeballs");
            Config = GetConfiguration();
            Config.Save(true);
            harmony.PatchAll();
        }
        [HarmonyPatch(typeof(EyeManager), "UpdateFromEyeTracking")]
        class EyeManagerUpdateFromEyeTrackingPatch
        {
            [HarmonyPrefix]
            static bool Prefix(EyeManager __instance, Predicate<ICollider> ____raycastFilter, float ____awayCloseLerp, float  ____leftIntermediatePupilSize, float ____rightIntermediatePupilSize, IEyeDataSourceComponent eyeData, ref bool simulatePupilSize)
            {
                Slot space = __instance.Slot.ActiveUserRoot?.Slot ?? __instance.Slot.GetObjectRoot(false);
                bool isTracking1 = eyeData.GetIsTracking(EyeSide.Left);
                float3 a = eyeData.GetPosition(EyeSide.Left);
                float3 direction1 = eyeData.GetDirection(EyeSide.Left);
                bool isTracking2 = eyeData.GetIsTracking(EyeSide.Right);
                float3 b = eyeData.GetPosition(EyeSide.Right);
                float3 direction2 = eyeData.GetDirection(EyeSide.Right);
                float3 localDirection;
                if (isTracking1 & isTracking2)
                {
                    float3 float3 = direction1 + direction2;
                    localDirection = float3 * 0.5f;
                }
                else
                    localDirection = !isTracking1 ? (!isTracking2 ? float3.Forward : direction2) : direction1;

                float local1 = __instance.Slot.SpaceScaleToLocal(MathX.Distance(in a, in b) * 0.5f, space);
                float spaceScale = eyeData.ConvergenceDistance;

                if (StopTouchingMyEyeballs.Config.GetValue(ENABLED))
                { 
                    spaceScale = eyeData.ConvergenceDistance > 0 ? eyeData.ConvergenceDistance : 10000f;
                }
                else {
                    if ((double)spaceScale <= 0.0)
                    {
                    PhysicsManager physics = __instance.Physics;
                        float3 eyeCenter = __instance.GlobalEyeCenter;
                        float3 globalDirection = __instance.Slot.LocalDirectionToGlobal(in localDirection);
                        Predicate<ICollider> raycastFilter = ____raycastFilter;
                        RaycastHit? raycastHit = physics.RaycastOne(in eyeCenter, in globalDirection, raycastFilter, false);
                        spaceScale = !raycastHit.HasValue ? 10000f : space.GlobalScaleToLocal(raycastHit.Value.Distance);
                    }
                }
                float local4 = __instance.Slot.SpaceScaleToLocal(spaceScale, space);

                __instance.LeftEyeTargetPoint.Value = ComputeTargetPoint(__instance, in direction1, new float3(-local1, 0.0f, 0.0f), local4);
                __instance.RightEyeTargetPoint.Value = ComputeTargetPoint(__instance, in direction2, new float3(local1, 0.0f, 0.0f), local4);
                __instance.TargetPoint.Value = ComputeTargetPoint(__instance, in localDirection, float3.Zero, local4);

                __instance.LeftEyeClose.Value = MathX.Lerp(MathX.Max(1f - eyeData.GetOpenness(EyeSide.Left), (float)__instance.LeftEyeCloseOverride), 1f, ____awayCloseLerp);
                __instance.RightEyeClose.Value = MathX.Lerp(MathX.Max(1f - eyeData.GetOpenness(EyeSide.Right), (float)__instance.RightEyeCloseOverride), 1f, ____awayCloseLerp);
                __instance.CombinedEyeClose.Value = MathX.Max(__instance.LeftEyeClose.Value, __instance.RightEyeClose.Value);

                float? num2 = eyeData.GetPupilDiameter(EyeSide.Left) * 1000f;
                float? num3 = eyeData.GetPupilDiameter(EyeSide.Right) * 1000f;
                if (num2.HasValue && num3.HasValue)
                {
                    simulatePupilSize = false;
                    num2 = MathX.SmoothLerp(__instance.LeftEyePupilSizeMillimeters.Value, num2.Value, ref ____leftIntermediatePupilSize, __instance.Time.Delta * (float)__instance.EyeTrackingPupilSizeSmoothSpeed);
                    num3 = MathX.SmoothLerp(__instance.RightEyePupilSizeMillimeters.Value, num3.Value, ref ____rightIntermediatePupilSize, __instance.Time.Delta * (float)__instance.EyeTrackingPupilSizeSmoothSpeed);
                    __instance.LeftEyePupilSizeMillimeters.Value = num2.Value;
                    __instance.RightEyePupilSizeMillimeters.Value = num3.Value;
                    __instance.CombinedEyePupilSizeMillimeters.Value = (__instance.LeftEyePupilSizeMillimeters.Value + __instance.RightEyePupilSizeMillimeters.Value) * 0.5f;
                }
                __instance.LeftEyeWiden.Value = eyeData.GetWiden(EyeSide.Left);
                __instance.RightEyeWiden.Value = eyeData.GetWiden(EyeSide.Right);
                __instance.CombinedEyeWiden.Value = (float)(((double)__instance.LeftEyeWiden.Value + (double)__instance.RightEyeWiden.Value) * 0.5);
                __instance.LeftEyeSqueeze.Value = eyeData.GetSqueeze(EyeSide.Left);
                __instance.RightEyeSqueeze.Value = eyeData.GetSqueeze(EyeSide.Right);
                __instance.CombinedEyeSqueeze.Value = (float)(((double)__instance.LeftEyeSqueeze.Value + (double)__instance.RightEyeSqueeze.Value) * 0.5);
                __instance.LeftEyeFrown.Value = eyeData.GetFrown(EyeSide.Left);
                __instance.RightEyeFrown.Value = eyeData.GetFrown(EyeSide.Right);
                __instance.CombinedEyeFrown.Value = (float)(((double)__instance.LeftEyeFrown.Value + (double)__instance.RightEyeFrown.Value) * 0.5);
                __instance.LeftEyeInnerBrowVertical.Value = eyeData.GetInnerBrowVertical(EyeSide.Left);
                __instance.RightEyeInnerBrowVertical.Value = eyeData.GetInnerBrowVertical(EyeSide.Right);
                __instance.CombinedEyeInnerBrowVertical.Value = (float)(((double)__instance.LeftEyeInnerBrowVertical.Value + (double)__instance.RightEyeInnerBrowVertical.Value) * 0.5);
                __instance.LeftEyeOuterBrowVertical.Value = eyeData.GetOuterBrowVertical(EyeSide.Left);
                __instance.RightEyeOuterBrowVertical.Value = eyeData.GetOuterBrowVertical(EyeSide.Right);
                __instance.CombinedEyeOuterBrowVertical.Value = (float)(((double)__instance.LeftEyeOuterBrowVertical.Value + (double)__instance.RightEyeOuterBrowVertical.Value) * 0.5);

                return false;

            }
            private static float3 ComputeTargetPoint(EyeManager eyeManager, in float3 direction, in float3 offset, float distance)
            {
                float3 localPoint = offset + direction * distance;
                return eyeManager.Slot.LocalPointToGlobal(in localPoint);
            }
        }
    }
}
