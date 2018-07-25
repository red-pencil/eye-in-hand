using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public abstract class IModalityAccessor
{
    protected List<IModalityAccessor> _subModalities = new List<IModalityAccessor>();
    public abstract string Name();
    public abstract bool Set(IModalityAccessor other);
    bool _enabled = true;

    public virtual void Calibrate()
    {
        foreach (var m in _subModalities)
            m.Calibrate();
    }

    public bool Enabled
    {
        get { return _enabled; }
        set { _enabled = value; }
    }

    public void EnableAll(bool e)
    {
        _enabled = e;
        foreach (var m in _subModalities)
            m.EnableAll(e);
    }

    public virtual IModalityAccessor[] GetSubModalities() { return _subModalities.ToArray(); }
}


[Serializable]
public class TxBodyInput : IModalityAccessor
{

	[Serializable]
	public class Event : UnityEvent<TxBodyInput>
	{
	}

    [Serializable]
    public class PositionModalityAccessor : IModalityAccessor
    {

		[Serializable]
		public class Event : UnityEvent<PositionModalityAccessor>
		{
		}


        string name;
        public override string Name()
        {
            return name;
        }
        public Vector3 value;

        public PositionModalityAccessor(string name) { this.name = name; }
        public override bool Set(IModalityAccessor other)
        {
            var o = other as PositionModalityAccessor;
            if (o == null)
                return false;
            Enabled = o.Enabled;
            value = o.value;
            return true;
        }
    }

    [Serializable]
    public class RotationModalityAccessor : IModalityAccessor
	{
		[Serializable]
		public class Event : UnityEvent<RotationModalityAccessor>
		{
		}
        string name;
        public override string Name()
        {
            return name;
        }
        public Quaternion value;
        public RotationModalityAccessor(string name) { this.name = name; }
        public override bool Set(IModalityAccessor other)
        {
            var o = other as RotationModalityAccessor;
            if (o == null)
                return false;
            Enabled = o.Enabled;
            if (!Enabled)
                return true;
            value = o.value;
            return true;
        }
    }

    [Serializable]
    public class JointModalityAccessor : IModalityAccessor
	{
		[Serializable]
		public class Event : UnityEvent<JointModalityAccessor>
		{
		}
        string _name;
        public JointModalityAccessor(string name)
        {
        }
        public override string Name()
        {
            return _name;
        }

        public override bool Set(IModalityAccessor other)
        {
            var o = other as JointModalityAccessor;
            if (o == null)
                return false;
            Enabled = o.Enabled;
            if (!Enabled)
                return true;
            Position = o.Position;
            Rotation = o.Rotation;
            return true;
        }


        Vector3 _calibPos = Vector3.zero;
        Quaternion _calibRot = Quaternion.identity;

        [SerializeField]
        Vector3 _pos = Vector3.zero;
        [SerializeField]
        Quaternion _rot = Quaternion.identity;

        public Vector3 Position
        {
            get
            {
                return _pos - _calibPos;
            }
            set
            {
                _pos = value;
            }
        }
        public Quaternion Rotation
        {
            get
            {
                return _calibRot * _rot;
            }
            set
            {
                _rot = value;
            }
        }



        public override void Calibrate()
        {
            _calibPos = _pos;
            _calibRot = Quaternion.Inverse(_rot);
        }

    }


    [Serializable]
    public enum SideType
    {
        Left,
        Right,
        None
    }

    [Serializable]
    public enum FingerType
    {
        Thumb,
        Index,
        Middle,
        Ring,
        Little,
        None
    }
    public enum FingerJointType
    {
        //Metacarpal,
        Proximal,
        Middle,
        Distal,
        None
    }
    /*
    [Serializable]
    public class FingerJointModalityAccessor : JointModalityAccessor
    {
        FingerJointType type;
        public FingerJointModalityAccessor(FingerJointType type) : base(type.ToString())
        {
            this.type = type;
        }
        public override bool Set(IModalityAccessor other)
        {
            var o = other as FingerJointModalityAccessor;
            if (o == null || o.type != type && (o.type != FingerJointType.None || type != FingerJointType.None))//constraint on the side
            {
                return false;
            }
            return base.Set(other);
        }
    }*/
    [Serializable]
    public class FingerModalityAccessor : IModalityAccessor
	{
		[Serializable]
		public class Event : UnityEvent<FingerModalityAccessor>
		{
		}
        SideType _side;
        public SideType Side
        {
            get { return _side; }
        }
        public FingerType finger;

        public FingerModalityAccessor(SideType side, FingerType finger)
        {
            this._side = side;
            this.finger = finger;
            _joints = new JointModalityAccessor[3];
            for (int i = 0; i < _joints.Length; ++i)
            {
                _joints[i] = new JointModalityAccessor(((FingerJointType)i).ToString());
                _subModalities.Add(_joints[i]);
            }
        }
        public override string Name()
        {
            return _side.ToString() + "_" + finger.ToString();
        }
        JointModalityAccessor[] _joints;
        public JointModalityAccessor[] Joints
        {
            get{ return _joints; }
        }

        public Quaternion Direction
        {
            get
            {
                return Quaternion.identity;
            }
        }

        public override bool Set(IModalityAccessor other)
        {
            var o = other as FingerModalityAccessor;
            if (o == null)// || o.Side != Side && (o.Side != SideType.None && Side != SideType.None))//constraint on the side
            {
                return false;
            }
            Enabled = o.Enabled;
            if (!Enabled)
                return true;
            for (int i = 0; i < 3; ++i)
            {
                Joints[i].Set(o.Joints[i]);
            }
            return true;
        }
    }


    [Serializable]
    public class HandModalityAccessor : IModalityAccessor
	{
		[Serializable]
		public class Event : UnityEvent<HandModalityAccessor>
		{
		}
        SideType _side;
        public SideType Side
        {
            set { _side = value; }
            get { return _side; }
        }
        public HandModalityAccessor(SideType s)
        {
            _side = s;
            Fingers = new FingerModalityAccessor[5];
            for (int i = 0; i < 5; ++i)
            {
                Fingers[i] = new FingerModalityAccessor(s, (FingerType)i);
                this._subModalities.Add(Fingers[i]);
            }
        }
        public override string Name()
        {
            return _side.ToString() + "_Hand";
        }
        public override bool Set(IModalityAccessor other)
        {
            var o = other as HandModalityAccessor;
            if (o == null)//|| o.Side != Side && (o.Side != SideType.None && Side != SideType.None))//constraint on the side
            {
                return false;
            }
            Enabled = o.Enabled;
            if (!Enabled)
                return true;
            for (int i = 0; i < 5; ++i)
            {
                Fingers[i].Set(o.Fingers[i]);
            }
            return true;
        }
        public FingerModalityAccessor[] Fingers;
    }


    [Serializable]
    public class ArmModalityAccessor : IModalityAccessor
	{
		[Serializable]
		public class Event : UnityEvent<ArmModalityAccessor>
		{
		}
        SideType _side;
        public SideType Side
        {
            get { return _side; }
        }
        public ArmModalityAccessor(SideType s) 
        {
            _side = s;
            shoulder = new JointModalityAccessor("Shoulder");
            Clavicle = new JointModalityAccessor("Clavicle");
            Elbow = new JointModalityAccessor("Elbow");
            wrist = new JointModalityAccessor( "Wrist");
            hand = new HandModalityAccessor(s);

            _subModalities.Add(shoulder);
            _subModalities.Add(Clavicle);
            _subModalities.Add(Elbow);
            _subModalities.Add(wrist);
            _subModalities.Add(hand);
        }
        public override string Name()
        {
            return _side.ToString() + "_Arm";
        }

        public override bool Set(IModalityAccessor other)
        {
            var o = other as ArmModalityAccessor;
            if (o == null)//|| o.Side != Side && (o.Side!= SideType.None && Side!= SideType.None))//constraint on the side
            {
                return false;
            }
            Enabled = o.Enabled;
            if (!Enabled)
                return true;
            shoulder.Set(o.shoulder);
            Clavicle.Set(o.Clavicle);
            wrist.Set(o.wrist);
            Elbow.Set(o.Elbow);
            hand.Set(o.hand);
            return true;
        }

        public JointModalityAccessor shoulder;
        public JointModalityAccessor Clavicle;
        public JointModalityAccessor Elbow;
        public JointModalityAccessor wrist;
        public HandModalityAccessor hand;
    }
    [Serializable]
    public class LegModalityAccessor : IModalityAccessor
	{
		[Serializable]
		public class Event : UnityEvent<TxBodyInput.LegModalityAccessor>
		{
		}


        SideType _side;
        public SideType Side
        {
            get { return _side; }
        }
        public LegModalityAccessor(SideType side)
        {
            _side = side;
			Hip = new JointModalityAccessor("Hip");
            knee = new JointModalityAccessor("Knee");
            foot = new JointModalityAccessor("Foot");


			_subModalities.Add(Hip);
            _subModalities.Add(knee);
            _subModalities.Add(foot);
        }
        public override string Name()
        {
            return _side.ToString() + "_Leg";
        }

        public override bool Set(IModalityAccessor other)
        {
            var o = other as LegModalityAccessor;
            if (o == null)//|| o.Side != Side && (o.Side != SideType.None && Side != SideType.None))//constraint on the side
            {
                return false;
            }
            Enabled = o.Enabled;
            if (!Enabled)
                return true;
            Hip.Set(o.Hip);
            knee.Set(o.knee);
            foot.Set(o.foot);
            return true;
        }
		public JointModalityAccessor Hip;
        public JointModalityAccessor knee;
        public JointModalityAccessor foot;
    }

    public JointModalityAccessor Head = new JointModalityAccessor("Head");
    public JointModalityAccessor Neck = new JointModalityAccessor("Neck");
    public JointModalityAccessor Chest = new JointModalityAccessor("Chest");
	public JointModalityAccessor Waist = new JointModalityAccessor("Waist");
    public ArmModalityAccessor LeftArm = new ArmModalityAccessor(SideType.Left);
    public ArmModalityAccessor RightArm = new ArmModalityAccessor(SideType.Right);
    public LegModalityAccessor LeftLeg = new LegModalityAccessor(SideType.Left);
    public LegModalityAccessor RightLeg = new LegModalityAccessor(SideType.Right);

    

    public override string Name()
    {
        return "Body";
    }

    public override bool Set(IModalityAccessor other)
    {
        var o = other as TxBodyInput;
        if (o == null )
        {
            return false;
        }
        Enabled = o.Enabled;
        if (!Enabled)
            return true;
        Head.Set(o.Head);
        Neck.Set(o.Neck);
        Chest.Set(o.Chest);
        Waist.Set(o.Waist);
        LeftArm.Set(o.LeftArm);
        RightArm.Set(o.RightArm);
        LeftLeg.Set(o.LeftLeg);
        RightLeg.Set(o.RightLeg);
        return true;
    }

    public TxBodyInput()
    {
        _subModalities.Add(Head);
        _subModalities.Add(Neck);
        _subModalities.Add(Chest);
        _subModalities.Add(Waist);
        _subModalities.Add(LeftArm);
        _subModalities.Add(RightArm);
        _subModalities.Add(LeftLeg);
        _subModalities.Add(RightLeg);
    }
}






