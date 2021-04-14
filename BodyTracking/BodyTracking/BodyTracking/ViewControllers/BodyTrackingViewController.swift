//
//  BodyTrackingViewController.swift
//  BodyTracking
//
//  Created by Fabio de Albuquerque Dela Antonio on 14/04/2021.
//

import UIKit
import ARKit

final class BodyTrackingViewController: UIViewController {

    private let sender: BodyTrackingSenderProtocol
    private var skeleton: Skeleton?

    @IBOutlet private weak var sceneView: ARSCNView!

    override var prefersStatusBarHidden: Bool {
        true
    }

    override var prefersHomeIndicatorAutoHidden: Bool {
        true
    }

    init(sender: BodyTrackingSenderProtocol) {
        self.sender = sender
        super.init(nibName: String(describing: type(of: self)), bundle: Bundle(for: type(of: self)))
    }

    required init?(coder aDecoder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }

    override func viewDidLoad() {
        super.viewDidLoad()
        title = "Body Tracking"
        configureDisplay()
        configureBackgroundEvent()
        configureScene()
    }

    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        prepareARConfiguration()
    }

    override func viewWillDisappear(_ animated: Bool) {
        super.viewWillDisappear(animated)
        sceneView.session.pause()
    }

    private func configureDisplay() {
        UIApplication.shared.isIdleTimerDisabled = true
    }

    private func configureBackgroundEvent() {
        NotificationCenter.default.addObserver(self, selector: #selector(willResignActive), name: UIApplication.willResignActiveNotification, object: nil)
    }

    private func configureScene() {
        sceneView.rendersCameraGrain = false
        sceneView.rendersMotionBlur = false

        // Light for the model
        sceneView.autoenablesDefaultLighting = true
        sceneView.automaticallyUpdatesLighting = true

        let scene = SCNScene()
        sceneView.scene = scene
        sceneView.session.delegate = self
    }

    private func prepareARConfiguration() {
        guard ARBodyTrackingConfiguration.isSupported else {
            return
        }

        let configuration = ARBodyTrackingConfiguration()
        sceneView.session.run(configuration)
    }

    private func invalidate() {
        // TODO
    }

    private func disconnect() {
        invalidate()
        dismiss(animated: false, completion: nil)
    }

    // MARK: - Actions

    @objc private func willResignActive() {
        disconnect()
    }

    @IBAction private func disconnectAction(_ sender: Any) {
        disconnect()
    }
}

extension BodyTrackingViewController: ARSessionDelegate {

    func session(_ session: ARSession, didUpdate frame: ARFrame) {
    }

    func session(_ session: ARSession, didUpdate anchors: [ARAnchor]) {
        guard let bodyAnchor = anchors.compactMap({ $0 as? ARBodyAnchor }).first else { return }

        if let skeleton = skeleton {
            skeleton.update(bodyAnchor: bodyAnchor)
        } else {
            let skeleton = Skeleton(bodyAnchor: bodyAnchor)
            sceneView.scene.rootNode.addChildNode(skeleton.mainNode)
            self.skeleton = skeleton
        }

        sender.send(bodyAnchor: bodyAnchor)
    }
}
