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

//    let client: TCPClient
//
//    init(client: TCPClient) {
//        self.client = client
//    }

    func send(bodyAnchor: ARBodyAnchor) {
        // TODO
    }

//    deinit {
//        client.close()
//    }
}
