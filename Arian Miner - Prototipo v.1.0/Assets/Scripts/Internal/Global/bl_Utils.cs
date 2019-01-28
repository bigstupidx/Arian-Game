using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Net;
#if UNITY_5_3_OR_NEWER|| UNITY_5_4
using UnityEngine.SceneManagement;
#endif

namespace Speedbox
{
    public static class bl_Utils
    {

        /// <summary>
        /// is Plataform mobile?
        /// </summary>
        public static bool IsMobile
        {
            get
            {
                bool mobile = false;
#if !UNITY_EDITOR && UNITY_ANDROID || UNITY_IPHONE || UNITY_IOS
            mobile = true;
#endif
                return mobile;
            }
        }
        public static void LoadLevel(string scene)
        {
#if UNITY_5_3_OR_NEWER || UNITY_5_4
        SceneManager.LoadScene(scene);
#else
            Application.LoadLevel(scene);
#endif
        }

        public static void LoadLevel(int scene)
        {
#if UNITY_5_3_OR_NEWER || UNITY_5_4
            SceneManager.LoadScene(scene);
#else
            Application.LoadLevel(scene);
#endif
        }

        #region NetworkUtils
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion



        #region ScreenShot
        public class ScreenShot
        {
            /// <summary>
            /// Call this to capture a custom, screenshot
            /// </summary>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <returns></returns>
            public static Texture2D CaptureCustomScreenshot(int width, int height)
            {
                Texture2D textured = new Texture2D(width, height, TextureFormat.RGB24, true, false);
                textured.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
                int miplevel = Screen.width / 800;
                Texture2D textured2 = new Texture2D(width >> miplevel, height >> miplevel, UnityEngine.TextureFormat.RGB24, false, false);
                textured2.SetPixels32(textured.GetPixels32(miplevel));
                textured2.Apply();
                return textured2;
            }
            /// <summary>
            /// Call this to capture a screenshot Automatic size
            /// </summary>
            /// <returns></returns>
            public static byte[] CaptureScreenshot()
            {
                UnityEngine.Texture2D textured = new UnityEngine.Texture2D(UnityEngine.Screen.width, UnityEngine.Screen.height, UnityEngine.TextureFormat.RGB24, false, false);
                textured.ReadPixels(new UnityEngine.Rect(0f, 0f, (float)UnityEngine.Screen.width, (float)UnityEngine.Screen.height), 0, 0);
                return textured.EncodeToPNG();
            }
            /// <summary>
            /// Call this to capture a custom size screenshot
            /// </summary>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <returns></returns>
            public static byte[] CaptureScreenshot(int width, int height)
            {
                Texture2D textured = new Texture2D(width, height, UnityEngine.TextureFormat.RGB24, false, false);
                textured.ReadPixels(new UnityEngine.Rect(0f, 0f, (float)width, (float)height), 0, 0);
                return textured.EncodeToPNG();
            }
        }
        #endregion

        #region Animator
        public class AnimatorUtils
        {
            /// <summary>
            /// Get the length of a animation clip in animator
            /// </summary>
            /// <param name="anim"></param>
            /// <param name="animClip"></param>
            /// <returns></returns>
            public static float GetAnimationLenght(Animator anim, string animClip)
            {
                float time = 1;
                RuntimeAnimatorController ac = anim.runtimeAnimatorController;    //Get Animator controller
                for (int i = 0; i < ac.animationClips.Length; i++)                 //For all animations
                {
                    if (ac.animationClips[i].name == animClip)        //If it has the same name as your clip
                    {
                        time = ac.animationClips[i].length;
                    }
                }

                return time;

            }

            public static IEnumerator WaitAnimationLenghtForDesactive(Animator anim, int layer = 0)
            {
                yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(layer).length);
                anim.gameObject.SetActive(false);
            }
        }
        #endregion

        #region Corrutines
        public class CorrutinesUtils : MonoBehaviour
        {
            public void WaitForDesactive(GameObject obj, float time)
            {
                StartCoroutine(WaitForDesactiveIE(obj, time));
            }

            public void WaitForDesactiveActive(GameObject desactive, GameObject active, float time)
            {
                StartCoroutine(WaitForDesactiveActiveIE(desactive, active, time));
            }

            private static IEnumerator WaitForDesactiveIE(GameObject obj, float time)
            {
                yield return new WaitForSeconds(time);
                obj.SetActive(false);
            }

            public static IEnumerator WaitActiveInArray(GameObject[] obj, int index, float time)
            {
                yield return new WaitForSeconds(time);
                foreach (GameObject o in obj) { o.SetActive(false); }
                obj[index].SetActive(true);
            }

            private static IEnumerator WaitForDesactiveActiveIE(GameObject desactive, GameObject active, float time)
            {
                yield return new WaitForSeconds(time);
                desactive.SetActive(false);
                active.SetActive(true);
            }

            /// <summary>
            /// Wait a time in a corrutine not affected by time.timeScale
            /// </summary>
            /// <param name="time"></param>
            /// <returns></returns>
            public static IEnumerator WaitForRealSeconds(float time)
            {
                float start = Time.realtimeSinceStartup;
                while (Time.realtimeSinceStartup < start + time)
                {
                    yield return null;
                }
            }
        }
        #endregion

        #region String
        public class StringUtils
        {
            public static string Md5Sum(string strToEncrypt)
            {
                System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
                byte[] bytes = ue.GetBytes(strToEncrypt);

                // encrypt bytes
                System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hashBytes = md5.ComputeHash(bytes);

                // Convert the encrypted bytes back to a string (base 16)
                string hashString = "";

                for (int i = 0; i < hashBytes.Length; i++)
                {
                    hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
                }

                return hashString.PadLeft(32, '0');
            }

            public static string GetValueBetween(string input, string startFlag, string endFlag, int startPos)
            {
                int startIndex = input.IndexOf(startFlag, startPos);
                int endIndex = input.IndexOf(endFlag, startPos);
                int valueLength = endIndex - startIndex - 1;
                string output = input.Substring(startIndex + 1, valueLength);
                return output;
            }

            public static string GetValueBetween(string input, string startFlag, string endFlag)
            {
                int startIndex = input.IndexOf(startFlag);
                int endIndex = input.IndexOf(endFlag);
                int valueLength = endIndex - startIndex - 1;
                string output = input.Substring(startIndex + 1, valueLength);
                return output;
            }

        }
        #endregion

        #region Matfh
        public class MathfUtils
        {

            /// <summary>
            /// a easi in version of lerp
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static float EaseLerp(float value)
            {
                return Mathf.SmoothStep(0, Mathf.SmoothStep(0, value, value), value);
            }

            public static float EaseLerpExtreme(float value)
            {
                return Mathf.SmoothStep(0, Mathf.SmoothStep(0, value, value), Mathf.SmoothStep(0, value, value));
            }


            public static float Hermite(float start, float end, float value)
            {
                return Mathf.Lerp(start, end, value * value * (3.0f - 2.0f * value));
            }

            public static float Sinerp(float start, float end, float value)
            {
                return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * 0.5f));
            }

            public static float Coserp(float start, float end, float value)
            {
                return Mathf.Lerp(start, end, 1.0f - Mathf.Cos(value * Mathf.PI * 0.5f));
            }

            public static float Berp(float start, float end, float value)
            {
                value = Mathf.Clamp01(value);
                value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
                return start + (end - start) * value;
            }

            public static float SmoothStep(float x, float min, float max)
            {
                x = Mathf.Clamp(x, min, max);
                float v1 = (x - min) / (max - min);
                float v2 = (x - min) / (max - min);
                return -2 * v1 * v1 * v1 + 3 * v2 * v2;
            }

            public static float Lerp(float start, float end, float value)
            {
                return ((1.0f - value) * start) + (value * end);
            }

            public static Vector3 NearestPoint(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
            {
                Vector3 lineDirection = Vector3.Normalize(lineEnd - lineStart);
                float closestPoint = Vector3.Dot((point - lineStart), lineDirection) / Vector3.Dot(lineDirection, lineDirection);
                return lineStart + (closestPoint * lineDirection);
            }

            public static Vector3 NearestPointStrict(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
            {
                Vector3 fullDirection = lineEnd - lineStart;
                Vector3 lineDirection = Vector3.Normalize(fullDirection);
                float closestPoint = Vector3.Dot((point - lineStart), lineDirection) / Vector3.Dot(lineDirection, lineDirection);
                return lineStart + (Mathf.Clamp(closestPoint, 0.0f, Vector3.Magnitude(fullDirection)) * lineDirection);
            }
            public static float Bounce(float x)
            {
                return Mathf.Abs(Mathf.Sin(6.28f * (x + 1f) * (x + 1f)) * (1f - x));
            }

            // test for value that is near specified float (due to floating point inprecision)
            // all thanks to Opless for this!
            public static bool Approx(float val, float about, float range)
            {
                return ((Mathf.Abs(val - about) < range));
            }

            // test if a Vector3 is close to another Vector3 (due to floating point inprecision)
            // compares the square of the distance to the square of the range as this 
            // avoids calculating a square root which is much slower than squaring the range
            public static bool Approx(Vector3 val, Vector3 about, float range)
            {
                return ((val - about).sqrMagnitude < range * range);
            }

            /*
              * CLerp - Circular Lerp - is like lerp but handles the wraparound from 0 to 360.
              * This is useful when interpolating eulerAngles and the object
              * crosses the 0/360 boundary.  The standard Lerp function causes the object
              * to rotate in the wrong direction and looks stupid. Clerp fixes that.
              */
            public static float Clerp(float start, float end, float value)
            {
                float min = 0.0f;
                float max = 360.0f;
                float half = Mathf.Abs((max - min) / 2.0f);//half the distance between min and max
                float retval = 0.0f;
                float diff = 0.0f;
                if ((end - start) < -half)
                {
                    diff = ((max - start) + end) * value;
                    retval = start + diff;
                }
                else if ((end - start) > half)
                {
                    diff = -((max - end) + start) * value;
                    retval = start + diff;
                }
                else
                {
                    retval = start + (end - start) * value;
                }

                return retval;

            }
        }
        #endregion

        #region Color
        public class ColorUtils
        {
            /// <summary>
            /// Note that Color32 and Color implictly convert to each other. You may pass a Color object to this method without first casting it.
            /// </summary>
            /// <param name="color"></param>
            /// <returns></returns>
            public static string ColorToHex(Color32 color)
            {
                string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
                return hex;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="hex"></param>
            /// <returns></returns>
            public static Color HexToColor(string hex)
            {
                byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                return new Color32(r, g, b, 255);
            }
        }
        #endregion

        #region Texture
        public class TextureUtil
        {
            /// colorize a texture
            public static Color[] Colorize(Color[] pixels, Color tint)
            {
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i].r = pixels[i].r - (1.0f - tint.r);
                    pixels[i].g = pixels[i].g - (1.0f - tint.g);
                    pixels[i].b = pixels[i].b - (1.0f - tint.b);
                }

                return pixels;
            }

            /// mask a texture using a second texture
            public static Color[] Mask(Color[] pixels, Color[] maskPixels)
            {
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i].a = maskPixels[i].a;
                    if (pixels[i].a <= 0.0f)
                    {
                        pixels[i] = new Color(0, 0, 0, 0);
                    }
                }

                return pixels;
            }

            /// paste one texture on top of another
            public static Color[] Paste(Color[] topPixels, Color[] bottomPixels)
            {
                for (int i = 0; i < bottomPixels.Length; i++)
                {
                    bottomPixels[i] = Color.Lerp(bottomPixels[i], topPixels[i], topPixels[i].a);
                }

                return bottomPixels;
            }
        }
        #endregion

        #region PlayerPrefabUtils
        public class PlayerPrefsX
        {
            static private int endianDiff1;
            static private int endianDiff2;
            static private int idx;
            static private byte[] byteBlock;
            enum ArrayType { Float, Int32, Bool, String, Vector2, Vector3, Quaternion, Color }

            public static bool SetBool(string name, bool value)
            {
                try
                {
                    PlayerPrefs.SetInt(name, value ? 1 : 0);
                }
                catch
                {
                    return false;
                }
                return true;
            }

            public static bool GetBool(string name)
            {
                return PlayerPrefs.GetInt(name) == 1;
            }

            public static bool GetBool(string name, bool defaultValue)
            {
                return (1 == PlayerPrefs.GetInt(name, defaultValue ? 1 : 0));
            }

            public static long GetLong(string key, long defaultValue)
            {
                int lowBits, highBits;
                SplitLong(defaultValue, out lowBits, out highBits);
                lowBits = PlayerPrefs.GetInt(key + "_lowBits", lowBits);
                highBits = PlayerPrefs.GetInt(key + "_highBits", highBits);

                // unsigned, to prevent loss of sign bit.
                ulong ret = (uint)highBits;
                ret = (ret << 32);
                return (long)(ret | (ulong)(uint)lowBits);
            }

            public static long GetLong(string key)
            {
                int lowBits = PlayerPrefs.GetInt(key + "_lowBits");
                int highBits = PlayerPrefs.GetInt(key + "_highBits");

                // unsigned, to prevent loss of sign bit.
                ulong ret = (uint)highBits;
                ret = (ret << 32);
                return (long)(ret | (ulong)(uint)lowBits);
            }

            private static void SplitLong(long input, out int lowBits, out int highBits)
            {
                // unsigned everything, to prevent loss of sign bit.
                lowBits = (int)(uint)(ulong)input;
                highBits = (int)(uint)(input >> 32);
            }

            public static void SetLong(string key, long value)
            {
                int lowBits, highBits;
                SplitLong(value, out lowBits, out highBits);
                PlayerPrefs.SetInt(key + "_lowBits", lowBits);
                PlayerPrefs.SetInt(key + "_highBits", highBits);
            }

            public static bool SetVector2(string key, Vector2 vector)
            {
                return SetFloatArray(key, new float[] { vector.x, vector.y });
            }

            static Vector2 GetVector2(string key)
            {
                var floatArray = GetFloatArray(key);
                if (floatArray.Length < 2)
                {
                    return Vector2.zero;
                }
                return new Vector2(floatArray[0], floatArray[1]);
            }

            public static Vector2 GetVector2(string key, Vector2 defaultValue)
            {
                if (PlayerPrefs.HasKey(key))
                {
                    return GetVector2(key);
                }
                return defaultValue;
            }

            public static bool SetVector3(string key, Vector3 vector)
            {
                return SetFloatArray(key, new float[] { vector.x, vector.y, vector.z });
            }

            public static Vector3 GetVector3(string key)
            {
                var floatArray = GetFloatArray(key);
                if (floatArray.Length < 3)
                {
                    return Vector3.zero;
                }
                return new Vector3(floatArray[0], floatArray[1], floatArray[2]);
            }

            public static Vector3 GetVector3(string key, Vector3 defaultValue)
            {
                if (PlayerPrefs.HasKey(key))
                {
                    return GetVector3(key);
                }
                return defaultValue;
            }

            public static bool SetQuaternion(string key, Quaternion vector)
            {
                return SetFloatArray(key, new float[] { vector.x, vector.y, vector.z, vector.w });
            }

            public static Quaternion GetQuaternion(string key)
            {
                var floatArray = GetFloatArray(key);
                if (floatArray.Length < 4)
                {
                    return Quaternion.identity;
                }
                return new Quaternion(floatArray[0], floatArray[1], floatArray[2], floatArray[3]);
            }

            public static Quaternion GetQuaternion(string key, Quaternion defaultValue)
            {
                if (PlayerPrefs.HasKey(key))
                {
                    return GetQuaternion(key);
                }
                return defaultValue;
            }

            public static bool SetColor(string key, Color color)
            {
                return SetFloatArray(key, new float[] { color.r, color.g, color.b, color.a });
            }

            public static Color GetColor(string key)
            {
                var floatArray = GetFloatArray(key);
                if (floatArray.Length < 4)
                {
                    return new Color(0.0f, 0.0f, 0.0f, 0.0f);
                }
                return new Color(floatArray[0], floatArray[1], floatArray[2], floatArray[3]);
            }

            public static Color GetColor(string key, Color defaultValue)
            {
                if (PlayerPrefs.HasKey(key))
                {
                    return GetColor(key);
                }
                return defaultValue;
            }

            public static bool SetBoolArray(string key, bool[] boolArray)
            {
                // Make a byte array that's a multiple of 8 in length, plus 5 bytes to store the number of entries as an int32 (+ identifier)
                // We have to store the number of entries, since the boolArray length might not be a multiple of 8, so there could be some padded zeroes
                var bytes = new byte[(boolArray.Length + 7) / 8 + 5];
                bytes[0] = System.Convert.ToByte(ArrayType.Bool);   // Identifier
                var bits = new BitArray(boolArray);
                bits.CopyTo(bytes, 5);
                Initialize();
                ConvertInt32ToBytes(boolArray.Length, bytes); // The number of entries in the boolArray goes in the first 4 bytes

                return SaveBytes(key, bytes);
            }

            public static bool[] GetBoolArray(string key)
            {
                if (PlayerPrefs.HasKey(key))
                {
                    var bytes = System.Convert.FromBase64String(PlayerPrefs.GetString(key));
                    if (bytes.Length < 5)
                    {
                        Debug.LogError("Corrupt preference file for " + key);
                        return new bool[0];
                    }
                    if ((ArrayType)bytes[0] != ArrayType.Bool)
                    {
                        Debug.LogError(key + " is not a boolean array");
                        return new bool[0];
                    }
                    Initialize();

                    // Make a new bytes array that doesn't include the number of entries + identifier (first 5 bytes) and turn that into a BitArray
                    var bytes2 = new byte[bytes.Length - 5];
                    System.Array.Copy(bytes, 5, bytes2, 0, bytes2.Length);
                    var bits = new BitArray(bytes2);
                    // Get the number of entries from the first 4 bytes after the identifier and resize the BitArray to that length, then convert it to a boolean array
                    bits.Length = ConvertBytesToInt32(bytes);
                    var boolArray = new bool[bits.Count];
                    bits.CopyTo(boolArray, 0);

                    return boolArray;
                }
                return new bool[0];
            }

            public static bool[] GetBoolArray(string key, bool defaultValue, int defaultSize)
            {
                if (PlayerPrefs.HasKey(key))
                {
                    return GetBoolArray(key);
                }
                var boolArray = new bool[defaultSize];
                for (int i = 0; i < defaultSize; i++)
                {
                    boolArray[i] = defaultValue;
                }
                return boolArray;
            }

            public static bool SetStringArray(string key, string[] stringArray)
            {
                var bytes = new byte[stringArray.Length + 1];
                bytes[0] = System.Convert.ToByte(ArrayType.String); // Identifier
                Initialize();

                // Store the length of each string that's in stringArray, so we can extract the correct strings in GetStringArray
                for (var i = 0; i < stringArray.Length; i++)
                {
                    if (stringArray[i] == null)
                    {
                        Debug.LogError("Can't save null entries in the string array when setting " + key);
                        return false;
                    }
                    if (stringArray[i].Length > 255)
                    {
                        Debug.LogError("Strings cannot be longer than 255 characters when setting " + key);
                        return false;
                    }
                    bytes[idx++] = (byte)stringArray[i].Length;
                }

                try
                {
                    PlayerPrefs.SetString(key, System.Convert.ToBase64String(bytes) + "|" + String.Join("", stringArray));
                }
                catch
                {
                    return false;
                }
                return true;
            }

            public static string[] GetStringArray(string key)
            {
                if (PlayerPrefs.HasKey(key))
                {
                    var completeString = PlayerPrefs.GetString(key);
                    var separatorIndex = completeString.IndexOf("|"[0]);
                    if (separatorIndex < 4)
                    {
                        Debug.LogError("Corrupt preference file for " + key);
                        return new string[0];
                    }
                    var bytes = System.Convert.FromBase64String(completeString.Substring(0, separatorIndex));
                    if ((ArrayType)bytes[0] != ArrayType.String)
                    {
                        Debug.LogError(key + " is not a string array");
                        return new string[0];
                    }
                    Initialize();

                    var numberOfEntries = bytes.Length - 1;
                    var stringArray = new string[numberOfEntries];
                    var stringIndex = separatorIndex + 1;
                    for (var i = 0; i < numberOfEntries; i++)
                    {
                        int stringLength = bytes[idx++];
                        if (stringIndex + stringLength > completeString.Length)
                        {
                            Debug.LogError("Corrupt preference file for " + key);
                            return new string[0];
                        }
                        stringArray[i] = completeString.Substring(stringIndex, stringLength);
                        stringIndex += stringLength;
                    }

                    return stringArray;
                }
                return new string[0];
            }

            public static string[] GetStringArray(string key, string defaultValue, int defaultSize)
            {
                if (PlayerPrefs.HasKey(key))
                {
                    return GetStringArray(key);
                }
                var stringArray = new string[defaultSize];
                for (int i = 0; i < defaultSize; i++)
                {
                    stringArray[i] = defaultValue;
                }
                return stringArray;
            }

            public static bool SetIntArray(string key, int[] intArray)
            {
                return SetValue(key, intArray, ArrayType.Int32, 1, ConvertFromInt);
            }

            public static bool SetFloatArray(string key, float[] floatArray)
            {
                return SetValue(key, floatArray, ArrayType.Float, 1, ConvertFromFloat);
            }

            public static bool SetVector2Array(string key, Vector2[] vector2Array)
            {
                return SetValue(key, vector2Array, ArrayType.Vector2, 2, ConvertFromVector2);
            }

            public static bool SetVector3Array(string key, Vector3[] vector3Array)
            {
                return SetValue(key, vector3Array, ArrayType.Vector3, 3, ConvertFromVector3);
            }

            public static bool SetQuaternionArray(string key, Quaternion[] quaternionArray)
            {
                return SetValue(key, quaternionArray, ArrayType.Quaternion, 4, ConvertFromQuaternion);
            }

            public static bool SetColorArray(string key, Color[] colorArray)
            {
                return SetValue(key, colorArray, ArrayType.Color, 4, ConvertFromColor);
            }

            private static bool SetValue<T>(string key, T array, ArrayType arrayType, int vectorNumber, System.Action<T, byte[], int> convert) where T : IList
            {
                var bytes = new byte[(4 * array.Count) * vectorNumber + 1];
                bytes[0] = System.Convert.ToByte(arrayType);    // Identifier
                Initialize();

                for (var i = 0; i < array.Count; i++)
                {
                    convert(array, bytes, i);
                }
                return SaveBytes(key, bytes);
            }

            private static void ConvertFromInt(int[] array, byte[] bytes, int i)
            {
                ConvertInt32ToBytes(array[i], bytes);
            }

            private static void ConvertFromFloat(float[] array, byte[] bytes, int i)
            {
                ConvertFloatToBytes(array[i], bytes);
            }

            private static void ConvertFromVector2(Vector2[] array, byte[] bytes, int i)
            {
                ConvertFloatToBytes(array[i].x, bytes);
                ConvertFloatToBytes(array[i].y, bytes);
            }

            private static void ConvertFromVector3(Vector3[] array, byte[] bytes, int i)
            {
                ConvertFloatToBytes(array[i].x, bytes);
                ConvertFloatToBytes(array[i].y, bytes);
                ConvertFloatToBytes(array[i].z, bytes);
            }

            private static void ConvertFromQuaternion(Quaternion[] array, byte[] bytes, int i)
            {
                ConvertFloatToBytes(array[i].x, bytes);
                ConvertFloatToBytes(array[i].y, bytes);
                ConvertFloatToBytes(array[i].z, bytes);
                ConvertFloatToBytes(array[i].w, bytes);
            }

            private static void ConvertFromColor(Color[] array, byte[] bytes, int i)
            {
                ConvertFloatToBytes(array[i].r, bytes);
                ConvertFloatToBytes(array[i].g, bytes);
                ConvertFloatToBytes(array[i].b, bytes);
                ConvertFloatToBytes(array[i].a, bytes);
            }

            public static int[] GetIntArray(string key)
            {
                var intList = new List<int>();
                GetValue(key, intList, ArrayType.Int32, 1, ConvertToInt);
                return intList.ToArray();
            }

            public static int[] GetIntArray(String key, int defaultValue, int defaultSize)
            {
                if (PlayerPrefs.HasKey(key))
                {
                    return GetIntArray(key);
                }
                var intArray = new int[defaultSize];
                for (int i = 0; i < defaultSize; i++)
                {
                    intArray[i] = defaultValue;
                }
                return intArray;
            }

            public static float[] GetFloatArray(String key)
            {
                var floatList = new List<float>();
                GetValue(key, floatList, ArrayType.Float, 1, ConvertToFloat);
                return floatList.ToArray();
            }

            public static float[] GetFloatArray(String key, float defaultValue, int defaultSize)
            {
                if (PlayerPrefs.HasKey(key))
                {
                    return GetFloatArray(key);
                }
                var floatArray = new float[defaultSize];
                for (int i = 0; i < defaultSize; i++)
                {
                    floatArray[i] = defaultValue;
                }
                return floatArray;
            }

            public static Vector2[] GetVector2Array(String key)
            {
                var vector2List = new List<Vector2>();
                GetValue(key, vector2List, ArrayType.Vector2, 2, ConvertToVector2);
                return vector2List.ToArray();
            }

            public static Vector2[] GetVector2Array(String key, Vector2 defaultValue, int defaultSize)
            {
                if (PlayerPrefs.HasKey(key))
                {
                    return GetVector2Array(key);
                }
                var vector2Array = new Vector2[defaultSize];
                for (int i = 0; i < defaultSize; i++)
                {
                    vector2Array[i] = defaultValue;
                }
                return vector2Array;
            }

            public static Vector3[] GetVector3Array(String key)
            {
                var vector3List = new List<Vector3>();
                GetValue(key, vector3List, ArrayType.Vector3, 3, ConvertToVector3);
                return vector3List.ToArray();
            }

            public static Vector3[] GetVector3Array(String key, Vector3 defaultValue, int defaultSize)
            {
                if (PlayerPrefs.HasKey(key))

                {
                    return GetVector3Array(key);
                }
                var vector3Array = new Vector3[defaultSize];
                for (int i = 0; i < defaultSize; i++)
                {
                    vector3Array[i] = defaultValue;
                }
                return vector3Array;
            }

            public static Quaternion[] GetQuaternionArray(String key)
            {
                var quaternionList = new List<Quaternion>();
                GetValue(key, quaternionList, ArrayType.Quaternion, 4, ConvertToQuaternion);
                return quaternionList.ToArray();
            }

            public static Quaternion[] GetQuaternionArray(String key, Quaternion defaultValue, int defaultSize)
            {
                if (PlayerPrefs.HasKey(key))
                {
                    return GetQuaternionArray(key);
                }
                var quaternionArray = new Quaternion[defaultSize];
                for (int i = 0; i < defaultSize; i++)
                {
                    quaternionArray[i] = defaultValue;
                }
                return quaternionArray;
            }

            public static Color[] GetColorArray(String key)
            {
                var colorList = new List<Color>();
                GetValue(key, colorList, ArrayType.Color, 4, ConvertToColor);
                return colorList.ToArray();
            }

            public static Color[] GetColorArray(String key, Color defaultValue, int defaultSize)
            {
                if (PlayerPrefs.HasKey(key))
                {
                    return GetColorArray(key);
                }
                var colorArray = new Color[defaultSize];
                for (int i = 0; i < defaultSize; i++)
                {
                    colorArray[i] = defaultValue;
                }
                return colorArray;
            }

            private static void GetValue<T>(String key, T list, ArrayType arrayType, int vectorNumber, Action<T, byte[]> convert) where T : IList
            {
                if (PlayerPrefs.HasKey(key))
                {
                    var bytes = System.Convert.FromBase64String(PlayerPrefs.GetString(key));
                    if ((bytes.Length - 1) % (vectorNumber * 4) != 0)
                    {
                        Debug.LogError("Corrupt preference file for " + key);
                        return;
                    }
                    if ((ArrayType)bytes[0] != arrayType)
                    {
                        Debug.LogError(key + " is not a " + arrayType.ToString() + " array");
                        return;
                    }
                    Initialize();

                    var end = (bytes.Length - 1) / (vectorNumber * 4);
                    for (var i = 0; i < end; i++)
                    {
                        convert(list, bytes);
                    }
                }
            }

            private static void ConvertToInt(List<int> list, byte[] bytes)
            {
                list.Add(ConvertBytesToInt32(bytes));
            }

            private static void ConvertToFloat(List<float> list, byte[] bytes)
            {
                list.Add(ConvertBytesToFloat(bytes));
            }

            private static void ConvertToVector2(List<Vector2> list, byte[] bytes)
            {
                list.Add(new Vector2(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
            }

            private static void ConvertToVector3(List<Vector3> list, byte[] bytes)
            {
                list.Add(new Vector3(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
            }

            private static void ConvertToQuaternion(List<Quaternion> list, byte[] bytes)
            {
                list.Add(new Quaternion(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
            }

            private static void ConvertToColor(List<Color> list, byte[] bytes)
            {
                list.Add(new Color(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
            }

            public static void ShowArrayType(String key)
            {
                var bytes = System.Convert.FromBase64String(PlayerPrefs.GetString(key));
                if (bytes.Length > 0)
                {
                    ArrayType arrayType = (ArrayType)bytes[0];
                    Debug.Log(key + " is a " + arrayType.ToString() + " array");
                }
            }

            private static void Initialize()
            {
                if (System.BitConverter.IsLittleEndian)
                {
                    endianDiff1 = 0;
                    endianDiff2 = 0;
                }
                else
                {
                    endianDiff1 = 3;
                    endianDiff2 = 1;
                }
                if (byteBlock == null)
                {
                    byteBlock = new byte[4];
                }
                idx = 1;
            }

            private static bool SaveBytes(String key, byte[] bytes)
            {
                try
                {
                    PlayerPrefs.SetString(key, System.Convert.ToBase64String(bytes));
                }
                catch
                {
                    return false;
                }
                return true;
            }

            private static void ConvertFloatToBytes(float f, byte[] bytes)
            {
                byteBlock = System.BitConverter.GetBytes(f);
                ConvertTo4Bytes(bytes);
            }

            private static float ConvertBytesToFloat(byte[] bytes)
            {
                ConvertFrom4Bytes(bytes);
                return System.BitConverter.ToSingle(byteBlock, 0);
            }

            private static void ConvertInt32ToBytes(int i, byte[] bytes)
            {
                byteBlock = System.BitConverter.GetBytes(i);
                ConvertTo4Bytes(bytes);
            }

            private static int ConvertBytesToInt32(byte[] bytes)
            {
                ConvertFrom4Bytes(bytes);
                return System.BitConverter.ToInt32(byteBlock, 0);
            }

            private static void ConvertTo4Bytes(byte[] bytes)
            {
                bytes[idx] = byteBlock[endianDiff1];
                bytes[idx + 1] = byteBlock[1 + endianDiff2];
                bytes[idx + 2] = byteBlock[2 - endianDiff2];
                bytes[idx + 3] = byteBlock[3 - endianDiff1];
                idx += 4;
            }

            private static void ConvertFrom4Bytes(byte[] bytes)
            {
                byteBlock[endianDiff1] = bytes[idx];
                byteBlock[1 + endianDiff2] = bytes[idx + 1];
                byteBlock[2 - endianDiff2] = bytes[idx + 2];
                byteBlock[3 - endianDiff1] = bytes[idx + 3];
                idx += 4;
            }
        }
        #endregion

        #region Render
        public class RenderUtils
        {
            public static bool IsVisibleFrom(Renderer renderer, Camera camera)
            {
                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
                return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
            }
        }
        #endregion
    }
}