////////////////////////////////////////////////////////////////////////////
// bl_Extensions
//
//
//                    Lovatto Studio 2016
////////////////////////////////////////////////////////////////////////////
using UnityEngine;

namespace Speedbox
{
    public static class bl_Extensions
    {

        public static void SetBool(this GameObject preb, string key, bool value)
        {
            int bi = (value == true) ? 1 : 0;
            PlayerPrefs.SetInt(key, bi);
        }

        public static bool GetBool(this GameObject preb, string key, bool defaultValue = false)
        {
            int dbi = (defaultValue == true) ? 1 : 0;
            int bi = PlayerPrefs.GetInt(key, dbi);
            bool v = (bi == 1) ? true : false;
            return v;
        }

        public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
        }
    }
}