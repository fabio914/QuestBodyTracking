using UnityEngine;
using System;
using System.Collections; 
using System.Collections.Generic;
using System.Net; 
using System.Net.Sockets; 
using System.Text; 
using System.Threading;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PosePayload {
    public float px;
    public float py;
    public float pz;
    public float qx;
    public float qy;
    public float qz;
    public float qw;

    public const int StructSize = 7 * sizeof(float);

    public OVRPose ToPose() {
        OVRPose result = new OVRPose();
        result.position = new Vector3(px, py, pz);
        result.orientation = new Quaternion(qx, qy, qz, qw);
        return result.flipZ();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct JointPayload {
    public PosePayload local;
    public PosePayload model;

    public const int StructSize = 2 * PosePayload.StructSize;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct SkeletonPayload {
    public uint protocolIdentifier;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst=5096)]
    public byte[] data; // 7 floats * 2 poses * 91 joints

    public const int StructSize = sizeof(uint) + 5096;
    public const uint identifier = 11235813;

    public static SkeletonPayload FromBytes(byte[] arr) {
        SkeletonPayload payload = new SkeletonPayload();

        int size = Marshal.SizeOf(payload);

        IntPtr ptr = Marshal.AllocHGlobal(size);

        Marshal.Copy(arr, 0, ptr, size);

        payload = (SkeletonPayload)Marshal.PtrToStructure(ptr, payload.GetType());
        Marshal.FreeHGlobal(ptr);

        return payload;
    }

    public JointPayload Joint(int index) {
        JointPayload payload = new JointPayload();
        int size = JointPayload.StructSize;
        IntPtr ptr = Marshal.AllocHGlobal(size);

        Marshal.Copy(data, (index * size), ptr, size);

        payload = (JointPayload)Marshal.PtrToStructure(ptr, payload.GetType());
        Marshal.FreeHGlobal(ptr);

        return payload;
    }
}

public class SkeletonServer: MonoBehaviour {
    public int port;

    private TcpListener tcpListener; 
    private Thread tcpListenerThread; 
    private TcpClient connectedTcpClient;

    private SkeletonPayload? skeletonPayload;
    
    // FIXME: Improve this....  
    public Transform root;
    public Transform hips_joint;
    public Transform left_upLeg_joint;
    public Transform left_leg_joint;
    public Transform left_foot_joint;
    public Transform left_toes_joint;
    public Transform left_toesEnd_joint;
    public Transform right_upLeg_joint;
    public Transform right_leg_joint;
    public Transform right_foot_joint;
    public Transform right_toes_joint;
    public Transform right_toesEnd_joint;
    public Transform spine_1_joint;
    public Transform spine_2_joint;
    public Transform spine_3_joint;
    public Transform spine_4_joint;
    public Transform spine_5_joint;
    public Transform spine_6_joint;
    public Transform spine_7_joint;
    public Transform right_shoulder_1_joint;
    public Transform right_arm_joint;
    public Transform right_forearm_joint;
    public Transform right_hand_joint;
    public Transform right_handThumbStart_joint;
    public Transform right_handThumb_1_joint;
    public Transform right_handThumb_2_joint;
    public Transform right_handThumbEnd_joint;
    public Transform right_handIndexStart_joint;
    public Transform right_handIndex_1_joint;
    public Transform right_handIndex_2_joint;
    public Transform right_handIndex_3_joint;
    public Transform right_handIndexEnd_joint;
    public Transform right_handMidStart_joint;
    public Transform right_handMid_1_joint;
    public Transform right_handMid_2_joint;
    public Transform right_handMid_3_joint;
    public Transform right_handMidEnd_joint;
    public Transform right_handRingStart_joint;
    public Transform right_handRing_1_joint;
    public Transform right_handRing_2_joint;
    public Transform right_handRing_3_joint;
    public Transform right_handRingEnd_joint;
    public Transform right_handPinkyStart_joint;
    public Transform right_handPinky_1_joint;
    public Transform right_handPinky_2_joint;
    public Transform right_handPinky_3_joint;
    public Transform right_handPinkyEnd_joint;
    public Transform left_shoulder_1_joint;
    public Transform left_arm_joint;
    public Transform left_forearm_joint;
    public Transform left_hand_joint;
    public Transform left_handThumbStart_joint;
    public Transform left_handThumb_1_joint;
    public Transform left_handThumb_2_joint;
    public Transform left_handThumbEnd_joint;
    public Transform left_handIndexStart_joint;
    public Transform left_handIndex_1_joint;
    public Transform left_handIndex_2_joint;
    public Transform left_handIndex_3_joint;
    public Transform left_handIndexEnd_joint;
    public Transform left_handMidStart_joint;
    public Transform left_handMid_1_joint;
    public Transform left_handMid_2_joint;
    public Transform left_handMid_3_joint;
    public Transform left_handMidEnd_joint;
    public Transform left_handRingStart_joint;
    public Transform left_handRing_1_joint;
    public Transform left_handRing_2_joint;
    public Transform left_handRing_3_joint;
    public Transform left_handRingEnd_joint;
    public Transform left_handPinkyStart_joint;
    public Transform left_handPinky_1_joint;
    public Transform left_handPinky_2_joint;
    public Transform left_handPinky_3_joint;
    public Transform left_handPinkyEnd_joint;
    public Transform head_joint;
    public Transform jaw_joint;
    public Transform chin_joint;
    public Transform nose_joint;
    public Transform right_eye_joint;
    public Transform right_eyeUpperLid_joint;
    public Transform right_eyeLowerLid_joint;
    public Transform right_eyeball_joint;
    public Transform left_eye_joint;
    public Transform left_eyeUpperLid_joint;
    public Transform left_eyeLowerLid_joint;
    public Transform left_eyeball_joint;
    public Transform neck_1_joint;
    public Transform neck_2_joint;
    public Transform neck_3_joint;
    public Transform neck_4_joint;

    public void Start() {
        // Consider starting the thread on OnEnable() and ending the Thread on OnDisable()
        tcpListenerThread = new Thread (new ThreadStart(ListenForIncomingRequests));         
        tcpListenerThread.IsBackground = true;         
        tcpListenerThread.Start();     
    }

    public void Update() {
        if (!skeletonPayload.HasValue) {
            return;
        }

        // FIXME: Improve this....
        root.localRotation = skeletonPayload.Value.Joint(0).local.ToPose().orientation;
        hips_joint.localRotation = skeletonPayload.Value.Joint(1).local.ToPose().orientation;
        left_upLeg_joint.localRotation = skeletonPayload.Value.Joint(2).local.ToPose().orientation;
        left_leg_joint.localRotation = skeletonPayload.Value.Joint(3).local.ToPose().orientation;
        left_foot_joint.localRotation = skeletonPayload.Value.Joint(4).local.ToPose().orientation;
        left_toes_joint.localRotation = skeletonPayload.Value.Joint(5).local.ToPose().orientation;
        left_toesEnd_joint.localRotation = skeletonPayload.Value.Joint(6).local.ToPose().orientation;
        right_upLeg_joint.localRotation = skeletonPayload.Value.Joint(7).local.ToPose().orientation;
        right_leg_joint.localRotation = skeletonPayload.Value.Joint(8).local.ToPose().orientation;
        right_foot_joint.localRotation = skeletonPayload.Value.Joint(9).local.ToPose().orientation;
        right_toes_joint.localRotation = skeletonPayload.Value.Joint(10).local.ToPose().orientation;
        right_toesEnd_joint.localRotation = skeletonPayload.Value.Joint(11).local.ToPose().orientation;
        spine_1_joint.localRotation = skeletonPayload.Value.Joint(12).local.ToPose().orientation;
        spine_2_joint.localRotation = skeletonPayload.Value.Joint(13).local.ToPose().orientation;
        spine_3_joint.localRotation = skeletonPayload.Value.Joint(14).local.ToPose().orientation;
        spine_4_joint.localRotation = skeletonPayload.Value.Joint(15).local.ToPose().orientation;
        spine_5_joint.localRotation = skeletonPayload.Value.Joint(16).local.ToPose().orientation;
        spine_6_joint.localRotation = skeletonPayload.Value.Joint(17).local.ToPose().orientation;
        spine_7_joint.localRotation = skeletonPayload.Value.Joint(18).local.ToPose().orientation;
        right_shoulder_1_joint.localRotation = skeletonPayload.Value.Joint(19).local.ToPose().orientation;
        right_arm_joint.localRotation = skeletonPayload.Value.Joint(20).local.ToPose().orientation;
        right_forearm_joint.localRotation = skeletonPayload.Value.Joint(21).local.ToPose().orientation;
        right_hand_joint.localRotation = skeletonPayload.Value.Joint(22).local.ToPose().orientation;
        right_handThumbStart_joint.localRotation = skeletonPayload.Value.Joint(23).local.ToPose().orientation;
        right_handThumb_1_joint.localRotation = skeletonPayload.Value.Joint(24).local.ToPose().orientation;
        right_handThumb_2_joint.localRotation = skeletonPayload.Value.Joint(25).local.ToPose().orientation;
        right_handThumbEnd_joint.localRotation = skeletonPayload.Value.Joint(26).local.ToPose().orientation;
        right_handIndexStart_joint.localRotation = skeletonPayload.Value.Joint(27).local.ToPose().orientation;
        right_handIndex_1_joint.localRotation = skeletonPayload.Value.Joint(28).local.ToPose().orientation;
        right_handIndex_2_joint.localRotation = skeletonPayload.Value.Joint(29).local.ToPose().orientation;
        right_handIndex_3_joint.localRotation = skeletonPayload.Value.Joint(30).local.ToPose().orientation;
        right_handIndexEnd_joint.localRotation = skeletonPayload.Value.Joint(31).local.ToPose().orientation;
        right_handMidStart_joint.localRotation = skeletonPayload.Value.Joint(32).local.ToPose().orientation;
        right_handMid_1_joint.localRotation = skeletonPayload.Value.Joint(33).local.ToPose().orientation;
        right_handMid_2_joint.localRotation = skeletonPayload.Value.Joint(34).local.ToPose().orientation;
        right_handMid_3_joint.localRotation = skeletonPayload.Value.Joint(35).local.ToPose().orientation;
        right_handMidEnd_joint.localRotation = skeletonPayload.Value.Joint(36).local.ToPose().orientation;
        right_handRingStart_joint.localRotation = skeletonPayload.Value.Joint(37).local.ToPose().orientation;
        right_handRing_1_joint.localRotation = skeletonPayload.Value.Joint(38).local.ToPose().orientation;
        right_handRing_2_joint.localRotation = skeletonPayload.Value.Joint(39).local.ToPose().orientation;
        right_handRing_3_joint.localRotation = skeletonPayload.Value.Joint(40).local.ToPose().orientation;
        right_handRingEnd_joint.localRotation = skeletonPayload.Value.Joint(41).local.ToPose().orientation;
        right_handPinkyStart_joint.localRotation = skeletonPayload.Value.Joint(42).local.ToPose().orientation;
        right_handPinky_1_joint.localRotation = skeletonPayload.Value.Joint(43).local.ToPose().orientation;
        right_handPinky_2_joint.localRotation = skeletonPayload.Value.Joint(44).local.ToPose().orientation;
        right_handPinky_3_joint.localRotation = skeletonPayload.Value.Joint(45).local.ToPose().orientation;
        right_handPinkyEnd_joint.localRotation = skeletonPayload.Value.Joint(46).local.ToPose().orientation;
        left_shoulder_1_joint.localRotation = skeletonPayload.Value.Joint(47).local.ToPose().orientation;
        left_arm_joint.localRotation = skeletonPayload.Value.Joint(48).local.ToPose().orientation;
        left_forearm_joint.localRotation = skeletonPayload.Value.Joint(49).local.ToPose().orientation;
        left_hand_joint.localRotation = skeletonPayload.Value.Joint(50).local.ToPose().orientation;
        left_handThumbStart_joint.localRotation = skeletonPayload.Value.Joint(51).local.ToPose().orientation;
        left_handThumb_1_joint.localRotation = skeletonPayload.Value.Joint(52).local.ToPose().orientation;
        left_handThumb_2_joint.localRotation = skeletonPayload.Value.Joint(53).local.ToPose().orientation;
        left_handThumbEnd_joint.localRotation = skeletonPayload.Value.Joint(54).local.ToPose().orientation;
        left_handIndexStart_joint.localRotation = skeletonPayload.Value.Joint(55).local.ToPose().orientation;
        left_handIndex_1_joint.localRotation = skeletonPayload.Value.Joint(56).local.ToPose().orientation;
        left_handIndex_2_joint.localRotation = skeletonPayload.Value.Joint(57).local.ToPose().orientation;
        left_handIndex_3_joint.localRotation = skeletonPayload.Value.Joint(58).local.ToPose().orientation;
        left_handIndexEnd_joint.localRotation = skeletonPayload.Value.Joint(59).local.ToPose().orientation;
        left_handMidStart_joint.localRotation = skeletonPayload.Value.Joint(60).local.ToPose().orientation;
        left_handMid_1_joint.localRotation = skeletonPayload.Value.Joint(61).local.ToPose().orientation;
        left_handMid_2_joint.localRotation = skeletonPayload.Value.Joint(62).local.ToPose().orientation;
        left_handMid_3_joint.localRotation = skeletonPayload.Value.Joint(63).local.ToPose().orientation;
        left_handMidEnd_joint.localRotation = skeletonPayload.Value.Joint(64).local.ToPose().orientation;
        left_handRingStart_joint.localRotation = skeletonPayload.Value.Joint(65).local.ToPose().orientation;
        left_handRing_1_joint.localRotation = skeletonPayload.Value.Joint(66).local.ToPose().orientation;
        left_handRing_2_joint.localRotation = skeletonPayload.Value.Joint(67).local.ToPose().orientation;
        left_handRing_3_joint.localRotation = skeletonPayload.Value.Joint(68).local.ToPose().orientation;
        left_handRingEnd_joint.localRotation = skeletonPayload.Value.Joint(69).local.ToPose().orientation;
        left_handPinkyStart_joint.localRotation = skeletonPayload.Value.Joint(70).local.ToPose().orientation;
        left_handPinky_1_joint.localRotation = skeletonPayload.Value.Joint(71).local.ToPose().orientation;
        left_handPinky_2_joint.localRotation = skeletonPayload.Value.Joint(72).local.ToPose().orientation;
        left_handPinky_3_joint.localRotation = skeletonPayload.Value.Joint(73).local.ToPose().orientation;
        left_handPinkyEnd_joint.localRotation = skeletonPayload.Value.Joint(74).local.ToPose().orientation;
        head_joint.localRotation = skeletonPayload.Value.Joint(75).local.ToPose().orientation;
        jaw_joint.localRotation = skeletonPayload.Value.Joint(76).local.ToPose().orientation;
        chin_joint.localRotation = skeletonPayload.Value.Joint(77).local.ToPose().orientation;
        nose_joint.localRotation = skeletonPayload.Value.Joint(78).local.ToPose().orientation;
        right_eye_joint.localRotation = skeletonPayload.Value.Joint(79).local.ToPose().orientation;
        right_eyeUpperLid_joint.localRotation = skeletonPayload.Value.Joint(80).local.ToPose().orientation;
        right_eyeLowerLid_joint.localRotation = skeletonPayload.Value.Joint(81).local.ToPose().orientation;
        right_eyeball_joint.localRotation = skeletonPayload.Value.Joint(82).local.ToPose().orientation;
        left_eye_joint.localRotation = skeletonPayload.Value.Joint(83).local.ToPose().orientation;
        left_eyeUpperLid_joint.localRotation = skeletonPayload.Value.Joint(84).local.ToPose().orientation;
        left_eyeLowerLid_joint.localRotation = skeletonPayload.Value.Joint(85).local.ToPose().orientation;
        left_eyeball_joint.localRotation = skeletonPayload.Value.Joint(86).local.ToPose().orientation;
        neck_1_joint.localRotation = skeletonPayload.Value.Joint(87).local.ToPose().orientation;
        neck_2_joint.localRotation = skeletonPayload.Value.Joint(88).local.ToPose().orientation;
        neck_3_joint.localRotation = skeletonPayload.Value.Joint(89).local.ToPose().orientation;
        neck_4_joint.localRotation = skeletonPayload.Value.Joint(90).local.ToPose().orientation;

        //root.localPosition = skeletonPayload.Value.Joint(0).local.ToPose().position;
        hips_joint.localPosition = skeletonPayload.Value.Joint(1).local.ToPose().position;
        left_upLeg_joint.localPosition = skeletonPayload.Value.Joint(2).local.ToPose().position;
        left_leg_joint.localPosition = skeletonPayload.Value.Joint(3).local.ToPose().position;
        left_foot_joint.localPosition = skeletonPayload.Value.Joint(4).local.ToPose().position;
        left_toes_joint.localPosition = skeletonPayload.Value.Joint(5).local.ToPose().position;
        left_toesEnd_joint.localPosition = skeletonPayload.Value.Joint(6).local.ToPose().position;
        right_upLeg_joint.localPosition = skeletonPayload.Value.Joint(7).local.ToPose().position;
        right_leg_joint.localPosition = skeletonPayload.Value.Joint(8).local.ToPose().position;
        right_foot_joint.localPosition = skeletonPayload.Value.Joint(9).local.ToPose().position;
        right_toes_joint.localPosition = skeletonPayload.Value.Joint(10).local.ToPose().position;
        right_toesEnd_joint.localPosition = skeletonPayload.Value.Joint(11).local.ToPose().position;
        spine_1_joint.localPosition = skeletonPayload.Value.Joint(12).local.ToPose().position;
        spine_2_joint.localPosition = skeletonPayload.Value.Joint(13).local.ToPose().position;
        spine_3_joint.localPosition = skeletonPayload.Value.Joint(14).local.ToPose().position;
        spine_4_joint.localPosition = skeletonPayload.Value.Joint(15).local.ToPose().position;
        spine_5_joint.localPosition = skeletonPayload.Value.Joint(16).local.ToPose().position;
        spine_6_joint.localPosition = skeletonPayload.Value.Joint(17).local.ToPose().position;
        spine_7_joint.localPosition = skeletonPayload.Value.Joint(18).local.ToPose().position;
        right_shoulder_1_joint.localPosition = skeletonPayload.Value.Joint(19).local.ToPose().position;
        right_arm_joint.localPosition = skeletonPayload.Value.Joint(20).local.ToPose().position;
        right_forearm_joint.localPosition = skeletonPayload.Value.Joint(21).local.ToPose().position;
        right_hand_joint.localPosition = skeletonPayload.Value.Joint(22).local.ToPose().position;
        right_handThumbStart_joint.localPosition = skeletonPayload.Value.Joint(23).local.ToPose().position;
        right_handThumb_1_joint.localPosition = skeletonPayload.Value.Joint(24).local.ToPose().position;
        right_handThumb_2_joint.localPosition = skeletonPayload.Value.Joint(25).local.ToPose().position;
        right_handThumbEnd_joint.localPosition = skeletonPayload.Value.Joint(26).local.ToPose().position;
        right_handIndexStart_joint.localPosition = skeletonPayload.Value.Joint(27).local.ToPose().position;
        right_handIndex_1_joint.localPosition = skeletonPayload.Value.Joint(28).local.ToPose().position;
        right_handIndex_2_joint.localPosition = skeletonPayload.Value.Joint(29).local.ToPose().position;
        right_handIndex_3_joint.localPosition = skeletonPayload.Value.Joint(30).local.ToPose().position;
        right_handIndexEnd_joint.localPosition = skeletonPayload.Value.Joint(31).local.ToPose().position;
        right_handMidStart_joint.localPosition = skeletonPayload.Value.Joint(32).local.ToPose().position;
        right_handMid_1_joint.localPosition = skeletonPayload.Value.Joint(33).local.ToPose().position;
        right_handMid_2_joint.localPosition = skeletonPayload.Value.Joint(34).local.ToPose().position;
        right_handMid_3_joint.localPosition = skeletonPayload.Value.Joint(35).local.ToPose().position;
        right_handMidEnd_joint.localPosition = skeletonPayload.Value.Joint(36).local.ToPose().position;
        right_handRingStart_joint.localPosition = skeletonPayload.Value.Joint(37).local.ToPose().position;
        right_handRing_1_joint.localPosition = skeletonPayload.Value.Joint(38).local.ToPose().position;
        right_handRing_2_joint.localPosition = skeletonPayload.Value.Joint(39).local.ToPose().position;
        right_handRing_3_joint.localPosition = skeletonPayload.Value.Joint(40).local.ToPose().position;
        right_handRingEnd_joint.localPosition = skeletonPayload.Value.Joint(41).local.ToPose().position;
        right_handPinkyStart_joint.localPosition = skeletonPayload.Value.Joint(42).local.ToPose().position;
        right_handPinky_1_joint.localPosition = skeletonPayload.Value.Joint(43).local.ToPose().position;
        right_handPinky_2_joint.localPosition = skeletonPayload.Value.Joint(44).local.ToPose().position;
        right_handPinky_3_joint.localPosition = skeletonPayload.Value.Joint(45).local.ToPose().position;
        right_handPinkyEnd_joint.localPosition = skeletonPayload.Value.Joint(46).local.ToPose().position;
        left_shoulder_1_joint.localPosition = skeletonPayload.Value.Joint(47).local.ToPose().position;
        left_arm_joint.localPosition = skeletonPayload.Value.Joint(48).local.ToPose().position;
        left_forearm_joint.localPosition = skeletonPayload.Value.Joint(49).local.ToPose().position;
        left_hand_joint.localPosition = skeletonPayload.Value.Joint(50).local.ToPose().position;
        left_handThumbStart_joint.localPosition = skeletonPayload.Value.Joint(51).local.ToPose().position;
        left_handThumb_1_joint.localPosition = skeletonPayload.Value.Joint(52).local.ToPose().position;
        left_handThumb_2_joint.localPosition = skeletonPayload.Value.Joint(53).local.ToPose().position;
        left_handThumbEnd_joint.localPosition = skeletonPayload.Value.Joint(54).local.ToPose().position;
        left_handIndexStart_joint.localPosition = skeletonPayload.Value.Joint(55).local.ToPose().position;
        left_handIndex_1_joint.localPosition = skeletonPayload.Value.Joint(56).local.ToPose().position;
        left_handIndex_2_joint.localPosition = skeletonPayload.Value.Joint(57).local.ToPose().position;
        left_handIndex_3_joint.localPosition = skeletonPayload.Value.Joint(58).local.ToPose().position;
        left_handIndexEnd_joint.localPosition = skeletonPayload.Value.Joint(59).local.ToPose().position;
        left_handMidStart_joint.localPosition = skeletonPayload.Value.Joint(60).local.ToPose().position;
        left_handMid_1_joint.localPosition = skeletonPayload.Value.Joint(61).local.ToPose().position;
        left_handMid_2_joint.localPosition = skeletonPayload.Value.Joint(62).local.ToPose().position;
        left_handMid_3_joint.localPosition = skeletonPayload.Value.Joint(63).local.ToPose().position;
        left_handMidEnd_joint.localPosition = skeletonPayload.Value.Joint(64).local.ToPose().position;
        left_handRingStart_joint.localPosition = skeletonPayload.Value.Joint(65).local.ToPose().position;
        left_handRing_1_joint.localPosition = skeletonPayload.Value.Joint(66).local.ToPose().position;
        left_handRing_2_joint.localPosition = skeletonPayload.Value.Joint(67).local.ToPose().position;
        left_handRing_3_joint.localPosition = skeletonPayload.Value.Joint(68).local.ToPose().position;
        left_handRingEnd_joint.localPosition = skeletonPayload.Value.Joint(69).local.ToPose().position;
        left_handPinkyStart_joint.localPosition = skeletonPayload.Value.Joint(70).local.ToPose().position;
        left_handPinky_1_joint.localPosition = skeletonPayload.Value.Joint(71).local.ToPose().position;
        left_handPinky_2_joint.localPosition = skeletonPayload.Value.Joint(72).local.ToPose().position;
        left_handPinky_3_joint.localPosition = skeletonPayload.Value.Joint(73).local.ToPose().position;
        left_handPinkyEnd_joint.localPosition = skeletonPayload.Value.Joint(74).local.ToPose().position;
        head_joint.localPosition = skeletonPayload.Value.Joint(75).local.ToPose().position;
        jaw_joint.localPosition = skeletonPayload.Value.Joint(76).local.ToPose().position;
        chin_joint.localPosition = skeletonPayload.Value.Joint(77).local.ToPose().position;
        nose_joint.localPosition = skeletonPayload.Value.Joint(78).local.ToPose().position;
        right_eye_joint.localPosition = skeletonPayload.Value.Joint(79).local.ToPose().position;
        right_eyeUpperLid_joint.localPosition = skeletonPayload.Value.Joint(80).local.ToPose().position;
        right_eyeLowerLid_joint.localPosition = skeletonPayload.Value.Joint(81).local.ToPose().position;
        right_eyeball_joint.localPosition = skeletonPayload.Value.Joint(82).local.ToPose().position;
        left_eye_joint.localPosition = skeletonPayload.Value.Joint(83).local.ToPose().position;
        left_eyeUpperLid_joint.localPosition = skeletonPayload.Value.Joint(84).local.ToPose().position;
        left_eyeLowerLid_joint.localPosition = skeletonPayload.Value.Joint(85).local.ToPose().position;
        left_eyeball_joint.localPosition = skeletonPayload.Value.Joint(86).local.ToPose().position;
        neck_1_joint.localPosition = skeletonPayload.Value.Joint(87).local.ToPose().position;
        neck_2_joint.localPosition = skeletonPayload.Value.Joint(88).local.ToPose().position;
        neck_3_joint.localPosition = skeletonPayload.Value.Joint(89).local.ToPose().position;
        neck_4_joint.localPosition = skeletonPayload.Value.Joint(90).local.ToPose().position;
    }  

    public const int MaxBufferLength = 65536;
    private void ListenForIncomingRequests () {         
        try {
            tcpListener = new TcpListener(IPAddress.Any, port);             
            tcpListener.Start();              
            Debug.Log("[SERVER] Server is listening");              
            
            byte[][] receivedBuffers = { new byte[SkeletonServer.MaxBufferLength], new byte[SkeletonServer.MaxBufferLength] };
            int receivedBufferIndex = 0;
            int receivedBufferDataSize = 0;

            while (true) {                 
                using (connectedTcpClient = tcpListener.AcceptTcpClient()) {

                    using (NetworkStream stream = connectedTcpClient.GetStream()) {                         
                        int length; 

                        int maximumDataSize = SkeletonServer.MaxBufferLength - receivedBufferDataSize;

                        while ((length = stream.Read(receivedBuffers[receivedBufferIndex], receivedBufferDataSize, maximumDataSize)) != 0) {                         

                            receivedBufferDataSize += length;

                            while (receivedBufferDataSize >= SkeletonPayload.StructSize) {
                                SkeletonPayload payload = SkeletonPayload.FromBytes(receivedBuffers[receivedBufferIndex]);

                                if (payload.protocolIdentifier != SkeletonPayload.identifier) {
                                    Debug.LogWarning("Header mismatch");
                                    stream.Close();
                                    connectedTcpClient.Close();
                                    return;
                                }

                                // Consider adding a lock
                                skeletonPayload = payload;

                                int newBufferIndex = 1 - receivedBufferIndex;
                                int newBufferDataSize = receivedBufferDataSize - SkeletonPayload.StructSize;

                                if (newBufferDataSize > 0) {
                                    Array.Copy(receivedBuffers[receivedBufferIndex], SkeletonPayload.StructSize, receivedBuffers[newBufferIndex], 0, newBufferDataSize);
                                }
                                receivedBufferIndex = newBufferIndex;
                                receivedBufferDataSize = newBufferDataSize;
                            }

                            maximumDataSize = SkeletonServer.MaxBufferLength - receivedBufferDataSize;                 
                        }                                             
                    }                 
                }             
            }         
        }         
        catch (SocketException socketException) {             
            Debug.Log("SocketException " + socketException.ToString());         
        }     
    }
}