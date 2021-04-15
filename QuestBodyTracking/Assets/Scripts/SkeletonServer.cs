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
public struct SkeletonPayload {
    public uint protocolIdentifier;
    public JointPayload root;
    public JointPayload hips_joint;
    public JointPayload left_upLeg_joint;
    public JointPayload left_leg_joint;
    public JointPayload left_foot_joint;
    public JointPayload left_toes_joint;
    public JointPayload left_toesEnd_joint;
    public JointPayload right_upLeg_joint;
    public JointPayload right_leg_joint;
    public JointPayload right_foot_joint;
    public JointPayload right_toes_joint;
    public JointPayload right_toesEnd_joint;
    public JointPayload spine_1_joint;
    public JointPayload spine_2_joint;
    public JointPayload spine_3_joint;
    public JointPayload spine_4_joint;
    public JointPayload spine_5_joint;
    public JointPayload spine_6_joint;
    public JointPayload spine_7_joint;
    public JointPayload right_shoulder_1_joint;
    public JointPayload right_arm_joint;
    public JointPayload right_forearm_joint;
    public JointPayload right_hand_joint;
    public JointPayload right_handThumbStart_joint;
    public JointPayload right_handThumb_1_joint;
    public JointPayload right_handThumb_2_joint;
    public JointPayload right_handThumbEnd_joint;
    public JointPayload right_handIndexStart_joint;
    public JointPayload right_handIndex_1_joint;
    public JointPayload right_handIndex_2_joint;
    public JointPayload right_handIndex_3_joint;
    public JointPayload right_handIndexEnd_joint;
    public JointPayload right_handMidStart_joint;
    public JointPayload right_handMid_1_joint;
    public JointPayload right_handMid_2_joint;
    public JointPayload right_handMid_3_joint;
    public JointPayload right_handMidEnd_joint;
    public JointPayload right_handRingStart_joint;
    public JointPayload right_handRing_1_joint;
    public JointPayload right_handRing_2_joint;
    public JointPayload right_handRing_3_joint;
    public JointPayload right_handRingEnd_joint;
    public JointPayload right_handPinkyStart_joint;
    public JointPayload right_handPinky_1_joint;
    public JointPayload right_handPinky_2_joint;
    public JointPayload right_handPinky_3_joint;
    public JointPayload right_handPinkyEnd_joint;
    public JointPayload left_shoulder_1_joint;
    public JointPayload left_arm_joint;
    public JointPayload left_forearm_joint;
    public JointPayload left_hand_joint;
    public JointPayload left_handThumbStart_joint;
    public JointPayload left_handThumb_1_joint;
    public JointPayload left_handThumb_2_joint;
    public JointPayload left_handThumbEnd_joint;
    public JointPayload left_handIndexStart_joint;
    public JointPayload left_handIndex_1_joint;
    public JointPayload left_handIndex_2_joint;
    public JointPayload left_handIndex_3_joint;
    public JointPayload left_handIndexEnd_joint;
    public JointPayload left_handMidStart_joint;
    public JointPayload left_handMid_1_joint;
    public JointPayload left_handMid_2_joint;
    public JointPayload left_handMid_3_joint;
    public JointPayload left_handMidEnd_joint;
    public JointPayload left_handRingStart_joint;
    public JointPayload left_handRing_1_joint;
    public JointPayload left_handRing_2_joint;
    public JointPayload left_handRing_3_joint;
    public JointPayload left_handRingEnd_joint;
    public JointPayload left_handPinkyStart_joint;
    public JointPayload left_handPinky_1_joint;
    public JointPayload left_handPinky_2_joint;
    public JointPayload left_handPinky_3_joint;
    public JointPayload left_handPinkyEnd_joint;
    public JointPayload head_joint;
    public JointPayload jaw_joint;
    public JointPayload chin_joint;
    public JointPayload nose_joint;
    public JointPayload right_eye_joint;
    public JointPayload right_eyeUpperLid_joint;
    public JointPayload right_eyeLowerLid_joint;
    public JointPayload right_eyeball_joint;
    public JointPayload left_eye_joint;
    public JointPayload left_eyeUpperLid_joint;
    public JointPayload left_eyeLowerLid_joint;
    public JointPayload left_eyeball_joint;
    public JointPayload neck_1_joint;
    public JointPayload neck_2_joint;
    public JointPayload neck_3_joint;
    public JointPayload neck_4_joint;

    public const int StructSize = sizeof(uint) + 91 * JointPayload.StructSize;
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
        root.localRotation = skeletonPayload.Value.root.local.ToPose().orientation;
        hips_joint.localRotation = skeletonPayload.Value.hips_joint.local.ToPose().orientation;
        left_upLeg_joint.localRotation = skeletonPayload.Value.left_upLeg_joint.local.ToPose().orientation;
        left_leg_joint.localRotation = skeletonPayload.Value.left_leg_joint.local.ToPose().orientation;
        left_foot_joint.localRotation = skeletonPayload.Value.left_foot_joint.local.ToPose().orientation;
        left_toes_joint.localRotation = skeletonPayload.Value.left_toes_joint.local.ToPose().orientation;
        left_toesEnd_joint.localRotation = skeletonPayload.Value.left_toesEnd_joint.local.ToPose().orientation;
        right_upLeg_joint.localRotation = skeletonPayload.Value.right_upLeg_joint.local.ToPose().orientation;
        right_leg_joint.localRotation = skeletonPayload.Value.right_leg_joint.local.ToPose().orientation;
        right_foot_joint.localRotation = skeletonPayload.Value.right_foot_joint.local.ToPose().orientation;
        right_toes_joint.localRotation = skeletonPayload.Value.right_toes_joint.local.ToPose().orientation;
        right_toesEnd_joint.localRotation = skeletonPayload.Value.right_toesEnd_joint.local.ToPose().orientation;
        spine_1_joint.localRotation = skeletonPayload.Value.spine_1_joint.local.ToPose().orientation;
        spine_2_joint.localRotation = skeletonPayload.Value.spine_2_joint.local.ToPose().orientation;
        spine_3_joint.localRotation = skeletonPayload.Value.spine_3_joint.local.ToPose().orientation;
        spine_4_joint.localRotation = skeletonPayload.Value.spine_4_joint.local.ToPose().orientation;
        spine_5_joint.localRotation = skeletonPayload.Value.spine_5_joint.local.ToPose().orientation;
        spine_6_joint.localRotation = skeletonPayload.Value.spine_6_joint.local.ToPose().orientation;
        spine_7_joint.localRotation = skeletonPayload.Value.spine_7_joint.local.ToPose().orientation;
        right_shoulder_1_joint.localRotation = skeletonPayload.Value.right_shoulder_1_joint.local.ToPose().orientation;
        right_arm_joint.localRotation = skeletonPayload.Value.right_arm_joint.local.ToPose().orientation;
        right_forearm_joint.localRotation = skeletonPayload.Value.right_forearm_joint.local.ToPose().orientation;
        right_hand_joint.localRotation = skeletonPayload.Value.right_hand_joint.local.ToPose().orientation;
        right_handThumbStart_joint.localRotation = skeletonPayload.Value.right_handThumbStart_joint.local.ToPose().orientation;
        right_handThumb_1_joint.localRotation = skeletonPayload.Value.right_handThumb_1_joint.local.ToPose().orientation;
        right_handThumb_2_joint.localRotation = skeletonPayload.Value.right_handThumb_2_joint.local.ToPose().orientation;
        right_handThumbEnd_joint.localRotation = skeletonPayload.Value.right_handThumbEnd_joint.local.ToPose().orientation;
        right_handIndexStart_joint.localRotation = skeletonPayload.Value.right_handIndexStart_joint.local.ToPose().orientation;
        right_handIndex_1_joint.localRotation = skeletonPayload.Value.right_handIndex_1_joint.local.ToPose().orientation;
        right_handIndex_2_joint.localRotation = skeletonPayload.Value.right_handIndex_2_joint.local.ToPose().orientation;
        right_handIndex_3_joint.localRotation = skeletonPayload.Value.right_handIndex_3_joint.local.ToPose().orientation;
        right_handIndexEnd_joint.localRotation = skeletonPayload.Value.right_handIndexEnd_joint.local.ToPose().orientation;
        right_handMidStart_joint.localRotation = skeletonPayload.Value.right_handMidStart_joint.local.ToPose().orientation;
        right_handMid_1_joint.localRotation = skeletonPayload.Value.right_handMid_1_joint.local.ToPose().orientation;
        right_handMid_2_joint.localRotation = skeletonPayload.Value.right_handMid_2_joint.local.ToPose().orientation;
        right_handMid_3_joint.localRotation = skeletonPayload.Value.right_handMid_3_joint.local.ToPose().orientation;
        right_handMidEnd_joint.localRotation = skeletonPayload.Value.right_handMidEnd_joint.local.ToPose().orientation;
        right_handRingStart_joint.localRotation = skeletonPayload.Value.right_handRingStart_joint.local.ToPose().orientation;
        right_handRing_1_joint.localRotation = skeletonPayload.Value.right_handRing_1_joint.local.ToPose().orientation;
        right_handRing_2_joint.localRotation = skeletonPayload.Value.right_handRing_2_joint.local.ToPose().orientation;
        right_handRing_3_joint.localRotation = skeletonPayload.Value.right_handRing_3_joint.local.ToPose().orientation;
        right_handRingEnd_joint.localRotation = skeletonPayload.Value.right_handRingEnd_joint.local.ToPose().orientation;
        right_handPinkyStart_joint.localRotation = skeletonPayload.Value.right_handPinkyStart_joint.local.ToPose().orientation;
        right_handPinky_1_joint.localRotation = skeletonPayload.Value.right_handPinky_1_joint.local.ToPose().orientation;
        right_handPinky_2_joint.localRotation = skeletonPayload.Value.right_handPinky_2_joint.local.ToPose().orientation;
        right_handPinky_3_joint.localRotation = skeletonPayload.Value.right_handPinky_3_joint.local.ToPose().orientation;
        right_handPinkyEnd_joint.localRotation = skeletonPayload.Value.right_handPinkyEnd_joint.local.ToPose().orientation;
        left_shoulder_1_joint.localRotation = skeletonPayload.Value.left_shoulder_1_joint.local.ToPose().orientation;
        left_arm_joint.localRotation = skeletonPayload.Value.left_arm_joint.local.ToPose().orientation;
        left_forearm_joint.localRotation = skeletonPayload.Value.left_forearm_joint.local.ToPose().orientation;
        left_hand_joint.localRotation = skeletonPayload.Value.left_hand_joint.local.ToPose().orientation;
        left_handThumbStart_joint.localRotation = skeletonPayload.Value.left_handThumbStart_joint.local.ToPose().orientation;
        left_handThumb_1_joint.localRotation = skeletonPayload.Value.left_handThumb_1_joint.local.ToPose().orientation;
        left_handThumb_2_joint.localRotation = skeletonPayload.Value.left_handThumb_2_joint.local.ToPose().orientation;
        left_handThumbEnd_joint.localRotation = skeletonPayload.Value.left_handThumbEnd_joint.local.ToPose().orientation;
        left_handIndexStart_joint.localRotation = skeletonPayload.Value.left_handIndexStart_joint.local.ToPose().orientation;
        left_handIndex_1_joint.localRotation = skeletonPayload.Value.left_handIndex_1_joint.local.ToPose().orientation;
        left_handIndex_2_joint.localRotation = skeletonPayload.Value.left_handIndex_2_joint.local.ToPose().orientation;
        left_handIndex_3_joint.localRotation = skeletonPayload.Value.left_handIndex_3_joint.local.ToPose().orientation;
        left_handIndexEnd_joint.localRotation = skeletonPayload.Value.left_handIndexEnd_joint.local.ToPose().orientation;
        left_handMidStart_joint.localRotation = skeletonPayload.Value.left_handMidStart_joint.local.ToPose().orientation;
        left_handMid_1_joint.localRotation = skeletonPayload.Value.left_handMid_1_joint.local.ToPose().orientation;
        left_handMid_2_joint.localRotation = skeletonPayload.Value.left_handMid_2_joint.local.ToPose().orientation;
        left_handMid_3_joint.localRotation = skeletonPayload.Value.left_handMid_3_joint.local.ToPose().orientation;
        left_handMidEnd_joint.localRotation = skeletonPayload.Value.left_handMidEnd_joint.local.ToPose().orientation;
        left_handRingStart_joint.localRotation = skeletonPayload.Value.left_handRingStart_joint.local.ToPose().orientation;
        left_handRing_1_joint.localRotation = skeletonPayload.Value.left_handRing_1_joint.local.ToPose().orientation;
        left_handRing_2_joint.localRotation = skeletonPayload.Value.left_handRing_2_joint.local.ToPose().orientation;
        left_handRing_3_joint.localRotation = skeletonPayload.Value.left_handRing_3_joint.local.ToPose().orientation;
        left_handRingEnd_joint.localRotation = skeletonPayload.Value.left_handRingEnd_joint.local.ToPose().orientation;
        left_handPinkyStart_joint.localRotation = skeletonPayload.Value.left_handPinkyStart_joint.local.ToPose().orientation;
        left_handPinky_1_joint.localRotation = skeletonPayload.Value.left_handPinky_1_joint.local.ToPose().orientation;
        left_handPinky_2_joint.localRotation = skeletonPayload.Value.left_handPinky_2_joint.local.ToPose().orientation;
        left_handPinky_3_joint.localRotation = skeletonPayload.Value.left_handPinky_3_joint.local.ToPose().orientation;
        left_handPinkyEnd_joint.localRotation = skeletonPayload.Value.left_handPinkyEnd_joint.local.ToPose().orientation;
        head_joint.localRotation = skeletonPayload.Value.head_joint.local.ToPose().orientation;
        jaw_joint.localRotation = skeletonPayload.Value.jaw_joint.local.ToPose().orientation;
        chin_joint.localRotation = skeletonPayload.Value.chin_joint.local.ToPose().orientation;
        nose_joint.localRotation = skeletonPayload.Value.nose_joint.local.ToPose().orientation;
        right_eye_joint.localRotation = skeletonPayload.Value.right_eye_joint.local.ToPose().orientation;
        right_eyeUpperLid_joint.localRotation = skeletonPayload.Value.right_eyeUpperLid_joint.local.ToPose().orientation;
        right_eyeLowerLid_joint.localRotation = skeletonPayload.Value.right_eyeLowerLid_joint.local.ToPose().orientation;
        right_eyeball_joint.localRotation = skeletonPayload.Value.right_eyeball_joint.local.ToPose().orientation;
        left_eye_joint.localRotation = skeletonPayload.Value.left_eye_joint.local.ToPose().orientation;
        left_eyeUpperLid_joint.localRotation = skeletonPayload.Value.left_eyeUpperLid_joint.local.ToPose().orientation;
        left_eyeLowerLid_joint.localRotation = skeletonPayload.Value.left_eyeLowerLid_joint.local.ToPose().orientation;
        left_eyeball_joint.localRotation = skeletonPayload.Value.left_eyeball_joint.local.ToPose().orientation;
        neck_1_joint.localRotation = skeletonPayload.Value.neck_1_joint.local.ToPose().orientation;
        neck_2_joint.localRotation = skeletonPayload.Value.neck_2_joint.local.ToPose().orientation;
        neck_3_joint.localRotation = skeletonPayload.Value.neck_3_joint.local.ToPose().orientation;
        neck_4_joint.localRotation = skeletonPayload.Value.neck_4_joint.local.ToPose().orientation;
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