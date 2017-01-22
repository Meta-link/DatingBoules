using System;
using UnityEngine;

[Serializable]
public struct Reaction
{
    public string action;
    public string reaction;
    public bool content;
}

public class BouleProperties : ScriptableObject {
    public string nom;
    public Sprite sprite;
    public Reaction[] reactions;
    public AudioClip b3, b2, b1, g1, g2, g3;

    public string getReaction(string a)
    {
        string s = "";
        foreach(Reaction r in reactions)
        {
            if(r.action == a)
            {
                s = r.reaction;
                break;
            }
        }
        return s;
    }

    public bool getContent(string a)
    {
        bool c = false;
        foreach (Reaction r in reactions)
        {
            if (r.action == a)
            {
                c = r.content;
                break;
            }
        }
        return c;
    }
}
