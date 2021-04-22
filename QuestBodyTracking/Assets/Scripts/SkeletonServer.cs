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
        root.rotation = skeletonPayload.Value.Joint(0).model.ToPose().orientation;
        hips_joint.rotation = skeletonPayload.Value.Joint(1).model.ToPose().orientation;
        left_upLeg_joint.rotation = skeletonPayload.Value.Joint(2).model.ToPose().orientation;
        left_leg_joint.rotation = skeletonPayload.Value.Joint(3).model.ToPose().orientation;
        left_foot_joint.rotation = skeletonPayload.Value.Joint(4).model.ToPose().orientation;
        left_toes_joint.rotation = skeletonPayload.Value.Joint(5).model.ToPose().orientation;
        left_toesEnd_joint.rotation = skeletonPayload.Value.Joint(6).model.ToPose().orientation;
        right_upLeg_joint.rotation = skeletonPayload.Value.Joint(7).model.ToPose().orientation;
        right_leg_joint.rotation = skeletonPayload.Value.Joint(8).model.ToPose().orientation;
        right_foot_joint.rotation = skeletonPayload.Value.Joint(9).model.ToPose().orientation;
        right_toes_joint.rotation = skeletonPayload.Value.Joint(10).model.ToPose().orientation;
        right_toesEnd_joint.rotation = skeletonPayload.Value.Joint(11).model.ToPose().orientation;
        spine_1_joint.rotation = skeletonPayload.Value.Joint(12).model.ToPose().orientation;
        spine_2_joint.rotation = skeletonPayload.Value.Joint(13).model.ToPose().orientation;
        spine_3_joint.rotation = skeletonPayload.Value.Joint(14).model.ToPose().orientation;
        spine_4_joint.rotation = skeletonPayload.Value.Joint(15).model.ToPose().orientation;
        spine_5_joint.rotation = skeletonPayload.Value.Joint(16).model.ToPose().orientation;
        spine_6_joint.rotation = skeletonPayload.Value.Joint(17).model.ToPose().orientation;
        spine_7_joint.rotation = skeletonPayload.Value.Joint(18).model.ToPose().orientation;
        right_shoulder_1_joint.rotation = skeletonPayload.Value.Joint(19).model.ToPose().orientation;
        right_arm_joint.rotation = skeletonPayload.Value.Joint(20).model.ToPose().orientation;
        right_forearm_joint.rotation = skeletonPayload.Value.Joint(21).model.ToPose().orientation;
        right_hand_joint.rotation = skeletonPayload.Value.Joint(22).model.ToPose().orientation;
        right_handThumbStart_joint.rotation = skeletonPayload.Value.Joint(23).model.ToPose().orientation;
        right_handThumb_1_joint.rotation = skeletonPayload.Value.Joint(24).model.ToPose().orientation;
        right_handThumb_2_joint.rotation = skeletonPayload.Value.Joint(25).model.ToPose().orientation;
        right_handThumbEnd_joint.rotation = skeletonPayload.Value.Joint(26).model.ToPose().orientation;
        right_handIndexStart_joint.rotation = skeletonPayload.Value.Joint(27).model.ToPose().orientation;
        right_handIndex_1_joint.rotation = skeletonPayload.Value.Joint(28).model.ToPose().orientation;
        right_handIndex_2_joint.rotation = skeletonPayload.Value.Joint(29).model.ToPose().orientation;
        right_handIndex_3_joint.rotation = skeletonPayload.Value.Joint(30).model.ToPose().orientation;
        right_handIndexEnd_joint.rotation = skeletonPayload.Value.Joint(31).model.ToPose().orientation;
        right_handMidStart_joint.rotation = skeletonPayload.Value.Joint(32).model.ToPose().orientation;
        right_handMid_1_joint.rotation = skeletonPayload.Value.Joint(33).model.ToPose().orientation;
        right_handMid_2_joint.rotation = skeletonPayload.Value.Joint(34).model.ToPose().orientation;
        right_handMid_3_joint.rotation = skeletonPayload.Value.Joint(35).model.ToPose().orientation;
        right_handMidEnd_joint.rotation = skeletonPayload.Value.Joint(36).model.ToPose().orientation;
        right_handRingStart_joint.rotation = skeletonPayload.Value.Joint(37).model.ToPose().orientation;
        right_handRing_1_joint.rotation = skeletonPayload.Value.Joint(38).model.ToPose().orientation;
        right_handRing_2_joint.rotation = skeletonPayload.Value.Joint(39).model.ToPose().orientation;
        right_handRing_3_joint.rotation = skeletonPayload.Value.Joint(40).model.ToPose().orientation;
        right_handRingEnd_joint.rotation = skeletonPayload.Value.Joint(41).model.ToPose().orientation;
        right_handPinkyStart_joint.rotation = skeletonPayload.Value.Joint(42).model.ToPose().orientation;
        right_handPinky_1_joint.rotation = skeletonPayload.Value.Joint(43).model.ToPose().orientation;
        right_handPinky_2_joint.rotation = skeletonPayload.Value.Joint(44).model.ToPose().orientation;
        right_handPinky_3_joint.rotation = skeletonPayload.Value.Joint(45).model.ToPose().orientation;
        right_handPinkyEnd_joint.rotation = skeletonPayload.Value.Joint(46).model.ToPose().orientation;
        left_shoulder_1_joint.rotation = skeletonPayload.Value.Joint(47).model.ToPose().orientation;
        left_arm_joint.rotation = skeletonPayload.Value.Joint(48).model.ToPose().orientation;
        left_forearm_joint.rotation = skeletonPayload.Value.Joint(49).model.ToPose().orientation;
        left_hand_joint.rotation = skeletonPayload.Value.Joint(50).model.ToPose().orientation;
        left_handThumbStart_joint.rotation = skeletonPayload.Value.Joint(51).model.ToPose().orientation;
        left_handThumb_1_joint.rotation = skeletonPayload.Value.Joint(52).model.ToPose().orientation;
        left_handThumb_2_joint.rotation = skeletonPayload.Value.Joint(53).model.ToPose().orientation;
        left_handThumbEnd_joint.rotation = skeletonPayload.Value.Joint(54).model.ToPose().orientation;
        left_handIndexStart_joint.rotation = skeletonPayload.Value.Joint(55).model.ToPose().orientation;
        left_handIndex_1_joint.rotation = skeletonPayload.Value.Joint(56).model.ToPose().orientation;
        left_handIndex_2_joint.rotation = skeletonPayload.Value.Joint(57).model.ToPose().orientation;
        left_handIndex_3_joint.rotation = skeletonPayload.Value.Joint(58).model.ToPose().orientation;
        left_handIndexEnd_joint.rotation = skeletonPayload.Value.Joint(59).model.ToPose().orientation;
        left_handMidStart_joint.rotation = skeletonPayload.Value.Joint(60).model.ToPose().orientation;
        left_handMid_1_joint.rotation = skeletonPayload.Value.Joint(61).model.ToPose().orientation;
        left_handMid_2_joint.rotation = skeletonPayload.Value.Joint(62).model.ToPose().orientation;
        left_handMid_3_joint.rotation = skeletonPayload.Value.Joint(63).model.ToPose().orientation;
        left_handMidEnd_joint.rotation = skeletonPayload.Value.Joint(64).model.ToPose().orientation;
        left_handRingStart_joint.rotation = skeletonPayload.Value.Joint(65).model.ToPose().orientation;
        left_handRing_1_joint.rotation = skeletonPayload.Value.Joint(66).model.ToPose().orientation;
        left_handRing_2_joint.rotation = skeletonPayload.Value.Joint(67).model.ToPose().orientation;
        left_handRing_3_joint.rotation = skeletonPayload.Value.Joint(68).model.ToPose().orientation;
        left_handRingEnd_joint.rotation = skeletonPayload.Value.Joint(69).model.ToPose().orientation;
        left_handPinkyStart_joint.rotation = skeletonPayload.Value.Joint(70).model.ToPose().orientation;
        left_handPinky_1_joint.rotation = skeletonPayload.Value.Joint(71).model.ToPose().orientation;
        left_handPinky_2_joint.rotation = skeletonPayload.Value.Joint(72).model.ToPose().orientation;
        left_handPinky_3_joint.rotation = skeletonPayload.Value.Joint(73).model.ToPose().orientation;
        left_handPinkyEnd_joint.rotation = skeletonPayload.Value.Joint(74).model.ToPose().orientation;
        head_joint.rotation = skeletonPayload.Value.Joint(75).model.ToPose().orientation;
        jaw_joint.rotation = skeletonPayload.Value.Joint(76).model.ToPose().orientation;
        chin_joint.rotation = skeletonPayload.Value.Joint(77).model.ToPose().orientation;
        nose_joint.rotation = skeletonPayload.Value.Joint(78).model.ToPose().orientation;
        right_eye_joint.rotation = skeletonPayload.Value.Joint(79).model.ToPose().orientation;
        right_eyeUpperLid_joint.rotation = skeletonPayload.Value.Joint(80).model.ToPose().orientation;
        right_eyeLowerLid_joint.rotation = skeletonPayload.Value.Joint(81).model.ToPose().orientation;
        right_eyeball_joint.rotation = skeletonPayload.Value.Joint(82).model.ToPose().orientation;
        left_eye_joint.rotation = skeletonPayload.Value.Joint(83).model.ToPose().orientation;
        left_eyeUpperLid_joint.rotation = skeletonPayload.Value.Joint(84).model.ToPose().orientation;
        left_eyeLowerLid_joint.rotation = skeletonPayload.Value.Joint(85).model.ToPose().orientation;
        left_eyeball_joint.rotation = skeletonPayload.Value.Joint(86).model.ToPose().orientation;
        neck_1_joint.rotation = skeletonPayload.Value.Joint(87).model.ToPose().orientation;
        neck_2_joint.rotation = skeletonPayload.Value.Joint(88).model.ToPose().orientation;
        neck_3_joint.rotation = skeletonPayload.Value.Joint(89).model.ToPose().orientation;
        neck_4_joint.rotation = skeletonPayload.Value.Joint(90).model.ToPose().orientation;
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