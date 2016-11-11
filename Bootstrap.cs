using CommunityCoreLibrary_AWorldWithoutHat;
using System;
using System.Reflection;
using UnityEngine;
using Verse;

namespace AWorldWithoutHat
{
    class Bootstrap : Def
    {
        public string ModName;

        static Bootstrap()
        {
            try
            {
                MethodInfo method1 = typeof(Verse.PawnRenderer).GetMethod("RenderPawnInternal", BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new Type[] { typeof(Vector3), typeof(Quaternion), typeof(bool), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(bool) },
                    null);
                MethodInfo method2 = typeof(PawnRenderer_Detour).GetMethod("RenderPawnInternal", BindingFlags.Static | BindingFlags.Public);
                if (!Detours.TryDetourFromTo(method1, method2))
                {
                    Log.Error("EVERYTHING IS BROKEN");
                    return;
                }
            }
            catch (Exception)
            {
                Log.Error("something is seriously wrong");
            }
        }
    }
}
