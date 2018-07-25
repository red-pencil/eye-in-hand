using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Events;
using System;

using Klak.Wiring;

[ModelBlock("Representation/Mechanical Avatar Block","Avatar", 150)]
//[BlockVisuals("Avatar", 150)]
public class MechanicalAvatarNode : AvatarNode
{

    Vector3[] OldVals=new Vector3[(int)HumanBodyBones.LastBone];
    Vector3[] _refPos = new Vector3[(int)HumanBodyBones.LastBone];
    bool[] changed = new bool[(int)HumanBodyBones.LastBone];

    public AudioClip clip;
    public AudioSource src;

    float[] scalers = new float[(int)HumanBodyBones.LastBone];

    protected override void SetRotation(HumanBodyBones bone, Quaternion rot)
    {
        int idx = (int)bone;
        Transform t = GetBoneTransform(bone);
        if (t != null)
        {
            var q = _refQuats[idx] * BonesSettings[idx].FixRotation(rot);// new Quaternion(rot.z,rot.y,-rot.x,rot.w);

            var e = q.eulerAngles;
            e.x = 10*(int)(e.x/10);
            e.y = 10 * (int)(e.y / 10);
            e.z = 10 * (int)(e.z / 10);

            if(changed[idx])
            {
                float diff = Vector3.Distance(OldVals[idx], e);
                if(diff>5)
                {
                    src.PlayOneShot(clip,scalers[idx]);
                }

                OldVals[idx] = e;

            }
            changed[(int)bone] = true;

            t.localRotation = Quaternion.Euler(e);

        }
    }
    protected  override void SetPosition(HumanBodyBones bone, Vector3 pos)
    {

        Transform t = GetBoneTransform(bone);
        if (t != null)
        {
            t.localPosition = _refPos[(int)bone] + pos;// new Quaternion(rot.z,rot.y,-rot.x,rot.w);
        }
    }

    protected override void ResetRef()
    {
        base.ResetRef();
        if (animator != null)
        {
            for (int i = 0; i < _refPos.Length; ++i)
            {
                var t = GetBoneTransform((HumanBodyBones)i);
                if (t != null)
                    _refPos[i] = t.localPosition;
            }
        }
    }
    private void Start()
    {
        for (int i = 0; i < scalers.Length; ++i)
            scalers[i] = 0.5f;


        for (int i = 24; i < 54; ++i)
        {
            scalers[i] = 0.1f;
        }
    }

}

