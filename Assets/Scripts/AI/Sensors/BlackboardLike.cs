using System.Collections.Generic;
using UnityEngine;

public class BlackboardLike : MonoBehaviour
{
    Dictionary<string, float> floats = new Dictionary<string, float>();
    Dictionary<string, bool> bools = new Dictionary<string, bool>();
    Dictionary<string, string> strings = new Dictionary<string, string>();

    public float Get01(string key, float def = 0f)
    {
        if (floats.TryGetValue(key, out var v)) return Mathf.Clamp01(v);
        return def;
    }
    public void Set01(string key, float value) => floats[key] = Mathf.Clamp01(value);

    public bool GetBool(string key, bool def = false)
    {
        if (bools.TryGetValue(key, out var v)) return v;
        return def;
    }
    public void SetBool(string key, bool value) => bools[key] = value;

    public string GetString(string key, string def = "")
    {
        if (strings.TryGetValue(key, out var v)) return v;
        return def;
    }
    public void SetString(string key, string value) => strings[key] = value;
}