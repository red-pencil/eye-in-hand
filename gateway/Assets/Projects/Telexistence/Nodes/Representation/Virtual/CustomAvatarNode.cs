using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Events;
using System;

using Klak.Wiring;

[ModelBlock("Representation/Custom Avatar Block","Avatar", 150)]
public class CustomAvatarNode : AvatarNode
{


    public Transform[] Bones = new Transform[(int)HumanBodyBones.LastBone];



    protected override Transform GetBoneTransform(HumanBodyBones bone)
    {
        return Bones[(int)bone];
    }

}

