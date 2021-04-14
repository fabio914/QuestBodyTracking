//
//  BodyTrackingSender.swift
//  BodyTracking
//
//  Created by Fabio de Albuquerque Dela Antonio on 14/04/2021.
//

import Foundation
import SwiftSocket
import ARKit

protocol BodyTrackingSenderProtocol {
    func send(bodyAnchor: ARBodyAnchor)
}

final class BodyTrackingSender: BodyTrackingSenderProtocol {
    let client: TCPClient

    init(client: TCPClient) {
        self.client = client
    }

    func send(bodyAnchor: ARBodyAnchor) {
        _ = client.send(data: SkeletonPayload(bodyAnchor: bodyAnchor).data)
    }

    deinit {
        client.close()
    }
}

struct PosePayload {
    let px: Float32
    let py: Float32
    let pz: Float32
    let qx: Float32
    let qy: Float32
    let qz: Float32
    let qw: Float32

    init(pose: Pose) {
        self.px = .init(pose.position.x)
        self.py = .init(pose.position.y)
        self.pz = .init(pose.position.z)
        self.qx = .init(pose.rotation.x)
        self.qy = .init(pose.rotation.y)
        self.qz = .init(pose.rotation.z)
        self.qw = .init(pose.rotation.w)
    }
}

struct JointPayload {
    let local: PosePayload
    let model: PosePayload

    init(skeleton: ARSkeleton3D, jointName: String) {
        let name = ARSkeleton3D.JointName(rawValue: jointName)
        guard let localTransform = skeleton.localTransform(for: name),
            let modelTransform = skeleton.modelTransform(for: name)
        else {
            self.local = .init(pose: .identity)
            self.model = .init(pose: .identity)
            return
        }

        // FIXME: Improve this... We're converting from Float to Double and then back to Float
        self.local = .init(pose: .init(localTransform))
        self.model = .init(pose: .init(modelTransform))
    }
}

struct SkeletonPayload {
    let protocolIdentifier: UInt32 = 11235813

    // This can be improved....
    let root: JointPayload
    let hips_joint: JointPayload
    let left_upLeg_joint: JointPayload
    let left_leg_joint: JointPayload
    let left_foot_joint: JointPayload
    let left_toes_joint: JointPayload
    let left_toesEnd_joint: JointPayload
    let right_upLeg_joint: JointPayload
    let right_leg_joint: JointPayload
    let right_foot_joint: JointPayload
    let right_toes_joint: JointPayload
    let right_toesEnd_joint: JointPayload
    let spine_1_joint: JointPayload
    let spine_2_joint: JointPayload
    let spine_3_joint: JointPayload
    let spine_4_joint: JointPayload
    let spine_5_joint: JointPayload
    let spine_6_joint: JointPayload
    let spine_7_joint: JointPayload
    let right_shoulder_1_joint: JointPayload
    let right_arm_joint: JointPayload
    let right_forearm_joint: JointPayload
    let right_hand_joint: JointPayload
    let right_handThumbStart_joint: JointPayload
    let right_handThumb_1_joint: JointPayload
    let right_handThumb_2_joint: JointPayload
    let right_handThumbEnd_joint: JointPayload
    let right_handIndexStart_joint: JointPayload
    let right_handIndex_1_joint: JointPayload
    let right_handIndex_2_joint: JointPayload
    let right_handIndex_3_joint: JointPayload
    let right_handIndexEnd_joint: JointPayload
    let right_handMidStart_joint: JointPayload
    let right_handMid_1_joint: JointPayload
    let right_handMid_2_joint: JointPayload
    let right_handMid_3_joint: JointPayload
    let right_handMidEnd_joint: JointPayload
    let right_handRingStart_joint: JointPayload
    let right_handRing_1_joint: JointPayload
    let right_handRing_2_joint: JointPayload
    let right_handRing_3_joint: JointPayload
    let right_handRingEnd_joint: JointPayload
    let right_handPinkyStart_joint: JointPayload
    let right_handPinky_1_joint: JointPayload
    let right_handPinky_2_joint: JointPayload
    let right_handPinky_3_joint: JointPayload
    let right_handPinkyEnd_joint: JointPayload
    let left_shoulder_1_joint: JointPayload
    let left_arm_joint: JointPayload
    let left_forearm_joint: JointPayload
    let left_hand_joint: JointPayload
    let left_handThumbStart_joint: JointPayload
    let left_handThumb_1_joint: JointPayload
    let left_handThumb_2_joint: JointPayload
    let left_handThumbEnd_joint: JointPayload
    let left_handIndexStart_joint: JointPayload
    let left_handIndex_1_joint: JointPayload
    let left_handIndex_2_joint: JointPayload
    let left_handIndex_3_joint: JointPayload
    let left_handIndexEnd_joint: JointPayload
    let left_handMidStart_joint: JointPayload
    let left_handMid_1_joint: JointPayload
    let left_handMid_2_joint: JointPayload
    let left_handMid_3_joint: JointPayload
    let left_handMidEnd_joint: JointPayload
    let left_handRingStart_joint: JointPayload
    let left_handRing_1_joint: JointPayload
    let left_handRing_2_joint: JointPayload
    let left_handRing_3_joint: JointPayload
    let left_handRingEnd_joint: JointPayload
    let left_handPinkyStart_joint: JointPayload
    let left_handPinky_1_joint: JointPayload
    let left_handPinky_2_joint: JointPayload
    let left_handPinky_3_joint: JointPayload
    let left_handPinkyEnd_joint: JointPayload
    let head_joint: JointPayload
    let jaw_joint: JointPayload
    let chin_joint: JointPayload
    let nose_joint: JointPayload
    let right_eye_joint: JointPayload
    let right_eyeUpperLid_joint: JointPayload
    let right_eyeLowerLid_joint: JointPayload
    let right_eyeball_joint: JointPayload
    let left_eye_joint: JointPayload
    let left_eyeUpperLid_joint: JointPayload
    let left_eyeLowerLid_joint: JointPayload
    let left_eyeball_joint: JointPayload
    let neck_1_joint: JointPayload
    let neck_2_joint: JointPayload
    let neck_3_joint: JointPayload
    let neck_4_joint: JointPayload

    init(bodyAnchor: ARBodyAnchor) {
        let skeleton = bodyAnchor.skeleton

        self.root = JointPayload(skeleton: skeleton, jointName: "root")
        self.hips_joint = JointPayload(skeleton: skeleton, jointName: "hips_joint")
        self.left_upLeg_joint = JointPayload(skeleton: skeleton, jointName: "left_upLeg_joint")
        self.left_leg_joint = JointPayload(skeleton: skeleton, jointName: "left_leg_joint")
        self.left_foot_joint = JointPayload(skeleton: skeleton, jointName: "left_foot_joint")
        self.left_toes_joint = JointPayload(skeleton: skeleton, jointName: "left_toes_joint")
        self.left_toesEnd_joint = JointPayload(skeleton: skeleton, jointName: "left_toesEnd_joint")
        self.right_upLeg_joint = JointPayload(skeleton: skeleton, jointName: "right_upLeg_joint")
        self.right_leg_joint = JointPayload(skeleton: skeleton, jointName: "right_leg_joint")
        self.right_foot_joint = JointPayload(skeleton: skeleton, jointName: "right_foot_joint")
        self.right_toes_joint = JointPayload(skeleton: skeleton, jointName: "right_toes_joint")
        self.right_toesEnd_joint = JointPayload(skeleton: skeleton, jointName: "right_toesEnd_joint")
        self.spine_1_joint = JointPayload(skeleton: skeleton, jointName: "spine_1_joint")
        self.spine_2_joint = JointPayload(skeleton: skeleton, jointName: "spine_2_joint")
        self.spine_3_joint = JointPayload(skeleton: skeleton, jointName: "spine_3_joint")
        self.spine_4_joint = JointPayload(skeleton: skeleton, jointName: "spine_4_joint")
        self.spine_5_joint = JointPayload(skeleton: skeleton, jointName: "spine_5_joint")
        self.spine_6_joint = JointPayload(skeleton: skeleton, jointName: "spine_6_joint")
        self.spine_7_joint = JointPayload(skeleton: skeleton, jointName: "spine_7_joint")
        self.right_shoulder_1_joint = JointPayload(skeleton: skeleton, jointName: "right_shoulder_1_joint")
        self.right_arm_joint = JointPayload(skeleton: skeleton, jointName: "right_arm_joint")
        self.right_forearm_joint = JointPayload(skeleton: skeleton, jointName: "right_forearm_joint")
        self.right_hand_joint = JointPayload(skeleton: skeleton, jointName: "right_hand_joint")
        self.right_handThumbStart_joint = JointPayload(skeleton: skeleton, jointName: "right_handThumbStart_joint")
        self.right_handThumb_1_joint = JointPayload(skeleton: skeleton, jointName: "right_handThumb_1_joint")
        self.right_handThumb_2_joint = JointPayload(skeleton: skeleton, jointName: "right_handThumb_2_joint")
        self.right_handThumbEnd_joint = JointPayload(skeleton: skeleton, jointName: "right_handThumbEnd_joint")
        self.right_handIndexStart_joint = JointPayload(skeleton: skeleton, jointName: "right_handIndexStart_joint")
        self.right_handIndex_1_joint = JointPayload(skeleton: skeleton, jointName: "right_handIndex_1_joint")
        self.right_handIndex_2_joint = JointPayload(skeleton: skeleton, jointName: "right_handIndex_2_joint")
        self.right_handIndex_3_joint = JointPayload(skeleton: skeleton, jointName: "right_handIndex_3_joint")
        self.right_handIndexEnd_joint = JointPayload(skeleton: skeleton, jointName: "right_handIndexEnd_joint")
        self.right_handMidStart_joint = JointPayload(skeleton: skeleton, jointName: "right_handMidStart_joint")
        self.right_handMid_1_joint = JointPayload(skeleton: skeleton, jointName: "right_handMid_1_joint")
        self.right_handMid_2_joint = JointPayload(skeleton: skeleton, jointName: "right_handMid_2_joint")
        self.right_handMid_3_joint = JointPayload(skeleton: skeleton, jointName: "right_handMid_3_joint")
        self.right_handMidEnd_joint = JointPayload(skeleton: skeleton, jointName: "right_handMidEnd_joint")
        self.right_handRingStart_joint = JointPayload(skeleton: skeleton, jointName: "right_handRingStart_joint")
        self.right_handRing_1_joint = JointPayload(skeleton: skeleton, jointName: "right_handRing_1_joint")
        self.right_handRing_2_joint = JointPayload(skeleton: skeleton, jointName: "right_handRing_2_joint")
        self.right_handRing_3_joint = JointPayload(skeleton: skeleton, jointName: "right_handRing_3_joint")
        self.right_handRingEnd_joint = JointPayload(skeleton: skeleton, jointName: "right_handRingEnd_joint")
        self.right_handPinkyStart_joint = JointPayload(skeleton: skeleton, jointName: "right_handPinkyStart_joint")
        self.right_handPinky_1_joint = JointPayload(skeleton: skeleton, jointName: "right_handPinky_1_joint")
        self.right_handPinky_2_joint = JointPayload(skeleton: skeleton, jointName: "right_handPinky_2_joint")
        self.right_handPinky_3_joint = JointPayload(skeleton: skeleton, jointName: "right_handPinky_3_joint")
        self.right_handPinkyEnd_joint = JointPayload(skeleton: skeleton, jointName: "right_handPinkyEnd_joint")
        self.left_shoulder_1_joint = JointPayload(skeleton: skeleton, jointName: "left_shoulder_1_joint")
        self.left_arm_joint = JointPayload(skeleton: skeleton, jointName: "left_arm_joint")
        self.left_forearm_joint = JointPayload(skeleton: skeleton, jointName: "left_forearm_joint")
        self.left_hand_joint = JointPayload(skeleton: skeleton, jointName: "left_hand_joint")
        self.left_handThumbStart_joint = JointPayload(skeleton: skeleton, jointName: "left_handThumbStart_joint")
        self.left_handThumb_1_joint = JointPayload(skeleton: skeleton, jointName: "left_handThumb_1_joint")
        self.left_handThumb_2_joint = JointPayload(skeleton: skeleton, jointName: "left_handThumb_2_joint")
        self.left_handThumbEnd_joint = JointPayload(skeleton: skeleton, jointName: "left_handThumbEnd_joint")
        self.left_handIndexStart_joint = JointPayload(skeleton: skeleton, jointName: "left_handIndexStart_joint")
        self.left_handIndex_1_joint = JointPayload(skeleton: skeleton, jointName: "left_handIndex_1_joint")
        self.left_handIndex_2_joint = JointPayload(skeleton: skeleton, jointName: "left_handIndex_2_joint")
        self.left_handIndex_3_joint = JointPayload(skeleton: skeleton, jointName: "left_handIndex_3_joint")
        self.left_handIndexEnd_joint = JointPayload(skeleton: skeleton, jointName: "left_handIndexEnd_joint")
        self.left_handMidStart_joint = JointPayload(skeleton: skeleton, jointName: "left_handMidStart_joint")
        self.left_handMid_1_joint = JointPayload(skeleton: skeleton, jointName: "left_handMid_1_joint")
        self.left_handMid_2_joint = JointPayload(skeleton: skeleton, jointName: "left_handMid_2_joint")
        self.left_handMid_3_joint = JointPayload(skeleton: skeleton, jointName: "left_handMid_3_joint")
        self.left_handMidEnd_joint = JointPayload(skeleton: skeleton, jointName: "left_handMidEnd_joint")
        self.left_handRingStart_joint = JointPayload(skeleton: skeleton, jointName: "left_handRingStart_joint")
        self.left_handRing_1_joint = JointPayload(skeleton: skeleton, jointName: "left_handRing_1_joint")
        self.left_handRing_2_joint = JointPayload(skeleton: skeleton, jointName: "left_handRing_2_joint")
        self.left_handRing_3_joint = JointPayload(skeleton: skeleton, jointName: "left_handRing_3_joint")
        self.left_handRingEnd_joint = JointPayload(skeleton: skeleton, jointName: "left_handRingEnd_joint")
        self.left_handPinkyStart_joint = JointPayload(skeleton: skeleton, jointName: "left_handPinkyStart_joint")
        self.left_handPinky_1_joint = JointPayload(skeleton: skeleton, jointName: "left_handPinky_1_joint")
        self.left_handPinky_2_joint = JointPayload(skeleton: skeleton, jointName: "left_handPinky_2_joint")
        self.left_handPinky_3_joint = JointPayload(skeleton: skeleton, jointName: "left_handPinky_3_joint")
        self.left_handPinkyEnd_joint = JointPayload(skeleton: skeleton, jointName: "left_handPinkyEnd_joint")
        self.head_joint = JointPayload(skeleton: skeleton, jointName: "head_joint")
        self.jaw_joint = JointPayload(skeleton: skeleton, jointName: "jaw_joint")
        self.chin_joint = JointPayload(skeleton: skeleton, jointName: "chin_joint")
        self.nose_joint = JointPayload(skeleton: skeleton, jointName: "nose_joint")
        self.right_eye_joint = JointPayload(skeleton: skeleton, jointName: "right_eye_joint")
        self.right_eyeUpperLid_joint = JointPayload(skeleton: skeleton, jointName: "right_eyeUpperLid_joint")
        self.right_eyeLowerLid_joint = JointPayload(skeleton: skeleton, jointName: "right_eyeLowerLid_joint")
        self.right_eyeball_joint = JointPayload(skeleton: skeleton, jointName: "right_eyeball_joint")
        self.left_eye_joint = JointPayload(skeleton: skeleton, jointName: "left_eye_joint")
        self.left_eyeUpperLid_joint = JointPayload(skeleton: skeleton, jointName: "left_eyeUpperLid_joint")
        self.left_eyeLowerLid_joint = JointPayload(skeleton: skeleton, jointName: "left_eyeLowerLid_joint")
        self.left_eyeball_joint = JointPayload(skeleton: skeleton, jointName: "left_eyeball_joint")
        self.neck_1_joint = JointPayload(skeleton: skeleton, jointName: "neck_1_joint")
        self.neck_2_joint = JointPayload(skeleton: skeleton, jointName: "neck_2_joint")
        self.neck_3_joint = JointPayload(skeleton: skeleton, jointName: "neck_3_joint")
        self.neck_4_joint = JointPayload(skeleton: skeleton, jointName: "neck_4_joint")
    }

    var data: Data {
        let length = MemoryLayout<SkeletonPayload>.size

        var copy = self
        let data = Data(bytes: &copy, count: length)
        return data
    }
}
